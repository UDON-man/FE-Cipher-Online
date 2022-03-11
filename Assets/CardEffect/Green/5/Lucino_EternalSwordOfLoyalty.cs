using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Lucino_EternalSwordOfLoyalty : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("女王の姉妹","", null, null, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, PowerUpCondition, true);
        cardEffects.Add(powerUpClass);

        bool PowerUpCondition(Unit unit)
        {
            if (unit == card.UnitContainingThisCharacter())
            {
                if (GManager.instance.turnStateMachine.AttackingUnit != null && GManager.instance.turnStateMachine.DefendingUnit != null)
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit == unit || GManager.instance.turnStateMachine.DefendingUnit == unit)
                    {
                        if (card.UnitContainingThisCharacter() == GManager.instance.turnStateMachine.AttackingUnit || card.UnitContainingThisCharacter() == GManager.instance.turnStateMachine.DefendingUnit)
                        {
                            if (card.Owner.SupportCards.Count((cardSource) => cardSource.UnitNames.Contains("エリンシア")) > 0)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        if (timing == EffectTiming.OnGrowAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("練達の剣技", "Expert Swordmanship", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if(IsExistOnField(hashtable,card))
                {
                    if (hashtable != null)
                    {
                        if (hashtable.ContainsKey("Unit"))
                        {
                            if (hashtable["Unit"] is Unit)
                            {
                                Unit Unit = (Unit)hashtable["Unit"];

                                if (Unit.Character.Owner == card.Owner)
                                {
                                    if (Unit == card.UnitContainingThisCharacter())
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
                CanNotBeEvadedClass canNotBeEvadedClass = new CanNotBeEvadedClass();
                canNotBeEvadedClass.SetUpCanNotBeEvadedClass((AttackingUnit) => AttackingUnit == card.UnitContainingThisCharacter(), (DefendingUnit) => true);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => canNotBeEvadedClass);

                yield return null;
            }
        }

        return cardEffects;
    }
}
