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
public class LobbyManager_RandomMatch : MonoBehaviourPunCallbacks
{
    [Header("メッセージテキスト")]
    public Text MessageText;

    [Header("時間テキスト")]
    public Text TimeText;

    [Header("タイトル画面に戻るボタン")]
    public GameObject ReturnButton;

    [Header("デッキ情報パネル")]
    public DeckInfoPanel deckInfoPanel;

    //BattleRule battleRule = BattleRule.Unlimited;

    public LoadingObject loadingObject;

    //既にあるランダムマッチ部屋の部屋名
    string RandomRoomName;

    bool endLoadingText = false;

    //ランダムマッチ部屋に必ず含まれる文字列
    string RandomKey
    {
        get
        {
            return "ランダムマッチ部屋";
        }
    }

    //入室処理中かどうか
    bool startJoin = false;

    #region ロビー画面を開く
    public void SetUpLobby()
    {
        ContinuousController.instance.isAI = false;
        ContinuousController.instance.isRandomMatch = true;
        this.gameObject.SetActive(true);
        //this.battleRule = battleRule;
        ContinuousController.instance.StartCoroutine(ConnectCoroutine());
    }
    #endregion

    #region ロビー画面を閉じる
    public void OffLobby()
    {
        this.gameObject.SetActive(false);
    }
    #endregion

    #region ロビー画面から出る
    public void CloseLobby()
    {
        ContinuousController.instance.StartCoroutine(CloseLobbyCoroutine());
    }
    public IEnumerator CloseLobbyCoroutine()
    {
        yield return ContinuousController.instance.StartCoroutine(loadingObject.StartLoading("Now Loading"));
        ReturnButton.SetActive(false);

        once1 = true;
        DoneCompleteMatching = true;

        m = true;
        n = true;

        #region ルームから退出
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }

        yield return new WaitWhile(() => PhotonNetwork.InRoom);
        #endregion

        OffLobby();

        yield return ContinuousController.instance.StartCoroutine(loadingObject.EndLoading());
    }
    #endregion

    #region 初期化
    public IEnumerator Init()
    {
        n = false;
        m = false;
        once1 = false;
        endLoadingText = false;
        time = 0;
        RandomRoomName = "";
        startJoin = false;
        DoneCompleteMatching = false;
        ContinuousController.instance.LoadingTextCoroutine = null;
        ReturnButton.SetActive(false);
        MessageText.text = "";
        TimeText.gameObject.SetActive(false);
        timer = 0;
        count = true;

        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        yield return new WaitWhile(() => PhotonNetwork.InLobby);

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }

        yield return new WaitWhile(() => PhotonNetwork.IsConnected);
    }
    #endregion

    #region 時間テキストカウントアップ
    int time = 0;

    IEnumerator TimeCountUp()
    {
        time = 0;
        TimeText.gameObject.SetActive(true);

        while(!DoneCompleteMatching)
        {
            string min = (time / 60).ToString();
            string sec = (time % 60).ToString();

            if(min.Length == 1)
            {
                min = $"0{min}";
            }

            if (sec.Length == 1)
            {
                sec = $"0{sec}";
            }

            TimeText.text = $"{min}:{sec}";
            time++;

            yield return new WaitForSeconds(1f);
        }

        TimeText.gameObject.SetActive(false);
    }
    #endregion

    #region Photon,ロビーに接続
    IEnumerator ConnectCoroutine()
    {
        yield return StartCoroutine(loadingObject.StartLoading("Now Loading"));

        if(ContinuousController.instance.BattleDeckData != null)
        {
            deckInfoPanel.SetUpDeckInfoPanel(ContinuousController.instance.BattleDeckData);
        }
        
        if (ReturnButton != null)
        {
            ReturnButton?.SetActive(false);
        }

        /*
        if (ContinuousController.instance.LoadingTextCoroutine != null)
        {
            ContinuousController.instance.StopCoroutine(ContinuousController.instance.LoadingTextCoroutine);
        }
        */

        endLoadingText = true;

        yield return ContinuousController.instance.StartCoroutine(Init());

        yield return new WaitForSeconds(1.5f);

        endLoadingText = false;

        ContinuousController.instance.LoadingTextCoroutine = ContinuousController.instance.StartCoroutine(SetWaitingText("Connecting"));

        MessageText.transform.localPosition = new Vector3(-227, -240, 0);

        yield return ContinuousController.instance.StartCoroutine(PhotonUtility.ConnectCoroutine());

        yield return ContinuousController.instance.StartCoroutine(PhotonUtility.SignUpBattleDeckData());

        yield return new WaitForSeconds(1.5f);

        StartRandomMatch();

        yield return new WaitWhile(() => !PhotonNetwork.InRoom);

        yield return new WaitForSeconds(0.1f);

        yield return ContinuousController.instance.StartCoroutine(loadingObject.EndLoading());

        ContinuousController.instance.StartCoroutine(TimeCountUp());

        ReturnButton.SetActive(true);
    }
    #endregion

    #region タイトルに戻る
    public void ReturnToTitle()
    {
        SceneManager.LoadSceneAsync("Opening");

        DoneCompleteMatching = true;
    }
    #endregion

    #region ルームリスト更新時のコールバック
    bool m;
    bool n;
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if(this.gameObject.activeSelf)
        {
            if (!PhotonNetwork.InRoom && PhotonNetwork.InLobby && !m)
            {
                m = true;
                PhotonNetwork.LeaveLobby();
            }

            if (!PhotonNetwork.InRoom && PhotonNetwork.InLobby && n)
            {
                GetRandomMatcingRoom(roomList);
            }
        }
        
    }
    #endregion

    #region ランダムマッチング部屋があるか検索
    public void GetRandomMatcingRoom(List<RoomInfo> roomInfo)
    {
        if (startJoin)
        {
            return;
        }

        m = false;
        n = false;

        if (roomInfo == null || roomInfo.Count == 0)
        {
            return;
        }

        RandomRoomName = null;

        for (int i = 0; i < roomInfo.Count; i++)
        {
            int p = roomInfo[i].PlayerCount;
            string n = roomInfo[i].Name;
            int m = roomInfo[i].MaxPlayers;
            object c = roomInfo[i].CustomProperties["RoomCreator"];

            if (p != 0 && m != 0 && c != null)
            {
                if (n.Contains(RandomKey))
                {
                    RandomRoomName = n;
                }
            }
        }
    }
    #endregion

    #region ロビーから出た時のコールバック
    public override void OnLeftLobby()
    {
        if (this.gameObject.activeSelf)
        {
            if (m)
            {
                n = true;
                PhotonNetwork.JoinLobby();
            }
        }
            
    }
    #endregion

    #region 部屋を作成する処理
    public IEnumerator CreateRoomCoroutine(bool isRandomMatch)
    {
        yield return new WaitWhile(() => !PhotonNetwork.IsConnectedAndReady);
        yield return new WaitWhile(() => !PhotonNetwork.InLobby);

        //作成する部屋の設定
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = true;   //ロビーで見える部屋にする
        roomOptions.IsOpen = true;      //他のプレイヤーの入室を許可する
        roomOptions.PublishUserId = true;

        roomOptions.MaxPlayers = 2;

        //ルームカスタムプロパティで部屋作成者を表示させるため、作成者の名前を格納
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
        {
            { "RoomCreator",PhotonNetwork.NickName },

        };

        //ロビーにカスタムプロパティの情報を表示させる
        roomOptions.CustomRoomPropertiesForLobby = new string[]
        {
            "RoomCreator",
        };

        string RoomName = StringUtils.GeneratePassword_AlpahabetNum(8);

        if (isRandomMatch)
        {
            RoomName += RandomKey;        
        }

        //RoomName += battleRule.ToString();

        //部屋作成
        PhotonNetwork.CreateRoom(RoomName, roomOptions, null);

        while(!PhotonNetwork.InRoom)
        {
            yield return null;
        }

        /*
        if(ContinuousController.instance.LoadingTextCoroutine != null)
        {
            ContinuousController.instance.StopCoroutine(ContinuousController.instance.LoadingTextCoroutine);
        }
        */

        endLoadingText = true;

        yield return new WaitForSeconds(0.2f);

        endLoadingText = false;

        ContinuousController.instance.LoadingTextCoroutine = StartCoroutine(SetWaitingText("Matching"));

        MessageText.transform.localPosition = new Vector3(-180, -240, 0);

        if (ReturnButton != null)
        {
            ReturnButton?.SetActive(true);
        }
    }
    #endregion

    #region ロビー入室後にランダムマッチ開始
    public void StartRandomMatch()
    {
        if(ReturnButton != null)
        {
            ReturnButton?.SetActive(false);
        }

        //部屋が無ければ部屋を立てる
        if (String.IsNullOrEmpty(RandomRoomName))
        {
            StartCoroutine(CreateRoomCoroutine(true));
        }

        //部屋があれば入る
        else
        {
            startJoin = true;

            PhotonNetwork.JoinRoom(RandomRoomName);
        }
    }
    #endregion


    IEnumerator SetWaitingText(string defultString)
    {
        float waitTime = 0.18f;

        int count = 0;

        while (!endLoadingText)
        {
            count++;

            if (count >= 4)
            {
                count = 0;
            }

            MessageText.text = defultString;//"マッチング中";

            for (int i = 0; i < count; i++)
            {
                MessageText.text += ".";
            }

            yield return new WaitForSeconds(waitTime);
        }
    }

    bool count = true;
    float timer = 0;
    Button ReturnButtonButton;
    private void Start()
    {
        ReturnButtonButton = ReturnButton.transform.GetChild(1).GetComponent<Button>();
    }
    private void LateUpdate()
    {
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                if(PhotonNetwork.IsMasterClient)
                {
                    StartCoroutine(GoNextScene());
                }

                if (ReturnButtonButton != null)
                {
                    ReturnButtonButton.enabled = false;
                }
            }

            else
            {
                if (ReturnButtonButton != null)
                {
                    ReturnButtonButton.enabled = true;
                }
            }
        }

        if(DoneCompleteMatching)
        {
            if (ReturnButtonButton != null)
            {
                ReturnButton.SetActive(false);
            }
        }
    }


    #region マッチングが完了したら次のシーンに遷移する
    bool DoneCompleteMatching = false;
    bool once1 = false;
    IEnumerator GoNextScene()
    {
        if (DoneCompleteMatching || once1)
        {
            yield break;
        }

        once1 = true;
        yield return new WaitForSeconds(0.1f);

        PhotonNetwork.CurrentRoom.IsVisible = false;

        photonView.RPC("GoToBattleScene", RpcTarget.All);
    }
    #endregion

    #region ランダムでお互いにデッキを作成してバトルシーンに移行する
    void GetRandomDeck_GoBattleScene()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            List<CEntity_Base> DeckRecipie = new List<CEntity_Base>();

            while(DeckRecipie.Count < 40)
            {
                CEntity_Base RandomCEntity = ContinuousController.instance.CardList[UnityEngine.Random.Range(0, ContinuousController.instance.CardList.Count)];

                if(DeckRecipie.Count((CEntity) => CEntity == RandomCEntity) < 4)
                {
                    DeckRecipie.Add(RandomCEntity);
                }
            }

            List<int> DeckRecipie_CardID = new List<int>();

            foreach (CEntity_Base cardEntity in DeckRecipie)
            {
                DeckRecipie_CardID.Add(ContinuousController.instance.CardList.IndexOf(cardEntity));
            }

            Hashtable hashtable = player.CustomProperties;

            if (hashtable.TryGetValue(ContinuousController.DeckDataPropertyKey, out object value))
            {
                hashtable[ContinuousController.DeckDataPropertyKey] = DeckRecipie_CardID.ToArray();
            }

            else
            {
                hashtable.Add(ContinuousController.DeckDataPropertyKey, DeckRecipie_CardID.ToArray());
            }

            player.SetCustomProperties(hashtable);
        }
    }
    #endregion

    [PunRPC]
    public void GoToBattleScene()
    {
        if (DoneCompleteMatching)
        {
            return;
        }

        /*
        if (ContinuousController.instance.LoadingTextCoroutine != null)
        {
            ContinuousController.instance.StopCoroutine(ContinuousController.instance.LoadingTextCoroutine);
        }
        */

        endLoadingText = true;
        DoneCompleteMatching = true;

        TimeText.gameObject.SetActive(false);
        MessageText.text = "Matching completed!";
        MessageText.transform.localPosition = new Vector3(-400, -240, 0);
        ReturnButton.SetActive(false);
        
        ContinuousController.instance.StartCoroutine(GoToBattleSceneCoroutine());
    }

    IEnumerator GoToBattleSceneCoroutine()
    {
        Debug.Log("マッチング完了!");

        yield return new WaitForSeconds(0.1f);
        //yield return ContinuousController.instance.StartCoroutine(loadingObject.StartLoading("Now Loading"));
        Opening.instance.MainCamera.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadSceneAsync("BattleScene",LoadSceneMode.Additive);
        yield return null;
    }
}

#region ランダム文字列の生成
public static class StringUtils
{
    public static string GeneratePassword_AlpahabetNum(int length)
    {
        string PASSWORD_CHARS =
        "0123456789abcdefghijklmnopqrstuvwxyz";

        var sb = new System.Text.StringBuilder(length);
        var r = new System.Random();

        for (int i = 0; i < length; i++)
        {
            int pos = r.Next(PASSWORD_CHARS.Length);
            char c = PASSWORD_CHARS[pos];
            sb.Append(c);
        }

        return sb.ToString();
    }

    public static string GeneratePassword_Num(int length)
    {
        string PASSWORD_CHARS =
        "0123456789";

        var sb = new System.Text.StringBuilder(length);
        var r = new System.Random();

        for (int i = 0; i < length; i++)
        {
            int pos = r.Next(PASSWORD_CHARS.Length);
            char c = PASSWORD_CHARS[pos];
            sb.Append(c);
        }

        return sb.ToString();
    }
}
#endregion