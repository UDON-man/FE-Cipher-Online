using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Thiamto_MercenarySubLeader : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("副長の務め","", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter(), true);
        cardEffects.Add(powerUpClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if (card.Owner.FieldUnit.Count((_unit) => _unit.Character.PlayCost <= 2) >= 2)
            {
                return true;
            }

            return false;
        }

        if (timing == EffectTiming.OnDiscardSuppot)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("後進の育成", "Maternal Nature",null, new List<Func<Hashtable, bool>>() { CanUseCondition1 }, -1, true,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition1(Hashtable hashtable)
            {
                if (GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                    {
                        foreach(CardSource cardSource in card.Owner.SupportCards)
                        {
                            foreach(string SupportUnitName in cardSource.UnitNames)
                            {
                                foreach(Unit unit in card.Owner.FieldUnit)
                                {
                                    if(unit.Character.UnitNames.Contains(SupportUnitName))
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

            IEnumerator ActivateCoroutine()
            {
                List<CardSource> SupportCards = new List<CardSource>();

                foreach (CardSource cardSource in card.Owner.SupportCards)
                {
                    SupportCards.Add(cardSource);
                }

                Unit targetUnit = null;

                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: CanTargetCondition,
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: true,
                    CanEndNotMax: false,
                    SelectUnitCoroutine: (unit) => SelectUnitCoroutine(unit),
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Custom,
                    cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));

                bool CanTargetCondition(Unit unit)
                {
                    if(unit.Character != null)
                    {
                        if (unit.Character.Owner == card.Owner)
                        {
                            foreach (CardSource SupportCard in SupportCards)
                            {
                                foreach (string SupportUnitName in SupportCard.UnitNames)
                                {
                                    if (unit.Character.UnitNames.Contains(SupportUnitName))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                    
                    return false;
                }

                IEnumerator SelectUnitCoroutine(Unit unit)
                {
                    targetUnit = unit;

                    yield return null;
                }

                if (targetUnit != null)
                {
                    card.Owner.SupportHandCard.gameObject.SetActive(false);
                    yield return ContinuousController.instance.StartCoroutine(new IGrow(targetUnit, SupportCards).Grow());
                }
            }
        }

        return cardEffects;
    }
}