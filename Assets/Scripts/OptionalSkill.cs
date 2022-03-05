using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using System.Linq;
using System;
public class OptionalSkill : MonoBehaviourPunCallbacks
{
    bool endSelect = false;
    bool useOptional = false;
    public IEnumerator SelectOptional(ICardEffect cardEffect,string Message,bool ShowOpponentMessage)
    {
        endSelect = false;
        useOptional = false;

        string _Message = $"Do you use {cardEffect.EffectName}?";

        if(!string.IsNullOrEmpty(Message))
        {
            _Message = Message;
        }

        yield return GManager.instance.photonWaitController.StartWait("PayReverseCost");

        if (cardEffect.card().Owner.isYou)
        {
            GManager.instance.commandText.OpenCommandText(_Message);

            List<Command_SelectCommand> commands = new List<Command_SelectCommand>()
            {
                new Command_SelectCommand("Use",() => photonView.RPC("SetUseOptional",RpcTarget.All,true),0),
                new Command_SelectCommand("Not Use",() => photonView.RPC("SetUseOptional",RpcTarget.All,false),1),
            };

            GManager.instance.selectCommandPanel.SetUpCommandButton(commands);
        }

        else
        {
            if(ShowOpponentMessage)
            {
                GManager.instance.commandText.OpenCommandText($"The opponent is selecting if use skill.");
            }
            
            if(GManager.instance.IsAI)
            {
                endSelect = true;
                this.useOptional = false;
            }
        }

        yield return new WaitWhile(() => !endSelect);
        endSelect = false;

        GManager.instance.commandText.CloseCommandText();
        yield return new WaitWhile(() => GManager.instance.commandText.gameObject.activeSelf);

        cardEffect.UseOptional = this.useOptional;
    }

    [PunRPC]
    public void SetUseOptional(bool useOptional)
    {
        this.useOptional = useOptional;
        endSelect = true;
    }
}
