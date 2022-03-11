using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class HandCard : MonoBehaviour
{
    [Header("カード画像")]
    public Image CardImage;

    [Header("プレイコスト背景")]
    public List<Image> PlayCostBackGround = new List<Image>();

    [Header("プレイコスト")]
    public Text PlayCostText;

    [Header("CCコスト背景")]
    public Image CCCostBackGround;

    [Header("CCコスト")]
    public Text CCCostText;

    [Header("選択アウトライン")]
    public GameObject Outline_Select;

    [Header("支援力テキスト")]
    public Text　SupportText;

    [Header("英語版画像")]
    public Image EnglishImage;

    [Header("スキル名")]
    public Text SkillNameText;

    [Header("表画像表示")]
    public Image ShowFaceCard;

    //このカードが表すカード
    public CardSource cardSource { get; set; }

    public UnityAction<HandCard> OnClickAction;

    public UnityAction <HandCard>BeginDragAction;
    public UnityAction<List<DropArea>> OnDragAction;
    public UnityAction<List<DropArea>> EndDragAction;
    public bool CanDrag;

    public List<Image> Outline_SelectImages { get; set; } = new List<Image>();

    public GameObject SupportSkillIconParent;
    public Image SupportSkillIcon_Attack;
    public Image SupportSkillIcon_Defence;

    private void Start()
    {
        OffSelectedIndexText();
        OnRemoveSelect();

        for (int i = 0; i < Outline_Select.transform.childCount; i++)
        {
            if (Outline_Select.transform.GetChild(i).GetComponent<Image>() != null)
            {
                Outline_SelectImages.Add(Outline_Select.transform.GetChild(i).GetComponent<Image>());
            }
        }

        if(SupportSkillIconParent != null)
        {
            SupportSkillIconParent.SetActive(false);
        }

        if(SkillNameText != null)
        {
            SkillNameText.transform.parent.gameObject.SetActive(false);
        }

        if(handCardCommandPanel != null)
        {
            handCardCommandPanel.CloseFieldUnitCommandPanel();
        }

        
    }

    public void SetSkillName(ICardEffect cardEffect)
    {
        if(SkillNameText != null)
        {
            SkillNameText.transform.parent.gameObject.SetActive(true);
            SkillNameText.text = cardEffect.GetEffectName();
        }
    }

    public void SetShowFaceCard()
    {
        if(ShowFaceCard != null && cardSource != null)
        {
            ShowFaceCard.gameObject.SetActive(true);
            ShowFaceCard.sprite = cardSource.cEntity_Base.CardImage;
        }
    }

    public void SetLanguage()
    {
        if(EnglishImage != null)
        {
            EnglishImage.gameObject.SetActive(false);

            if (cardSource != null)
            {
                if (cardSource.cEntity_Base.CardImage_English != null)
                {
                    if (ContinuousController.instance.language == Language.ENG)
                    {
                        EnglishImage.gameObject.SetActive(true);
                        EnglishImage.sprite = cardSource.cEntity_Base.CardImage_English;
                    }
                }
            }
        }
    }

    public void SetOutlineColor(Color color)
    {
        foreach (Image image in Outline_SelectImages)
        {
            image.color = color;
        }
    }

    public void SetBlueOutline()
    {
        SetOutlineColor(DataBase.SelectColor_Blue);
    }

    public void SetOrangeOutline()
    {
        SetOutlineColor(DataBase.SelectColor_Orange);
    }

    public void OnOutline()
    {
        Outline_Select.SetActive(true);
    }

    public void OnSelect()
    {
        OnOutline();

        SetBlueOutline();
    }

    public void OnRemoveSelect()
    {
        Outline_Select.SetActive(false);
    }

    public void SetUpHandCard(CardSource _cardSource)
    {
        cardSource = _cardSource;

        //SetUpHandCardImage();

        //カード画像
        if (_cardSource.Owner.isYou)
        {
            SetUpHandCardImage();
            
        }

        else
        {
            SetUpReverseCard();
        }
    }

    public void SetUpReverseCard()
    {
        PlayCostBackGround[0].transform.parent.gameObject.SetActive(false);
        CCCostBackGround.transform.parent.gameObject.SetActive(false);

        if(SupportText != null)
        {
            SupportText.transform.parent.gameObject.SetActive(false);
        }
        
        CardImage.sprite = ContinuousController.instance.ReverseCard;

        if(EnglishImage != null)
        {
            EnglishImage.gameObject.SetActive(false);
        }

        if (ShowFaceCard != null)
        {
            ShowFaceCard.gameObject.SetActive(false);
        }
    }

    public bool ShowOpponent { get; set; } = false;
    bool onYourHand()
    {
        bool onYourHand = false;

        if (GManager.instance != null)
        {
            if (GManager.instance.You.HandCardObjects.Contains(this))
            {
                onYourHand = true;
            }

            if(ShowOpponent)
            {
                return true;
            }
        }

        return onYourHand;
    }

    int count = 0;
    int UpdateFrame = 4;
    private void Update()
    {
        #region 例外チェック
        if (cardSource == null)
        {
            return;
        }
        #endregion

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

        if (onYourHand())
        {
            SetUpHandCardImage();
        }
    }

    public void SetUpHandCardImage()
    {
        SetLanguage();
        //カード画像
        CardImage.sprite = cardSource.cEntity_Base.CardImage;

        //プレイコスト背景色
        for (int i = 0; i < PlayCostBackGround.Count; i++)
        {
            if (i < cardSource.cEntity_Base.cardColors.Count)
            {
                PlayCostBackGround[i].color = DataBase.CardColor_ColorLightDictionary[cardSource.cEntity_Base.cardColors[i]];
            }

            else
            {
                PlayCostBackGround[i].color = DataBase.CardColor_ColorLightDictionary[cardSource.cEntity_Base.cardColors[0]];
            }
        }

        //プレイコスト
        PlayCostText.text = cardSource.cEntity_Base.PlayCost.ToString();

        PlayCostBackGround[0].transform.parent.gameObject.SetActive(true);

        if (cardSource.cEntity_Base.CCCost > 0)
        {
            //CCコスト背景
            CCCostBackGround.gameObject.SetActive(cardSource.cEntity_Base.HasCC);

            //CCコスト
            CCCostText.text = cardSource.cEntity_Base.CCCost.ToString();

            CCCostBackGround.transform.parent.gameObject.SetActive(true);
        }

        else
        {
            CCCostBackGround.transform.parent.gameObject.SetActive(false);
        }

        if(onYourHand())
        {
            if(cardSource.Owner.isYou)
            {
                foreach (Unit unit in GManager.instance.You.FieldUnit)
                {
                    if (cardSource.CanCCFromTargetUnit(unit))
                    {
                        if (cardSource.CCCost(unit) > 0)
                        {
                            //CCコスト背景
                            CCCostBackGround.gameObject.SetActive(true);

                            //CCコスト
                            CCCostText.text = cardSource.CCCost(unit).ToString();

                            CCCostBackGround.transform.parent.gameObject.SetActive(true);
                        }
                    }
                }
            }
            
        }
        
        //支援力
        if(SupportText!= null)
        {
            SupportText.transform.parent.gameObject.SetActive(false);
        }

        if (ShowFaceCard != null)
        {
            ShowFaceCard.gameObject.SetActive(false);
        }
    }

    public void AddClickTarget(UnityAction<HandCard> _OnClickAction)
    {
        OnClickAction = _OnClickAction;

        OnSelect();
    }

    public void RemoveClickTarget()
    {
        
        OnClickAction = null;
        OnRemoveSelect();
    }

    public void AddDragTarget(UnityAction<HandCard> _BeginDragAction, UnityAction<List<DropArea>> _OnDropAction, UnityAction<List<DropArea>> _OnDragAction)
    {
        BeginDragAction = _BeginDragAction;
        EndDragAction = _OnDropAction;
        OnDragAction = _OnDragAction;

        CanDrag = true;

        OnSelect();
    }

    public void RemoveDragTarget()
    {
        BeginDragAction = null;
        EndDragAction = null;
        OnDragAction = null;

        CanDrag = false;

        OnRemoveSelect();
    }

    public void PointerClick(BaseEventData eventData)
    {
        #region 右クリック
        if (Input.GetMouseButtonUp(1))
        {
            if (transform.parent.GetComponent<HandContoller>() != null)
            {
                if (transform.parent.GetComponent<HandContoller>().isDragging)
                {
                    return;
                }
            }

            if (cardSource != null)
            {
                if (cardSource.cEntity_Base == null)
                {
                    return;
                }

                bool CanLook = false;

                if (!GManager.instance.Opponent.HandCardObjects.Contains(this))
                {
                    if (cardSource.Owner.BondCards.Contains(cardSource))
                    {
                        CanLook = true;
                    }

                    if (!cardSource.IsReverse)
                    {
                        CanLook = true;
                    }
                }

                if(ShowOpponent)
                {
                    CanLook = true;
                }

                if(CanLook)
                {
                    GManager.instance.cardDetail.OpenCardDetail(cardSource, true);
                }
            }
        }
        #endregion

        #region 左クリック
        else if (Input.GetMouseButtonUp(0))
        {
            OnClickAction?.Invoke(this);
        }
        #endregion
    }

    public void ShowSupport()
    {
        if(cardSource != null)
        {
            SupportText.transform.parent.gameObject.SetActive(true);

            SupportText.text = $"{cardSource.SupportPower}";
        }
    }

    [Header("選択順テキスト")]
    public Text SelectedIndexText;

    public void OffSelectedIndexText()
    {
        if(SelectedIndexText != null)
        {
            SelectedIndexText.transform.parent.gameObject.SetActive(false);
        }
    }

    public void SetSelectedIndexText(int index)
    {
        if (SelectedIndexText != null)
        {
            SelectedIndexText.transform.parent.gameObject.SetActive(true);
            SelectedIndexText.text = $"{index}";
        }
    }

    [Header("絆・手札起動効果表示")]
    public HandCardCommandPanel handCardCommandPanel;
}

[System.Serializable]
public class HandCardCommandPanel
{
    [Header("コマンドパネルオブジェクト")]
    public GameObject CommandPanel;

    [Header("コマンドパネル背景")]
    public RectTransform CommandPanelBackGround;

    [Header("コマンドボタン")]
    public List<HandCardCommandButton> Buttons = new List<HandCardCommandButton>();

    public void SetUpHandCardCommandPanel(List<HandCardCommand> HandCardCommands, HandCard handCard,Color32 color)
    {
        for (int i = 0; i < Buttons.Count; i++)
        {
            if (i < HandCardCommands.Count)
            {
                Buttons[i].SetUpHandCardCommandButton(HandCardCommands[i].ButtonMessage, HandCardCommands[i].OnClickAction, HandCardCommands[i].Active,color);
            }

            else
            {
                Buttons[i].CloseFieldUnitCommandButton();
            }
        }

        CommandPanelBackGround.sizeDelta = new Vector2(CommandPanelBackGround.sizeDelta.x, 18 * (HandCardCommands.Count + 1));

        CommandPanel.SetActive(true);
    }

    public void CloseFieldUnitCommandPanel()
    {
        if(CommandPanel != null)
        {
            CommandPanel.SetActive(false);
        }
    }
}

public class HandCardCommand
{
    public HandCardCommand(string ButtonMessage, UnityAction OnClickAction, bool Active)
    {
        this.ButtonMessage = ButtonMessage;
        this.OnClickAction = OnClickAction;
        this.Active = Active;
    }

    public string ButtonMessage { get; set; }
    public UnityAction OnClickAction { get; set; }
    public bool Active { get; set; }
}

[System.Serializable]
public class HandCardCommandButton : FieldUnitCommandButton
{
    public void SetUpHandCardCommandButton(string ButtonMessage, UnityAction OnClickAtion, bool Active,Color color)
    {
        SetUpFieldUnitCommandButton(ButtonMessage, OnClickAtion, Active);
        button.GetComponent<Image>().color = color;
    }
}
