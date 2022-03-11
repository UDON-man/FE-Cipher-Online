using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Nesara_KilvasKing : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnEnterFieldAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("仕事の報酬", "Recompense for Labor", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, false,card);
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

                            if(Unit.Character != null)
                            {
                                if (Unit.Character.Owner == card.Owner)
                                {
                                    if (Unit != card.UnitContainingThisCharacter())
                                    {
                                        if (Unit.Weapons.Contains(Weapon.Beast))
                                        {
                                            if (card.Owner.FieldUnit.Count((unit) => unit != Unit && unit != card.UnitContainingThisCharacter() && unit.Weapons.Contains(Weapon.Beast)) > 0)
                                            {
                                                return true;
                                            }
                                        }
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
                yield return ContinuousController.instance.StartCoroutine(new IDraw(card.Owner,1).Draw());
            }
        }

        else if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("疾風の刃", "Vortex", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, null, 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                WeaponChangeClass weaponChangeClass = new WeaponChangeClass();
                weaponChangeClass.SetUpWeaponChangeClass((CardSource, Weapons) => { Weapons.Add(Weapon.MagicBook); return Weapons; }, CanWeaponChangeCondition);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => weaponChangeClass);

                bool CanWeaponChangeCondition(CardSource cardSource)
                {
                    if (card.UnitContainingThisCharacter() != null)
                    {
                        if (cardSource == card.UnitContainingThisCharacter().Character)
                        {
                            return true;
                        }
                    }

                    return false;
                }

                RangeUpClass rangeUpClass = new RangeUpClass();
                rangeUpClass.SetUpRangeUpClass((unit, Range) => { Range.Add(1); Range.Add(2); return Range; }, (unit) => unit == card.UnitContainingThisCharacter());
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => rangeUpClass);

                PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
                powerUpByEnemy.SetUpPowerUpByEnemyWeapon("飛行特効", (enemyUnit, Power) => Power + 30, (unit) => unit == card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Wing), PowerUpByEnemy.Mode.Attacking,card);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => powerUpByEnemy);

                yield return null;
            }
        }

        return cardEffects;
    }
}