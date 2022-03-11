using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Photon.Pun;
using System;
using Photon;

public class NextPhaseButton : MonoBehaviourPunCallbacks
{
    [Header("ボタンテキスト")]
    public Text ButtonText;

    [Header("ボタン")]
    public Button Button;

    [Header("ボタン画像")]
    public Image ButtonImage;

    public GameObject Outline;

    public GameObject Cover;

    public Sprite MyTurnSprite;
    public Sprite OpponentTurnSprite;

    private void Awake()
    {
        if (Cover != null)
        {
            Cover.SetActive(false);

            Button.gameObject.SetActive(false);
            Outline.SetActive(false);
        }
    }

    public void OnClick()
    {
        if (!GManager.instance.turnStateMachine.isSync && !GManager.instance.turnStateMachine.IsSelecting && !GManager.instance.turnStateMachine.isExecuting)
        {
            if(GManager.instance.turnStateMachine.gameContext.TurnPlayer.isYou)
            {
                if(GManager.instance.turnStateMachine.gameContext.TurnPhase == GameContext.phase.Bond || GManager.instance.turnStateMachine.gameContext.TurnPhase == GameContext.phase.Deploy || GManager.instance.turnStateMachine.gameContext.TurnPhase == GameContext.phase.Action)
                {
                    photonView.RPC("NextPhase", RpcTarget.All);
                    StartCoroutine(CoverCoroutine(1.5f));
                }
            }
            
        }
    }

    public IEnumerator CoverCoroutine(float waitTime)
    {
        if (Cover != null)
        {
            Cover.SetActive(true);

            yield return new WaitForSeconds(waitTime);

            Cover.SetActive(false);
        }
    }

    bool oldActive = false;
    private void Update()
    {
        oldActive = Button.gameObject.activeSelf;
        

        bool active = false;

        if (GManager.instance != null)
        {
            if(GManager.instance.turnStateMachine != null)
            {
                if (GManager.instance.turnStateMachine.endGame || !GManager.instance.turnStateMachine.DoseStartGame)
                {
                    return;
                }

                if(GManager.instance.turnStateMachine.gameContext != null)
                {
                    if(GManager.instance.turnStateMachine.gameContext.TurnPlayer != null)
                    {
                        if (GManager.instance.turnStateMachine.gameContext.TurnPlayer.isYou)
                        {
                            if (!GManager.instance.turnStateMachine.isSync && !GManager.instance.turnStateMachine.IsSelecting && !GManager.instance.turnStateMachine.isExecuting)
                            {
                                switch (GManager.instance.turnStateMachine.gameContext.TurnPhase)
                                {
                                    case GameContext.phase.Bond:
                                        active = true;
                                        ButtonText.text = "Not Set\nBond";
                                        break;

                                    case GameContext.phase.Deploy:
                                        active = true;
                                        ButtonText.text = "End\nDeploy";
                                        break;

                                    case GameContext.phase.Action:
                                        active = true;
                                        ButtonText.text = "End\nTurn";
                                        break;
                                }
                            }
                        }

                        else
                        {
                            active = true;
                            ButtonText.text = "Opponent\nTurn";
                        }

                        foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players)
                        {
                            if (player.SupportCards.Count > 0)
                            {
                                active = false;
                            }
                        }
                    }
                    
                }
                
            }
            
        }

        if(!active)
        {
            Button.gameObject.SetActive(false);
            Outline.SetActive(false);
        }

        else
        {
            Button.gameObject.SetActive(true);
            

            if (GManager.instance.turnStateMachine.gameContext.TurnPlayer.isYou)
            {
                Button.enabled = true;
                Outline.SetActive(true);

                if (!oldActive)
                {
                    StartCoroutine(CoverCoroutine(0.3f));
                }
            }

            else
            {
                Button.enabled = false;
                Outline.SetActive(false);

                Cover.SetActive(true);
            }
        }
    }

    [PunRPC]
    public void NextPhase()
    {
        GManager.instance.turnStateMachine.NextPhase();
    }

    public void SwitchTurnSprite()
    {
        if (GManager.instance.turnStateMachine.gameContext.TurnPlayer.isYou)
        {
            SetMyTurnSprite();
        }

        else
        {
            EnemyTurnSprite();
        }

        Button.gameObject.SetActive(false);
        Outline.gameObject.SetActive(false);
    }

    public void SetMyTurnSprite()
    {
        ButtonImage.sprite = MyTurnSprite;
    }

    public void EnemyTurnSprite()
    {
        ButtonImage.sprite = OpponentTurnSprite;
    }
}
