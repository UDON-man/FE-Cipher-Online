using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateNewDeckButton : MonoBehaviour
{
    public EditDeck editDeck;
    public SelectDeck selectDeck;
    public GameObject Outline;

    public YesNoObject CreateNewDeckWayObject;

    public YesNoObject yesNoObject;

    private void Start()
    {
        CreateNewDeckWayObject.Off();
    }

    #region 新デッキの作成
    public void CreateNewDeck()
    {
        DeckData deckData = new DeckData(DeckData.GetDeckCode("NewDeck", null, new List<CEntity_Base>()));

        ContinuousController.instance.DeckDatas.Insert(0, deckData);

        editDeck.SetUpCreateDeck(deckData,true);

        ContinuousController.instance.StartCoroutine(selectDeck.SetDeckList());

        CreateNewDeckWayObject.Off();
    }
    #endregion

    #region デッキコードから作成ボタンが押された時の処理
    public void OnClickFromDeckCode()
    {
        CreateNewDeckWayObject.Off();

        string deckCode = GUIUtility.systemCopyBuffer;

        if(!DeckData.IsValidDeckCode(deckCode))
        {
            yesNoObject.SetUpYesNoObject(new List<UnityEngine.Events.UnityAction>() { null }, new List<string>() { "OK" }, "Error!\nDeck code could not be read.", true);
            return;
        }

        DeckData deckData = new DeckData(deckCode);

        if (deckData.DeckName == "新しいデッキ" || deckData.DeckName == "NewDeck")
        {
            deckData.DeckName = "NewDeck";
        }

        ContinuousController.instance.DeckDatas.Insert(0, deckData);

        editDeck.SetUpCreateDeck(deckData,true);

        ContinuousController.instance.StartCoroutine(selectDeck.SetDeckList());
    }
    #endregion

    public void OnClick()
    {
        selectDeck.deckInfoPanel.SetUpDeckInfoPanel(null);

        CreateNewDeckWayObject.Open();

        Outline.SetActive(true);

        for (int j = 0; j < this.transform.parent.childCount; j++)
        {
            if (this.transform.parent.GetChild(j) != this.transform)
            {
                if (this.transform.parent.GetChild(j).GetComponent<DeckInfoPrefab>() != null)
                {
                    this.transform.parent.GetChild(j).GetComponent<DeckInfoPrefab>().Outline.SetActive(false);
                }
            }
        }
    }

    public void OnEnter()
    {
        if (Opening.instance != null)
            this.gameObject.transform.localScale = Opening.instance.DeckInfoPrefabExpandScale;
    }

    public void OnExit()
    {
        if (Opening.instance != null)
            this.gameObject.transform.localScale = Opening.instance.DeckInfoPrefabStartScale;
    }

    private void OnEnable()
    {
        OnExit();
    }
}
