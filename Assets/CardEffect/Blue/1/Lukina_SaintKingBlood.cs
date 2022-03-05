using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Lukina_SaintKingBlood : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[0].SetUpICardEffect("聖なる血脈", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>(){ CanUseCondition}, 1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Royal Bloodline";
            }

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
                PowerUpClass powerUpClass = new PowerUpClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter());

                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add(powerUpClass);

                foreach(Unit unit in card.Owner.FieldUnit)
                {
                    if(unit.Character.UnitNames.Contains("クロム"))
                    {
                        PowerUpClass powerUpClass1 = new PowerUpClass();
                        powerUpClass1.SetUpPowerUpClass((_unit, Power) => Power + 10, (_unit) => _unit == unit);

                        unit.UntilEachTurnEndUnitEffects.Add(powerUpClass1);
                        break;
                    }
                }

                yield return null;
            }
        }

        return cardEffects;
    }
}

