using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Lachesis_LittleLionSister : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("大地の剣", "Earth Sword", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, null, -1, false,card);
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

                yield return null;
            }
        }

        else if (timing == EffectTiming.OnDestroyDuringBattleAlly)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("煌めく刃", "Glimmering Blade",new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable,card))
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                    {
                        if (card.cEntity_EffectController.GetUseCountThisTurn("大地の剣") > 0)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                PowerModifyClass powerUpClass1 = new PowerModifyClass();
                powerUpClass1.SetUpPowerUpClass((unit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter(), true);
                card.UnitContainingThisCharacter().UntilOpponentTurnEndEffects.Add((_timing) => powerUpClass1);
                yield return null;
            }
        }

        return cardEffects;
    }
}
