using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Glay_SaizouHeir : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpClass powerUpClass = new PowerUpClass();
        powerUpClass.SetUpICardEffect("サイゾウの名前", null, null, -1, false);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, PowerUpCondition);
        powerUpClass.SetCCS(card.UnitContainingThisCharacter());
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
                            if (card.Owner.SupportCards.Count((cardSource) => cardSource.Weapons.Contains(Weapon.DarkWeapon)) > 0)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        if (timing == EffectTiming.OnEndAttackAnyone)
        {
            activateClass[0].SetUpICardEffect("忍の戦法", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Ninja Tactics";
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if(GManager.instance.turnStateMachine.AttackingUnit != null)
                {
                    if(GManager.instance.turnStateMachine.AttackingUnit.Character != null)
                    {
                        if (GManager.instance.turnStateMachine.AttackingUnit.Character.Owner == card.Owner)
                        {
                            if (GManager.instance.turnStateMachine.AttackingUnit.Weapons.Contains(Weapon.DarkWeapon))
                            {
                                if(card.Owner.GetFrontUnits().Contains(GManager.instance.turnStateMachine.AttackingUnit))
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
                Hashtable hashtable = new Hashtable();
                hashtable.Add("cardEffect", activateClass[0]);
                yield return ContinuousController.instance.StartCoroutine(new IMoveUnit(new List<Unit>() { GManager.instance.turnStateMachine.AttackingUnit }, true,hashtable).MoveUnits());
            }
        }


        return cardEffects;
    }
}

