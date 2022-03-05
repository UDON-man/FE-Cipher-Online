using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using System;
using System.Linq;

public enum Critical_EvasionMode
{
    Critical,
    Evasion,
}
public class Critical_Evasion : MonoBehaviourPunCallbacks
{
    bool endSelect;
    public CardSource DiscardCard { get; set; }
    public SelectCardEffect.Mode handmode { get; set; }
    
   public IEnumerator CriticalCoroutine(Critical_EvasionMode mode)
   {
        GManager.instance.turnStateMachine.IsSelecting = true;

        Unit TargetUnit()
        {
            if(mode == Critical_EvasionMode.Critical)
            {
                return GManager.instance.turnStateMachine.AttackingUnit;
            }

            else
            {
                return GManager.instance.turnStateMachine.DefendingUnit;
            }
        }

        DiscardCard = null;
        endSelect = false;

        handmode = SelectCardEffect.Mode.DiscardFromHand;

        yield return GManager.instance.photonWaitController.StartWait("SelectCritical_Evade");

        if (TargetUnit().Character.Owner.isYou)
        {
            if(!SkipSelect())
            {
                if (mode == Critical_EvasionMode.Critical)
                {
                    if (GManager.instance.You.HandCards.Count((handCardSource) => CardSource.IsSameUnitName(TargetUnit().Character, handCardSource)) > 0)
                    {
                        GManager.instance.commandText.OpenCommandText("Do you do critical Hit?");
                    }

                    else
                    {
                        GManager.instance.commandText.OpenCommandText("You can't do critical Hit.");
                    }
                }

                else if (mode == Critical_EvasionMode.Evasion)
                {
                    if (GManager.instance.You.HandCards.Count((handCardSource) => CardSource.IsSameUnitName(TargetUnit().Character, handCardSource)) > 0)
                    {
                        GManager.instance.commandText.OpenCommandText("Do you evade?");
                    }

                    else
                    {
                        GManager.instance.commandText.OpenCommandText("You can't evade.");
                    }
                }

                #region 手札のカードに必殺攻撃・回避として使うクリック操作を追加
                List<HandCard> PreSelectHandCard = new List<HandCard>();

                int MaxCount = 1;

                foreach (HandCard handCard in GManager.instance.You.HandCardObjects)
                {
                    if (CardSource.IsSameUnitName(TargetUnit().Character, handCard.cardSource))
                    {
                        handCard.AddClickTarget(OnClickHandCard);
                    }
                }

                CheckEndSelect();

                void OnClickHandCard(HandCard _handCard)
                {
                    if (PreSelectHandCard.Contains(_handCard))
                    {
                        PreSelectHandCard.Remove(_handCard);
                    }

                    else
                    {
                        if (PreSelectHandCard.Count < MaxCount)
                        {
                            PreSelectHandCard.Add(_handCard);
                        }

                        else
                        {
                            if (PreSelectHandCard.Count > 0)
                            {
                                PreSelectHandCard.RemoveAt(PreSelectHandCard.Count - 1);
                                PreSelectHandCard.Add(_handCard);
                            }
                        }
                    }

                    CheckEndSelect();
                }

                void CheckEndSelect()
                {
                    if (PreSelectHandCard.Count == MaxCount)
                    {
                        GManager.instance.selectCommandPanel.SetUpCommandButton(new List<Command_SelectCommand>() { new Command_SelectCommand(Message(), SelectDiscardCard, 0) });

                        string Message()
                        {
                            switch (mode)
                            {
                                case Critical_EvasionMode.Critical:
                                    return "Critical Hit";

                                case Critical_EvasionMode.Evasion:
                                    return "Evade";
                            }

                            return null;
                        }

                        void SelectDiscardCard()
                        {
                            ContinuousController.instance.StartCoroutine(SelectDiscardCoroutine());
                        }

                        IEnumerator SelectDiscardCoroutine()
                        {
                            foreach (HandCard handCard in GManager.instance.You.HandCardObjects)
                            {
                                handCard.RemoveClickTarget();
                                handCard.OnRemoveSelect();
                            }

                            GManager.instance.BackButton.CloseSelectCommandButton();
                            DiscardCard = PreSelectHandCard[0].cardSource;

                            foreach(ICardEffect cardEffect in TargetUnit().EffectList(EffectTiming.BeforeDiscardCritical_EvasionCard))
                            {
                                if(cardEffect.CanUse(null))
                                {
                                    if(cardEffect is ActivateICardEffect)
                                    {
                                        yield return new WaitForSeconds(0.7f);
                                        yield return ContinuousController.instance.StartCoroutine(((ActivateICardEffect)cardEffect).Activate(null));
                                    }
                                }
                            }

                            int CardID = PreSelectHandCard[0].cardSource.cardIndex;
                            int handmodeID = (int)handmode;
                            photonView.RPC("SetDiscardCard_Critical_Evasion", RpcTarget.All, CardID, handmodeID);
                        }
                    }

                    else
                    {
                        GManager.instance.selectCommandPanel.CloseSelectCommandPanel();
                    }

                    foreach (HandCard handCard in GManager.instance.You.HandCardObjects)
                    {
                        handCard.OnRemoveSelect();

                        if (CardSource.IsSameUnitName(TargetUnit().Character, handCard.cardSource))
                        {
                            if (PreSelectHandCard.Contains(handCard))
                            {
                                handCard.OnSelect();
                                handCard.SetOrangeOutline();
                            }

                            else
                            {
                                handCard.OnOutline();
                                handCard.SetBlueOutline();
                            }
                        }
                    }
                }
                #endregion

                //戻るボタン表示(選択し直しに戻る)
                GManager.instance.BackButton.OpenSelectCommandButton("Return", () => photonView.RPC("EndSelect", RpcTarget.All), 0);
            }
            
        }

        else
        {
            if (mode == Critical_EvasionMode.Critical)
            {
                GManager.instance.commandText.OpenCommandText("The opponent is selecting if he does critical hit.");
            }

            else if (mode == Critical_EvasionMode.Evasion)
            {
                GManager.instance.commandText.OpenCommandText("The opponent is selecting if he evades.");
            }

            #region AI
            if (GManager.instance.IsAI)
            {
                if (TargetUnit().Character.Owner.HandCards.Count((handCardSource) => CardSource.IsSameUnitName(TargetUnit().Character, handCardSource)) == 0)
                {
                    EndSelect();
                }

                else
                {
                    bool doDiscard = false;

                    if (mode == Critical_EvasionMode.Critical)
                    {
                        if (GManager.instance.turnStateMachine.AttackingUnit.Power < GManager.instance.turnStateMachine.DefendingUnit.Power)
                        {
                            if (GManager.instance.turnStateMachine.AttackingUnit.Power * 2 >= GManager.instance.turnStateMachine.DefendingUnit.Power)
                            {
                                if (GManager.instance.turnStateMachine.AttackingUnit != GManager.instance.turnStateMachine.AttackingUnit.Character.Owner.Lord)
                                {
                                    if(GManager.instance.turnStateMachine.DefendingUnit != GManager.instance.turnStateMachine.DefendingUnit.Character.Owner.Lord)
                                    {
                                        doDiscard = true;
                                    }
                                    
                                    else
                                    {
                                        if(GManager.instance.You.OrbCount == 0)
                                        {
                                            doDiscard = true;
                                        }

                                        if(GManager.instance.turnStateMachine.AttackingUnit.Strike >= 2)
                                        {
                                            if(GManager.instance.You.OrbCount >= GManager.instance.turnStateMachine.AttackingUnit.Strike)
                                            {
                                                doDiscard = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }                        
                    }

                    else if(mode == Critical_EvasionMode.Evasion)
                    {
                        if (GManager.instance.turnStateMachine.AttackingUnit.Power >= GManager.instance.turnStateMachine.DefendingUnit.Power)
                        {
                            if (GManager.instance.turnStateMachine.DefendingUnit == GManager.instance.turnStateMachine.DefendingUnit.Character.Owner.Lord)
                            {
                                if((GManager.instance.turnStateMachine.AttackingUnit.Strike >= 2 && GManager.instance.turnStateMachine.DefendingUnit.Characters.Count > 1) || GManager.instance.Opponent.OrbCount == 0)
                                {
                                    
                                    doDiscard = true;
                                }
                            }

                            else
                            {
                                if(GManager.instance.turnStateMachine.DefendingUnit.Character.cEntity_Base.Power >= GManager.instance.You.Lord.Character.cEntity_Base.Power)
                                {
                                    if (GManager.instance.You.FieldUnit.Count((_unit) =>
                                    {
                                        if (_unit != GManager.instance.turnStateMachine.AttackingUnit)
                                        {
                                            if (_unit.CanAttack)
                                            {
                                                if (_unit.CanAttachTargetUnit(GManager.instance.turnStateMachine.DefendingUnit))
                                                {
                                                    if (_unit.Power >= GManager.instance.turnStateMachine.DefendingUnit.Character.cEntity_Base.Power)
                                                    {
                                                        return true;
                                                    }
                                                }
                                            }
                                        }

                                        return false;
                                    }) == 0)
                                    {
                                        doDiscard = true;
                                    }
                                }
                            }
                        }
                    }

                    if (doDiscard)
                    {
                        foreach (CardSource cardSource in TargetUnit().Character.Owner.HandCards)
                        {
                            if (CardSource.IsSameUnitName(TargetUnit().Character, cardSource))
                            {
                                SetDiscardCard_Critical_Evasion(cardSource.cardIndex,(int)SelectCardEffect.Mode.DiscardFromHand);

                                break;
                            }
                        }
                    }

                    else
                    {
                        EndSelect();
                    }
                }
            }
            #endregion
        }

        while (!endSelect)
        {
            if(SkipSelect())
            {
                photonView.RPC("EndSelect", RpcTarget.All);
                break;
            }

            yield return null;
        }

        #region 選択をスキップ
        bool SkipSelect()
        {
            if (DiscardCard == null)
            {
                if (TargetUnit().Character.Owner.isYou)
                {
                    if (mode == Critical_EvasionMode.Critical)
                    {
                        if (GManager.instance.optionPanel.SkipCriticalToggle.isOn)
                        {
                            if (!GManager.instance.optionPanel.gameObject.activeSelf)
                            {
                                if (GManager.instance.turnStateMachine.AttackingUnit.Power >= GManager.instance.turnStateMachine.DefendingUnit.Power || TargetUnit().Character.Owner.HandCards.Count((handCardSource) => CardSource.IsSameUnitName(TargetUnit().Character, handCardSource)) == 0)
                                {
                                    return true;
                                }
                            }
                        }
                    }

                    else if (mode == Critical_EvasionMode.Evasion)
                    {
                        if (GManager.instance.optionPanel.SkipEvasionToggle.isOn)
                        {
                            if (!GManager.instance.optionPanel.gameObject.activeSelf)
                            {
                                if (GManager.instance.turnStateMachine.AttackingUnit.Power < GManager.instance.turnStateMachine.DefendingUnit.Power || TargetUnit().Character.Owner.HandCards.Count((handCardSource) => CardSource.IsSameUnitName(TargetUnit().Character, handCardSource)) == 0)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }
        #endregion

        yield return new WaitWhile(() => !endSelect);
        endSelect = false;

        GManager.instance.commandText.CloseCommandText();
        yield return new WaitWhile(() => GManager.instance.commandText.gameObject.activeSelf);

        #region 選択された手札のカードを捨てる
        if (DiscardCard != null)
        {
            if(handmode == SelectCardEffect.Mode.DiscardFromHand)
            {
                yield return StartCoroutine(DiscardCard.cardOperation.DiscardFromHand(null));
            }

            else if(handmode == SelectCardEffect.Mode.PutLibraryTop)
            {
                yield return StartCoroutine(DiscardCard.cardOperation.PutLibraryTopFromHand());
            }

            StartCoroutine(ShowHandCard(handmode,DiscardCard));
        }
        #endregion

        foreach(Player player in GManager.instance.turnStateMachine.gameContext.Players)
        {
            foreach(HandCard handCard in player.HandCardObjects)
            {
                handCard.OnRemoveSelect();
                handCard.RemoveClickTarget();
            }
        }
   }

    IEnumerator ShowHandCard(SelectCardEffect.Mode handmode,CardSource DiscardCard)
    {
        yield return new WaitForSeconds(1.7f);

        string Message()
        {
            if(handmode == SelectCardEffect.Mode.DiscardFromHand)
            {
                return "Discarded Card";
            }

            else if (handmode == SelectCardEffect.Mode.PutLibraryTop)
            {
                return "Deck Top Card";
            }

            return "Selected Card";
        }

        yield return ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(new List<CardSource>() { DiscardCard }, Message(), true));
    }

    [PunRPC]
    public void SetDiscardCard_Critical_Evasion(int _cardIndex,int handmodeID)
    {
        handmode = (SelectCardEffect.Mode)Enum.ToObject(typeof(SelectCardEffect.Mode), handmodeID);
        DiscardCard = GManager.instance.turnStateMachine.gameContext.ActiveCardList[_cardIndex];
        EndSelect();
    }

    [PunRPC]
    public void EndSelect()
    {
        endSelect = true;
    }
}
