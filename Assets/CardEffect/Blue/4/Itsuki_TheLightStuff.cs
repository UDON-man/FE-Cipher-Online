using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Itsuki_TheLightStuff : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[0].SetUpICardEffect("揺るぎなき決意", new List<Cost>() { new ReverseCost(3, (cardSource) => true), new DiscardHandCost(1, (cardSource) => cardSource.UnitNames.Contains("蒼井樹") || cardSource.UnitNames.Contains("クロム")) }, null, -1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Firm Resolve";
            }

            IEnumerator ActivateCoroutine()
            {
                foreach (Unit unit in card.Owner.FieldUnit)
                {
                    PowerUpClass powerUpClass = new PowerUpClass();
                    powerUpClass.SetUpPowerUpClass((_unit, Power) => Power + 30, (_unit) => _unit == unit);
                    unit.UntilEachTurnEndUnitEffects.Add(powerUpClass);
                }

                yield return null;
            }
        }

        PowerUpClass powerUpClass1 = new PowerUpClass();
        powerUpClass1.SetUpICardEffect("ロードオブローズ",new List<Cost>(),new List<System.Func<Hashtable, bool>>(),-1,false);
        powerUpClass1.SetUpPowerUpClass((unit,Power) => Power + 40, CanPowerUpCondition);
        cardEffects.Add(powerUpClass1);

        bool CanPowerUpCondition(Unit unit)
        {
            if(unit.Character.Owner == card.Owner)
            {
                if (unit.Character.cEntity_EffectController.GetCardEffects(EffectTiming.None).Count((cardEffect) => cardEffect.isCF) > 0)
                {
                    return true;
                }
            }
            
            return false;
        }

        return cardEffects;
    }
}
