using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Fericia_CuteDestrroyGod : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("まかせてください!", "Leave it to me!", new List<Cost>() { new TapCost(), new ReverseCost(1, (cardSource) => true) }, null, -1, false, card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                Hashtable hashtable = new Hashtable();
                hashtable.Add("cardEffect", activateClass);
                hashtable.Add("Unit", new Unit(card.Owner.Lord.Characters));
                IDestroyUnit destroyUnit = new IDestroyUnit(card.Owner.Lord, 1, BreakOrbMode.Hand, hashtable);
                yield return ContinuousController.instance.StartCoroutine(destroyUnit.Destroy());

                if(destroyUnit.Destroyed && !GManager.instance.turnStateMachine.endGame)
                {
                    SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                    selectUnitEffect.SetUp(
                        SelectPlayer: card.Owner,
                        CanTargetCondition: (unit) => unit.Character.Owner.Lord != unit && unit.Character.Owner != card.Owner,
                        CanTargetCondition_ByPreSelecetedList: null,
                        CanEndSelectCondition: null,
                        MaxCount: 1,
                        CanNoSelect: false,
                        CanEndNotMax: false,
                        SelectUnitCoroutine: null,
                        AfterSelectUnitCoroutine: null,
                        SelectUnitEffect.Mode.Destroy,
                        cardEffect: activateClass);

                    yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));
                }
            }
        }

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("負けませんよー!", "", null, null, -1, false, card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, CanPowerUpCondition, true);
        cardEffects.Add(powerUpClass);

        bool CanPowerUpCondition(Unit unit)
        {
            if (card.UnitContainingThisCharacter() == unit)
            {
                if (card.Owner.OrbCards.Count == 0)
                {
                    return true;
                }
            }

            return false;
        }

        return cardEffects;
    }
}
