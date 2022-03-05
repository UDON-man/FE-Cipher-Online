using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Yashiro_FirstClassBeyondFirstClass : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDestroyDuringBattleAlly)
        {
            activateClass[0].SetUpICardEffect("因果切断", new List<Cost>() { new ReverseCost(1, (cardSource) => true), new DiscardHandCost(1, (cardSource) => cardSource.UnitNames.Contains("剣弥代") || cardSource.UnitNames.Contains("ナバール")) },new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Black Rain";
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
                List<Unit> targetUnits = new List<Unit>();

                foreach(Unit unit in card.Owner.Enemy.FieldUnit)
                {
                    if(unit != unit.Character.Owner.Lord && unit.Character.PlayCost <= 2)
                    {
                        targetUnits.Add(unit);
                    }
                }

                foreach (Unit unit in targetUnits)
                {
                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("cardEffect", activateClass[0]);
                    yield return ContinuousController.instance.StartCoroutine(new IDestroyUnit(unit, 1, BreakOrbMode.Hand, hashtable).Destroy());
                }
            }
        }

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("一流の輝き", (enemyUnit, Power) => Power + 10, (unit) => unit == this.card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Character.PlayCost <= 2, PowerUpByEnemy.Mode.Defending);
        cardEffects.Add(powerUpByEnemy);

        return cardEffects;
    }
}
