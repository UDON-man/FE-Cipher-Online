using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Ronkue_FeriaDemonGod : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnEndAttackAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("双鬼斬", "Double-edged Fury", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (card.UnitContainingThisCharacter() != null)
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                    {
                        if (card.Owner.FieldUnit.Count((unit) => unit.Weapons.Contains(Weapon.Sword) && card.Owner.GetFrontUnits().Contains(unit) && unit != card.UnitContainingThisCharacter()) >= 2)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                PowerModifyClass powerUpClass = new PowerModifyClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power - 20, (unit) => unit == card.UnitContainingThisCharacter(), true);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);

                Hashtable hashtable = new Hashtable();
                hashtable.Add("cardEffect", activateClass);
                yield return ContinuousController.instance.StartCoroutine(card.UnitContainingThisCharacter().UnTap(hashtable));
                yield return new WaitForSeconds(0.2f);
            }
        }

        PowerModifyClass _powerUpClass = new PowerModifyClass();
        _powerUpClass.SetUpICardEffect("剣の冴え","", null, null, -1, false,card);
        _powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, PowerUpCondition, true);
        cardEffects.Add(_powerUpClass);

        bool PowerUpCondition(Unit unit)
        {
            if(GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
            {
                if (unit == card.UnitContainingThisCharacter())
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit != null && GManager.instance.turnStateMachine.DefendingUnit != null)
                    {
                        if (GManager.instance.turnStateMachine.AttackingUnit == unit || GManager.instance.turnStateMachine.DefendingUnit == unit)
                        {
                            if (card.UnitContainingThisCharacter() == GManager.instance.turnStateMachine.AttackingUnit || card.UnitContainingThisCharacter() == GManager.instance.turnStateMachine.DefendingUnit)
                            {
                                if (card.Owner.SupportCards.Count((cardSource) => cardSource.Weapons.Contains(Weapon.Sword)) > 0)
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

        return cardEffects;
    }
}

