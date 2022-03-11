using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Selice_LightPrince : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnSetBond)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("光を継ぐ者", "Heir of Light", new List<Cost>() , new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, true, card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (IsExistOnField(null, card))
                {
                    if(hashtable != null)
                    {
                        if(hashtable.ContainsKey("Card"))
                        {
                            if(hashtable["Card"] is CardSource)
                            {
                                CardSource cardSource = (CardSource)hashtable["Card"];

                                if(cardSource != null)
                                {
                                    if(cardSource.Owner == card.Owner && cardSource.cardColors.Contains(CardColor.Yellow))
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
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner != card.Owner && unit.Character.Owner.GetBackUnits().Contains(unit),
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: true,
                    CanEndNotMax: false,
                    SelectUnitCoroutine: null,
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Move,
                    cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));
            }
        }

        else if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("奇跡の双光", "Miraculous Twin Lights", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, false, card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if(IsExistOnField(null,card))
                {
                    if (card.Owner.FieldUnit.Count((unit) => unit.Character.UnitNames.Contains("シグルド")) > 0)
                    {
                        return true;
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                Unit masterUnit = null;

                foreach (Unit unit in card.Owner.FieldUnit)
                {
                    if (unit.Character.UnitNames.Contains("シグルド"))
                    {
                        masterUnit = unit;
                        break;
                    }
                }

                if (masterUnit != null)
                {
                    PowerModifyClass powerUpClass = new PowerModifyClass();
                    powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == masterUnit, true);
                    masterUnit.UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);

                    PowerModifyClass powerUpClass1 = new PowerModifyClass();
                    powerUpClass1.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter(), true);
                    card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass1);
                }

                yield return null;
            }
        }

        return cardEffects;
    }
}
