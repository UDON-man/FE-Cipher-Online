using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Mozume_WorkWellGirl : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnAttackAnyone)
        {
            activateClass[0].SetUpICardEffect("下剋上", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Underdog";
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable))
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                    {
                        if(GManager.instance.turnStateMachine.DefendingUnit != null)
                        {
                            if(GManager.instance.turnStateMachine.DefendingUnit.Power >= 80)
                            {
                                return true;
                            }
                        }
                        
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                PowerUpClass powerUpClass = new PowerUpClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 30, (unit) => unit == card.UnitContainingThisCharacter());
                card.UnitContainingThisCharacter().UntilEndBattleEffects.Add(powerUpClass);
                yield return null;
            }
        }

        return cardEffects;
    }
}