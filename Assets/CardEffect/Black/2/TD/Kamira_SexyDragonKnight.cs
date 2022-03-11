using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class Kamira_SexyDragonKnight : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDestroyDuringBattleAlly)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("闇の高鳴り", "Dark Pulsation", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false, card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (GManager.instance.turnStateMachine.AttackingUnit != null)
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit.Character != null)
                    {
                        if (GManager.instance.turnStateMachine.AttackingUnit.Character.Owner == card.Owner)
                        {
                            if(GManager.instance.turnStateMachine.AttackingUnit != card.UnitContainingThisCharacter())
                            {
                                if (GManager.instance.turnStateMachine.AttackingUnit.Character.cardColors.Contains(CardColor.Black))
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
                PowerModifyClass powerUpClass = new PowerModifyClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter(), true);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);

                yield return null;
            }
        }

        else if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("フィンブル", "Fimbulvetr",new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, null, -1, false, card);
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

                PowerModifyClass powerUpClass = new PowerModifyClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power - 10, (unit) => unit == card.UnitContainingThisCharacter(), true);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);

                yield return null;
            }
        }

        return cardEffects;
    }
}