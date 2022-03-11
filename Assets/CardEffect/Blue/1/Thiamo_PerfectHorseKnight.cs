using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Thiamo_PerfectHorseKnight : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDestroyDuringBattleAlly)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("疾風迅雷", "Galeforce", new List<Cost>() , new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable,card))
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                    {
                        return true;
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                Hashtable hashtable = new Hashtable();
                hashtable.Add("cardEffect", activateClass);
                yield return ContinuousController.instance.StartCoroutine(card.UnitContainingThisCharacter().UnTap(hashtable));

                yield return new WaitForSeconds(0.2f);
            }
        }

        else if(timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("手製の手槍", "Handcrafted Javelin", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>(), -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass.SetCCS(card.UnitContainingThisCharacter());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                WeaponChangeClass weaponChangeClass = new WeaponChangeClass();
                weaponChangeClass.SetUpWeaponChangeClass(ChangeWeapons, CanWeaponChangeCondition);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => weaponChangeClass);

                List<Weapon> ChangeWeapons(CardSource cardSource, List<Weapon> Weapons)
                {
                    if(Weapons.Contains(Weapon.Wing))
                    {
                        Weapons.Add(Weapon.Lance);
                    }

                    return Weapons;
                }

                bool CanWeaponChangeCondition(CardSource cardSource)
                {
                    if (cardSource.UnitContainingThisCharacter() != null)
                    {
                        if (cardSource.Owner == card.Owner)
                        {
                            return true;
                        }
                    }

                    return false;
                }

                RangeUpClass rangeUpClass = new RangeUpClass();
                rangeUpClass.SetUpRangeUpClass((unit, Range) => { Range.Add(1); Range.Add(2); return Range; }, (unit) => unit.Weapons.Contains(Weapon.Wing) && unit.Character.Owner == card.Owner);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => rangeUpClass);
                
                yield return null;
            }
        }

        return cardEffects;
    }
}

