using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;


public class DeckInfoPanel : MonoBehaviour
{
    [Header("デッキ情報パネルオブジェクト")]
    public GameObject DeckInfoPanelObject;

    [Header("キーカード画像")]
    public Image KeyCardImage;

    [Header("デッキ名InputField")]
    public InputField DeckName;

    [Header("デッキ名Text")]
    public Text DeckNameText;

    [Header("カード色カウント")]
    public List<CardColorCount> cardColorCountList = new List<CardColorCount>();

    //[Header("コストカウント")]
    //public List<CostCount> costCountList = new List<CostCount>();

    [Header("デッキ編集ボタン")]
    public Button EditDeckButton;

    [Header("デッキ削除ボタン")]
    public Button DeleteDeckButton;

    [Header("デッキ表示ボタン")]
    public Button ShowDeckButton;
    
    [Header("デッキコード取得ボタン")]
    public Button GetDeckCodeButton;

    [Header("YesNoパネル")]
    public YesNoObject yesNoObject;

    [Header("デッキ選択")]
    public SelectDeck selectDeck;

    [Header("バトルデッキ選択")]
    public SelectBattleDeck selectBattleDeck;

    [Header("デッキ編集")]
    public EditDeck editDeck;

    [Header("デッキカード一覧")]
    public DeckListPanel deckListPanel;

    [Header("デッキ枚数テキスト")]
    public Text DeckCountText;

    public bool isFromSelectDeck;

    public DeckData ShowingDeckData = null;

    private void Start()
    {
        if(yesNoObject != null)
        {
            yesNoObject.gameObject.SetActive(false);
        }
    }

    public void SetUpDeckInfoPanel(DeckData deckData)
    {
        DeckInfoPanelObject.SetActive(deckData != null);

        if (deckData != null)
        {
            if (GetComponent<Animator>() != null)
            {
                GetComponent<Animator>().SetInteger("Open", 1);
                GetComponent<Animator>().SetInteger("Close", 0);
            }

            ShowingDeckData = deckData;

            //キーカード画像
            KeyCardImage.gameObject.SetActive(deckData.MainCharacter != null);

            if (deckData.MainCharacter != null)
            {
                KeyCardImage.sprite = deckData.MainCharacter.CardImage;
            }

            //デッキ名
            if(DeckName != null)
            {
                DeckName.text = deckData.DeckName;
            }

            if(DeckNameText != null)
            {
                DeckNameText.text = deckData.DeckName;

                DeckNameText.transform.parent.gameObject.SetActive(deckData.GetThisDeckCode() != DeckData.EmptyDeckData().GetThisDeckCode());
            }

            //各色の枚数
            if(cardColorCountList != null)
            {
                if(cardColorCountList.Count > 0)
                {
                    foreach (CardColorCount cardColorCount in cardColorCountList)
                    {
                        if(cardColorCount != null)
                        {
                            cardColorCount.SetUpCardColorCount(deckData);

                            ContinuousController.instance.StartCoroutine(cardColorCount.SetIconScale());
                        }
                    }
                }
            }
            

            //デッキ枚数
            if(DeckCountText != null)
            {
                DeckCountText.text = $"{deckData.DeckCards().Count}/50";

                if (deckData.IsValidDeckData())
                {
                    DeckCountText.color = new Color32(69, 255, 69, 255);
                }

                else
                {
                    DeckCountText.color = new Color32(255, 64, 64, 255);
                }
            }

            //デッキコード取得ボタン
            if(GetDeckCodeButton != null)
            {
                GetDeckCodeButton.gameObject.SetActive(deckData.IsValidDeckData());
            }
        }
    }

    public void OnClickEditButton()
    {
        if (ShowingDeckData != null)
        {
            editDeck.SetUpCreateDeck(ShowingDeckData,isFromSelectDeck);
        }
    }

    public void OnClickShowDeckButton()
    {
        if(ShowingDeckData != null)
        {
            ContinuousController.instance.StartCoroutine(deckListPanel.SetUpDeckListPanel(ShowingDeckData));
        }
    }

    public void OnClickDeleteDeckButton()
    {
        if (ShowingDeckData != null)
        {
            List<UnityAction> Commands = new List<UnityAction>()
            {
                () =>
                    {
                        if(isFromSelectDeck)
                        {
                            ContinuousController.instance.DeckDatas.Remove(ShowingDeckData);
                            ContinuousController.instance.StartCoroutine(selectDeck.SetDeckList());
                            selectDeck.ResetDeckInfoPanel();
                            ContinuousController.instance.SaveDeckDatas();
                        }
                    },

                    null
            };

            List<string> CommandTexts = new List<string>()
            {
                 "Yes",
                 "No"
            };

            yesNoObject.SetUpYesNoObject(Commands, CommandTexts, $"Do you want to delete {ShowingDeckData.DeckName}?",false);
        } 
    }

    public void OnClickGetDeckCodeButton()
    {
        if (ShowingDeckData != null)
        {
            #region クリップボードにデッキコードをコピー

            string DeckCode = ShowingDeckData.GetThisDeckCode();

            string NewDeckCode = null;

            int max = DeckCode.Split(',').Length;

            for(int i=0;i<max;i++)
            {
                if(i == 0)
                {
                    NewDeckCode += null+ ",";
                }

                else
                {
                    NewDeckCode += DeckCode.Split(',')[i] + ",";
                }
            }

            NewDeckCode.Replace("_", "ｬ");
            NewDeckCode.Replace("*", "ｭ");

            GUIUtility.systemCopyBuffer = NewDeckCode;
            #endregion

            List <UnityAction> Commands = new List<UnityAction>()
            {
                null
            };

            List<string> CommandTexts = new List<string>()
            {
                "OK"
            };

            yesNoObject.SetUpYesNoObject(Commands, CommandTexts, $"The deck code of {ShowingDeckData.DeckName} was\ncopied to the clipboard!", false);
        }
    }

    public void SetDeckName(string text)
    {
        if(string.IsNullOrEmpty(text))
        {
            text = ShowingDeckData.DeckName;
            DeckName.text = ShowingDeckData.DeckName;
        }

        if (ShowingDeckData.DeckName == text)
        {
            return;
        }

        text = EditDeck.ValidateDeckName(text);

        while (text.Length > 10)
        {
            text = text.Substring(0, text.Length - 1);
        }

        ShowingDeckData.DeckName = text;

        ContinuousController.instance.SaveDeckDatas();
        
        if(isFromSelectDeck)
        {
            if (selectDeck != null)
            {
                if (editDeck != null)
                {
                    if (editDeck.CreateDeckObject.activeSelf)
                    {
                        return;
                    }
                }

                ContinuousController.instance.StartCoroutine(selectDeck.SetDeckList());

                for (int i = 0; i < selectDeck.deckInfoPrefabParentScroll.content.childCount; i++)
                {
                    if (selectDeck.deckInfoPrefabParentScroll.content.GetChild(i).GetComponent<DeckInfoPrefab>() != null)
                    {
                        if (selectDeck.deckInfoPrefabParentScroll.content.GetChild(i).GetComponent<DeckInfoPrefab>().thisDeckData == ShowingDeckData)
                        {
                            selectDeck.deckInfoPrefabParentScroll.content.GetChild(i).GetComponent<DeckInfoPrefab>().OnClick();
                        }
                    }
                }
            }
        }

        else
        {
            if (selectBattleDeck != null)
            {
                if (editDeck != null)
                {
                    if (editDeck.CreateDeckObject.activeSelf)
                    {
                        return;
                    }
                }

                selectBattleDeck.SetDeckList();

                for (int i = 0; i < selectBattleDeck.deckInfoPrefabParentScroll.content.childCount; i++)
                {
                    if (selectBattleDeck.deckInfoPrefabParentScroll.content.GetChild(i).GetComponent<DeckInfoPrefab>() != null)
                    {
                        if (selectBattleDeck.deckInfoPrefabParentScroll.content.GetChild(i).GetComponent<DeckInfoPrefab>().thisDeckData == ShowingDeckData)
                        {
                            selectBattleDeck.deckInfoPrefabParentScroll.content.GetChild(i).GetComponent<DeckInfoPrefab>().OnClick();
                        }
                    }
                }
            }
        }
        
    }

    public UnityAction OnClickSelectDeckAction;

    public void OnClickSelectDeck()
    {
        OnClickSelectDeckAction?.Invoke();
    }
}

[Serializable]
public class CardColorCount
{
    [Header("対応する色")]
    public CardColor cardColor;

    [Header("カウント")]
    public Text CountText;

    public Image icon;

    public void SetUpCardColorCount(DeckData deckData)
    {
        CountText.text = $"{deckData.DeckCards().Count((card)=>card.cardColors.Contains(cardColor))}";
    }

    public IEnumerator SetIconScale()
    {
        if(icon != null)
        {
            icon.transform.localScale = new Vector3(1.02f, 1.02f, 1);
            yield return null;
            icon.transform.localScale = new Vector3(1, 1, 1);
        }
    }
}

[Serializable]
public class CostCount
{
    [Header("枚数カウントテキスト")]
    public Text CountText;

    [Header("枚数スライダー")]
    public Slider CountSlier;

    //条件
    public Func<CEntity_Base, bool> CountCondition;

    public void SetUpCostCount(DeckData deckData)
    {
        int count = deckData.DeckCards().Count((card) => CountCondition(card));

        CountText.text = $"{count}";
        CountSlier.value = (float)count / 10;
    }
}
