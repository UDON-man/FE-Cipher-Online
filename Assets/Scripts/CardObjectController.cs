using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;
using DG.Tweening;

//カードに関する操作(カードオブジェクトを操作する)
public class CardObjectController : MonoBehaviour
{
    #region 両プレイヤーのライブラリーにカードを生成
    public static IEnumerator CreatePlayerDecks(CardSource CardPrefab, GameContext gameContext)
    {
        yield return null;

        StarterDeckData starterDeckData = ContinuousController.instance.GetComponent<StarterDeck>().starterDeckDatas[UnityEngine.Random.Range(0, ContinuousController.instance.GetComponent<StarterDeck>().starterDeckDatas.Count)];

        DeckData RandomStarterDeck = new DeckData(starterDeckData.DeckCode);

        #region Photonクライアントの内、マスタークライアントと非マスタークライアントを抽出
        Photon.Realtime.Player MasterPlayer = null;
        Photon.Realtime.Player nonMasterPlayer = null;

        if(PhotonNetwork.IsMasterClient)
        {
            MasterPlayer = PhotonNetwork.LocalPlayer;

            foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
            {
                if(player != PhotonNetwork.LocalPlayer)
                {
                    nonMasterPlayer = player;
                    break;
                }
            }
        }

        else
        {
            nonMasterPlayer = PhotonNetwork.LocalPlayer;

            foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
            {
                if (player != PhotonNetwork.LocalPlayer)
                {
                    MasterPlayer = player;
                    break;
                }
            }
        }

        /*
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
        */
        #endregion

        #region そのPhotonクライアントのデッキレシピ(カード番号の配列)
        List<CEntity_Base> DeckRecipie(Photon.Realtime.Player player)
        {
            #region 対人戦
            if (!GManager.instance.IsAI)
            {
                Hashtable hashtable = player.CustomProperties;

                if (HasDeckRecipie(player))
                {
                    if (hashtable.TryGetValue(ContinuousController.DeckDataPropertyKey, out object value))
                    {
                        DeckData deckData = new DeckData((string)value);

                        return RandomUtility.ShuffledDeckCards(deckData.DeckCards());
                    }
                }

                else
                {
                    Debug.Log("!HasDeckRecipie");
                }

                return null;
            }
            #endregion

            #region デバッグモード
            else
            {
                #region プレイヤーのデッキ
                if (player == MasterPlayer)
                {
                    if(ContinuousController.instance.BattleDeckData == null)
                    {
                        foreach (DeckData deckData in ContinuousController.instance.DeckDatas)
                        {
                            if (deckData.IsValidDeckData())
                            {
                                return RandomUtility.ShuffledDeckCards(deckData.DeckCards());
                            }
                        }
                    }

                    else
                    {
                        if (ContinuousController.instance.BattleDeckData.IsValidDeckData())
                        {
                            return RandomUtility.ShuffledDeckCards(ContinuousController.instance.BattleDeckData.DeckCards());
                        }
                    }
                    
                    return RandomUtility.ShuffledDeckCards(RandomStarterDeck.DeckCards());
                }
                #endregion

                #region AIのデッキ
                else
                {
                    return RandomUtility.ShuffledDeckCards(RandomStarterDeck.DeckCards());
                }
                #endregion

            }
            #endregion
        }
        #endregion

        #region そのPhotonクライアントのキーカード
        CEntity_Base KeyCard(Photon.Realtime.Player player)
        {
            #region 対人戦
            if (!GManager.instance.IsAI)
            {
                Hashtable hashtable = player.CustomProperties;

                if (HasDeckRecipie(player))
                {
                    if (hashtable.TryGetValue(ContinuousController.DeckDataPropertyKey, out object value))
                    {
                        DeckData deckData = new DeckData((string)value);

                        if (player == PhotonNetwork.LocalPlayer)
                        {
                            Debug.Log($"あなたの主人公:{deckData.MainCharacter.CardName}");
                        }

                        else
                        {
                            Debug.Log($"相手の主人公:{deckData.MainCharacter.CardName}");
                        }

                        return deckData.MainCharacter;
                    }
                }

                else
                {
                    Debug.Log("!HasDeckRecipie");
                }
            }
            #endregion

            #region デバッグモード
            else
            {
                #region プレイヤーの主人公
                if (player == MasterPlayer)
                {
                    if(ContinuousController.instance.BattleDeckData == null)
                    {
                        foreach (DeckData deckData in ContinuousController.instance.DeckDatas)
                        {
                            if (deckData.IsValidDeckData())
                            {
                                return deckData.MainCharacter;
                            }
                        }
                    }

                    else
                    {
                        if(ContinuousController.instance.BattleDeckData.IsValidDeckData())
                        {
                            if(ContinuousController.instance.BattleDeckData.MainCharacter.PlayCost == 1)
                            {
                                return ContinuousController.instance.BattleDeckData.MainCharacter;
                            }
                        }
                    }
                    
                }
                #endregion

                #region AIのキーカード
                else
                {
                    return RandomStarterDeck.MainCharacter;
                }
                #endregion

            }
            #endregion

            return null;
        }
        #endregion

        #region そのPhotonクライアントがデッキレシピのカスタムプロパティを持っているか判定
        bool HasDeckRecipie(Photon.Realtime.Player _player)
        {
            Hashtable _hashtable = _player.CustomProperties;

            if (_hashtable.TryGetValue(ContinuousController.DeckDataPropertyKey, out object value))
            {
                DeckData deckData = new DeckData((string)value);

                if (deckData.IsValidDeckData())
                {
                    return true;
                }

                else
                {
                    Debug.Log("キーは持ってるけど不正なデッキ");
                }
            }

            else
            {
                Debug.Log("そもそもキーを持ってない");
            }

            return false;
        }
        #endregion

        #region 取得したデッキレシピを元にカードを生成
        int CardIndex = 0;

        foreach (CEntity_Base cEntity_Base in DeckRecipie(MasterPlayer))
        {
            CreateOneLibraryCard(0, cEntity_Base, CardIndex);
            CardIndex++;
        }

        foreach (CEntity_Base cEntity_Base in DeckRecipie(nonMasterPlayer))
        {
            CreateOneLibraryCard(1, cEntity_Base, CardIndex);
            CardIndex++;
        }

        #region そのプレイヤーのライブラリにカードを1枚生成
        void CreateOneLibraryCard(int _PlayerID, CEntity_Base cEntity_Base, int _cardIndex)
        {
            Player player = gameContext.PlayerFromID(_PlayerID);

            CardSource card = Instantiate(CardPrefab, player.CardSorcesParent).GetComponent<CardSource>();

            card.SetBaseData(cEntity_Base, player);

            card.cEntity_EffectController.AddCardEffect(cEntity_Base.ClassName);

            card.SetUpCardIndex(_cardIndex);

            card.Owner.LibraryCards.Add(card);

            gameContext.ActiveCardList.Add(card);
        }
        #endregion

        #region Playerクラスにデッキレシピ・キーカードを格納
        List<CEntity_Base> cEntity_Bases = new List<CEntity_Base>();

        cEntity_Bases = new List<CEntity_Base>();

        foreach (CEntity_Base cEntity_Base in DeckRecipie(MasterPlayer))
        {
            cEntity_Bases.Add(cEntity_Base);
        }

        gameContext.PlayerFromID(0).DeckRecipie = cEntity_Bases;
        gameContext.PlayerFromID(0).LordCard = KeyCard(MasterPlayer);

        cEntity_Bases = new List<CEntity_Base>();

        foreach (CEntity_Base cEntity_Base in DeckRecipie(nonMasterPlayer))
        {
            cEntity_Bases.Add(cEntity_Base);
        }

        gameContext.PlayerFromID(1).DeckRecipie = cEntity_Bases;
        gameContext.PlayerFromID(1).LordCard = KeyCard(nonMasterPlayer);
        #endregion
        #endregion
    }
    #endregion

    #region 手札のカードを生成する
    public static IEnumerator AddHandCard(CardSource cardSource, bool isDraw)
    {
        cardSource.SetFace();

        if(!cardSource.Owner.HandCards.Contains(cardSource))
        {
            cardSource.Owner.HandCards.Add(cardSource);
        }

        if (isDraw)
        {
            if (GManager.instance.turnStateMachine.DoseStartGame)
            {
                yield return ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().AddHandCardEffect(cardSource));
            }
        }

        HandCard handCard = Instantiate(GManager.instance.handCardPrefab, cardSource.Owner.HandTransform);

        handCard.GetComponent<Draggable_HandCard>().startScale = handCard.transform.localScale;

        handCard.GetComponent<Draggable_HandCard>().DefaultY = -9;

        handCard.SetUpHandCard(cardSource);

        AlignHand(cardSource.Owner);

        cardSource.ShowingHandCard = handCard;

        yield return new WaitForSeconds(Time.deltaTime * 1.5f);

        #region 手札に加わるエフェクト
        if (GManager.instance.turnStateMachine.DoseStartGame)
        {
            cardSource.Owner.HandTransform.GetComponent<GridLayoutGroup>().enabled = false;

            Vector3 startPosition = Vector3.zero;
            Vector3 targetPositon = handCard.transform.localPosition;

            if (cardSource.Owner.isYou)
            {
                startPosition = handCard.transform.localPosition + new Vector3(0, 70, 0);
            }

            else
            {
                startPosition = handCard.transform.localPosition - new Vector3(0, 70, 0);
            }

            handCard.transform.localPosition = startPosition;

            bool end = false;
            var sequence = DOTween.Sequence();
            sequence
                .Append(handCard.transform.DOLocalMove(targetPositon, 0.15f).SetEase(Ease.OutBack))
                .AppendCallback(() => { end = true; });

            yield return new WaitWhile(() => !end);
            end = false;

            cardSource.Owner.HandTransform.GetComponent<GridLayoutGroup>().enabled = true;
        }
        #endregion
    }
    #endregion

    #region 手札のカードを整列
    public static void AlignHand(Player player)
    {
        int n = player.HandTransform.childCount;
        float scale = player.HandTransform.transform.localScale.x;

        if (n > 0)
        {
            float CardWidth = player.HandTransform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x;

            //そのままで領域内に収まるなら
            if (n * scale * CardWidth < player.HandWidth)
            {
                player.HandTransform.GetComponent<GridLayoutGroup>().spacing = new Vector2(0, 0);
            }

            //そのままでははみ出る場合
            else
            {
                float delta = (n * scale * CardWidth - player.HandWidth) / ((n - 1) * scale);

                player.HandTransform.GetComponent<GridLayoutGroup>().spacing = new Vector2(-delta, 0);
            }
        }
    }
    #endregion

    #region 新たなユニットをプレイ
    public static IEnumerator CreateNewUnit(Unit unit,bool isFront)
    {
        if (unit.Character != null)
        {
            FieldUnitCard fieldUnitCard = null;

            if(isFront)
            {
                unit.Character.Owner.FrontUnits.Add(unit);

                fieldUnitCard = Instantiate(GManager.instance.fieldUnitCardPrefab, unit.Character.Owner.FrontZoneTransform);
            }

            else
            {
                unit.Character.Owner.BackUnits.Add(unit);

                fieldUnitCard = Instantiate(GManager.instance.fieldUnitCardPrefab, unit.Character.Owner.BackZoneTransform);
            }
            
            fieldUnitCard.SetFieldUnitCard(unit);

            fieldUnitCard.StartScale = fieldUnitCard.transform.localScale;

            unit.ShowingFieldUnitCard = fieldUnitCard;
        }

        yield return null;
    }
    #endregion

    #region 場を離れる際に初期化
    public static void RemoveField(CardSource cardSource)
    {
        cardSource.Init();
    }
    #endregion

    #region 墓地にカードを追加
    public static void AddTrashCard(CardSource cardSource)
    {
        if (!cardSource.Owner.TrashCards.Contains(cardSource))
        {
            cardSource.SetFace();
            cardSource.Owner.TrashCards.Insert(0, cardSource);

            cardSource.Init();
        }
    }
    #endregion

    #region オーブにカードを追加する
    public static IEnumerator AddOrbCard(CardSource cardSource, bool isFace)
    {
        if(isFace)
        {
            cardSource.SetFace();
        }

        else
        {
            cardSource.SetReverse();
        }
        
        cardSource.Owner.OrbCards.Add(cardSource);

        yield break;
    }
    #endregion

    #region 支援エリアににカードを追加
    public static IEnumerator SetSupportCard(CardSource card)
    {
        card.Owner.SupportCards.Add(card);

        yield return null;
    }
    #endregion

    #region 絆ゾーンににカードを追加
    public static IEnumerator SetBondCard(CardSource card)
    {
        card.Owner.BondCards.Add(card);

        yield return null;
    }
    #endregion

    #region 場のカードの移動

    #region 前衛から後衛に移動
    public static IEnumerator MoveFromFrontToBack(List<Unit> MoveUnit)
    {
        if (MoveUnit.Count > 0)
        {
            if (MoveUnit[0].Characters.Count > 0)
            {
                if (MoveUnit[0].Character != null)
                {
                    List<FieldUnitCard> fieldUnitCards = new List<FieldUnitCard>();

                    //移動対象のUnitに対応する場のカードオブジェクトを取得
                    foreach (Unit unit in MoveUnit)
                    {
                        fieldUnitCards.Add(unit.ShowingFieldUnitCard);
                    }

                    float delta = 18.2f;

                    delta = 10;

                    float moveTime = 0.3f;

                    bool end = false;

                    foreach (FieldUnitCard fieldUnitCard in fieldUnitCards)
                    {
                        if(fieldUnitCard.thisUnit == null)
                        {
                            continue;
                        }

                        int index = fieldUnitCards.IndexOf(fieldUnitCard);

                        Vector3 targetPos = new Vector3(delta * (index), fieldUnitCard.thisUnit.Character.Owner.BackZoneTransform.position.y, fieldUnitCard.thisUnit.Character.Owner.BackZoneTransform.position.z);

                        //targetPos = new Vector3(fieldUnitCard.transform.position.x, MoveUnit[0].Character.Owner.BackZoneTransform.position.y, MoveUnit[0].Character.Owner.BackZoneTransform.position.z);
                        if (fieldUnitCard.thisUnit.Character.Owner.BackZoneTransform.childCount > 0)
                        {
                            Transform RightLimit = fieldUnitCard.thisUnit.Character.Owner.BackZoneTransform.GetChild(fieldUnitCard.thisUnit.Character.Owner.BackZoneTransform.childCount - 1);

                            targetPos += new Vector3(RightLimit.transform.position.x + delta * (index + 1), 0, 0);
                        }



                        fieldUnitCard.transform.SetParent(null);

                        var sequence = DOTween.Sequence();

                        sequence
                            .Append(DOTween.To(() => fieldUnitCard.transform.position, (x) => fieldUnitCard.transform.position = x, targetPos, moveTime))
                            .Join(DOTween.To(() => fieldUnitCard.transform.localScale, (x) => fieldUnitCard.transform.localScale = x, new Vector3(1.2f * 0.95f, 1.2f * 0.95f, 1.2f * 0.95f), moveTime))
                            .AppendCallback(() => { sequence.Kill(); end = true; });

                        sequence.Play();
                    }

                    while (!end)
                    {
                        yield return null;
                    }

                    foreach (FieldUnitCard fieldUnitCard in fieldUnitCards)
                    {
                        fieldUnitCard.transform.SetParent(fieldUnitCard.thisUnit.Character.Owner.BackZoneTransform);

                        fieldUnitCard.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);

                        fieldUnitCard.thisUnit.DoneMoveThisTurn = true;
                    }
                }

            }
        }
    }
    #endregion

    #region 後衛から前衛に移動
    public static IEnumerator MoveFromBackToFront(List<Unit> MoveUnit)
    {
        if (MoveUnit.Count > 0)
        {
            if (MoveUnit[0].Characters.Count > 0)
            {
                if (MoveUnit[0].Character != null)
                {
                    List<FieldUnitCard> fieldUnitCards = new List<FieldUnitCard>();

                    //移動対象のマジンに対応する場のカードオブジェクトを取得
                    foreach (Unit unit in MoveUnit)
                    {
                        fieldUnitCards.Add(unit.ShowingFieldUnitCard);
                    }

                    float delta = 18.2f;

                    delta = 10;

                    float moveTime = 0.3f;

                    bool end = false;

                    foreach (FieldUnitCard fieldUnitCard in fieldUnitCards)
                    {
                        if(fieldUnitCard.thisUnit == null)
                        {
                            continue;
                        }

                        int index = fieldUnitCards.IndexOf(fieldUnitCard);

                        Vector3 targetPos = new Vector3(delta * (index), fieldUnitCard.thisUnit.Character.Owner.FrontZoneTransform.position.y, fieldUnitCard.thisUnit.Character.Owner.FrontZoneTransform.position.z);

                        //targetPos = new Vector3(fieldUnitCard.transform.position.x, MoveUnit[0].Character.Owner.FrontZoneTransform.position.y, MoveUnit[0].Character.Owner.FrontZoneTransform.position.z);

                        if (fieldUnitCard.thisUnit.Character.Owner.FrontZoneTransform.childCount > 0)
                        {
                            Transform RightLimit = fieldUnitCard.thisUnit.Character.Owner.FrontZoneTransform.GetChild(fieldUnitCard.thisUnit.Character.Owner.FrontZoneTransform.childCount - 1);

                            targetPos += new Vector3(RightLimit.transform.position.x + delta * (index + 1), 0, 0);
                        }

                        fieldUnitCard.transform.SetParent(null);

                        var sequence = DOTween.Sequence();

                        sequence
                            .Append(DOTween.To(() => fieldUnitCard.transform.position, (x) => fieldUnitCard.transform.position = x, targetPos, moveTime))
                            .Join(DOTween.To(() => fieldUnitCard.transform.localScale, (x) => fieldUnitCard.transform.localScale = x, new Vector3(1.2f * 1.2f, 1.2f * 1.2f, 1.2f * 1.2f), moveTime))
                            .AppendCallback(() => { sequence.Kill(); end = true; });

                        sequence.Play();
                    }

                    while (!end)
                    {
                        yield return null;
                    }

                    foreach (FieldUnitCard fieldUnitCard in fieldUnitCards)
                    {
                        fieldUnitCard.transform.SetParent(fieldUnitCard.thisUnit.Character.Owner.FrontZoneTransform);

                        fieldUnitCard.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);

                        fieldUnitCard.thisUnit.DoneMoveThisTurn = true;
                    }
                }

            }
        }
    }
    #endregion

    #endregion

    #region ライブラリーをシャッフル
    public static IEnumerator Shuffle(Player player)
    {
        player.LibraryCards = RandomUtility.ShuffledDeckCards(player.LibraryCards);

        yield return null;
    }
    #endregion

    #region 支援失敗処理
    public static IEnumerator MissSupport(Player player)
    {
        yield return new WaitForSeconds(0.1f);

        foreach (CardSource cardSource in player.SupportCards)
        {
            player.TrashCards.Add(cardSource);
        }

        player.SupportCards = new List<CardSource>();

        player.SupportHandCard.gameObject.SetActive(false);
    }
    #endregion
}
