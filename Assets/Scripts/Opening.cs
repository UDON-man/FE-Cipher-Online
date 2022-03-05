using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using UnityEngine.UI;

public class Opening : MonoBehaviour
{
    [Header("読み込み中表示オブジェクト")]
    public LoadingObject LoadingObject;

    [Header("読み込み中表示オブジェクト(明るい)")]
    public LoadingObject LoadingObject_light;

    [Header("読み込み中表示オブジェクト_アンロード")]
    public LoadingObject LoadingObject_Unload;

    [Header("ホーム")]
    public HomeMode home;

    [Header("デッキ")]
    public DeckMode deck;

    [Header("バトル")]
    public BattleMode battle;

    [Header("ソーシャル")]
    public SocialMode social;

    [Header("クリック時エフェクト")]
    public GameObject OnClickEffect;

    [Header("キャンバス")]
    public RectTransform canvasRect;

    public CheckUpdate checkUpdate;

    public static Opening instance = null;

    public Text VerText;

    public ResizeWindow resizeWindow;

    public OptionPanel optionPanel;

    public AudioClip bgm;

    public GameObject ModeButtons;

    public Vector3 DeckInfoPrefabStartScale;

    public Vector3 DeckInfoPrefabExpandScale;

    public Camera MainCamera;

    public Title title;
    
    private void Awake()
    {
        instance = this;
    }

    public void OffModeButtons()
    {
        ModeButtons.SetActive(false);
    }

    public void OnModeButtons()
    {
        ModeButtons.SetActive(true);
    }

    public void CreateOnClickEffect()
    {
        GameObject effect = Instantiate(OnClickEffect, canvasRect.transform);

        var mousePos = Input.mousePosition;
        var magnification = canvasRect.sizeDelta.x / Screen.width;
        mousePos.x = mousePos.x * magnification - canvasRect.sizeDelta.x / 2;
        mousePos.y = mousePos.y * magnification - canvasRect.sizeDelta.y / 2;
        mousePos.z = 0;// -0.5f;// transform.localPosition.z;

        effect.transform.localPosition = mousePos;

        effect.transform.localRotation = Quaternion.EulerAngles(new Vector3(77, 0, 0));

        StartCoroutine(Effects.DeleteCoroutine(effect));
    }

    private void Start()
    {
        home.OffHome();

        deck.OffDeck();

        battle.OffBattle();

        social.offSocial();

        StartCoroutine(Init());
    }

    public IEnumerator Init()
    {
        yield return StartCoroutine(LoadingObject.StartLoading("Now Loading"));

        LoadingObject_light.gameObject.SetActive(false);
        LoadingObject_Unload.gameObject.SetActive(false);

        yield return StartCoroutine(ContinuousController.LoadCoroutine());

        home.SetUpHome();
        home.OffHome();
        

        optionPanel.Init();

        yield return StartCoroutine(deck.editDeck.InitEditDeck());

        if(PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();

            while(PhotonNetwork.IsConnected)
            {
                yield return null;
            }
        }

        yield return new WaitForSeconds(0.1f);

        

        VerText.text = $"Ver{ContinuousController.instance.GameVer}";

        ContinuousController.instance.bGMObject.StartPlayBGM(bgm);

        //yield return ContinuousController.instance.StartCoroutine(deck.selectDeck.SetDeckList());

        deck.SetUpDeckMode();

        yield return new WaitForSeconds(0.15f);

        deck.OffDeck();
        title.SetUpTitle();

        yield return StartCoroutine(LoadingObject.EndLoading());
    }
}
