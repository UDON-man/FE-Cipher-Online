using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class Zero_Saddhist : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("飛行特効", (enemyUnit, Power) => Power + 30, (unit) => unit == card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Wing), PowerUpByEnemy.Mode.Attacking,card);
        cardEffects.Add(powerUpByEnemy);

        if (timing == EffectTiming.OnDiscardHand)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("イイねぇ、その顔…", "Hey, nice face...", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (card.UnitContainingThisCharacter() != null)
                {
                    if (card.Owner.FieldUnit.Contains(card.UnitContainingThisCharacter()))
                    {
                        if (hashtable != null)
                        {
                            #region 相手が手札を捨てた場合
                            if (hashtable.Contains("Card"))
                            {
                                if(hashtable["Card"] is CardSource)
                                {
                                    CardSource cardSource = (CardSource)hashtable["Card"];

                                    if(cardSource != null)
                                    {
                                        if(cardSource.Owner == card.Owner.Enemy)
                                        {
                                            #region 自分のスキルで捨てられた場合
                                            if (hashtable.ContainsKey("cardEffect"))
                                            {
                                                if (hashtable["cardEffect"] is ICardEffect)
                                                {
                                                    ICardEffect cardEffect = (ICardEffect)hashtable["cardEffect"];

                                                    if (cardEffect != null)
                                                    {
                                                        if (cardEffect.card() != null)
                                                        {
                                                            if (cardEffect.card().Owner == card.Owner)
                                                            {
                                                                return true;
                                                            }
                                                        }
                                                    }

                                                }
                                            }
                                            #endregion
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                PowerModifyClass powerUpClass = new PowerModifyClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 40, (unit) => unit == card.UnitContainingThisCharacter(), true);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);

                yield return null;
            }
        }

        return cardEffects;
    }
}