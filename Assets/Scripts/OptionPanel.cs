using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionPanel : MonoBehaviour
{
    public VolumePanel volumePanel;
    public GameObject ResizeWindowPanel;
    public Button SurrenderButton;
    public Toggle SkipCriticalToggle;
    public Toggle SkipEvasionToggle;

    public Text SurrenderButtonText;
    public Text SkipCriticalToggleText;
    public Text SkipEvasionToggleText;
    public Text SwitchLanguageButtonText;
    public void SwitchOptionPanel()
    {
        if (this.gameObject.activeSelf)
        {
            CloseOptionPanel();
        }

        else
        {
            OpenOptionPanel();
        }
    }
    public void OpenOptionPanel()
    {
        if (this.gameObject.activeSelf)
        {
            return;
        }

        this.gameObject.SetActive(true);

        GetComponent<Animator>().SetInteger("Close", 0);
    }

    public void CloseOptionPanel()
    {
        if (!this.gameObject.activeSelf)
        {
            return;
        }

        GetComponent<Animator>().SetInteger("Close", 1);
    }

    public void Off()
    {
        this.gameObject.SetActive(false);
    }

    public void Init()
    {
        Off();

        if(volumePanel != null)
        {
            volumePanel.Init();
        }

        if(ResizeWindowPanel != null)
        {
            ResizeWindowPanel.SetActive(false);
        }

        if (SurrenderButton != null)
        {
            SetSurrenderButton();
        }  
    }

    private void Update()
    {
        SetSurrenderButton();
    }

    void SetSurrenderButton()
    {
        if (SurrenderButton != null)
        {
            SurrenderButton.interactable = false;

            if (GManager.instance.turnStateMachine != null)
            {
                if (GManager.instance.turnStateMachine.gameContext != null)
                {
                    if(GManager.instance.turnStateMachine.gameContext.TurnPlayer != null)
                    {
                        if (GManager.instance.turnStateMachine.gameContext.TurnPlayer.isYou)
                        {
                            if (!GManager.instance.turnStateMachine.IsSelecting && !GManager.instance.turnStateMachine.isSync)
                            {
                                SurrenderButton.interactable = true;
                            }
                        }
                    }
                    
                }
            }
        }
    }
}
