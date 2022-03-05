using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System;
using System.Linq;
using UnityEngine.Networking;
using System.IO;
using UnityEngine.Events;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ContinuousController : MonoBehaviour
{
    [Header("ゲームのVer")]
    public float GameVer;

    [Header("ゲームに登場するカードリスト")]
    public List<CEntity_Base> CardList = new List<CEntity_Base>();

    [Header("カード裏面")]
    public Sprite ReverseCard;
    public Sprite ReverseCard_Mask;

    [Header("SEプレハブ")]
    public SoundObject soundObject;

    public DeckData BattleDeckData { get; set; }

    public bool NeedUpdate { get; set; }

    public bool isRandomMatch { get; set; }

    #region バトル用デッキデータを保存するプロパティのキー
    public static string DeckDataPropertyKey
    {
        get
        {
            return "BattleDeckData";
        }
    }
    #endregion

    #region プレイヤー名データを保存するプロパティのキー
    public static string PlayerNameKey
    {
        get
        {
            return "PlayerNameKey";
        }
    }
    #endregion

    #region 勝利数データを保存するプロパティのキー
    public static string WinCountKey
    {
        get
        {
            return "WinCountKey";
        }
    }
    #endregion

    [Header("プレイヤー名文字数制限")]
    public int PlayerNameMaxLength;

    #region データ保存用シーンを呼び出す
    public static IEnumerator LoadCoroutine()
    {
        if (ContinuousController.instance == null)
        {
            SceneManager.LoadSceneAsync("ContinuousControllerScene", LoadSceneMode.Additive);

            while (ContinuousController.instance == null || DataBase.instance == null)
            {
                yield return null;
            }

            ContinuousController.instance.Init();
        }
    }
    #endregion

    #region デッキレシピのリスト
    public List<DeckData> DeckDatas = new List<DeckData>();
    #endregion

    #region デッキレシピのキー
    public string DeckDatasPlayerPrefsKey { get { return "DeckDatas3"; } }
    #endregion

    public static ContinuousController instance = null;

    private void Awake()
    {
        instance = this;
    }

    public void Init()
    {
        PlayerPrefs.DeleteKey("DeckDatas");
        PlayerPrefs.DeleteKey("DeckDatas2");

        UnityEngine.Random.InitState(RandomUtility.getRamdom());

        DeckDatas = PlayerPrefsUtil.LoadList<DeckData>(DeckDatasPlayerPrefsKey);

        GetComponent<StarterDeck>().SetStarterDecks();
        LoadLanguage();
        SaveDeckDatas();

        LoadPlayerName();
        LoadWinCount();

        LoadVolume();

        DontDestroyOnLoad(gameObject);
    }

    public void SaveDeckDatas()
    {
        PlayerPrefsUtil.SaveList(DeckDatasPlayerPrefsKey, DeckDatas);

        PlayerPrefs.Save();
    }

    #region プレイヤー名
    string playerName;
    public string PlayerName
    {
        get
        {
            if (string.IsNullOrEmpty(playerName))
            {
                return "Player";
            }
            return playerName;
        }

        set
        {
            playerName = value;
        }
    }

    public void SavePlayerName(string PlayerName)
    {
        this.PlayerName = PlayerName;
        PlayerPrefs.SetString("PlayerName", PlayerName);
        PlayerPrefs.Save();
    }
    public void LoadPlayerName()
    {
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            PlayerName = PlayerPrefs.GetString("PlayerName");
        }


        if (string.IsNullOrEmpty(PlayerName))
        {
            PlayerName = "プレイヤー";
        }
    }
    #endregion

    #region 勝利数
    public int WinCount { get; set; }

    public void SaveWinCount()
    {
        PlayerPrefs.SetInt("WinCount", WinCount);
        PlayerPrefs.Save();
    }
    public void LoadWinCount()
    {
        if (PlayerPrefs.HasKey("WinCount"))
        {
            WinCount = PlayerPrefs.GetInt("WinCount");
        }

    }
    #endregion

    #region 音量
    public float BGMVolume { get; set; }
    public float SEVolume { get; set; }

    public void SetBGMVolume(float BGMVolume)
    {
        this.BGMVolume = BGMVolume;

        PlayerPrefs.SetFloat("BGMVolume", BGMVolume);
        PlayerPrefs.Save();
    }

    public void SetSEVolume(float SEVolume)
    {
        this.SEVolume = SEVolume;

        PlayerPrefs.SetFloat("SEVolume", SEVolume);
        PlayerPrefs.Save();
    }

    public void ChangeBGMVolume(AudioSource audioSource)
    {
        audioSource.volume = BGMVolume * 0.25f * 0.8f;
    }

    public void ChangeSEVolume(AudioSource audioSource)
    {
        audioSource.volume = SEVolume * 0.5f * 0.8f;
    }

    public void LoadVolume()
    {
        BGMVolume = 0.5f;
        SEVolume = 0.5f;

        if (PlayerPrefs.HasKey("BGMVolume"))
        {
            BGMVolume = PlayerPrefs.GetFloat("BGMVolume");
        }

        if (PlayerPrefs.HasKey("SEVolume"))
        {
            SEVolume = PlayerPrefs.GetFloat("SEVolume");
        }
    }
    #endregion

    #region PlaySE(AudioClip clip)
    public SoundObject PlaySE(AudioClip clip)
    {
        SoundObject _soundObject = Instantiate(soundObject);

        _soundObject.PlaySE(clip);

        return _soundObject;
    }
    #endregion

    public BGMObject bGMObject;

    public Coroutine LoadingTextCoroutine;

    bool endBattle = false;
    public void EndBattle()
    {
        if(!endBattle)
        {
            endBattle = true;
            StartCoroutine(EndBattleCoroutine());
        }
    }

    public IEnumerator EndBattleCoroutine()
    {
        if(Opening.instance == null)
        {
            yield break;
        }

        isAI = false;
        UnityEngine.Random.InitState(RandomUtility.getRamdom());
        var unload = SceneManager.UnloadSceneAsync("BattleScene");
        yield return unload;

        yield return Resources.UnloadUnusedAssets();

        yield return StartCoroutine(Opening.instance.LoadingObject_Unload.StartLoading("Now Loading"));
        Opening.instance.MainCamera.gameObject.SetActive(true);
        Opening.instance.LoadingObject_light.gameObject.SetActive(false);
        yield return ContinuousController.instance.StartCoroutine(PhotonUtility.SetPlayerName());

        if (isRandomMatch)
        {
            Debug.Log("ランダムマッチからアンロード");
            yield return StartCoroutine(Opening.instance.battle.lobbyManager_RandomMatch.CloseLobbyCoroutine());
            yield return StartCoroutine(Opening.instance.battle.selectBattleMode.SetUpSelectBattleModeCoroutine());
        }

        else
        {
            Debug.Log("ルームマッチからアンロード");
            yield return StartCoroutine(Opening.instance.battle.roomManager.Init(true));
            yield return new WaitForSeconds(0.1f);
        }

        Opening.instance.LoadingObject.gameObject.SetActive(false);
        yield return StartCoroutine(Opening.instance.LoadingObject_Unload.EndLoading());
        endBattle = false;

        if(!isRandomMatch)
        {
            Hashtable PlayerProp = PhotonNetwork.LocalPlayer.CustomProperties;

            if (PlayerProp.TryGetValue("isBattle", out object value))
            {
                PlayerProp["isBattle"] = false;
            }

            else
            {
                PlayerProp.Add("isBattle", false);
            }

            PhotonNetwork.LocalPlayer.SetCustomProperties(PlayerProp);
        }
    }

    int count = 0;
    int UpdateFrame = 40;
    void LateUpdate()
    {
        #region 数フレームに一回だけ更新
        count++;

        if (count < UpdateFrame)
        {
            return;
        }

        else
        {
            count = 0;
        }
        #endregion

        if (PhotonNetwork.InRoom)
        {
            if (!isAI)
            {
                bool notEnterOther = false;

                if (PhotonNetwork.PlayerList.Length == 1)
                {
                    if (GManager.instance != null)
                    {
                        notEnterOther = true;
                    }
                }

                if (notEnterOther)
                {
                    if(PhotonNetwork.CurrentRoom.MaxPlayers != 1)
                    {
                        PhotonNetwork.CurrentRoom.MaxPlayers = 1;
                    }
                }

                else 
                {
                    if (PhotonNetwork.CurrentRoom.MaxPlayers != 2)
                    {
                        PhotonNetwork.CurrentRoom.MaxPlayers = 2;
                    }
                }
            }
        }
    }

    public bool isAI  = false;

    public Language language { get; set; }

    public void SaveLanguage()
    {
        if(language == Language.JPN)
        {
            PlayerPrefs.SetInt("Language", 0);
        }

        else
        {
            PlayerPrefs.SetInt("Language", 1);
        }
    }

    public void LoadLanguage()
    {
        if(PlayerPrefs.HasKey("Language"))
        {
            if(PlayerPrefs.GetInt("Language") == 0)
            {
                language = Language.JPN;
            }

            else if(PlayerPrefs.GetInt("Language") == 1)
            {
                language = Language.ENG;
            }
        }
    }

    //乱数列の共有が終わったフラグ
    public bool DoneSetRandom = false;

    [PunRPC]
    public void SetRandom(int random)
    {
        UnityEngine.Random.InitState(random);
        DoneSetRandom = true;
    }
}

public enum Language
{
    JPN,
    ENG,
}

#region 乱数を管理
public static class RandomUtility
{
    private static System.Random random;
    public static int getRamdom()
    {
        int _max = 1500000000;

        if (random == null)
        {
            random = new System.Random((int)DateTime.Now.Ticks);
        }

        return random.Next(0, _max);
    }

    #region IsSucceedProbability(float Probability)
    public static bool IsSucceedProbability(float Probability)
    {
        if (Probability >= 1)
        {
            return true;
        }

        if (Probability <= 0)
        {
            return false;
        }

        float random = UnityEngine.Random.Range(0f, 1f);

        if (random <= Probability)
        {
            return true;
        }

        return false;
    }
    #endregion

    #region デッキをシャッフル
    public static List<CEntity_Base> ShuffledDeckCards(List<CEntity_Base> DeckCards)
    {
        List<CEntity_Base> CardDatas = new List<CEntity_Base>();

        foreach (CEntity_Base cEntity_Base in DeckCards)
        {
            CardDatas.Add(cEntity_Base);
        }

        // 整数 n の初期値はデッキの枚数
        int n = CardDatas.Count;

        for (int i = 0; i < 20; i++)
        {
            // nが1より小さくなるまで繰り返す
            while (n > 1)
            {
                n--;

                // kは 0 ～ n+1 の間のランダムな値
                int k = UnityEngine.Random.Range(0, n + 1);

                // k番目のカードをtempに代入
                CEntity_Base temp = CardDatas[k];
                CardDatas[k] = CardDatas[n];
                CardDatas[n] = temp;
            }
        }


        return CardDatas;
    }

    public static List<CardSource> ShuffledDeckCards(List<CardSource> DeckCards)
    {
        List<CardSource> CardDatas = new List<CardSource>();

        foreach (CardSource cardSource in DeckCards)
        {
            CardDatas.Add(cardSource);
        }

        // 整数 n の初期値はデッキの枚数
        int n = CardDatas.Count;

        for (int i = 0; i < 20; i++)
        {
            // nが1より小さくなるまで繰り返す
            while (n > 1)
            {
                n--;

                // kは 0 ～ n+1 の間のランダムな値
                int k = UnityEngine.Random.Range(0, n + 1);

                // k番目のカードをtempに代入
                CardSource temp = CardDatas[k];
                CardDatas[k] = CardDatas[n];
                CardDatas[n] = temp;
            }
        }


        return CardDatas;
    }

    #endregion
}
#endregion

#region Photonへの接続を管理
public class PhotonUtility
{
    #region Photonから切断
    public static IEnumerator DisconnectCoroutine()
    {
        #region ルームから退出
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }

        yield return new WaitWhile(() => PhotonNetwork.InRoom);
        #endregion

        #region ロビーから退出
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        yield return new WaitWhile(() => PhotonNetwork.InLobby);
        #endregion

        #region Photonから切断
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }

        yield return new WaitWhile(() => PhotonNetwork.IsConnected);
        #endregion
    }
    #endregion

    #region Photonサーバー・ロビーに接続
    public static IEnumerator ConnectCoroutine()
    {
        #region Photonに接続
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.NickName = ContinuousController.instance.PlayerName;
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = ContinuousController.instance.GameVer.ToString();
        }

        yield return new WaitWhile(() => !PhotonNetwork.IsConnectedAndReady);
        #endregion

        #region プレイヤー名をカスタムプロパティに保存
        /*
        Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;

        object value;

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
        */

        yield return ContinuousController.instance.StartCoroutine(SetPlayerName());
        #endregion

        #region 勝利数をカスタムプロパティに保存
        Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;

        object value;

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

        #region ロビーに接続
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        yield return new WaitWhile(() => !PhotonNetwork.InLobby);

        yield return new WaitUntil(() => PhotonNetwork.InLobby && PhotonNetwork.IsConnectedAndReady);
        #endregion
    }
    #endregion

    #region プレイヤー名をプロパティに保存
    public static IEnumerator SetPlayerName()
    {
        Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;

        object value;

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
    }
    #endregion

    #region デッキデータをカスタムプロパティに保存
    public static IEnumerator SignUpBattleDeckData()
    {
        
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
    }
    #endregion

    #region デッキデータのカスタムプロパティを削除
    public static IEnumerator DeleteBattleDeckData()
    {
        Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;

        if (hash.TryGetValue(ContinuousController.DeckDataPropertyKey, out object value))
        {
            hash.Remove(ContinuousController.DeckDataPropertyKey);
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        while (true)
        {
            Hashtable _hash = PhotonNetwork.LocalPlayer.CustomProperties;

            if (!_hash.TryGetValue(ContinuousController.DeckDataPropertyKey, out value))
            {
                break;
            }

            yield return null;
        }
    }
    #endregion
}
#endregion