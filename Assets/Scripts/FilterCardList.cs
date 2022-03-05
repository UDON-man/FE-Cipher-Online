using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using System.Linq;

public class FilterCardList : MonoBehaviour
{
    [Header("色ボタン")]
    public List<FilteColorButton> filteColorButtons = new List<FilteColorButton>();

    [Header("出撃コスト")]
    public FilterMinMax filterPlayCost;

    [Header("射程")]
    public FilterMinMax filterRange;

    [Header("戦闘力")]
    public FilterMinMax filterPower;

    [Header("支援力")]
    public FilterMinMax filterSupportPower;

    [Header("デッキ編集")]
    public EditDeck editDeck;

    
    public Func<CEntity_Base, bool> OnlyMatchPlayCost()
    {
        bool _OnlyMatchPlayCost(CEntity_Base cEntity_Base)
        {
            int min = 0;
            int max = 200;

            int value;

            if (int.TryParse(filterPlayCost.MinInputField.text, out value))
            {
                min = value;
            }

            if (int.TryParse(filterPlayCost.MaxInputField.text, out value))
            {
                max = value;
            }

            if (min <= cEntity_Base.PlayCost && cEntity_Base.PlayCost <= max)
            {
                return true;
            }

            return false;
        }

        return _OnlyMatchPlayCost;
    }

    public Func<CEntity_Base, bool> OnlyMatchRange()
    {
        bool _OnlyMatchRange(CEntity_Base cEntity_Base)
        {
            int min = 0;
            int max = 200;

            int value;

            if (int.TryParse(filterRange.MinInputField.text, out value))
            {
                min = value;
            }

            if (int.TryParse(filterRange.MaxInputField.text, out value))
            {
                max = value;
            }

            if(cEntity_Base.Ranges.Count > 0)
            {
                int minRange = cEntity_Base.Ranges.Min();
                int maxRange = cEntity_Base.Ranges.Max();

                if(min <= minRange && maxRange <= max)
                {
                    return true;
                }
            }

            return false;
        }

        return _OnlyMatchRange;
    }

    public Func<CEntity_Base,bool> OnlyMatchPower()
    {
        bool _OnlyMatchPower(CEntity_Base cEntity_Base)
        {
            int min = 0;
            int max = 200;

            int value;

            if (int.TryParse(filterPower.MinInputField.text, out value))
            {
                min = value;
            }

            if (int.TryParse(filterPower.MaxInputField.text, out value))
            {
                max = value;
            }

            if(min <= cEntity_Base.Power && cEntity_Base.Power <= max)
            {
                return true;
            }

            return false;
        }

        return _OnlyMatchPower;
    }

    public Func<CEntity_Base, bool> OnlyMatchSupportPower()
    {
        bool _OnlyMatchSupportPower(CEntity_Base cEntity_Base)
        {
            int min = 0;
            int max = 200;

            int value;

            if (int.TryParse(filterSupportPower.MinInputField.text, out value))
            {
                min = value;
            }

            if (int.TryParse(filterSupportPower.MaxInputField.text, out value))
            {
                max = value;
            }

            if (min <= cEntity_Base.SupportPower && cEntity_Base.SupportPower <= max)
            {
                return true;
            }

            return false;
        }

        return _OnlyMatchSupportPower;
    }

    public Func<CEntity_Base, bool> OnlyContainsColor()
    {
        bool _OnlyContainsColor(CEntity_Base cEntity_Base)
        {
            bool OK_Color = false;

            foreach(CardColor cardColor in OnColors)
            {
                if (cEntity_Base.cardColors.Contains(cardColor))
                {
                    OK_Color = true;
                }
            }

            return OK_Color;
        }

        return _OnlyContainsColor;
    }

    public List<CardColor> OnColors
    {
        get
        {
            List<CardColor> _OnColors = new List<CardColor>();

            foreach(FilteColorButton filteColorButton in filteColorButtons)
            {
                if(filteColorButton.On)
                {
                    _OnColors.Add(filteColorButton.cardColor);
                }
            }

            return _OnColors;
        }
    }

    public void Init(UnityAction onClickFilterColorButton)
    {
        foreach (FilteColorButton filteColorButton in filteColorButtons)
        {
            filteColorButton.Init(onClickFilterColorButton);
        }

        if(filterPlayCost != null)
        {
            filterPlayCost.Init();
        }

        if(filterRange != null)
        {
            filterRange.Init();
        }

        if (filterPower != null)
        {
            filterPower.Init();
        }

        if (filterSupportPower != null)
        {
            filterSupportPower.Init();
        }
    }
}

[Serializable]
public class FilteColorButton
{
    public CardColor cardColor;
    public Image Icon;
    public bool On;
    public float SelectedScale;
    public float NotSelectedScale;

    public UnityAction OnClickAction;

    public void Init(UnityAction _OnClickAction)
    {
        OnClickAction = null;
        On = false;
        OnClickFilteColorButton();
        OnClickAction = _OnClickAction;

        Icon.GetComponent<Button>().onClick.RemoveAllListeners();
        Icon.GetComponent<Button>().onClick.AddListener(OnClickFilteColorButton);
    }

    public void OnClickFilteColorButton()
    {
        On = !On;

        if(On)
        {
            Icon.transform.localScale = new Vector3(SelectedScale, SelectedScale, 1);
            Icon.GetComponent<Shadow>().enabled = true;
        }

        else
        {
            Icon.transform.localScale = new Vector3(NotSelectedScale, NotSelectedScale, 1);
            Icon.GetComponent<Shadow>().enabled = false;
        }

        OnClickAction?.Invoke();
    }
}

[Serializable]
public class FilterMinMax
{
    public InputField MinInputField;
    public InputField MaxInputField;

    public void Init()
    {
        if(MinInputField != null)
        {
            MinInputField.text = "";
        }
        
        if(MaxInputField != null)
        {
            MaxInputField.text = "";
        }
    }
}