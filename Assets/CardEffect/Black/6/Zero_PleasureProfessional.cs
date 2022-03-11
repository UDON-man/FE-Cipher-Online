using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Zero_PleasureProfessional : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDestroyedAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("一緒にイコうぜ?", "Up for a ride?",new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (hashtable != null)
                {
                    if (hashtable.ContainsKey("Unit"))
                    {
                        if (hashtable["Unit"] is Unit)
                        {
                            Unit Unit = (Unit)hashtable["Unit"];

                            if (Unit.Character != null)
                            {
                                if (Unit.Character.Owner == card.Owner && Unit.Character != card)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add(UntilTurnEndAction);

                ICardEffect UntilTurnEndAction(EffectTiming _timing)
                {
                    if (_timing == EffectTiming.OnDestroyDuringBattleAlly)
                    {
                        ActivateClass activateClass1 = new ActivateClass();
                        activateClass1.SetUpICardEffect("カードを1枚引く", "Draw\n1 card.", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition1 }, -1, false, card);
                        activateClass1.SetUpActivateClass((hashtable1) => ActivateCoroutine1());
                        return activateClass1;

                        bool CanUseCondition1(Hashtable hashtable1)
                        {
                            if (card.UnitContainingThisCharacter() != null)
                            {
                                if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                                {
                                    return true;
                                }
                            }

                            return false;
                        }

                        IEnumerator ActivateCoroutine1()
                        {
                            yield return ContinuousController.instance.StartCoroutine(new IDraw(card.Owner, 1).Draw());
                        }
                    }

                    return null;
                }

                yield return null;
            }
        }

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("飛行特効", (enemyUnit, Power) => Power + 30, (unit) => unit == card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Wing), PowerUpByEnemy.Mode.Attacking,card);
        cardEffects.Add(powerUpByEnemy);

        return cardEffects;
    }
}

