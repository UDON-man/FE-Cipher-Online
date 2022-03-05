using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Mamori_EveyonesSister : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[0].SetUpICardEffect("まもってあげたい", new List<Cost>() { new DiscardHandCost(1, (cardSource) => cardSource.UnitNames.Contains("源まもり") || cardSource.UnitNames.Contains("ドーガ")) }, null, -1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Raise Defense";
            }

            IEnumerator ActivateCoroutine()
            {
                foreach(Unit unit in card.Owner.FieldUnit)
                {
                    CanNotMoveBySkillClass canNotMoveBySkillClass = new CanNotMoveBySkillClass();
                    canNotMoveBySkillClass.SetUpICardEffect("", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
                    canNotMoveBySkillClass.SetUpCanNotMoveBySkillClass((_unit) => _unit == unit && _unit.Weapons.Contains(Weapon.Armor));
                    unit.UntilOpponentTurnEndEffects.Add(canNotMoveBySkillClass);

                    CanNotDestroyedBySkillClass canNotDestroyedBySkillClass = new CanNotDestroyedBySkillClass();
                    canNotDestroyedBySkillClass.SetUpICardEffect("", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
                    canNotDestroyedBySkillClass.SetUpCanNotDestroyedBySkillClass((_unit) => _unit == unit && _unit.Weapons.Contains(Weapon.Armor),(skill) => true);
                    unit.UntilOpponentTurnEndEffects.Add(canNotDestroyedBySkillClass);

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

        PowerUpClass powerUpClass = new PowerUpClass();
        powerUpClass.SetUpICardEffect("雨音のメモリー", null, null, -1, false);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + card.Owner.FieldUnit.Count((_unit) => _unit != unit && _unit.Weapons.Contains(Weapon.Armor)), (unit) => unit == card.UnitContainingThisCharacter());
        cardEffects.Add(powerUpClass);

        return cardEffects;
    }
}
