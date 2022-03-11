using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Toma_AwakenHero : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("暴虐のスロットル", "Violent Throttle", new List<Cost>() { new DiscardHandCost(1, (cardSource) => cardSource.UnitNames.Contains("赤城斗馬") || cardSource.UnitNames.Contains("カイン")) }, new List<Func<Hashtable, bool>>() { (hashtable) => IsExistOnField(hashtable,card) }, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                List<Unit> targetUnits = new List<Unit>();
                foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players_ForTurnPlayer)
                {
                    foreach (Unit unit in player.GetBackUnits())
                    {
                        targetUnits.Add(unit);
                    }
                }

                Hashtable hashtable = new Hashtable();
                hashtable.Add("cardEffect", activateClass);
                yield return ContinuousController.instance.StartCoroutine(new IMoveUnit(targetUnits, true, hashtable).MoveUnits());

                PowerModifyClass powerUpClass = new PowerModifyClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power * 2, (unit) => unit == card.UnitContainingThisCharacter(),false);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);
            }
        }

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("ジャンプ!! 鳳牙!!!", (enemyUnit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Dragon), PowerUpByEnemy.Mode.Attacking, card);
        cardEffects.Add(powerUpByEnemy);

        return cardEffects;
    }
}
