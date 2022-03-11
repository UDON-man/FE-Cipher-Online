using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Fericia_WellWorkingMaid : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("こっちですよー!", "Didn't see that coming, did'ja!", new List<Cost>() , new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine0());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (!card.Owner.Lord.IsTapped)
                {
                    return true;
                }

                return false;
            }

            IEnumerator ActivateCoroutine0()
            {
                yield return ContinuousController.instance.StartCoroutine(card.Owner.Lord.Tap());
                yield return new WaitForSeconds(0.2f);

                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner != card.Owner && unit.Character.Owner.GetBackUnits().Contains(unit),
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: false,
                    CanEndNotMax: false,
                    SelectUnitCoroutine: null,
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Move,
                    cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));
            }

            ActivateClass activateClass1 = new ActivateClass();
            activateClass1.SetUpICardEffect("が、がんばります!", "Let me!", new List<Cost>() , new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
            activateClass1.SetUpActivateClass((hashtable) => ActivateCoroutine1());
            cardEffects.Add(activateClass1);

            IEnumerator ActivateCoroutine1()
            {
                yield return ContinuousController.instance.StartCoroutine(card.Owner.Lord.Tap());
                yield return new WaitForSeconds(0.2f);

                PowerModifyClass powerUpClass = new PowerModifyClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter(), true);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);
            }
        }

        return cardEffects;
    }
}