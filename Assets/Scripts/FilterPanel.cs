using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class FilterPanel : OffAnimation
{
    public FilterCardList filterCardList;
    public FilterCardList OtherFilterCardList;

    public void Init()
    {
        filterCardList.Init(null);
        Off();
    }
    public void OpenFilterPanel()
    {
        foreach(FilteColorButton filteColorButton in OtherFilterCardList.filteColorButtons)
        {
            foreach(FilteColorButton filteColorButton1 in filterCardList.filteColorButtons)
            {
                if(filteColorButton.cardColor == filteColorButton1.cardColor)
                {
                    filteColorButton1.On = !filteColorButton.On;
                    filteColorButton1.OnClickFilteColorButton();
                }
            }
        }

        this.gameObject.SetActive(true);
        GetComponent<Animator>().SetInteger("Close", 0);
    }

    public void OnClickOKButton()
    {
        foreach (FilteColorButton filteColorButton in OtherFilterCardList.filteColorButtons)
        {
            foreach (FilteColorButton filteColorButton1 in filterCardList.filteColorButtons)
            {
                if (filteColorButton.cardColor == filteColorButton1.cardColor)
                {
                    filteColorButton.OnClickAction = null;
                    filteColorButton.On = !filteColorButton1.On;
                    filteColorButton.OnClickFilteColorButton();
                    filteColorButton.OnClickAction = filterCardList.editDeck.ShowPoolCard_MatchCondition;
                }
            }
        }

        GetComponent<Animator>().SetInteger("Close", 1);

        filterCardList.editDeck.ShowPoolCard_MatchCondition();
    }

    public void OnClickDeleteButton()
    {
        filterCardList.Init(null);
    }
}
