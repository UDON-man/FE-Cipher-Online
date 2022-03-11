using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Mamori_EveyonesSister : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("まもってあげたい", "Raise Defense",new List<Cost>() { new DiscardHandCost(1, (cardSource) => cardSource.UnitNames.Contains("源まもり") || cardSource.UnitNames.Contains("ドーガ")) }, null, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                foreach(Unit unit in card.Owner.FieldUnit)
                {
                    CanNotMoveBySkillClass canNotMoveBySkillClass = new CanNotMoveBySkillClass();
                    canNotMoveBySkillClass.SetUpICardEffect("", "",new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
                    canNotMoveBySkillClass.SetUpCanNotMoveBySkillClass((_unit) => _unit == unit && _unit.Weapons.Contains(Weapon.Armor));
                    unit.UntilOpponentTurnEndEffects.Add((_timing) => canNotMoveBySkillClass);

                    CanNotDestroyedBySkillClass canNotDestroyedBySkillClass = new CanNotDestroyedBySkillClass();
                    canNotDestroyedBySkillClass.SetUpICardEffect("", "",new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
                    canNotDestroyedBySkillClass.SetUpCanNotDestroyedBySkillClass((_unit) => _unit == unit && _unit.Weapons.Contains(Weapon.Armor),(skill) => true);
                    unit.UntilOpponentTurnEndEffects.Add((_timing) => canNotDestroyedBySkillClass);

                    bool CanUseCondition(Hashtable hashtable)
                    {
                        if(GManager.instance.turnStateMachine.gameContext.NonTurnPlayer == card.Owner)
                        {
                            return true;
                        }

                        return false;
                    }
                }

                yield return null;
            }
        }

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("雨音のメモリー", "",null, null, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + card.Owner.FieldUnit.Count((_unit) => _unit != unit && _unit.Weapons.Contains(Weapon.Armor)), (unit) => unit == card.UnitContainingThisCharacter(), true);
        cardEffects.Add(powerUpClass);

        return cardEffects;
    }
}
