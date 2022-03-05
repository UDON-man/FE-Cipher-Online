using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SwitchLanguage : MonoBehaviour
{
    public Text SwitchLanguageButtonText;

    public void SetLanguage()
    {
        if (ContinuousController.instance.language == Language.ENG)
        {
            SwitchLanguageButtonText.text = "To JPN";
        }

        else
        {
            SwitchLanguageButtonText.text = "To ENG";
        }
    }

    public void OnClickSwitchLanguageButton()
    {
        if (ContinuousController.instance.language == Language.ENG)
        {
            ContinuousController.instance.language = Language.JPN;
        }

        else
        {
            ContinuousController.instance.language = Language.ENG;
        }

        SetLanguage();

        ContinuousController.instance.SaveLanguage();
    }
}
