using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class LufureOtoko_MemoryLessGunshi : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDiscardHand)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("巧みな軍略", "Clever Tactics", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if(card.Owner.TrashCards.Contains(card))
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
                                                if (cardEffect.card().Owner == card.Owner.Enemy)
                                                {
                                                    if(cardSource == card)
                                                    {
                                                        if(card.Owner.OrbCards.Count < card.Owner.Enemy.OrbCards.Count)
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
                

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                if (card.Owner.OrbCount < card.Owner.Enemy.OrbCount)
                {
                    yield return StartCoroutine(new IAddOrbFromLibrary(card.Owner, 1).AddOrb());
                }
            }
        }

        return cardEffects;
    }

    #region 計略の紋章
    public override List<ICardEffect> SupportEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> supportEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnSetSupport)
        {
            ActivateClass activateClass_Support = new ActivateClass();
            activateClass_Support.SetUpICardEffect("計略の紋章", "Tactical Emblem", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true,card);
            activateClass_Support.SetUpActivateClass((hashtable) => ActivateCoroutine());
            supportEffects.Add(activateClass_Support);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (card.Owner.SupportCards.Contains(card))
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit != null)
                    {
                        if (GManager.instance.turnStateMachine.AttackingUnit.Character != null)
                        {
                            if (GManager.instance.turnStateMachine.AttackingUnit.Character.Owner == card.Owner)
                            {
                                if (GManager.instance.turnStateMachine.AttackingUnit.Character.cardColors.Contains(CardColor.Blue))
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
                    CanTargetCondition: (unit) => unit.Character.Owner != card.Owner && unit != GManager.instance.turnStateMachine.DefendingUnit,
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: true,
                    CanEndNotMax: false,
                    SelectUnitCoroutine: null,
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Move,
                    cardEffect:activateClass_Support);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));
            }
        }

        return supportEffects;
    }
    #endregion
}