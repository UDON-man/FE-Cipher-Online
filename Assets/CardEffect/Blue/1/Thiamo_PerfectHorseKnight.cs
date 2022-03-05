using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Thiamo_PerfectHorseKnight : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDestroyDuringBattleAlly)
        {
            activateClass[0].SetUpICardEffect("疾風迅雷", new List<Cost>() , new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Galeforce";
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable))
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
                hashtable.Add("cardEffect", activateClass[0]);
                yield return ContinuousController.instance.StartCoroutine(card.UnitContainingThisCharacter().UnTap(hashtable));

                yield return new WaitForSeconds(0.2f);
            }
        }

        else if(timing == EffectTiming.OnDeclaration)
        {
            activateClass[1].SetUpICardEffect("手製の手槍", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>(), -1, false);
            activateClass[1].SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass[1].SetCCS(card.UnitContainingThisCharacter());
            cardEffects.Add(activateClass[1]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[1].EffectName = "Handcrafted Javelin";
            }

            IEnumerator ActivateCoroutine()
            {
                WeaponChangeClass weaponChangeClass = new WeaponChangeClass();
                weaponChangeClass.SetUpWeaponChangeClass(ChangeWeapons, CanWeaponChangeCondition);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add(weaponChangeClass);

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
                rangeUpClass.SetUpRangeUpClass((unit, Range) => { Range.Add(1); Range.Add(2); return Range; }, (unit) => unit.Weapons.Contains(Weapon.Wing) && unit.Character.Owner == this.card.Owner);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add(rangeUpClass);
                
                yield return null;
            }
        }

        return cardEffects;
    }
}

