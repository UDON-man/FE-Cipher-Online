using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.Events;

public class RoomManager : MonoBehaviourPunCallbacks
{
    bool isReady { get; set; }
    int playerCount { get; set; }

    bool endSetUp { get; set; }

    //BattleRule battleRule { get; set; }

    [Header("準備完了ボタンテキスト")]
    public Text ReadyButtonText;

    [Header("準備完了ボタンカバー")]
    public GameObject ReadyButtonCover;

    [Header("プレイヤー情報プレハブ")]
    public GameObject PlayerElementPrefab;//部屋情報Prefab

    [Header("プレイヤー情報プレハブの親")]
    public GameObject PlayerParent;//ScrolViewのcontentオブジェクト

    [Header("読み込み中オブジェクト")]
    public LoadingObject loadingObject;

    [Header("もどるボタン")]
    public GameObject ReturnButton;

    [Header("ルームIDテキスト")]
    public Text RoomIDText;

    [Header("Unlimitedオブジェクト")]
    public GameObject UnlimitedObject;

    [Header("JCSFinalオブジェクト")]
    public GameObject JCSFinalObject;

    [Header("デッキ情報パネル")]
    public DeckInfoPanel deckInfoPanel;

    [Header("バトルデッキ選択")]
    public SelectBattleDeck selectBattleDeck;

    [Header("YesNoパネル")]
    public YesNoObject yesNoObject;

    [Header("無効なデッキ表示")]
    public GameObject InvalidDeckObject;

    [Header("デッキ無表示")]
    public GameObject NoDeckSetObject;

    public GameObject Parent;

    #region アンロードから復帰

    #endregion

    #region ルーム画面を開く

    #region ルーム画面を開く
    public void SetUpRoom()//BattleRule battleRule)
    {
        //ContinuousController.instance.StartCoroutine(SetUpRoomCoroutine(battleRule));
        ContinuousController.instance.StartCoroutine(SetUpRoomCoroutine());
    }

    public IEnumerator SetUpRoomCoroutine()//(BattleRule battleRule)
    {
        ContinuousController.instance.isAI = false;
        ContinuousController.instance.isRandomMatch = false;

        yield return ContinuousController.instance.StartCoroutine(loadingObject.StartLoading("Now Loading"));

        endSetUp = false;

        //this.gameObject.SetActive(true);
        Parent.SetActive(true);

        if(!PhotonNetwork.InRoom)
        {
            //yield return ContinuousController.instance.StartCoroutine(CreateRoomCoroutine(battleRule));
            yield return ContinuousController.instance.StartCoroutine(CreateRoomCoroutine());
        }

        yield return ContinuousController.instance.StartCoroutine(ShowRoomInfo());

        yield return ContinuousController.instance.StartCoroutine(Init(false));

        SetDeckInfoPanel();
        CheckReadyButton();

        yield return ContinuousController.instance.StartCoroutine(loadingObject.EndLoading());

        endSetUp = true;
    }
    #endregion

    #region 部屋を作成する処理
    bool CreateRoomFailed = false;
    public IEnumerator CreateRoomCoroutine()
    {
        CreateRoomFailed = false;

        yield return new WaitWhile(() => !PhotonNetwork.IsConnectedAndReady);

        if(!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        yield return new WaitWhile(() => !PhotonNetwork.InLobby);

        while(true)
        {
            //作成する部屋の設定
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.IsVisible = true;   //ロビーで見える部屋にする
            roomOptions.IsOpen = true;      //他のプレイヤーの入室を許可する
            roomOptions.PublishUserId = true;

            roomOptions.MaxPlayers = 2;

            //ルームカスタムプロパティで部屋作成者を表示させるため、作成者の名前を格納
            roomOptions.CustomRoomProperties = new Hashtable()
            {
                { "RoomCreator",PhotonNetwork.NickName },
                //{ "BattleRule",battleRule.ToString() },

            };

            //ロビーにカスタムプロパティの情報を表示させる
            roomOptions.CustomRoomPropertiesForLobby = new string[]
            {
                "RoomCreator",
                //"BattleRule",
            };

            string RoomName = StringUtils.GeneratePassword_Num(5);

            //部屋作成
            PhotonNetwork.CreateRoom(RoomName, roomOptions, null);

            while (true)
            {
                if (CreateRoomFailed || PhotonNetwork.InRoom)
                {
                    break;
                }

                yield return null;

                Debug.Log("ルームが出来るまで待機");
            }

            //ルーム作成成功
            if(!CreateRoomFailed && PhotonNetwork.InRoom)
            {
                yield break;
            }

            Debug.Log("ルーム作成失敗");
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        CreateRoomFailed = true;
    }
    #endregion

    #region ルーム画面を開いた時に初期化
    public IEnumerator Init(bool OnUnload)
    {
        selectBattleDeck.Off();
        DoneStartBattle = false;
        isReady = false;
        
        playerCount = 0;//PhotonNetwork.CurrentRoom.PlayerCount;
        DestroyChildObject();//PlayerElementを削除
        GetPlayers();

        if(!OnUnload)
        {
            yield return ContinuousController.instance.StartCoroutine(SignUpBattleDeck(FirstValidDeckData()));
            SetReadyProperty(PhotonNetwork.LocalPlayer, isReady);
        }

        else
        {
            yield return ContinuousController.instance.StartCoroutine(PhotonUtility.SignUpBattleDeckData());
            
        }
    }

    #region デッキリストの中で最初の適正デッキ
    DeckData FirstValidDeckData()
    {
        if(ContinuousController.instance.DeckDatas != null)
        {
            foreach (DeckData deckData in ContinuousController.instance.DeckDatas)
            {
                if(deckData != null)
                {
                    if(deckData.IsValidDeckData())
                    {
                        return deckData;
                    }
                }
            }
        }
        
        return null;
    }
    #endregion

    #endregion

    #region ルームの情報を取得・UIに反映
    public IEnumerator ShowRoomInfo()
    {
        yield return new WaitWhile(() => !PhotonNetwork.IsConnectedAndReady);
        yield return new WaitWhile(() => !PhotonNetwork.InRoom);

        #region ルーム名
        string RoomName = PhotonNetwork.CurrentRoom.Name;
        RoomIDText.text = RoomName;
        #endregion
    }
    #endregion

    #endregion

    #region バトルデッキを登録
    public IEnumerator SignUpBattleDeck(DeckData deckData)
    {
        ContinuousController.instance.BattleDeckData = deckData;

        bool IsSignUpDeckData()
        {
            if (deckData != null)
            {
                if (ContinuousController.instance.DeckDatas != null)
                {
                    if (ContinuousController.instance.DeckDatas.Contains(deckData))
                    {
                        if (deckData.IsValidDeckData())
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        if(IsSignUpDeckData())
        {
            #region デッキデータをカスタムプロパティに保存
            yield return ContinuousController.instance.StartCoroutine(PhotonUtility.SignUpBattleDeckData());
            #endregion
        }

        else
        {
            #region デッキデータのカスタムプロパティを削除
            yield return ContinuousController.instance.StartCoroutine(PhotonUtility.DeleteBattleDeckData());
            #endregion
        }
    }
    #endregion

    #region ルーム画面を閉じる
    public void CloseRoom()
    {
        ContinuousController.instance.StartCoroutine(CloseRoomCoroutine());
    }

    public IEnumerator CloseRoomCoroutine()
    {
        yield return ContinuousController.instance.StartCoroutine(loadingObject.StartLoading("Now Loading"));

        Off();

        isReady = false;
        endSetUp = false;

        SetReadyProperty(PhotonNetwork.LocalPlayer, isReady);

        #region ルームから退出
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }

        yield return new WaitWhile(() => PhotonNetwork.InRoom);
        #endregion

        yield return ContinuousController.instance.StartCoroutine(loadingObject.EndLoading());
    }

    public void Off()
    {
        //this.gameObject.SetActive(false);
        Parent.SetActive(false);
    }
    #endregion

    #region 準備完了を表すプロパティのキー
    public static string ReadyKey()
    {
        if(PhotonNetwork.InRoom)
        {
            return "IsReady" + PhotonNetwork.CurrentRoom.Name;
        }

        return "IsReady";// + PhotonNetwork.CurrentRoom.Name;
    }
    #endregion

    #region 準備完了ボタンが押された時の処理
    public void GetReady()
    {
        isReady = !isReady;

        SetReadyProperty(PhotonNetwork.LocalPlayer, isReady);

        photonView.RPC("DestroyChildObject", RpcTarget.All);
        photonView.RPC("GetPlayers", RpcTarget.All);
    }
    #endregion

    #region 準備完了を表すプロパティを設定する
    void SetReadyProperty(Photon.Realtime.Player player,bool _isReady)
    {
        Hashtable PlayerProp = player.CustomProperties;

        object value;

        if (PlayerProp.TryGetValue(ReadyKey(), out value))
        {
            PlayerProp[ReadyKey()] = _isReady;
        }

        else
        {
            PlayerProp.Add(ReadyKey(), _isReady);
        }

        player.SetCustomProperties(PlayerProp);
    }
    #endregion

    #region ルームにいるプレイヤー全員が準備完了か判定
    public bool AllPlayerIsReady()
    {
        foreach (var p in PhotonNetwork.PlayerList)
        {
            object value;

            if (p.CustomProperties.TryGetValue(ReadyKey(), out value))
            {
                if (!(bool)value)
                {
                    return false;
                }
            }

            else
            {
                return false;
            }
        }

        return true;
    }
    #endregion

    #region プレイヤーの人数・待機状態を監視→OKならバトル開始
    bool DoneStartBattle;
    public void CheckPlayerState()
    {
        if (PhotonNetwork.InRoom && endSetUp)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers && AllPlayerIsReady())
            {
                if (PhotonNetwork.IsMasterClient && !DoneStartBattle)
                {
                    DoneStartBattle = true;

                    Hashtable RoomProp = PhotonNetwork.CurrentRoom.CustomProperties;

                    photonView.RPC("GoToBattleScene", RpcTarget.All);
                }

            }

            else if (playerCount != PhotonNetwork.CurrentRoom.PlayerCount)
            {
                playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
                DestroyChildObject();//PlayerElementを削除
                GetPlayers();
            }
        }
    }
    #endregion

    #region バトル開始
    [PunRPC]
    public void GoToBattleScene()
    {
        StartCoroutine(GoToBattleSceneCoroutine());
    }

    IEnumerator GoToBattleSceneCoroutine()
    {
        yield return ContinuousController.instance.StartCoroutine(Opening.instance.LoadingObject_Unload.StartLoading("Now Loading"));
        object value;

        DoneStartBattle = true;

        Hashtable PlayerProp = PhotonNetwork.LocalPlayer.CustomProperties;

        List<DictionaryEntry> dictionaryEntries = new List<DictionaryEntry>();

        foreach (DictionaryEntry dictionaryEntry in PlayerProp)
        {
            dictionaryEntries.Add(dictionaryEntry);
        }

        foreach (DictionaryEntry dictionaryEntry in dictionaryEntries)
        {
            if (dictionaryEntry.Key.ToString().Contains("PhotonWaitController"))
            {
                if (PlayerProp.TryGetValue(dictionaryEntry.Key.ToString(), out value))
                {
                    PlayerProp.Remove(dictionaryEntry.Key.ToString());
                }
            }
        }

        if (PlayerProp.TryGetValue("isBattle", out value))
        {
            PlayerProp["isBattle"] = true;
        }

        else
        {
            PlayerProp.Add("isBattle", true);
        }

        if (PlayerProp.TryGetValue(ReadyKey(), out value))
        {
            PlayerProp[ReadyKey()] = false;
        }

        else
        {
            PlayerProp.Add(ReadyKey(), false);
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(PlayerProp);
        yield return new WaitForSeconds(0.1f);

        //Hashtable hashtable = new Hashtable();
        //PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
        yield return new WaitForSeconds(0.1f);

        Opening.instance.MainCamera.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);

        SceneManager.LoadSceneAsync("BattleScene", LoadSceneMode.Additive);
        yield return ContinuousController.instance.StartCoroutine(Opening.instance.LoadingObject_Unload.EndLoading());
    }
    #endregion

    #region ルームのプレイヤーの情報に対応してPlayerElementを作成
    [PunRPC]
    public void GetPlayers()
    {
        if (PhotonNetwork.PlayerList == null || PhotonNetwork.CurrentRoom.PlayerCount == 0)
        {
            return;
        }

        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            GameObject PlayerElement = Instantiate(PlayerElementPrefab, PlayerParent.transform);

            object value;

            string PlayerName = "Player";

            #region プレイヤー名をカスタムプロパティから取得
            Hashtable hash = PhotonNetwork.PlayerList[i].CustomProperties;

            if (hash.TryGetValue(ContinuousController.PlayerNameKey, out value))
            {
                PlayerName = (string)value;
            }
            #endregion

            if (PhotonNetwork.PlayerList[i].CustomProperties.TryGetValue(ReadyKey(), out value))
            {
                PlayerElement.GetComponent<CPlayerElement>().SetPlayerInfo(PlayerName, (bool)value);
            }

            else
            {
                PlayerElement.GetComponent<CPlayerElement>().SetPlayerInfo(PlayerName, false);
            }
        }
    }
    #endregion

    #region 表示されているPlayerElementを削除
    [PunRPC]
    public void DestroyChildObject()
    {
        for (int i = 0; i < PlayerParent.transform.childCount; ++i)
        {
            Destroy(PlayerParent.transform.GetChild(i).gameObject);
        }
    }
    #endregion

    #region 準備完了ボタンのチェック
    public void CheckReadyButton()
    {
        bool oldIsReady = isReady;

        if (CanReady())
        {
            ReadyButtonCover.SetActive(false);

            if (isReady)
            {
                ReadyButtonText.text = "In preparation";
            }

            else
            {
                ReadyButtonText.text = "Get ready";
            }
        }

        else
        {
            ReadyButtonCover.SetActive(true);
            ReadyButtonText.text = "Get ready";
            isReady = false;
            SetReadyProperty(PhotonNetwork.LocalPlayer, isReady);

            if(oldIsReady)
            {
                photonView.RPC("DestroyChildObject", RpcTarget.All);
                photonView.RPC("GetPlayers", RpcTarget.All);
            }
        }
    }

    bool CanReady()
    {
        if(!DoneStartBattle)
        {
            if (ContinuousController.instance.BattleDeckData != null && ContinuousController.instance.DeckDatas != null)
            {
                if (ContinuousController.instance.DeckDatas.Contains(ContinuousController.instance.BattleDeckData))
                {
                    if (ContinuousController.instance.BattleDeckData.IsValidDeckData())
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
    #endregion

    #region バトルデッキ選択開始
    bool OldIsReady = false;
    public void OnClickSelectBattleDeckButton()
    {
        selectBattleDeck.SetUpSelectBattleDeck(selectBattleDeck.OnClickSelectButton_RoomMatch);

        OldIsReady = isReady;

        isReady = false;

        SetReadyProperty(PhotonNetwork.LocalPlayer, isReady);

        photonView.RPC("DestroyChildObject", RpcTarget.All);
        photonView.RPC("GetPlayers", RpcTarget.All);
    }
    #endregion

    #region バトルデッキ選択終了
    public void EndSelectBattleDeck()
    {
        ContinuousController.instance.StartCoroutine(EndSelectBattleDeckCoroutine());
    }

    IEnumerator EndSelectBattleDeckCoroutine()
    {
        if (this.gameObject.activeSelf)
        {
            isReady = OldIsReady;

            SetReadyProperty(PhotonNetwork.LocalPlayer, isReady);

            yield return ContinuousController.instance.StartCoroutine(SignUpBattleDeck(ContinuousController.instance.BattleDeckData));

            photonView.RPC("DestroyChildObject", RpcTarget.All);
            photonView.RPC("GetPlayers", RpcTarget.All);
        }
    }
    #endregion

    #region デッキ情報パネルを表示
    public void SetDeckInfoPanel()
    {
        if (ContinuousController.instance.BattleDeckData == null)
        {
            deckInfoPanel.SetUpDeckInfoPanel(DeckData.EmptyDeckData());

            NoDeckSetObject.SetActive(true);
            InvalidDeckObject.SetActive(false);
        }

        else
        {
            deckInfoPanel.SetUpDeckInfoPanel(ContinuousController.instance.BattleDeckData);

            NoDeckSetObject.SetActive(false);
            InvalidDeckObject.SetActive(!ContinuousController.instance.BattleDeckData.IsValidDeckData());
        }
    }
    #endregion

    #region ルームIDコピーボタンを押したときの処理
    public void OnClickCopyRoomIDButton()
    {
        if(PhotonNetwork.InRoom)
        {
            #region クリップボードにデッキコードをコピー
            GUIUtility.systemCopyBuffer = PhotonNetwork.CurrentRoom.Name;
            #endregion

            List<UnityAction> Commands = new List<UnityAction>()
            {
                null
            };

            List<string> CommandTexts = new List<string>()
            {
                "OK"
            };

            yesNoObject.SetUpYesNoObject(Commands, CommandTexts, $"The Room ID was copied to the clipboard!", false);
        }
    }
    #endregion

    public void Update()
    {
        if(GManager.instance == null)
        {
            if (endSetUp && !DoneStartBattle)
            {
                SetDeckInfoPanel();
                CheckReadyButton();
                CheckPlayerState();
            }
        }
    }
}