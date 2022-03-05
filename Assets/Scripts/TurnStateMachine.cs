using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class TurnStateMachine : MonoBehaviourPunCallbacks
{
    //ゲーム状況を管理するクラス
    public GameContext gameContext;

    //何らかのカードを選択中
    public bool IsSelecting = false;

    //同期中
    public bool isSync;

    //先行の1ターン目かどうか
    public bool isFirstPlayerFirstTurn { get; set; } = true;

    #region ステートマシンの初期化とコルーチンの開始・乱数列の共有
    

    public IEnumerator Init()
    {
        yield return StartCoroutine(GManager.instance.LoadingObject.StartLoading("Now Loading"));

        yield return StartCoroutine(ContinuousController.LoadCoroutine());

        //GManager.instance.optionPanel.Init();

        #region デバッグモード
        if (GManager.instance.IsAI)
        {
            ContinuousController.instance.isRandomMatch = true;

            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.NickName = "Player";
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = ContinuousController.instance.GameVer.ToString();
            }

            yield return new WaitWhile(() => !PhotonNetwork.IsConnectedAndReady);

            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }

            yield return new WaitWhile(() => !PhotonNetwork.InLobby);

            if (!PhotonNetwork.InRoom)
            {
                //作成する部屋の設定
                RoomOptions roomOptions = new RoomOptions();
                roomOptions.IsVisible = false;   //ロビーで見えない部屋にする
                roomOptions.IsOpen = false;      //他のプレイヤーの入室を許可する
                roomOptions.PublishUserId = true;

                roomOptions.MaxPlayers = 1;

                string RoomName = StringUtils.GeneratePassword_AlpahabetNum(50);

                //部屋作成
                PhotonNetwork.CreateRoom(RoomName, roomOptions, null);
            }

            yield return new WaitWhile(() => !PhotonNetwork.InRoom);
        }
        #endregion

        gameContext = new GameContext(GManager.instance.You, GManager.instance.Opponent);

        #region 初期化
        foreach (Player player in gameContext.Players)
        {
            for (int i = 0; i < player.HandTransform.childCount; i++)
            {
                Destroy(player.HandTransform.GetChild(i).gameObject);
            }

            for (int i = 0; i < player.FrontZoneTransform.childCount; i++)
            {
                Destroy(player.FrontZoneTransform.GetChild(i).gameObject);
            }

            for (int i = 0; i < player.BackZoneTransform.childCount; i++)
            {
                Destroy(player.BackZoneTransform.GetChild(i).gameObject);
            }

            player.OffMBCountText();
        }
        #endregion

        Debug.Log("プレイヤー名を設定開始");

        #region プレイヤー名を設定

        #region Photonクライアントの内、マスタークライアントと非マスタークライアントを抽出
        Photon.Realtime.Player MasterPlayer = null;
        Photon.Realtime.Player nonMasterPlayer = null;

        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber != PhotonNetwork.CurrentRoom.MasterClientId)
            {
                nonMasterPlayer = player;
            }

            else
            {
                MasterPlayer = player;
            }
        }
        #endregion

        #region 各プレイヤー名を保存・表示
        SetPlayerName(0, PlayerName(MasterPlayer));
        SetPlayerName(1, PlayerName(nonMasterPlayer));
        #endregion

        #region そのPhotonクライアントのプレイヤー名
        string PlayerName(Photon.Realtime.Player player)
        {
            #region 対人戦
            if (!GManager.instance.IsAI)
            {
                Hashtable hashtable = player.CustomProperties;

                if (HasPlayerName(player))
                {
                    if (hashtable.TryGetValue(ContinuousController.PlayerNameKey, out object value))
                    {
                        string playerName = (string)value;

                        Debug.Log($"playername:{playerName}");
                        return playerName;
                    }
                }

                Debug.Log($"playername:無");
                return "";
            }
            #endregion

            #region デバッグモード
            else
            {
                #region プレイヤーのデッキ
                if (player == MasterPlayer)
                {
                    return ContinuousController.instance.PlayerName;
                }
                #endregion

                #region AIのデッキ
                else
                {
                    return "AI";
                }
                #endregion

            }
            #endregion

            #region そのPhotonクライアントがプレイヤー名のカスタムプロパティを持っているか判定
            bool HasPlayerName(Photon.Realtime.Player _player)
            {
                Hashtable _hashtable = _player.CustomProperties;

                if (_hashtable.TryGetValue(ContinuousController.PlayerNameKey, out object value))
                {
                    string playerName = (string)value;

                    if (!string.IsNullOrEmpty(playerName))
                    {
                        if (playerName.Length <= ContinuousController.instance.PlayerNameMaxLength)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
            #endregion
        }
        #endregion

        #region Playerクラスにプレイヤー名を格納・UI表示
        void SetPlayerName(int _PlayerID, string _PlayerName)
        {
            Player player = gameContext.PlayerFromID(_PlayerID);
            player.PlayerName = _PlayerName;

            player.OffPlayerName();

            //player.PlayerNameText.transform.parent.gameObject.SetActive(true);
            player.PlayerNameText.gameObject.SetActive(true);
            player.PlayerNameText.text = player.PlayerName;
        }
        #endregion
        #endregion

        //yield return GManager.instance.photonWaitController.StartWait("WaitStartSetRandom");

        Debug.Log("乱数初期値設定開始");

        if (PhotonNetwork.IsMasterClient)
        {
            ContinuousController.instance.GetComponent<PhotonView>().RPC("SetRandom", RpcTarget.All, RandomUtility.getRamdom());
        }

        yield return new WaitWhile(() => !ContinuousController.instance.DoneSetRandom);
        ContinuousController.instance.DoneSetRandom = false;

        yield return GManager.instance.photonWaitController.StartWait("EndSetRandom");

        Debug.Log("デッキカード生成開始");
        yield return StartCoroutine(CardObjectController.CreatePlayerDecks(GManager.instance.CardPrefab, gameContext));
        Debug.Log("デッキカード生成終了");
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(GameStateMachine());

        GManager.instance.GetComponent<Effects>().Init();

        yield return StartCoroutine(GManager.instance.LoadingObject.EndLoading());
        Debug.Log("初期化処理終了");
    }
    #endregion

    #region ターンの進行を管理
    int maxLoopCount = 1;
    int LoopCount = 0;
    public IEnumerator GameStateMachine()
    {
        yield return StartCoroutine(StartGame());

        while (true)
        {
            if (endGame)
            {
                yield break;
            }

            gameContext.SwitchTurnPlayer();

            maxLoopCount = 1;
            LoopCount = 0;
            

            while(LoopCount < maxLoopCount)
            {
                yield return StartCoroutine(StartTrun());

                yield return StartCoroutine(UnTapPhase());

                yield return StartCoroutine(MarchPhase());

                yield return StartCoroutine(DrawPhase());

                yield return StartCoroutine(BondPhase());

                yield return StartCoroutine(CountBondPhase());

                yield return StartCoroutine(DeployPhase());

                LoopCount++;
            }
            

            yield return StartCoroutine(ActionPhase());

            if (endGame)
            {
                yield break;
            }

            yield return StartCoroutine(EndPhase());
        }
    }
    #endregion

    #region ゲーム開始時の処理
    public bool DoseStartGame { get; set; } = false;

    bool isDraw = false;
    bool endSelect = false;
    IEnumerator StartGame()
    {
        yield return GManager.instance.photonWaitController.StartWait("StartGame");

        #region 主人公を設置
        List<Player> _players = new List<Player>();
        _players.Add(GManager.instance.You);
        _players.Add(GManager.instance.Opponent);

        foreach (Player player in _players)
        {
            if(player.isYou)
            {
                Debug.Log("あなたの主人公を設置");
            }

            else
            {
                Debug.Log("相手の主人公を設置");
            }
            
            CardSource LordCardSource = null;

            foreach(CardSource cardSource in player.LibraryCards)
            {
                if(cardSource.cEntity_Base == player.LordCard)
                {
                    LordCardSource = cardSource;
                }
            }

            player.LibraryCards.Remove(LordCardSource);

            yield return StartCoroutine(new IPlayUnit(LordCardSource, null, true,false,null,false).PlayUnit());

            player.Lord = player.FieldUnit[0];
        }
        #endregion

        #region 先攻後攻の決定
        gameContext.TurnPlayer = gameContext.PlayerFromID(UnityEngine.Random.Range(0, 2));
        Debug.Log("先攻後攻決定");
        #endregion

        #region 手札を引く
        foreach (Player player in gameContext.Players)
        {
            if (player.isYou)
            {
                Debug.Log("あなたの初手");
            }

            else
            {
                Debug.Log("相手の初手");
            }
            yield return StartCoroutine(new IDraw(player, 6).Draw());
        }
        #endregion

        #region マリガン
        List<Player> players = new List<Player>();

        foreach (Player player in gameContext.Players)
        {
            if(player == gameContext.NonTurnPlayer)
            {
                players.Add(player);
            }
        }

        foreach (Player player in gameContext.Players)
        {
            if (player == gameContext.TurnPlayer)
            {
                players.Add(player);
            }
        }

        foreach (Player player in players)
        {
            isDraw = false;
            endSelect = false;

            yield return GManager.instance.photonWaitController.StartWait($"Mulligun{gameContext.Players.IndexOf(player)}");

            if (!player.isYou)
            {
                GManager.instance.commandText.OpenCommandText("The opponent is selecting if redraw.");
            }

            else
            {
                GManager.instance.commandText.OpenCommandText("Select if redraw.");
            }

            if (player.isYou)
            {
                yield return StartCoroutine(GManager.instance.selectCardPanel.OpenSelectCardPanel(
                    Message: "Do you redraw your hand?",
                    NotSelectButtonMessage: "Not Redraw",
                    EndSelectButtonMessage: "Redraw",
                    _OnClickNotSelectButtonAction: () => SetRedraw_RPC(false),
                    _OnClickEndSelectButtonAction: () => SetRedraw_RPC(true),
                    RootCardSources: player.HandCards,
                    _CanTargetCondition: (cardSource) => false,
                    _CanTargetCondition_ByPreSelecetedList: null,
                    _CanEndSelectCondition: null,
                    _MaxCount: 0,
                     _CanEndNotMax: true,
                    _CanNoSelect: () => true,
                    CanLookReverseCard: true,
                    skillInfos: null));

                void SetRedraw_RPC(bool _isDraw)
                {
                    photonView.RPC("SetRedraw", RpcTarget.All,_isDraw);
                }
            }

            else
            {
                if(GManager.instance.IsAI)
                {
                    //主人公のレベルアップ先を引いているなら
                    if(player.HandCards.Count((cardSource) => cardSource.CanLevelUpFromTargetUnit(player.Lord)&&cardSource.cEntity_Base.Power > player.Lord.Character.cEntity_Base.Power) > 0)
                    {
                        SetRedraw(false);
                    }

                    else
                    {
                        SetRedraw(true);
                    }
                    
                }
            }

            yield return new WaitWhile(() => !endSelect);
            endSelect = false;

            GManager.instance.selectCardPanel.CloseSelectCardPanel();

            GManager.instance.commandText.CloseCommandText();
            yield return new WaitWhile(() => GManager.instance.commandText.gameObject.activeSelf);

            if (isDraw)
            {
                #region マリガン
                #region HandCardを削除
                List<HandCard> HandCardObjects = new List<HandCard>();

                foreach (HandCard handCard in player.HandCardObjects)
                {
                    HandCardObjects.Add(handCard);
                }

                for (int i = 0; i < HandCardObjects.Count; i++)
                {
                    Destroy(HandCardObjects[i].gameObject);
                }
                #endregion

                #region CardSourceをLibraryに移動
                List<CardSource> HandCards = new List<CardSource>();

                foreach(CardSource cardSource in player.HandCards)
                {
                    HandCards.Add(cardSource);
                }

                foreach(CardSource cardSource in HandCards)
                {
                    player.HandCards.Remove(cardSource);
                    player.LibraryCards.Add(cardSource);
                }
                #endregion

                #region シャッフル
                yield return ContinuousController.instance.StartCoroutine(CardObjectController.Shuffle(player));
                #endregion

                #region 6枚引き直す
                yield return StartCoroutine(new IDraw(player, 6).Draw());
                #endregion

                #endregion
            }
        }
        #endregion

        #region オーブに置く
        foreach (Player player in gameContext.Players)
        {
            yield return StartCoroutine(new IAddOrbFromLibrary(player, 5).AddOrb());
        }
        #endregion

        #region 主人公を表にする
        foreach (Player player in gameContext.Players)
        {
            player.GetFrontUnits()[0].Character.IsReverse = false;
        }
        #endregion

        DoseStartGame = true;
    }

    [PunRPC]
    void SetRedraw(bool _isDraw)
    {
        isDraw = _isDraw;
        endSelect = true;
    }
    #endregion

    #region ターン開始時の処理
    IEnumerator StartTrun()
    {
        yield return null;

        foreach (Player player in gameContext.Players)
        {
            player.BondConsumed = 0;
            player.bondObject.ResetBondObject();
        }

        gameContext.NonTurnPlayer.bondObject.GetComponent<Image>().color = new Color32(207, 207, 207, 255);

        gameContext.TurnPlayer.bondObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);

        GManager.instance.showTurnPlayerObject.ShowTurnPlayer(gameContext.TurnPlayer);

        GManager.instance.nextPhaseButton.SwitchTurnSprite();

        yield return new WaitWhile(() => !GManager.instance.showTurnPlayerObject.isClose);

        #region ターン開始時の効果を処理
        List<SkillInfo> skillInfos = new List<SkillInfo>();

        foreach (Player player in gameContext.Players_ForTurnPlayer)
        {
            foreach (Unit unit in player.FieldUnit)
            {
                foreach (ICardEffect cardEffect in unit.EffectList(EffectTiming.OnStartTurn))
                {
                    if (cardEffect is ActivateICardEffect)
                    {
                        if (cardEffect.CanUse(null))
                        {
                            skillInfos.Add(new SkillInfo(cardEffect,null));
                        }
                    }
                }
            }
        }

        yield return ContinuousController.instance.StartCoroutine(GManager.instance.availableMultipleSkills.ActivateMultipleSkills(skillInfos));
        #endregion
    }
    #endregion

    #region アンタップフェイズ
    IEnumerator UnTapPhase()
    {
        isSync = true;
        gameContext.TurnPhase = GameContext.phase.UnTap;
        Debug.Log($"{gameContext.TurnPlayer}:アンタップフェイズ");
        yield return GManager.instance.photonWaitController.StartWait("UnTapPhase");
        isSync = false;

        bool doUntap = false;

        foreach (Unit unit in gameContext.TurnPlayer.FieldUnit)
        {
            if (unit.IsTapped)
            {
                doUntap = true;
                yield return ContinuousController.instance.StartCoroutine(unit.UnTap(null));
            }
        }

        if (doUntap)
        {
            yield return new WaitForSeconds(0.2f);
        }
    }
    #endregion

    #region 進軍フェイズ
    IEnumerator MarchPhase()
    {
        isSync = true;
        gameContext.TurnPhase = GameContext.phase.March;
        Debug.Log($"{gameContext.TurnPlayer}:進軍フェイズ");
        yield return GManager.instance.photonWaitController.StartWait("March");
        isSync = false;

        yield return ContinuousController.instance.StartCoroutine(March.CheckMarch());
    }
    #endregion

    #region ドローフェイズ
    IEnumerator DrawPhase()
    {
        isSync = true;
        gameContext.TurnPhase = GameContext.phase.Draw;
        Debug.Log($"{gameContext.TurnPlayer}:ドローフェイズ");
        yield return GManager.instance.photonWaitController.StartWait("DrawPhase");
        isSync = false;

        if(!isFirstPlayerFirstTurn)
        {
            yield return StartCoroutine(new IDraw(gameContext.TurnPlayer, 1).Draw());
        }
    }
    #endregion

    #region 絆フェイズ
    IEnumerator BondPhase()
    {
        isSync = true;
        gameContext.TurnPhase = GameContext.phase.Bond;
        Debug.Log($"{gameContext.TurnPlayer}:絆フェイズ");
        yield return GManager.instance.photonWaitController.StartWait("BondPhase");
        isSync = false;
        IsSelecting = false;

        #region 絆セット待機状態

        #region 操作待機
        {
            #region 自ターン中
            if (gameContext.You == gameContext.TurnPlayer)
            {
                #region 手札のカードに絆として置くドラッグ操作を追加
                List<HandCard> PreSelectHandCard = new List<HandCard>();

                if (gameContext.TurnPlayer.HandCards.Count == 0)
                {
                    photonView.RPC("NextPhase", RpcTarget.All);
                }

                else
                {
                    //GManager.instance.ShowReverseUnit.SetActive(true);
                    GManager.instance.commandText.OpenCommandText("Bond Phase : Select a card to set to bond.");

                    foreach (HandCard handCard in gameContext.TurnPlayer.HandCardObjects)
                    {
                        handCard.RemoveDragTarget();

                        if(handCard.cardSource.CanSetBondThisCard)
                        {
                            handCard.AddDragTarget(null,OnDropBondCard, OnDragBondCard);

                            #region ドロップ時の処理
                            void OnDropBondCard(List<DropArea> dropAreas)
                            {
                                #region 絆セット
                                bool isOnHand = dropAreas.Count((dropArea) => dropArea.IsChild(GManager.instance.You.HandDropArea.gameObject)) > 0;

                                #region 手札領域でドロップしていない場合
                                if (!isOnHand)
                                {
                                    #region 絆ゾーン領域でドロップしていた場合絆セット
                                    if (dropAreas.Count((dropArea) => dropArea.IsChild(GManager.instance.You.bondObject.gameObject)) > 0)
                                    {
                                        #region 手札のカードの選択待機状態を解除
                                        OffHandCardTarget(gameContext.TurnPlayer);
                                        #endregion

                                        #region 絆ゾーンにセット
                                        photonView.RPC("SetBond", RpcTarget.All, handCard.cardSource.cardIndex);
                                        #endregion

                                        GManager.instance.You.bondObject.SetDefaultSize();

                                        return;
                                    }
                                    #endregion
                                }
                                #endregion

                                handCard.GetComponent<Draggable_HandCard>().ReturnDefaultPosition();

                                #endregion
                            }
                            #endregion

                            #region ドラッグ中の処理
                            void OnDragBondCard(List<DropArea> dropAreas)
                            {
                                bool isOnHand = dropAreas.Count((dropArea) => dropArea.IsChild(GManager.instance.You.HandDropArea.gameObject)) > 0;

                                handCard.SetBlueOutline();

                                GManager.instance.You.bondObject.SetDefaultSize();

                                if (!isOnHand)
                                {
                                    if (dropAreas.Count((dropArea) => dropArea.IsChild(GManager.instance.You.bondObject.gameObject)) > 0)
                                    {
                                        handCard.SetOrangeOutline();

                                        GManager.instance.You.bondObject.SetExpandSize();
                                    }
                                }
                            }
                            #endregion
                        }

                    }
                }

                #endregion
            }
            #endregion

            #region 相手ターン中
            else
            {
                GManager.instance.commandText.OpenCommandText("Bond Phse : The opponent is selecting a card.");
            }
            #endregion
        }
        #endregion

        #region デバッグモード
        if (GManager.instance.IsAI)
        {
            if (!gameContext.TurnPlayer.isYou)
            {
                if (gameContext.TurnPlayer.HandCards.Count((cardSource) => cardSource.CanSetBondThisCard) > 0)
                {
                    bool setBond = false;

                    if(gameContext.TurnPlayer.HandCards.Count > 0)
                    {
                        if(gameContext.TurnPlayer.BondCards.Count < 6)
                        {
                            if(RandomUtility.IsSucceedProbability(0.9f))
                            {
                                setBond = true;
                            }
                        }
                    }

                    if(setBond)
                    {
                        CardSource bondCard = null;
                        foreach(CardSource cardSource in gameContext.TurnPlayer.HandCards)
                        {
                            if(cardSource.CanLevelUpFromTargetUnit(gameContext.TurnPlayer.Lord) || !cardSource.CanSetBondThisCard)
                            {
                                continue;
                            }

                            bondCard = cardSource;
                            break;
                        }

                        if(bondCard != null)
                        {
                            SetBond(bondCard.cardIndex);
                        }

                        else
                        {
                            gameContext.TurnPhase = GameContext.phase.CountBond;
                        }
                    }

                    else
                    {
                        gameContext.TurnPhase = GameContext.phase.CountBond;
                    }
                }

                else
                {
                    gameContext.TurnPhase = GameContext.phase.CountBond;
                }
                
            }
        }
        #endregion

        yield return new WaitWhile(() => gameContext.TurnPhase == GameContext.phase.Bond);
        GManager.instance.You.bondObject.SetDefaultSize();
        GManager.instance.selectCommandPanel.CloseSelectCommandPanel();
        #endregion
    }

    [PunRPC]
    public void SetBond(int _cardIndex)
    {
        StartCoroutine(SetBondCoroutine(_cardIndex));
    }

    IEnumerator SetBondCoroutine(int _cardIndex)
    {
        CardSource cardSource = GManager.instance.turnStateMachine.gameContext.ActiveCardList[_cardIndex];
        IsSelecting = true;
        GManager.instance.commandText.CloseCommandText();
        yield return new WaitWhile(() => GManager.instance.commandText.gameObject.activeSelf);

        yield return StartCoroutine(cardSource.cardOperation.SetBondFromHand(true));
        ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(new List<CardSource>() { cardSource }, "Added Bond Card", true));
        NextPhase();
    }
    #endregion

    #region 絆カウントフェイズ
    IEnumerator CountBondPhase()
    {
        foreach (Player player in gameContext.Players)
        {
            OffHandCardTarget(player);
        }

        GManager.instance.commandText.CloseCommandText();
        yield return new WaitWhile(() => GManager.instance.commandText.gameObject.activeSelf);

        isSync = true;
        gameContext.TurnPhase = GameContext.phase.CountBond;
        Debug.Log($"{gameContext.TurnPlayer}:絆カウントフェイズ");
        yield return GManager.instance.photonWaitController.StartWait("CountBondPhase");
        isSync = false;

        gameContext.TurnPlayer.SetBond();

        yield return StartCoroutine(gameContext.TurnPlayer.bondObject.CountBondPhaseCountUpMP(gameContext.TurnPlayer));
    }
    #endregion

    #region 出撃フェイズ
    IEnumerator DeployPhase()
    {
        #region 手札・場のカードの選択待機状態を解除
        OffHandCardTarget(gameContext.TurnPlayer);
        OffFieldCardTarget(gameContext.TurnPlayer);
        #endregion

        isSync = true;
        gameContext.TurnPhase = GameContext.phase.Deploy;
        Debug.Log($"{gameContext.TurnPlayer}:出撃フェイズ");
        yield return GManager.instance.photonWaitController.StartWait("DeployPhase");
        isSync = false;

        #region 出撃フェイズ開始時の効果
        List<SkillInfo> skillInfos = new List<SkillInfo>();

        foreach (Player player in gameContext.Players_ForTurnPlayer)
        {
            foreach (Unit unit in player.FieldUnit)
            {
                foreach (ICardEffect cardEffect in unit.EffectList(EffectTiming.OnStartDeployPhase))
                {
                    if (cardEffect is ActivateICardEffect)
                    {
                        if (cardEffect.CanUse(null))
                        {
                            skillInfos.Add(new SkillInfo(cardEffect, null));
                            //yield return ContinuousController.instance.StartCoroutine(((ActivateICardEffect)cardEffect).Activate_Effect_Optional_Cost_Execute(null));
                        }
                    }
                }
            }
        }

        yield return ContinuousController.instance.StartCoroutine(GManager.instance.availableMultipleSkills.ActivateMultipleSkills(skillInfos));
        #endregion

        yield return StartCoroutine(AddUseHandCard());

        #region デバッグモード
        if (GManager.instance.IsAI)
        {
            if (!gameContext.TurnPlayer.isYou)
            {
                bool endMainPhase = false;

                while (!endMainPhase)
                {

                StartSelectDeploy:;

                    yield return null;

                    if (RandomUtility.IsSucceedProbability(0.93f) && !isFirstPlayerFirstTurn)
                    {
                        if (gameContext.TurnPlayer.HandCards.Count((cardSource) => cardSource.CanUseCard()) > 0)
                        {
                            #region 主人公をレベルアップ
                            foreach (CardSource cardSource in gameContext.TurnPlayer.HandCards)
                            {
                                if(cardSource.CanUseCard() && cardSource.CanLevelUpFromTargetUnit(gameContext.TurnPlayer.Lord) && cardSource.cEntity_Base.Power > gameContext.TurnPlayer.Lord.Character.cEntity_Base.Power)
                                {
                                    yield return StartCoroutine(PlayUnitCoroutine(cardSource.cardIndex, gameContext.TurnPlayer.FieldUnit.IndexOf(gameContext.TurnPlayer.Lord), false));
                                    goto StartSelectDeploy;
                                }
                            }
                            #endregion

                            #region 主人公以外のユニットをCC
                            foreach (Unit unit in gameContext.TurnPlayer.FieldUnit)
                            {
                                if(unit != gameContext.TurnPlayer.Lord)
                                {
                                    foreach (CardSource cardSource in gameContext.TurnPlayer.HandCards)
                                    {
                                        if (cardSource.CanUseCard() && cardSource.CanCCFromTargetUnit(unit) && cardSource.cEntity_Base.Power > unit.Character.cEntity_Base.Power)
                                        {
                                            yield return StartCoroutine(PlayUnitCoroutine(cardSource.cardIndex, gameContext.TurnPlayer.FieldUnit.IndexOf(unit), false));
                                            goto StartSelectDeploy;
                                        }
                                    }
                                }
                            }
                            #endregion

                            #region 新規プレイ
                            List<CardSource> CanPlayNewUnitCards = new List<CardSource>();

                            foreach (CardSource cardSource in gameContext.TurnPlayer.HandCards)
                            {
                                if (cardSource.CanUseCard() && cardSource.CanPlayAsNewUnit())
                                {
                                    CanPlayNewUnitCards.Add(cardSource);
                                }
                            }

                            if(CanPlayNewUnitCards.Count > 0)
                            {
                                foreach (CardSource cardSource in CanPlayNewUnitCards.OrderByDescending(value => value.cEntity_Base.Power).ToList())
                                {
                                    if (RandomUtility.IsSucceedProbability(0.9f))
                                    {
                                        bool isFront = true;

                                        if (cardSource.Range.Count((range) => range >= 2) > 0)
                                        {
                                            isFront = false;
                                        }

                                        else if (cardSource.Range.Count == 0)
                                        {
                                            isFront = false;
                                        }

                                        yield return StartCoroutine(PlayUnitCoroutine(cardSource.cardIndex, -1, isFront));
                                        goto StartSelectDeploy;
                                    }
                                }
                            }
                            #endregion
                        }
                    }

                    else
                    {
                        endMainPhase = true;
                    }
                }

                gameContext.TurnPhase = GameContext.phase.Action;
            }
        }
            #endregion

        yield return new WaitWhile(() => gameContext.TurnPhase == GameContext.phase.Deploy);

        #region 手札・場のカードの選択待機状態を解除
        OffHandCardTarget(gameContext.TurnPlayer);
        OffFieldCardTarget(gameContext.TurnPlayer);
        #endregion

        GManager.instance.commandText.CloseCommandText();
        yield return new WaitWhile(() => GManager.instance.commandText.gameObject.activeSelf);
    }

    #region 手札のカードを使用待機状態にする
    public IEnumerator AddUseHandCard()
    {
        if (gameContext.TurnPhase == GameContext.phase.Deploy)
        {
            yield return GManager.instance.photonWaitController.StartWait("AddUseHandCard");

            if (gameContext.You == gameContext.TurnPlayer)
            {
                #region リセット
                List<HandCard> handCards = new List<HandCard>();

                foreach (CardSource cardSource in GManager.instance.You.HandCards)
                {
                    handCards.Add(cardSource.ShowingHandCard);
                }

                GManager.instance.You.HandTransform.GetComponent<GridLayoutGroup>().enabled = false;

                foreach (HandCard handCard in handCards)
                {
                    handCard.transform.SetParent(GManager.instance.You.HandTransform);
                }

                GManager.instance.You.HandTransform.GetComponent<GridLayoutGroup>().enabled = true;

                foreach (Player player in gameContext.Players)
                {
                    foreach(FieldUnitCard fieldUnitCard in player.FieldUnitObjects)
                    {
                        fieldUnitCard.Outline_Select.gameObject.SetActive(false);
                    }
                }

                IsSelecting = false;

                OffFieldCardTarget(gameContext.TurnPlayer);
                OffHandCardTarget(gameContext.TurnPlayer);
                #endregion

                if (gameContext.TurnPlayer.HandCards.Count((_card) => _card.CanUseCard()) == 0)
                {
                    photonView.RPC("NextPhase", RpcTarget.All);
                }

                else
                {
                    GManager.instance.commandText.OpenCommandText("Deploy Phase : Select a card in your hand to deploy.");

                    foreach (HandCard handCard in gameContext.TurnPlayer.HandCardObjects)
                    {
                        AddUseCard(handCard);
                    }
                }
            }

            else
            {
                GManager.instance.commandText.OpenCommandText("Deploy Phase : The opponent is selecting a card.");
            }
        }
    }

    public void AddUseCard(HandCard handCard)
    {
        handCard.RemoveDragTarget();

        if (handCard.cardSource.CanUseCard())
        {
            handCard.AddDragTarget(BeginDrag,OnDropCard, OnDragCard);

            #region ドラッグ開始時の処理
            void BeginDrag(HandCard handCard1)
            {
                foreach (FieldUnitCard fieldUnitCard in GManager.instance.You.FieldUnitObjects)
                {
                    fieldUnitCard.RemoveSelectEffect();
                    fieldUnitCard.Outline_Select.gameObject.SetActive(false);
                }

                GManager.instance.You.PlayMat_Back_Select.gameObject.SetActive(false);
                GManager.instance.You.PlayMat_Front_Select.gameObject.SetActive(false);

                foreach (FieldUnitCard fieldUnitCard in GManager.instance.You.FieldUnitObjects)
                {
                    if (handCard1.cardSource.CanUseTargetUnit(fieldUnitCard.thisUnit))
                    {
                        fieldUnitCard.RemoveSelectEffect();
                        fieldUnitCard.Outline_Select.gameObject.SetActive(true);
                        fieldUnitCard.SetBlueOutline();
                    }

                    else
                    {
                        fieldUnitCard.RemoveSelectEffect();
                    }
                }

                if (handCard.cardSource.CanPlayAsNewUnit())
                {
                    GManager.instance.You.PlayMat_Back_Select.gameObject.SetActive(true);
                    GManager.instance.You.PlayMat_Back_Select.color = DataBase.SelectColor_Blue;
                    GManager.instance.You.PlayMat_Front_Select.gameObject.SetActive(true);
                    GManager.instance.You.PlayMat_Front_Select.color = DataBase.SelectColor_Blue;
                }
            }
            #endregion

            #region ドラッグ終了時の処理
            void OnDropCard(List<DropArea> dropAreas)
            {
                foreach (FieldUnitCard fieldUnitCard in gameContext.TurnPlayer.FieldUnitObjects)
                {
                    fieldUnitCard.RemoveSelectEffect();
                    fieldUnitCard.Outline_Select.gameObject.SetActive(false);
                }

                GManager.instance.You.PlayMat_Back_Select.gameObject.SetActive(false);
                GManager.instance.You.PlayMat_Front_Select.gameObject.SetActive(false);

                #region ユニットカードをプレイ

                // bool isOnUnit = false;

                bool isOnHand = dropAreas.Count((dropArea) => dropArea.IsChild(GManager.instance.You.HandDropArea.gameObject)) > 0;

                #region 手札領域でドロップしていない場合
                if (!isOnHand)
                {
                    #region 場のユニットでドロップしているかチェック、レベルアップ・CCできるなら重ねてプレイ
                    foreach (DropArea dropArea in dropAreas)
                    {
                        foreach (FieldUnitCard fieldUnitCard in gameContext.TurnPlayer.FieldUnitObjects)
                        {
                            if (dropArea.IsChild(fieldUnitCard.gameObject))
                            {
                                if(handCard.cardSource.CanUseTargetUnit(fieldUnitCard.thisUnit))
                                {
                                    #region 手札のカードの選択待機状態を解除
                                    OffHandCardTarget(gameContext.TurnPlayer);
                                    #endregion

                                    photonView.RPC("PlayUnit_RPC", RpcTarget.All, handCard.cardSource.cardIndex, gameContext.TurnPlayer.FieldUnit.IndexOf(fieldUnitCard.thisUnit),false);

                                    foreach (FieldUnitCard _fieldUnitCard in GManager.instance.You.FieldUnitObjects)
                                    {
                                        _fieldUnitCard.RemoveSelectEffect();
                                    }

                                    return;
                                }

                                //isOnUnit = true;
                            }
                        }
                    }
                    #endregion

                    #region 新規プレイをチェック
                    //if (!isOnUnit)
                    {
                        #region 新規プレイできるなら
                        if (handCard.cardSource.CanPlayAsNewUnit())
                        {
                            #region フィールド領域でドロップしていた場合新規にプレイ
                            if (dropAreas.Count((dropArea) => dropArea.IsChild(GManager.instance.You.PlayMat_Front) || dropArea.IsChild(GManager.instance.You.PlayMat_Back)) > 0)
                            {
                                #region 手札のカードの選択待機状態を解除
                                OffHandCardTarget(gameContext.TurnPlayer);
                                #endregion

                                if (dropAreas.Count((dropArea) => dropArea.IsChild(GManager.instance.You.PlayMat_Front)) > 0)
                                {
                                    photonView.RPC("PlayUnit_RPC", RpcTarget.All, handCard.cardSource.cardIndex, 1000000, true);
                                }

                                else if (dropAreas.Count((dropArea) => dropArea.IsChild(GManager.instance.You.PlayMat_Back)) > 0)
                                {
                                    photonView.RPC("PlayUnit_RPC", RpcTarget.All, handCard.cardSource.cardIndex, 1000000, false);
                                }

                                foreach (FieldUnitCard fieldUnitCard in GManager.instance.You.FieldUnitObjects)
                                {
                                    fieldUnitCard.RemoveSelectEffect();
                                }

                                return;
                            }
                            #endregion
                        }
                        #endregion

                    }
                    #endregion
                }
                #endregion

                handCard.GetComponent<Draggable_HandCard>().ReturnDefaultPosition();

                #endregion
            }
            #endregion

            #region ドラッグ中の処理
            void OnDragCard(List<DropArea> dropAreas)
            {
                bool isOnHand = dropAreas.Count((dropArea) => dropArea.IsChild(GManager.instance.You.HandDropArea.gameObject)) > 0;

                FieldUnitCard selectingFieldUnitCard = null;

                handCard.SetBlueOutline();

                if (!isOnHand)
                {
                    foreach (DropArea dropArea in dropAreas)
                    {
                        foreach (FieldUnitCard fieldUnitCard in GManager.instance.You.FieldUnitObjects)
                        {
                            if (dropArea.IsChild(fieldUnitCard.gameObject))
                            {
                                if (handCard.cardSource.CanUseTargetUnit(fieldUnitCard.thisUnit))
                                {
                                    selectingFieldUnitCard = fieldUnitCard;

                                    break;
                                }
                            }
                        }
                    }

                    if (dropAreas.Count((dropArea) => dropArea.IsChild(GManager.instance.You.PlayMat)) > 0)
                    {
                        if (selectingFieldUnitCard != null)
                        {
                            if (handCard.cardSource.CanUseTargetUnit(selectingFieldUnitCard.thisUnit))
                            {
                                handCard.SetOrangeOutline();
                            }
                        }

                        else
                        {
                            if(handCard.cardSource.CanPlayAsNewUnit())
                            {
                                handCard.SetOrangeOutline();
                            }
                        }
                    }
                }

                foreach (FieldUnitCard fieldUnitCard in GManager.instance.You.FieldUnitObjects)
                {
                    if (handCard.cardSource.CanUseTargetUnit(fieldUnitCard.thisUnit))
                    {
                        if (selectingFieldUnitCard != null)
                        {
                            if (fieldUnitCard != selectingFieldUnitCard)
                            {
                                fieldUnitCard.RemoveSelectEffect();
                                fieldUnitCard.Outline_Select.gameObject.SetActive(true);
                                fieldUnitCard.SetBlueOutline();
                            }

                            else
                            {
                                fieldUnitCard.OnSelectEffect(1.2f);
                            }

                        }

                        else
                        {
                            fieldUnitCard.RemoveSelectEffect();
                            fieldUnitCard.Outline_Select.gameObject.SetActive(true);
                            fieldUnitCard.SetBlueOutline();
                        }
                    }

                    else
                    {
                        fieldUnitCard.RemoveSelectEffect();
                    }
                }

                if (handCard.cardSource.CanPlayAsNewUnit())
                {
                    if (dropAreas.Count((dropArea) => dropArea.IsChild(GManager.instance.You.PlayMat_Front)) > 0)
                    {
                        GManager.instance.You.PlayMat_Front_Select.color = DataBase.SelectColor_Orange;
                        GManager.instance.You.PlayMat_Back_Select.color = DataBase.SelectColor_Blue;
                    }

                    else if (dropAreas.Count((dropArea) => dropArea.IsChild(GManager.instance.You.PlayMat_Back)) > 0)
                    {
                        GManager.instance.You.PlayMat_Front_Select.color = DataBase.SelectColor_Blue;
                        GManager.instance.You.PlayMat_Back_Select.color = DataBase.SelectColor_Orange;
                    }

                    else
                    {
                        GManager.instance.You.PlayMat_Front_Select.color = DataBase.SelectColor_Blue;
                        GManager.instance.You.PlayMat_Back_Select.color = DataBase.SelectColor_Blue;
                    }
                }
            }
            #endregion
        }
    }

    [PunRPC]
    public void PlayUnit_RPC(int cardIndex, int unitIndex,bool isFront)
    {
        StartCoroutine(PlayUnitCoroutine(cardIndex, unitIndex,isFront));
    }

    IEnumerator PlayUnitCoroutine(int cardIndex, int unitIndex, bool isFront)
    {
        //GManager.instance.ShowDrag.SetActive(false);
        foreach (Player player in gameContext.Players)
        {
            foreach (FieldUnitCard fieldUnitCard in player.FieldUnitObjects)
            {
                fieldUnitCard.Outline_Select.gameObject.SetActive(false);
            }
        }

        GManager.instance.commandText.CloseCommandText();
        yield return new WaitWhile(() => GManager.instance.commandText.gameObject.activeSelf);

        IsSelecting = true;

        CardSource cardSource = gameContext.ActiveCardList[cardIndex];

        //絆データを減らす
        cardSource.cardOperation.ReduceBond(unitIndex);

        //絆のUI表示を変更
        StartCoroutine(cardSource.Owner.bondObject.CountDownBond(cardSource.Owner));

        //ソウルを場に出す処理
        yield return StartCoroutine(cardSource.cardOperation.PlayUnit(unitIndex,isFront));

        IsSelecting = false;
    }

    #endregion

    #endregion

    #region 行動フェイズ
    public Unit AttackingUnit { get; set; } = null;
    public Unit DefendingUnit { get; set; } = null;
    public Unit MovingUnit { get; set; } = null;
    public Unit UseSkillUnit { get; set; } = null;
    public ICardEffect CardEffect { get; set; } = null;
    public bool isDestroydeByBattle { get; set; } = false;
    public bool endTapAttackingUnit { get; set; } = false;
    public IEnumerator ActionPhase()
    {
        #region 手札・場のカードの選択待機状態を解除
        OffHandCardTarget(gameContext.TurnPlayer);
        OffFieldCardTarget(gameContext.TurnPlayer);
        #endregion

        isSync = true;
        gameContext.TurnPhase = GameContext.phase.Action;
        Debug.Log($"{gameContext.TurnPlayer}:出撃フェイズ");
        yield return GManager.instance.photonWaitController.StartWait("ActionPhase");
        isSync = false;

        #region 攻撃・防御エフェクトを削除
        foreach (Player player in gameContext.Players)
        {
            foreach (FieldUnitCard fieldUnitCard in player.FieldUnitObjects)
            {
                fieldUnitCard.OffAttackerDefenderEffect();
            }
        }
        #endregion

        #region 攻撃・移動・スキル使用可能なユニットが存在する、またはターン終了するまで繰り返し
        while (gameContext.TurnPlayer.FieldUnit.Count((unit) => unit.CanAttack || unit.CanMoveDurinAction || unit.CanDeclareSkill(null)) > 0 && !endGame)
        {
            #region パラメータリセット
            ResetActionPhaseParameter();
            #endregion

            #region 攻撃・移動・スキル使用を使用可能状態にする
            yield return GManager.instance.photonWaitController.StartWait("SetAttackerOrDeclareSkill");

            yield return StartCoroutine(SetActionUnit());

            #region 使用可能にする
            IEnumerator SetActionUnit()
            {
                //Debug.Log("SetAttackerOrDeclareSkill");

                IsSelecting = false;

                #region リセット
                #region フィールドカードをリセット
                foreach (Player player in gameContext.Players)
                {
                    foreach(FieldUnitCard fieldUnitCard in player.FieldUnitObjects)
                    {
                        fieldUnitCard.RemoveDragTarget();
                        fieldUnitCard.OffAttackerDefenderEffect();
                        fieldUnitCard.OffClickTarget();
                        fieldUnitCard.fieldUnitCommandPanel.CloseFieldUnitCommandPanel();

                        if(!fieldUnitCard.thisUnit.Character.Owner.isYou)
                        {
                            fieldUnitCard.RemoveSelectEffect();
                        }

                        else
                        {
                            if (!(fieldUnitCard.thisUnit.CanAttack || fieldUnitCard.thisUnit.CanMoveDurinAction || fieldUnitCard.thisUnit.CanDeclareSkill(null)))
                            {
                                fieldUnitCard.RemoveSelectEffect();
                            }
                        }
                    }
                }
                #endregion

                GManager.instance.You.PlayMat_Front_Select.gameObject.SetActive(false);
                GManager.instance.You.PlayMat_Back_Select.gameObject.SetActive(false);
                //コマンドパネル閉じる
                GManager.instance.selectCommandPanel.CloseSelectCommandPanel();

                //もどるボタン閉じる
                GManager.instance.BackButton.CloseSelectCommandButton();

                //コマンドテキスト閉じる
                GManager.instance.commandText.CloseCommandText();
                yield return new WaitWhile(() => GManager.instance.commandText.gameObject.activeSelf);
                #endregion

                foreach (FieldUnitCard fieldUnitCard in gameContext.TurnPlayer.FieldUnitObjects)
                {
                    if (fieldUnitCard.thisUnit.CanAttack || fieldUnitCard.thisUnit.CanMoveDurinAction || fieldUnitCard.thisUnit.CanDeclareSkill(null))
                    {
                        if (gameContext.TurnPlayer.isYou)
                        {
                            GManager.instance.commandText.OpenCommandText("Action Phase : Select your unit.");

                            fieldUnitCard.AddClickTarget((_fieldUnitCard) => StartCoroutine(OnClick_Select()));

                            fieldUnitCard.OnSelectEffect(1.1f);

                            IEnumerator OnClick_Select()
                            {
                                IsSelecting = true;

                                #region 他のフィールドカードをリセット
                                foreach (FieldUnitCard _fieldUnitCard in gameContext.TurnPlayer.FieldUnitObjects)
                                {
                                    if (_fieldUnitCard != fieldUnitCard)
                                    {
                                        _fieldUnitCard.RemoveSelectEffect();
                                    }
                                }

                                foreach (Player player in gameContext.Players)
                                {
                                    OffFieldCardTarget(player);
                                }
                                #endregion

                                List<FieldUnitCommand> FieldUnitCommands = new List<FieldUnitCommand>();

                                #region 攻撃コマンド
                                FieldUnitCommand AttackCommand = new FieldUnitCommand("Attack", () => StartCoroutine(SelectDefender()), fieldUnitCard.thisUnit.CanAttack);
                                FieldUnitCommands.Add(AttackCommand);

                                IEnumerator SelectDefender()
                                {
                                    foreach(FieldUnitCard fieldUnitCard1 in gameContext.TurnPlayer.FieldUnitObjects)
                                    {
                                        if(fieldUnitCard1 != fieldUnitCard)
                                        {
                                            fieldUnitCard1.RemoveDragTarget();
                                        }
                                    }

                                    fieldUnitCard.fieldUnitCommandPanel.CloseFieldUnitCommandPanel();

                                    GManager.instance.commandText.CloseCommandText();
                                    yield return new WaitWhile(() => GManager.instance.commandText.gameObject.activeSelf);

                                    GManager.instance.commandText.OpenCommandText("Select the opponent's unit to be attacked.");

                                    foreach(FieldUnitCard OpponentFieldUnitCard in gameContext.NonTurnPlayer.FieldUnitObjects)
                                    {
                                        if(fieldUnitCard.thisUnit.CanAttachTargetUnit(OpponentFieldUnitCard.thisUnit))
                                        {
                                            //敵ユニットクリックで防御側決定
                                            OpponentFieldUnitCard.AddClickTarget((_fieldUnitCard) => SetAttackerDefender_RPC());

                                            //選択エフェクト
                                            OpponentFieldUnitCard.OnSelectEffect(1.1f);

                                            void SetAttackerDefender_RPC()
                                            {
                                                #region フィールドカードをリセット
                                                foreach (Player player in gameContext.Players)
                                                {
                                                    foreach (FieldUnitCard _fieldUnitCard in player.FieldUnitObjects)
                                                    {
                                                        _fieldUnitCard.CloseFieldUnitCommandPanel();
                                                        _fieldUnitCard.OffClickTarget();
                                                        _fieldUnitCard.RemoveSelectEffect();
                                                    }
                                                }
                                                #endregion

                                                photonView.RPC("SetAttackerDefender", RpcTarget.All, gameContext.TurnPlayer.FieldUnit.IndexOf(fieldUnitCard.thisUnit), gameContext.NonTurnPlayer.FieldUnit.IndexOf(OpponentFieldUnitCard.thisUnit));
                                            }
                                        }
                                    }

                                    //自ユニットクリックでコマンドパネルを閉じて選択し直しに戻る
                                    fieldUnitCard.AddClickTarget((_fieldUnitCard) => StartCoroutine(SetActionUnit()));

                                    //戻るボタン表示(選択し直しに戻る)
                                    GManager.instance.BackButton.OpenSelectCommandButton("Return", () => StartCoroutine(SetActionUnit()), 0);
                                }
                                #endregion

                                #region 移動コマンド
                                FieldUnitCommand MoveCommand = new FieldUnitCommand("Move", OnClick_SetMovingUnit_RPC, fieldUnitCard.thisUnit.CanMoveDurinAction);
                                FieldUnitCommands.Add(MoveCommand);

                                void OnClick_SetMovingUnit_RPC()
                                {
                                    #region フィールドカードをリセット
                                    foreach (Player player in gameContext.Players)
                                    {
                                        foreach (FieldUnitCard _fieldUnitCard in player.FieldUnitObjects)
                                        {
                                            _fieldUnitCard.CloseFieldUnitCommandPanel();
                                            _fieldUnitCard.OffClickTarget();
                                            _fieldUnitCard.RemoveDragTarget();
                                        }
                                    }
                                    #endregion

                                    photonView.RPC("SetMovingUnit", RpcTarget.All, gameContext.TurnPlayer.FieldUnit.IndexOf(fieldUnitCard.thisUnit));
                                }
                                #endregion

                                #region 起動効果コマンド

                                List<ICardEffect> cardEffects = new List<ICardEffect>();

                                foreach(ICardEffect cardEffect in fieldUnitCard.thisUnit.EffectList(EffectTiming.OnDeclaration))
                                {
                                    cardEffects.Add(cardEffect);
                                }

                                cardEffects.Reverse();

                                foreach (ICardEffect cardEffect in cardEffects)
                                {
                                    if (cardEffect is ActivateICardEffect)
                                    {
                                        FieldUnitCommand SkillCommand = new FieldUnitCommand(cardEffect.EffectName, OnClick_SetUseSkillUnit_RPC, fieldUnitCard.thisUnit.CanDeclareThisSkill(cardEffect,null));
                                        FieldUnitCommands.Add(SkillCommand);

                                        void OnClick_SetUseSkillUnit_RPC()
                                        {
                                            #region フィールドカードをリセット
                                            foreach (Player player in gameContext.Players)
                                            {
                                                foreach (FieldUnitCard _fieldUnitCard in player.FieldUnitObjects)
                                                {
                                                    _fieldUnitCard.CloseFieldUnitCommandPanel();
                                                    _fieldUnitCard.OffClickTarget();
                                                    _fieldUnitCard.RemoveDragTarget();
                                                }
                                            }
                                            #endregion

                                            photonView.RPC("SetUseSkillUnit", RpcTarget.All, gameContext.TurnPlayer.FieldUnit.IndexOf(fieldUnitCard.thisUnit), fieldUnitCard.thisUnit.Character.cEntity_EffectController.GetCardEffects(EffectTiming.OnDeclaration).IndexOf(cardEffect));
                                        }
                                    }
                                }
                                #endregion

                                //コマンドパネルを開く
                                fieldUnitCard.fieldUnitCommandPanel.SetUpFieldUnitCommandPanel(FieldUnitCommands,fieldUnitCard);

                                //再度クリックするとコマンドパネルを閉じて選択し直しに戻る
                                fieldUnitCard.AddClickTarget((_fieldUnitCard) => StartCoroutine(SetActionUnit()));

                                //閉じるボタンを押してもコマンドパネルを閉じて選択し直しに戻る
                                //fieldUnitCard.OnClickCloseFieldUnitCommandPanelButtonAction = () => StartCoroutine(SetActionUnit());

                                yield return null;
                            }

                            AddAttackDragTarget(fieldUnitCard);
                        }

                        else
                        {
                            GManager.instance.commandText.OpenCommandText("Action Phase : The opponent is selecting a unit.");
                        }

                    }
                }
            }
            #endregion

            #region ドラッグターゲット
            void AddAttackDragTarget(FieldUnitCard fieldUnitCard)
            {
                if(fieldUnitCard.thisUnit.CanAttack || fieldUnitCard.thisUnit.CanMoveDurinAction)
                {
                    fieldUnitCard.AddDragTarget(OnBeginDragAction,OnDragAction,OnEndDragAction);

                    void OnBeginDragAction(FieldUnitCard _fieldUnitCard)
                    {
                        if(gameContext.TurnPlayer.FieldUnitObjects.Count((_fieldUnitCard1) => _fieldUnitCard1.fieldUnitCommandPanel.CommandPanel.activeSelf) == 0)
                        {
                            IsSelecting = true;

                            GManager.instance.commandText.OpenCommandText("Select the opponent's unit to be attacked or move.");

                            _fieldUnitCard.CloseFieldUnitCommandPanel();

                            TargetArrow targetArrow = GManager.instance.CreateTargetArrow();

                            targetArrow.SetTargetArrow(fieldUnitCard.GetLocalCanvasPosition(), Draggable.GetLocalPosition(Input.mousePosition, targetArrow.transform));

                            foreach(FieldUnitCard fieldUnitCard1 in gameContext.TurnPlayer.FieldUnitObjects)
                            {
                                if(fieldUnitCard1 != _fieldUnitCard)
                                {
                                    fieldUnitCard1.RemoveSelectEffect();
                                    fieldUnitCard1.OffClickTarget();
                                    fieldUnitCard1.RemoveDragTarget();
                                }
                            }

                            foreach (FieldUnitCard enemyFieldUnitCard in gameContext.NonTurnPlayer.FieldUnitObjects)
                            {
                                if (_fieldUnitCard.thisUnit.CanAttachTargetUnit(enemyFieldUnitCard.thisUnit))
                                {
                                    enemyFieldUnitCard.OnSelectEffect(1.1f);
                                    enemyFieldUnitCard.SetBlueOutline();
                                }
                            }

                            if(_fieldUnitCard.thisUnit.CanMoveDurinAction)
                            {
                                if(_fieldUnitCard.thisUnit.Character.Owner.GetBackUnits().Contains(_fieldUnitCard.thisUnit))
                                {
                                    GManager.instance.You.PlayMat_Front_Select.gameObject.SetActive(true);
                                    GManager.instance.You.PlayMat_Front_Select.color = DataBase.SelectColor_Blue;
                                }

                                else if (_fieldUnitCard.thisUnit.Character.Owner.GetFrontUnits().Contains(_fieldUnitCard.thisUnit))
                                {
                                    GManager.instance.You.PlayMat_Back_Select.gameObject.SetActive(true);
                                    GManager.instance.You.PlayMat_Back_Select.color = DataBase.SelectColor_Blue;
                                }
                            }
                        }
                    }

                    void OnDragAction(FieldUnitCard _fieldUnitCard,List<DropArea> dropAreas)
                    {
                        _fieldUnitCard.CloseFieldUnitCommandPanel();

                        TargetArrow targetArrow = null;

                        for(int i=0;i<GManager.instance.targetArrowParent.childCount;i++)
                        {
                            if(GManager.instance.targetArrowParent.GetChild(i).GetComponent<TargetArrow>() != null && GManager.instance.targetArrowParent.GetChild(i).gameObject.activeSelf)
                            {
                                targetArrow = GManager.instance.targetArrowParent.GetChild(i).GetComponent<TargetArrow>();
                            }
                        }

                        if(targetArrow != null)
                        {
                            targetArrow.SetTargetArrow(fieldUnitCard.GetLocalCanvasPosition(), Draggable.GetLocalPosition(Input.mousePosition, targetArrow.transform));

                            foreach (FieldUnitCard enemyFieldUnitCard in GManager.instance.Opponent.FieldUnitObjects)
                            {
                                if (_fieldUnitCard.thisUnit.CanAttachTargetUnit(enemyFieldUnitCard.thisUnit))
                                {
                                    bool OnSelect = false;

                                    if (dropAreas.Count((dropArea) => dropArea.IsChild(enemyFieldUnitCard.gameObject)) > 0)
                                    {
                                        if (_fieldUnitCard.thisUnit.CanAttachTargetUnit(enemyFieldUnitCard.thisUnit))
                                        {
                                            OnSelect = true;
                                        }
                                    }

                                    if (OnSelect)
                                    {
                                        enemyFieldUnitCard.OnSelectEffect(1.1f);
                                        enemyFieldUnitCard.SetOrangeOutline();
                                    }

                                    else
                                    {
                                        enemyFieldUnitCard.OnSelectEffect(1.1f);
                                        enemyFieldUnitCard.SetBlueOutline();
                                    }
                                }
                            }

                            if (_fieldUnitCard.thisUnit.CanMoveDurinAction)
                            {
                                if (_fieldUnitCard.thisUnit.Character.Owner.GetBackUnits().Contains(_fieldUnitCard.thisUnit))
                                {
                                    if(dropAreas.Count((dropArea) => dropArea.IsChild(GManager.instance.You.PlayMat_Front)) >0)
                                    {
                                        GManager.instance.You.PlayMat_Front_Select.gameObject.SetActive(true);
                                        GManager.instance.You.PlayMat_Front_Select.color = DataBase.SelectColor_Orange;
                                    }

                                    else
                                    {
                                        GManager.instance.You.PlayMat_Front_Select.gameObject.SetActive(true);
                                        GManager.instance.You.PlayMat_Front_Select.color = DataBase.SelectColor_Blue;
                                    }
                                    
                                }

                                else if (_fieldUnitCard.thisUnit.Character.Owner.GetFrontUnits().Contains(_fieldUnitCard.thisUnit))
                                {
                                    if (dropAreas.Count((dropArea) => dropArea.IsChild(GManager.instance.You.PlayMat_Back)) > 0)
                                    {
                                        GManager.instance.You.PlayMat_Back_Select.gameObject.SetActive(true);
                                        GManager.instance.You.PlayMat_Back_Select.color = DataBase.SelectColor_Orange;
                                    }

                                    else
                                    {
                                        GManager.instance.You.PlayMat_Back_Select.gameObject.SetActive(true);
                                        GManager.instance.You.PlayMat_Back_Select.color = DataBase.SelectColor_Blue;
                                    }
                                }
                            }
                        }
                        
                    }

                    void OnEndDragAction(FieldUnitCard _fieldUnitCard, List<DropArea> dropAreas)
                    {
                        IsSelecting = false;

                        TargetArrow targetArrow = null;

                        for (int i = 0; i < GManager.instance.targetArrowParent.childCount; i++)
                        {
                            if (GManager.instance.targetArrowParent.GetChild(i).GetComponent<TargetArrow>() != null && GManager.instance.targetArrowParent.GetChild(i).gameObject.activeSelf)
                            {
                                targetArrow = GManager.instance.targetArrowParent.GetChild(i).GetComponent<TargetArrow>();
                            }
                        }

                        if (targetArrow != null)
                        {
                            GManager.instance.You.PlayMat_Front_Select.gameObject.SetActive(false);
                            GManager.instance.You.PlayMat_Back_Select.gameObject.SetActive(false);

                            _fieldUnitCard.CloseFieldUnitCommandPanel();

                            Destroy(targetArrow.gameObject);

                            foreach (DropArea dropArea in dropAreas)
                            {
                                foreach (FieldUnitCard enemyFieldUnitCard in GManager.instance.Opponent.FieldUnitObjects)
                                {
                                    if (dropArea.IsChild(enemyFieldUnitCard.gameObject))
                                    {
                                        if (_fieldUnitCard.thisUnit.CanAttachTargetUnit(enemyFieldUnitCard.thisUnit))
                                        {
                                            #region フィールドカードをリセット
                                            foreach (Player player in gameContext.Players)
                                            {
                                                foreach (FieldUnitCard _fieldUnitCard1 in player.FieldUnitObjects)
                                                {
                                                    _fieldUnitCard1.CloseFieldUnitCommandPanel();
                                                    _fieldUnitCard1.OffClickTarget();
                                                    _fieldUnitCard1.RemoveSelectEffect();
                                                    _fieldUnitCard1.RemoveDragTarget();
                                                }
                                            }
                                            #endregion

                                            photonView.RPC("SetAttackerDefender", RpcTarget.All, gameContext.TurnPlayer.FieldUnit.IndexOf(_fieldUnitCard.thisUnit), gameContext.NonTurnPlayer.FieldUnit.IndexOf(enemyFieldUnitCard.thisUnit));
                                            return;
                                        }
                                    }
                                }

                                if(dropArea.IsChild(gameContext.TurnPlayer.PlayMat_Front))
                                {
                                    if(gameContext.TurnPlayer.GetBackUnits().Contains(_fieldUnitCard.thisUnit))
                                    {
                                        photonView.RPC("SetMovingUnit", RpcTarget.All, gameContext.TurnPlayer.FieldUnit.IndexOf(_fieldUnitCard.thisUnit));
                                        return;
                                    }
                                }

                                if (dropArea.IsChild(gameContext.TurnPlayer.PlayMat_Back))
                                {
                                    if (gameContext.TurnPlayer.GetFrontUnits().Contains(_fieldUnitCard.thisUnit))
                                    {
                                        photonView.RPC("SetMovingUnit", RpcTarget.All, gameContext.TurnPlayer.FieldUnit.IndexOf(_fieldUnitCard.thisUnit));
                                        return;
                                    }
                                }
                            }

                            StartCoroutine(SetActionUnit());
                        }
                            
                    }
                }
            }
            #endregion

            #endregion

            #region 攻撃・移動・スキル使用ユニット選択待機
            while (AttackingUnit == null && MovingUnit == null && UseSkillUnit == null)
            {
                yield return null;

                if (endGame)
                {
                    yield break;
                }

                #region デバッグモード
                if (GManager.instance.IsAI)
                {
                    if (!gameContext.TurnPlayer.isYou)
                    {
                        if(RandomUtility.IsSucceedProbability(0.08f))
                        {
                            //Debug.Log("低確率で行動フェイズ終了");
                            goto EndActionPhase;
                        }

                        if ((gameContext.TurnPlayer.FieldUnit.Count((unit) => DoAttackTargets(unit).Count > 0) == 0))
                        {
                            if(RandomUtility.IsSucceedProbability(0.8f))
                            {
                                //Debug.Log("攻撃するユニットがいないので終了");
                                goto EndActionPhase;
                            }
                        }

                        if (gameContext.TurnPlayer.FieldUnit.Count((unit) => DoAttackTargets(unit).Count > 0 || unit.CanMoveDurinAction || unit.CanDeclareSkill(null)) == 0)
                        {
                            //Debug.Log("行動できるユニットがいないので終了");
                            goto EndActionPhase;
                        }

                        foreach (Unit unit in gameContext.TurnPlayer.FieldUnit.OrderBy(value => value.Power).ToList())
                        {
                            #region 攻撃
                            if (unit.CanAttack)
                            {
                                if (DoAttackTargets(unit).Count > 0)
                                {
                                    foreach(Unit _unit in DoAttackTargets(unit))
                                    {
                                        //Debug.Log($"{unit.Character}は{_unit.Character}を攻撃対象に入れている");
                                    }

                                    #region 攻撃対象の敵ユニットリストを分類
                                    List<Unit> NonlordTargets = new List<Unit>();

                                    foreach (Unit _unit in DoAttackTargets(unit))
                                    {
                                        if (_unit != GManager.instance.You.Lord)
                                        {
                                            NonlordTargets.Add(_unit);
                                        }
                                    }

                                    List<Unit> OverMyLordUnits = new List<Unit>();
                                    List<Unit> UnderMyLordUnits = new List<Unit>();

                                    foreach (Unit _unit in NonlordTargets)
                                    {
                                        if(_unit.Power >= GManager.instance.Opponent.Lord.Power)
                                        {
                                            OverMyLordUnits.Add(_unit);
                                        }

                                        else
                                        {
                                            UnderMyLordUnits.Add(_unit);
                                        }
                                    }
                                    #endregion

                                    #region 主人公以外の敵が攻撃対象にあれば
                                    if (NonlordTargets.Count > 0)
                                    {
                                        #region 主人公に攻撃を当てる可能性のある敵から優先して攻撃
                                        if (OverMyLordUnits.Count > 0)
                                        {
                                            AttackingUnit = unit;
                                            DefendingUnit = OverMyLordUnits[UnityEngine.Random.Range(0, OverMyLordUnits.Count)];
                                            break;
                                        }
                                        #endregion

                                        #region 主人公が攻撃対象に無ければ
                                        if (DoAttackTargets(unit).Count((_unit) => _unit == GManager.instance.You.Lord) == 0)
                                        {
                                            if(UnderMyLordUnits.Count > 0)
                                            {
                                                AttackingUnit = unit;
                                                DefendingUnit = UnderMyLordUnits[UnityEngine.Random.Range(0, UnderMyLordUnits.Count)];
                                                break;
                                            }
                                        }
                                        #endregion
                                    }
                                    #endregion

                                    #region 主人公が攻撃対象にあれば
                                    if (DoAttackTargets(unit).Count((_unit) => _unit == GManager.instance.You.Lord)>0)
                                    {
                                        AttackingUnit = unit;
                                        DefendingUnit = GManager.instance.You.Lord;
                                        break;
                                    }
                                    #endregion
                                }
                            }
                            #endregion

                            #region 移動
                            if (unit.CanMoveDurinAction)
                            {
                                if(unit.Range.Count > 0)
                                {
                                    if (DoAttackTargets(unit).Count == 0)
                                    {
                                        if (RandomUtility.IsSucceedProbability(0.4f))
                                        {
                                            MovingUnit = unit;
                                            break;
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                    }

                    #region 対象のユニットが攻撃対象に入るか
                    bool DoAttack(Unit AttackingUnit,Unit DefendingUnit)
                    {
                        if (AttackingUnit.CanAttachTargetUnit(DefendingUnit))
                        {
                            if(AttackingUnit != AttackingUnit.Character.Owner.Lord)
                            {
                                if (AttackingUnit.CanCritical)
                                {
                                    return true;
                                }
                            }

                            if (DefendingUnit.Power <= AttackingUnit.Power)
                            {
                                return true;
                            }

                            if (DefendingUnit.Power - AttackingUnit.Power <= 10)
                            {
                                return true;
                            }
                        }

                        return false;
                    }
                    #endregion

                    #region 攻撃対象に入る敵ユニットリスト
                    List<Unit> DoAttackTargets(Unit AttackingUnit)
                    {
                        List<Unit> _DoAttackTargets = new List<Unit>();

                        foreach (Unit DefendingUnit in AttackingUnit.Character.Owner.Enemy.FieldUnit)
                        {
                            if(DoAttack(AttackingUnit,DefendingUnit))
                            {
                                _DoAttackTargets.Add(DefendingUnit);
                            }
                        }

                        return _DoAttackTargets;
                    }
                    #endregion
                }
                #endregion

                if (gameContext.TurnPhase != GameContext.phase.Action)
                {
                    goto EndActionPhase;
                }
            }

            #region フィールドカードをリセット
            foreach(Player player in gameContext.Players)
            {
                foreach (FieldUnitCard _fieldUnitCard in player.FieldUnitObjects)
                {
                    _fieldUnitCard.RemoveDragTarget();
                    _fieldUnitCard.CloseFieldUnitCommandPanel();
                    _fieldUnitCard.OffClickTarget();
                    _fieldUnitCard.RemoveSelectEffect();
                }
            }
            #endregion

            GManager.instance.You.PlayMat_Front_Select.gameObject.SetActive(false);
            GManager.instance.You.PlayMat_Back_Select.gameObject.SetActive(false);

            //戻るボタンを閉じる
            GManager.instance.BackButton.CloseSelectCommandButton();

            GManager.instance.selectCommandPanel.CloseSelectCommandPanel();

            GManager.instance.commandText.CloseCommandText();
            yield return new WaitWhile(() => GManager.instance.commandText.gameObject.activeSelf);
            #endregion

            #region 移動
            if(MovingUnit != null)
            {
                //Debug.Log("移動開始");
                yield return new WaitForSeconds(0.1f);
                yield return StartCoroutine(new IMoveUnitDuringAction(MovingUnit).MoveUnit());
            }
            #endregion

            #region 攻撃
            else if(AttackingUnit != null)
            {
                if(DefendingUnit != null)
                {
                    List<SkillInfo> skillInfos = new List<SkillInfo>();
                    IsSelecting = true;
                    //Debug.Log("攻撃開始");
                    AttackingUnit.DoneAttackThisTurn = true;
                    //タップ
                    yield return ContinuousController.instance.StartCoroutine(AttackingUnit.Tap());
                    AttackingUnit.ShowingFieldUnitCard.SetAttackerEffect();

                    yield return new WaitForSeconds(0.2f);

                    endTapAttackingUnit = true;
                    DefendingUnit.ShowingFieldUnitCard.SetDefenderEffect();

                    #region ターゲット矢印
                    yield return GManager.instance.OnTargetArrow(
                    GManager.instance.turnStateMachine.AttackingUnit.ShowingFieldUnitCard.GetLocalCanvasPosition(),
                    GManager.instance.turnStateMachine.DefendingUnit.ShowingFieldUnitCard.GetLocalCanvasPosition(),
                    GManager.instance.turnStateMachine.AttackingUnit.ShowingFieldUnitCard,
                    GManager.instance.turnStateMachine.DefendingUnit.ShowingFieldUnitCard);
                    #endregion

                    #region 「他のユニットが攻撃した時」効果を使用
                    skillInfos = new List<SkillInfo>();
                    foreach (Player player in gameContext.Players_ForTurnPlayer)
                    {
                        foreach(Unit unit in player.FieldUnit)
                        {
                            foreach (ICardEffect cardEffect in unit.Character.cEntity_EffectController.GetCardEffects(EffectTiming.OnAttackAnyone))
                            {
                                if (cardEffect is ActivateICardEffect)
                                {
                                    if (cardEffect.CanUse(null))
                                    {
                                        skillInfos.Add(new SkillInfo(cardEffect, null));
                                        //yield return ContinuousController.instance.StartCoroutine(((ActivateICardEffect)effect).Activate_Effect_Optional_Cost_Execute(null));
                                    }
                                }
                            }
                        }
                    }
                    yield return ContinuousController.instance.StartCoroutine(GManager.instance.availableMultipleSkills.ActivateMultipleSkills(skillInfos));
                    #endregion

                    #region 「他のユニットが攻撃された時」効果を使用
                    skillInfos = new List<SkillInfo>();
                    foreach (Unit _unit in gameContext.NonTurnPlayer.FieldUnit)
                    {
                        foreach (ICardEffect cardEffect in _unit.Character.cEntity_EffectController.GetCardEffects(EffectTiming.OnAttackedAlly))
                        {
                            if (cardEffect is ActivateICardEffect)
                            {
                                if (cardEffect.CanUse(null))
                                {
                                    skillInfos.Add(new SkillInfo(cardEffect, null));
                                    //yield return ContinuousController.instance.StartCoroutine(((ActivateICardEffect)effect).Activate_Effect_Optional_Cost_Execute(null));
                                }
                            }
                        }
                    }
                    yield return ContinuousController.instance.StartCoroutine(GManager.instance.availableMultipleSkills.ActivateMultipleSkills(skillInfos));

                    List<CardSource> HandCards = new List<CardSource>();

                    foreach (CardSource cardSource in gameContext.NonTurnPlayer.HandCards)
                    {
                        HandCards.Add(cardSource);
                    }

                    foreach (CardSource cardSource in HandCards)
                    {
                        foreach (ICardEffect effect in cardSource.cEntity_EffectController.GetCardEffects(EffectTiming.OnAttackedAlly))
                        {
                            if (effect is ActivateICardEffect)
                            {
                                if (effect.CanUse(null))
                                {
                                    yield return ContinuousController.instance.StartCoroutine(((ActivateICardEffect)effect).Activate_Optional_Effect_Cost_Execute(null));
                                }
                            }
                        }
                    }
                    #endregion

                    #region 支援
                    foreach (Player player in gameContext.Players_ForTurnPlayer)
                    {
                        yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(player));
                        yield return StartCoroutine(new IAddSupportFromLibrary(player).AddSupport());
                    }

                    #region 支援失敗
                    foreach (Player player in gameContext.Players)
                    {
                        if (!CanSupport(player))
                        {
                            yield return ContinuousController.instance.StartCoroutine(CardObjectController.MissSupport(player));
                        }

                    }
                    #endregion

                    #region 支援カードが置かれた時の効果
                    skillInfos = new List<SkillInfo>();
                    foreach (Player player in gameContext.Players_ForTurnPlayer)
                    {
                        foreach (Unit unit in player.FieldUnit)
                        {
                            foreach (ICardEffect cardEffect in unit.EffectList(EffectTiming.OnSetSupportBeforeSupportSkill))
                            {
                                if (CanSupport(player))
                                {
                                    if (cardEffect is ActivateICardEffect)
                                    {
                                        if (cardEffect.CanUse(null))
                                        {
                                            skillInfos.Add(new SkillInfo(cardEffect, null));
                                        }
                                    }
                                }
                            }
                        }
                    }
                    yield return ContinuousController.instance.StartCoroutine(GManager.instance.availableMultipleSkills.ActivateMultipleSkills(skillInfos));

                    skillInfos = new List<SkillInfo>();
                    foreach (Player player in gameContext.Players_ForTurnPlayer)
                    {
                        foreach(CardSource cardSource in player.SupportCards)
                        {
                            foreach(ICardEffect cardEffect in cardSource.cEntity_EffectController.GetSupportEffects(EffectTiming.OnSetSupport))
                            {
                                if(CanSupport(player))
                                {
                                    if (cardEffect is ActivateICardEffect)
                                    {
                                        if (cardEffect.CanUse(null))
                                        {
                                            skillInfos.Add(new SkillInfo(cardEffect, null));
                                        }
                                    }
                                }
                            }
                        }

                        foreach(Unit unit in player.FieldUnit)
                        {
                            foreach(ICardEffect cardEffect in unit.EffectList(EffectTiming.OnSetSupport))
                            {
                                if (CanSupport(player))
                                {
                                    if (cardEffect is ActivateICardEffect)
                                    {
                                        if (cardEffect.CanUse(null))
                                        {
                                            skillInfos.Add(new SkillInfo(cardEffect, null));
                                        }
                                    }
                                }
                            }
                        }
                    }
                    yield return ContinuousController.instance.StartCoroutine(GManager.instance.availableMultipleSkills.ActivateMultipleSkills(skillInfos));

                    bool CanSupport(Player player)
                    {
                        if (player.SupportCards.Count > 0)
                        {
                            if (AttackingUnit.Character.Owner == player)
                            {
                                if (!AttackingUnit.CanNotSupportThisUnit(player.SupportCards[0]))
                                {
                                    return true;
                                }
                            }

                            else if (DefendingUnit.Character.Owner == player)
                            {

                                if (!DefendingUnit.CanNotSupportThisUnit(player.SupportCards[0]))
                                {
                                    return true;
                                }
                            }
                        }
                        

                        return false;
                    }
                    #endregion

                    #endregion

                    #region 必殺攻撃
                    if (AttackingUnit.CanCritical)
                    {
                        //必殺攻撃選択待機
                        yield return StartCoroutine(GetComponent<Critical_Evasion>().CriticalCoroutine(Critical_EvasionMode.Critical));

                        if (GetComponent<Critical_Evasion>().DiscardCard != null)
                        {
                            AttackingUnit.UntilEndBattleEffects.Add(new Critical());

                            yield return StartCoroutine(GManager.instance.cutIn.OpenCutIn(Critical_EvasionMode.Critical, GetComponent<Critical_Evasion>().DiscardCard));

                            yield return new WaitForSeconds(0.2f);

                            #region 「味方が必殺攻撃した時」効果
                            skillInfos = new List<SkillInfo>();
                            foreach (Player player in gameContext.Players_ForTurnPlayer)
                            {
                                foreach(Unit unit in player.FieldUnit)
                                {
                                    foreach(ICardEffect cardEffect in unit.EffectList(EffectTiming.OnCriticalAnyone))
                                    {
                                        if (cardEffect is ActivateICardEffect)
                                        {
                                            if (cardEffect.CanUse(null))
                                            {
                                                skillInfos.Add(new SkillInfo(cardEffect, null));
                                            }
                                        }
                                    }
                                }
                            }
                            yield return ContinuousController.instance.StartCoroutine(GManager.instance.availableMultipleSkills.ActivateMultipleSkills(skillInfos));
                            #endregion
                        }
                    }
                    #endregion

                    #region 神速回避
                    if (AttackingUnit.CanBeEvaded(DefendingUnit))
                    {
                        //神速回避選択待機
                        yield return StartCoroutine(GetComponent<Critical_Evasion>().CriticalCoroutine(Critical_EvasionMode.Evasion));

                        if (GetComponent<Critical_Evasion>().DiscardCard != null)
                        {
                            yield return StartCoroutine(GManager.instance.cutIn.OpenCutIn(Critical_EvasionMode.Evasion, GetComponent<Critical_Evasion>().DiscardCard));

                            yield return new WaitForSeconds(0.2f);
                        }
                    }
                    #endregion

                    bool NotHit = GetComponent<Critical_Evasion>().DiscardCard != null && AttackingUnit.CanBeEvaded(DefendingUnit);

                    for (int i = 0; i < 5; i++)
                    {
                        GManager.instance.OffTargetArrow();
                        yield return new WaitForSeconds(Time.deltaTime);
                    }

                    yield return StartCoroutine(GManager.instance.GetComponent<Effects>().AttackAnimationCoroutine());

                    if (!NotHit)
                    {
                        yield return StartCoroutine(new IBattle(AttackingUnit, DefendingUnit).Battle());
                    }

                    else
                    {
                        #region 「味方が神速回避した時」効果
                        skillInfos = new List<SkillInfo>();
                        foreach (Player player in gameContext.Players_ForTurnPlayer)
                        {
                            foreach (Unit unit in player.FieldUnit)
                            {
                                foreach (ICardEffect cardEffect in unit.EffectList(EffectTiming.OnEvadeAnyone))
                                {
                                    if (cardEffect is ActivateICardEffect)
                                    {
                                        if (cardEffect.CanUse(null))
                                        {
                                            skillInfos.Add(new SkillInfo(cardEffect, null));
                                        }
                                    }
                                }
                            }
                        }
                        yield return ContinuousController.instance.StartCoroutine(GManager.instance.availableMultipleSkills.ActivateMultipleSkills(skillInfos));
                        #endregion
                    }

                    #region 支援エリアのカードが退避に落ちる時の効果
                    skillInfos = new List<SkillInfo>();
                    foreach (Player player in gameContext.Players_ForTurnPlayer)
                    {
                        foreach(Unit unit in player.FieldUnit)
                        {
                            foreach(ICardEffect cardEffect in unit.EffectList(EffectTiming.OnDiscardSuppot))
                            {
                                if (cardEffect is ActivateICardEffect)
                                {
                                    if (cardEffect.CanUse(null))
                                    {
                                        skillInfos.Add(new SkillInfo(cardEffect, null));
                                        //yield return StartCoroutine(((ActivateICardEffect)cardEffect).Activate_Effect_Optional_Cost_Execute(null));
                                    }
                                }
                            }
                        }

                        foreach(CardSource cardSource in player.SupportCards)
                        {
                            foreach (ICardEffect cardEffect in cardSource.cEntity_EffectController.GetSupportEffects(EffectTiming.OnDiscardSuppot))
                            {
                                if (cardEffect is ActivateICardEffect)
                                {
                                    if (cardEffect.CanUse(null))
                                    {
                                        skillInfos.Add(new SkillInfo(cardEffect, null));
                                        //yield return StartCoroutine(((ActivateICardEffect)cardEffect).Activate_Effect_Optional_Cost_Execute(null));
                                    }
                                }
                            }
                        }
                    }
                    yield return ContinuousController.instance.StartCoroutine(GManager.instance.availableMultipleSkills.ActivateMultipleSkills(skillInfos));
                    #endregion

                    #region　支援エリアのカードを退避に送る
                    foreach (Player player in gameContext.Players)
                    {
                        foreach (CardSource cardSource in player.SupportCards)
                        {
                            player.TrashCards.Add(cardSource);
                        }

                        player.SupportCards = new List<CardSource>();

                        player.SupportHandCard.gameObject.SetActive(false);
                    }
                    #endregion

                    #region 「攻撃終了した時」効果
                    skillInfos = new List<SkillInfo>();
                    foreach (Player player in gameContext.Players_ForTurnPlayer)
                    {
                        foreach (Func<EffectTiming, ActivateICardEffect> UntilTurnEndAction in player.UntilTurnEndActions)
                        {
                            ActivateICardEffect activateICardEffect = UntilTurnEndAction(EffectTiming.OnEndAttackAnyone);

                            if (activateICardEffect != null)
                            {
                                if (((ICardEffect)activateICardEffect).CanUse(null))
                                {
                                    skillInfos.Add(new SkillInfo((ICardEffect)activateICardEffect, null));
                                    //yield return ContinuousController.instance.StartCoroutine(activateICardEffect.Activate_Effect_Optional_Cost_Execute(null));
                                }
                            }
                        }

                        foreach (Unit unit in player.FieldUnit)
                        {
                            foreach (ICardEffect cardEffect in unit.EffectList(EffectTiming.OnEndAttackAnyone))
                            {
                                if (cardEffect is ActivateICardEffect)
                                {
                                    if (cardEffect.CanUse(null))
                                    {
                                        skillInfos.Add(new SkillInfo(cardEffect, null));
                                        //yield return ContinuousController.instance.StartCoroutine(((ActivateICardEffect)cardEffect).Activate_Effect_Optional_Cost_Execute(null));
                                    }
                                }
                            }
                        }
                    }
                    yield return ContinuousController.instance.StartCoroutine(GManager.instance.availableMultipleSkills.ActivateMultipleSkills(skillInfos));
                    #endregion
                }
            }
            #endregion

            #region 起動効果
            else if (UseSkillUnit != null)
            {
                if(CardEffect != null)
                {
                    if(CardEffect is ActivateICardEffect)
                    {
                        if (CardEffect.CanUse(null))
                        {
                            yield return StartCoroutine(((ActivateICardEffect)CardEffect).Activate_Effect_Optional_Cost_Execute(null));
                        }
                    }
                    
                }
            }
            #endregion

        }

    EndActionPhase:;

        #region 行動フェイズ終了
        GManager.instance.selectCommandPanel.CloseSelectCommandPanel();
        GManager.instance.commandText.CloseCommandText();
        yield return new WaitWhile(() => GManager.instance.commandText.gameObject.activeSelf);
        GManager.instance.OffTargetArrow();

        ResetActionPhaseParameter();

        #endregion

        #endregion
    }

    #region パラメータリセット
    void ResetActionPhaseParameter()
    {
        #region 場のカード表示・バトル終了時効果リセット
        foreach (Player player in gameContext.Players)
        {
            foreach (FieldUnitCard fieldUnitCard in player.FieldUnitObjects)
            {
                fieldUnitCard.OffAttackerDefenderEffect();
                fieldUnitCard.RemoveSelectEffect();
            }

            foreach (Unit unit in player.FieldUnit)
            {
                unit.UntilEndBattleEffects = new List<ICardEffect>();
            }

            player.UntilEndBattleEffects = new List<ICardEffect>();
        }
        #endregion

        #region 手札・場のカードの選択待機状態を解除
        OffHandCardTarget(gameContext.TurnPlayer);
        OffFieldCardTarget(gameContext.TurnPlayer);
        #endregion

        AttackingUnit = null;
        DefendingUnit = null;
        MovingUnit = null;
        UseSkillUnit = null;
        CardEffect = null;
        isDestroydeByBattle = false;
        endTapAttackingUnit = false;
        IsSelecting = false;
        isSync = false;
        
    }
    #endregion

    #region 移動ユニット決定
    [PunRPC]
    public void SetMovingUnit(int unitIndex)
    {
        MovingUnit = gameContext.TurnPlayer.FieldUnit[unitIndex];
    }
    #endregion

    #region 攻撃ユニット、防御ユニット決定
    [PunRPC]
    public void SetAttackerDefender(int AttackerUnitIndex,int DefenderUnitIndex)
    {
        DefendingUnit = gameContext.NonTurnPlayer.FieldUnit[DefenderUnitIndex];
        AttackingUnit = gameContext.TurnPlayer.FieldUnit[AttackerUnitIndex];
    }
    #endregion

    #region スキル使用ユニット決定
    [PunRPC]
    public void SetUseSkillUnit(int unitIndex,int skillIndex)
    {
        UseSkillUnit = gameContext.TurnPlayer.FieldUnit[unitIndex];

        if(0 <= skillIndex && skillIndex < UseSkillUnit.Character.cEntity_EffectController.GetCardEffects(EffectTiming.OnDeclaration).Count)
        {
            CardEffect = UseSkillUnit.Character.cEntity_EffectController.GetCardEffects(EffectTiming.OnDeclaration)[skillIndex];
        }
    }
    #endregion

    #endregion

    #region 終了フェイズ
    public IEnumerator EndPhase()
    {
        foreach(Player player in gameContext.Players)
        {
            player.BondConsumed = 0;
        }

        #region 手札・場のカードの選択待機状態を解除
        OffHandCardTarget(gameContext.TurnPlayer);
        OffFieldCardTarget(gameContext.TurnPlayer);
        #endregion

        isSync = true;
        gameContext.TurnPhase = GameContext.phase.End;
        Debug.Log($"{gameContext.TurnPlayer}:終了フェイズ");
        yield return GManager.instance.photonWaitController.StartWait("EndPhase");
        isSync = false;

        isFirstPlayerFirstTurn = false;

        #region ターン終了時の効果
        List<SkillInfo> skillInfos = new List<SkillInfo>();
        foreach(Player player in gameContext.Players_ForTurnPlayer)
        {
            foreach(Func<EffectTiming, ActivateICardEffect> UntilTurnEndAction in player.UntilTurnEndActions)
            {
                ActivateICardEffect activateICardEffect = UntilTurnEndAction(EffectTiming.OnEndTurn);

                if (activateICardEffect != null)
                {
                    if(((ICardEffect)activateICardEffect).CanUse(null))
                    {
                        skillInfos.Add(new SkillInfo((ICardEffect)activateICardEffect, null));
                    }
                }
            }

            foreach(Unit unit in player.FieldUnit)
            {
                foreach(ICardEffect cardEffect in unit.EffectList(EffectTiming.OnEndTurn))
                {
                    if (cardEffect is ActivateICardEffect)
                    {
                        if (cardEffect.CanUse(null))
                        {
                            //skillInfos.Add(new SkillInfo(cardEffect, null));

                            if (cardEffect.isNotCheck_Effect)
                            {
                                yield return ContinuousController.instance.StartCoroutine(((ActivateICardEffect)cardEffect).Activate(null));
                            }

                            else
                            {
                                skillInfos.Add(new SkillInfo(cardEffect, null));
                            }
                        }
                    }
                }
            }
        }
        yield return ContinuousController.instance.StartCoroutine(GManager.instance.availableMultipleSkills.ActivateMultipleSkills(skillInfos));
        #endregion

        #region ターン終了時までの効果をリセット
        foreach (Player player in gameContext.Players)
        {
            player.UntilTurnEndEffects = new List<ICardEffect>();
            //player.OnTurnEndEffects = new List<ICardEffect>();

            player.UntilTurnEndActions = new List<Func<EffectTiming, ActivateICardEffect>>();

            foreach (Unit unit in player.FieldUnit)
            {
                unit.UntilEachTurnEndUnitEffects = new List<ICardEffect>();
                unit.DoneAttackThisTurn = false;
                unit.DoneMoveThisTurn = false;
            }
        }

        foreach(Unit unit in gameContext.TurnPlayer.FieldUnit)
        {
            unit.UntilOwnerTurnEndUnitEffects = new List<ICardEffect>();
        }

        gameContext.NonTurnPlayer.UntilOpponentTurnEndEffects = new List<ICardEffect>();

        foreach (Unit unit in gameContext.NonTurnPlayer.FieldUnit)
        {
            unit.UntilOpponentTurnEndEffects = new List<ICardEffect>();
        }
        #endregion

        #region スキルの使用回数をリセット
        foreach (Player player in gameContext.Players)
        {
            foreach (Unit unit in player.FieldUnit)
            {
                foreach(CardSource cardSource in unit.Characters)
                {
                    cardSource.cEntity_EffectController.Init();
                }
            }
        }
        #endregion
    }
    #endregion

    #region 選択状態リセット
    #region 手札のカードの選択待機状態を解除
    public void OffHandCardTarget(Player player)
    {
        foreach (HandCard _handCard in player.HandCardObjects)
        {
            if (_handCard != null)
            {
                _handCard.RemoveDragTarget();
                _handCard.RemoveClickTarget();
            }
        }
    }
    #endregion

    #region 場のカードの選択待機状態を解除
    public void OffFieldCardTarget(Player player)
    {
        foreach (FieldUnitCard fieldUnitCard in player.FieldUnitObjects)
        {
            fieldUnitCard.OffClickTarget();
        }
    }
    #endregion
    #endregion

    #region ゲーム終了
    public bool endGame { get; set; } = false;
    public void OnClickSurrenderButton()
    {
        photonView.RPC("Surrender", RpcTarget.All);
    }

    [PunRPC]
    public void Surrender()
    {
        EndGame(gameContext.NonTurnPlayer);
    }

    public void EndGame(Player Winner)
    {
        GManager.instance.photonWaitController.ResetKeys();

        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable());

        if(!ContinuousController.instance.isRandomMatch && !ContinuousController.instance.isAI)
        {
            Debug.Log("プレイヤープロパティ初期化");
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable());
        }
        
        endGame = true;

        if (gameContext.TurnPlayer != null)
        {
            OffFieldCardTarget(gameContext.TurnPlayer);

            OffHandCardTarget(gameContext.TurnPlayer);
        }

        GManager.instance.optionPanel.CloseOptionPanel();

        GManager.instance.LoadingObject.gameObject.SetActive(false);

        GManager.instance.ResultText.gameObject.SetActive(true);

        GManager.instance.ReturnToTitleButton.SetActive(true);

        GManager.instance.commandText.CloseCommandText();

        ContinuousController.instance.bGMObject.StopPlayBGM();

        if (Winner != null)
        {
            //勝ち
            if (Winner.isYou)
            {
                GManager.instance.ResultText.text = "You Win!";

                if (!GManager.instance.IsAI)
                {
                    ContinuousController.instance.WinCount++;
                    ContinuousController.instance.SaveWinCount();
                }
            }

            //負け
            else
            {
                GManager.instance.ResultText.text = "You Lose...";
            }
        }

        else
        {
            GManager.instance.ResultText.text = "Disconnected...";
        }

        StopAllCoroutines();
    }

    [PunRPC]
    public void Surrender(bool isTurnPlayer)
    {
        Player winner = null;

        if (isTurnPlayer)
        {
            winner = GManager.instance.turnStateMachine.gameContext.TurnPlayer;
        }

        else
        {
            winner = GManager.instance.turnStateMachine.gameContext.NonTurnPlayer;
        }

        GManager.instance.turnStateMachine.EndGame(winner);
    }
    #endregion

    #region 次のフェイズに行く
    [PunRPC]
    public void NextPhase()
    {
        int CurrentPhaseID = (int)GManager.instance.turnStateMachine.gameContext.TurnPhase;

        int NextPhaseID = ++CurrentPhaseID;

        int MaxPhaseCount = Enum.GetNames(typeof(GameContext.phase)).Length;

        if (NextPhaseID >= MaxPhaseCount)
        {
            NextPhaseID = 0;
        }

        isSync = true;
        GManager.instance.turnStateMachine.gameContext.TurnPhase = (GameContext.phase)Enum.ToObject(typeof(GameContext.phase), NextPhaseID);
    }
    #endregion
}
