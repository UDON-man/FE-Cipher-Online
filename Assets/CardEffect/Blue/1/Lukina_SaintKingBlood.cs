using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Lukina_SaintKingBlood : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("聖なる血脈", "Royal Bloodline",new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>(){ CanUseCondition}, 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (card.Owner.FieldUnit.Count((unit) => unit.Character.UnitNames.Contains("クロム")) == 0)
                {
                    return false;
                }

                return true;
            }

            IEnumerator ActivateCoroutine()
            {
                PowerModifyClass powerUpClass = new PowerModifyClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter(), true);

                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);

                foreach(Unit unit in card.Owner.FieldUnit)
                {
                    if(unit.Character.UnitNames.Contains("クロム"))
                    {
                        PowerModifyClass powerUpClass1 = new PowerModifyClass();
                        powerUpClass1.SetUpPowerUpClass((_unit, Power) => Power + 10, (_unit) => _unit == unit, true);

                        unit.UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass1);
                        break;
                    }
                }

                yield return null;
            }
        }

        return cardEffects;
    }
}

