using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Linq;
using System;

public class LobbyManager_FriendMatch : MonoBehaviourPunCallbacks
{
    public GameObject RoomParent;//ScrolViewのcontentオブジェクト
    public GameObject RoomElementPrefab;//部屋情報Prefab

    public GameObject CreateRoomButton;
    public Text MessageText;

    public static LobbyManager_FriendMatch instance = null;

    string RandomKey = "ランダムマッチ部屋";

    public OptionPanel optionPanel;

    public void ReturnToTitle()
    {
        SceneManager.LoadSceneAsync("Opening");
    }

    private void Awake()
    {
        MessageText.text = "接続中...";

        instance = this;

        optionPanel.Init();
    }

    private void Update()
    {
        if (PrivateLobby.activeSelf)
        {
            if (PhotonNetwork.InLobby)
            {
                CreateRoomButton.SetActive(true);
                ReturnButton.SetActive(true);
            }

            else
            {
                CreateRoomButton.SetActive(false);
                ReturnButton.SetActive(false);
            }
        }
    }

    #region GetRooms
    public void GetRooms(List<RoomInfo> roomInfo)
    {
        if (startJoin)
        {
            return;
        }

        m = false;
        n = false;
        if (roomInfo == null || roomInfo.Count == 0)
        {
            MessageText.text = "部屋がありません";

            return;
        }

        int roomCount = 0;

        for (int i = 0; i < roomInfo.Count; i++)
        {
            int p = roomInfo[i].PlayerCount;
            string n = roomInfo[i].Name;
            int m = roomInfo[i].MaxPlayers;
            object c = roomInfo[i].CustomProperties["RoomCreator"];

            if (p != 0 && m != 0 && c != null)
            {
                if (!n.Contains(RandomKey))
                {
                    roomCount++;
                    GameObject RoomElement = Instantiate(RoomElementPrefab, RoomParent.transform);
                    RoomElement.GetComponent<CRoomElement>().SetRoomInfo(roomInfo[i].Name, roomInfo[i].PlayerCount, roomInfo[i].CustomProperties["RoomCreator"].ToString());

                    RoomElement.GetComponent<CRoomElement>().OnClick = EnterRoom;

                    void EnterRoom()
                    {
                        if (!PhotonNetwork.InLobby)
                        {
                            return;
                        }

                        PrivateRoom.SetActive(true);
                        PrivateLobby.SetActive(false);


                        StartCoroutine(wait());

                        //roomnameの部屋に入室
                        PhotonNetwork.JoinRoom(RoomElement.GetComponent<CRoomElement>().RoomName);

                        MessageText.text = "2人が準備完了なら始まります";
                    }

                    IEnumerator wait()
                    {
                        ReturnButton.SetActive(false);

                        yield return new WaitWhile(() => !PhotonNetwork.InRoom);

                        ReturnButton.SetActive(true);
                    }
                }
            }
        }

        if (roomCount > 0)
        {
            MessageText.text = "部屋が" + roomCount + "個あります";
        }

    }

    //RoomElementを一括削除
    public static void DestroyChildObject(Transform parent_trans)
    {
        for (int i = 0; i < parent_trans.childCount; ++i)
        {

            Destroy(parent_trans.GetChild(i).gameObject);
        }
    }
    #endregion

    bool m;
    bool n;
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (!PhotonNetwork.InRoom && PhotonNetwork.InLobby && !m)
        {
            m = true;
            PhotonNetwork.LeaveLobby();
        }

        if (!PhotonNetwork.InRoom && PhotonNetwork.InLobby && n)
        {
            DestroyChildObject(RoomParent.transform);   //RoomElementを削除

            GetRooms(roomList);
        }

    }

    public override void OnLeftLobby()
    {
        if (m)
        {
            n = true;
            PhotonNetwork.JoinLobby();
        }

    }

    // マスターサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if (isWaitingRandom)
        {
            startJoin = false;

            PrivateLobby.SetActive(false);
            PrivateRoom.SetActive(false);
        }
    }

    public IEnumerator CreateRoomCoroutine()
    {
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
            { "RoomCreator",ContinuousController.instance.PlayerName },

        };

        //ロビーにカスタムプロパティの情報を表示させる
        roomOptions.CustomRoomPropertiesForLobby = new string[]
        {
            "RoomCreator",
        };

        string RoomName = StringUtils.GeneratePassword(8);

        //部屋作成
        PhotonNetwork.CreateRoom(RoomName, roomOptions, null);
    }

    bool OnceCreatedPrivateRoom = false;

    public void OnClick_CreateRoomButton()
    {
        if (OnceCreatedPrivateRoom)
        {
            return;
        }

        OnceCreatedPrivateRoom = true;

        StartCoroutine(CreateRoomCoroutine());

        StartCoroutine(Wait_CreatePrivateRoom());
    }

    IEnumerator Wait_CreatePrivateRoom()
    {
        PrivateLobby.SetActive(false);
        ReturnButton.SetActive(false);

        MessageText.text = "部屋作成中...";

        yield return new WaitWhile(() => !PhotonNetwork.InRoom);


        PrivateRoom.SetActive(true);
        ReturnButton.SetActive(true);

        MessageText.text = "2人が準備完了なら始まります";

        OnceCreatedPrivateRoom = false;
    }

    public static class StringUtils
    {
        private const string PASSWORD_CHARS =
            "0123456789abcdefghijklmnopqrstuvwxyz";

        public static string GeneratePassword(int length)
        {
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

    public GameObject PrivateLobby;
    public GameObject PrivateRoom;

    bool isWaitingRandom = false;
    bool startJoin = false;

    public GameObject ReturnButton;

    private void Start()
    {
        PrivateRoom.SetActive(false);
        PrivateLobby.SetActive(false);

        StartCoroutine(ConnectCoroutine());
    }

    IEnumerator ConnectCoroutine()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.NickName = ContinuousController.instance.PlayerName;
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = ContinuousController.instance.GameVer.ToString();
        }

        MessageText.text = "接続中...";
        yield return new WaitWhile(() => !PhotonNetwork.IsConnected);

        #region デッキデータをカスタムプロパティに保存
        Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;

        if (hash.TryGetValue(ContinuousController.DeckDataPropertyKey, out object value))
        {
            hash[ContinuousController.DeckDataPropertyKey] = ContinuousController.instance.BattleDeckData.GetThisDeckCode();
        }

        else
        {
            hash.Add(ContinuousController.DeckDataPropertyKey, ContinuousController.instance.BattleDeckData.GetThisDeckCode());
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        while (true)
        {
            Hashtable _hash = PhotonNetwork.LocalPlayer.CustomProperties;

            if (_hash.TryGetValue(ContinuousController.DeckDataPropertyKey, out value))
            {
                if ((string)value == ContinuousController.instance.BattleDeckData.GetThisDeckCode())
                {
                    break;
                }
            }

            yield return null;
        }
        #endregion

        #region プレイヤー名をカスタムプロパティに保存
        hash = PhotonNetwork.LocalPlayer.CustomProperties;

        if (hash.TryGetValue(ContinuousController.PlayerNameKey, out value))
        {
            hash[ContinuousController.PlayerNameKey] = ContinuousController.instance.PlayerName;
        }

        else
        {
            hash.Add(ContinuousController.PlayerNameKey, ContinuousController.instance.PlayerName);
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        while (true)
        {
            Hashtable _hash = PhotonNetwork.LocalPlayer.CustomProperties;

            if (_hash.TryGetValue(ContinuousController.PlayerNameKey, out value))
            {
                if ((string)value == ContinuousController.instance.PlayerName)
                {
                    break;
                }
            }

            yield return null;
        }
        #endregion

        #region 勝利数をカスタムプロパティに保存
        hash = PhotonNetwork.LocalPlayer.CustomProperties;

        if (hash.TryGetValue(ContinuousController.WinCountKey, out value))
        {
            hash[ContinuousController.WinCountKey] = ContinuousController.instance.WinCount;
        }

        else
        {
            hash.Add(ContinuousController.WinCountKey, ContinuousController.instance.WinCount);
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        while (true)
        {
            Hashtable _hash = PhotonNetwork.LocalPlayer.CustomProperties;

            if (_hash.TryGetValue(ContinuousController.WinCountKey, out value))
            {
                if ((int)value == ContinuousController.instance.WinCount)
                {
                    break;
                }

            }

            yield return null;
        }
        #endregion

        yield return new WaitWhile(() => !PhotonNetwork.InLobby);
        MessageText.text = "";
        PrivateRoom.SetActive(false);
        PrivateLobby.SetActive(true);

        
    }
}