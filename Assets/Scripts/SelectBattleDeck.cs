using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Linq;

public class SelectBattleDeck : MonoBehaviour
{
    [Header("デッキ選択オブジェクト")]
    public GameObject SelectDeckObject;

    [Header("デッキ情報タブプレハブ")]
    public DeckInfoPrefab deckInfoPrefab;

    [Header("デッキ情報タブを置くScrollRect")]
    public ScrollRect deckInfoPrefabParentScroll;

    [Header("デッキ情報パネル親")]
    public GameObject deckInfoPanelParent;

    [Header("デッキ情報パネル")]
    public DeckInfoPanel deckInfoPanel;

    [Header("アニメーター")]
    public Animator anim;

    [Header("デッキ選択決定ボタン")]
    public Button SelectDeckButton;

    [Header("読み込み中オブジェクト")]
    public LoadingObject loadingObject;

    [Header("デッキ選択テキスト")]
    public Text SelectDeckTitleText;

    [Header("ランダムマッチ")]
    public LobbyManager_RandomMatch lobbyManager_RandomMatch;

    [Header("デッキ編集")]
    public EditDeck editDeck;

    [Header("無効なデッキ表示")]
    public GameObject InvalidDeckObject;
    //public BattleRule battleRule { get; set; }

    Image SelectDeckButtonImage;

    private void Start()
    {
        SelectDeckButtonImage = SelectDeckButton.GetComponent<Image>();
    }

    public void OnClickEditDeckButton()
    {
        editDeck.EndEditAction = () =>
        {
            SetSelectDeckButton();

            InvalidDeckObject.SetActive(!deckInfoPanel.ShowingDeckData.IsValidDeckData());
        };
    }

    public void SetSelectDeckButton()
    {
        SelectDeckButton.enabled = false;

        SelectDeckButtonImage.color = new Color32(144, 144, 144, 255);

        if (deckInfoPanel.ShowingDeckData != null)
        {
            if (deckInfoPanel.ShowingDeckData.DeckCardIDs != null)
            {
                if (deckInfoPanel.ShowingDeckData.IsValidDeckData())
                {
                    SelectDeckButton.enabled = true;

                    SelectDeckButtonImage.color = Color.white;
                }
            }
        }
    }

    bool once = false;
    public void OnClickSelectButton_RandomMatch()
    {
        if(once || deckInfoPanel.ShowingDeckData == null)
        {
            return;
        }

        ContinuousController.instance.StartCoroutine(SetOnce());

        ContinuousController.instance.BattleDeckData = deckInfoPanel.ShowingDeckData;

        lobbyManager_RandomMatch.SetUpLobby();
    }

    public void OnClickSelectButton_BotMatch()
    {
        if (once || deckInfoPanel.ShowingDeckData == null)
        {
            return;
        }

        ContinuousController.instance.StartCoroutine(SetOnce());

        ContinuousController.instance.BattleDeckData = deckInfoPanel.ShowingDeckData;
    }

    public void OnClickSelectButton_RoomMatch()
    {
        if (once || deckInfoPanel.ShowingDeckData == null)
        {
            return;
        }

        StartCoroutine(OnClickSelectButton_RoomMatchCoroutine());
    }

    IEnumerator OnClickSelectButton_RoomMatchCoroutine()
    {
        ContinuousController.instance.StartCoroutine(SetOnce());

        ContinuousController.instance.BattleDeckData = deckInfoPanel.ShowingDeckData;

        Off();

        yield return ContinuousController.instance.StartCoroutine(PhotonUtility.SignUpBattleDeckData());
    }

    IEnumerator SetOnce()
    {
        once = true;
        yield return new WaitForSeconds(1f);
        once = false;
    }

    public void Off()
    {
        this.transform.parent.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
    }

    public void CloseDeckInfo()
    {
        deckInfoPanelParent.GetComponent<Animator>().SetInteger("Open", 0);
        deckInfoPanelParent.GetComponent<Animator>().SetInteger("Close", 1);
    }

    public void SetUpSelectBattleDeck(UnityAction OnClickSelectButtonAction)
    {
        if (SelectDeckObject.activeSelf)
        {
            return;
        }

        //this.battleRule = battleRule;

        SelectDeckObject.transform.parent.gameObject.SetActive(true);
        SelectDeckObject.SetActive(true);

        deckInfoPanelParent.SetActive(false);

        CloseDeckInfo();
        anim.SetInteger("Open", 1);
        anim.SetInteger("Close", 0);

        SelectDeckTitleText.text = "Select Deck";

        /*
        switch (battleRule)
        {
            case BattleRule.JCS_Final:
                SelectDeckTitleText.text = "Select Deck - JCS Final";
                break;

            case BattleRule.Unlimited:
                SelectDeckTitleText.text = "Select Deck - Unlimited";
                break;
        }
        */

        SetDeckList();

        deckInfoPanel.OnClickSelectDeckAction = OnClickSelectButtonAction;
    }

    public void SetDeckList()
    {
        for (int i = 0; i < deckInfoPrefabParentScroll.content.childCount; i++)
        {
            Destroy(deckInfoPrefabParentScroll.content.GetChild(i).gameObject);
        }

        for (int i = 0; i < ContinuousController.instance.DeckDatas.Count; i++)
        {
            DeckInfoPrefab _deckInfoPrefab = Instantiate(deckInfoPrefab, deckInfoPrefabParentScroll.content);

            _deckInfoPrefab.scrollRect.content = deckInfoPrefabParentScroll.content;

            _deckInfoPrefab.scrollRect.viewport = deckInfoPrefabParentScroll.viewport;

            _deckInfoPrefab.scrollRect.verticalScrollbar = deckInfoPrefabParentScroll.verticalScrollbar;

            _deckInfoPrefab.SetUpDeckInfoPrefab(ContinuousController.instance.DeckDatas[i]);

            _deckInfoPrefab.OnClickAction = (deckdata) =>
            {
                deckInfoPanelParent.SetActive(true);

                deckInfoPanelParent.GetComponent<Animator>().SetInteger("Open", 1);
                deckInfoPanelParent.GetComponent<Animator>().SetInteger("Close", 0);

                deckInfoPanel.SetUpDeckInfoPanel(deckdata);

                SetSelectDeckButton();

                InvalidDeckObject.SetActive(!deckInfoPanel.ShowingDeckData.IsValidDeckData());

                Opening.instance.CreateOnClickEffect();
            };
        }
    }
}
