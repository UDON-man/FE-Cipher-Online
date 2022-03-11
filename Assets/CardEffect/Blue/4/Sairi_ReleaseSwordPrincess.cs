using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
public class Sairi_ReleaseSwordPrincess : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("解放軍の指揮官","", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter(), true);
        cardEffects.Add(powerUpClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if (card.Owner.GetBackUnits().Count((unit) => unit.Power <= 40 && unit != card.UnitContainingThisCharacter()) > 0)
            {
                return true;
            }

            return false;
        }

        if(timing == EffectTiming.OnEvadeAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("流水の剣", "Flowing Sword", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition1 }, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition1(Hashtable hashtable)
            {
                if(IsExistOnField(hashtable,card))
                {
                    if(GManager.instance.turnStateMachine.DefendingUnit != null)
                    {
                        if(GManager.instance.turnStateMachine.DefendingUnit == card.UnitContainingThisCharacter())
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                int plusPower = 10;

                if(card.UnitContainingThisCharacter() != null)
                {
                    if(card.UnitContainingThisCharacter().IsClassChanged())
                    {
                        plusPower = 20;
                    }
                }

                PowerModifyClass powerUpClass1 = new PowerModifyClass();
                powerUpClass1.SetUpPowerUpClass((unit, Power) => Power + plusPower, (unit) => unit == card.UnitContainingThisCharacter(), true);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass1);

                yield return null;
            }
        }

        return cardEffects;
    }
}
