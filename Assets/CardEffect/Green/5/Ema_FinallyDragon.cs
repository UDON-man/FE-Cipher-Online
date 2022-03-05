using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Photon.Pun;

public class Ema_FinallyDragon : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnEnterFieldAnyone)
        {
            activateClass[0].SetUpICardEffect("はりきっていきましょー!", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Let's go enthusiastically!";
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable))
                {
                    if (hashtable != null)
                    {
                        if (hashtable.ContainsKey("Unit"))
                        {
                            if (hashtable["Unit"] is Unit)
                            {
                                Unit Unit = (Unit)hashtable["Unit"];

                                if (Unit == card.UnitContainingThisCharacter())
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner == card.Owner && unit != card.UnitContainingThisCharacter(),
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: true,
                    CanEndNotMax: false,
                    SelectUnitCoroutine: null,
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Move);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));
            }

        }

        else if (timing == EffectTiming.OnDiscardHand)
        {
            activateClass[1].SetUpICardEffect("そうはいきません!", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true);
            activateClass[1].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[1]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[1].EffectName = "You don't have it your way!";
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if (card.Owner.TrashCards.Contains(card))
                {
                    if (hashtable != null)
                    {
                        if (hashtable.ContainsKey("Card"))
                        {
                            if (hashtable["Card"] is CardSource)
                            {
                                if (hashtable.ContainsKey("cardEffect"))
                                {
                                    if (hashtable["cardEffect"] is ICardEffect)
                                    {
                                        ICardEffect cardEffect = (ICardEffect)hashtable["cardEffect"];
                                        CardSource cardSource = (CardSource)hashtable["Card"];

                                        if (cardEffect != null && cardSource != null)
                                        {
                                            if (cardEffect.card() != null)
                                            {
                                                if (cardEffect.card().Owner == this.card.Owner.Enemy)
                                                {
                                                    if (cardSource == this.card)
                                                    {
                                                        if (card.Owner.BondCards.Count((_cardSource) => !_cardSource.IsReverse && _cardSource.cardColors.Contains(CardColor.Green)) > 0)
                                                        {
                                                            if (this.card.CanPlayAsNewUnit())
                                                            {
                                                                return true;
                                                            }

                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }


                            }
                        }
                    }
                }


                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                #region 前衛・後衛を指定
                if (card.Owner.isYou)
                {
                    GManager.instance.commandText.OpenCommandText("Which do you deploy on Front or Back?");

                    List<Command_SelectCommand> command_SelectCommands = new List<Command_SelectCommand>()
                            {
                                new Command_SelectCommand("Front Raw",() => photonView.RPC("SetIsFront_Ema",RpcTarget.All,true),0),
                                new Command_SelectCommand("Back Raw",() => photonView.RPC("SetIsFront_Ema",RpcTarget.All,false),1),
                            };

                    GManager.instance.selectCommandPanel.SetUpCommandButton(command_SelectCommands);
                }

                yield return new WaitWhile(() => !endSelect);
                endSelect = false;

                GManager.instance.commandText.CloseCommandText();
                yield return new WaitWhile(() => GManager.instance.commandText.gameObject.activeSelf);

                #endregion

                Hashtable hashtable = new Hashtable();
                hashtable.Add("cardEffect", activateClass[1]);
                yield return StartCoroutine(new IPlayUnit(card, null, isFront, true, hashtable, false).PlayUnit());
            }
        }

        return cardEffects;
    }

    bool isFront;
    bool endSelect;
    [PunRPC]
    public void SetIsFront_Ema(bool isFront)
    {
        this.isFront = isFront;
        endSelect = true;
    }
}