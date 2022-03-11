using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Cilius_MaskedBlackKnight : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnStartTurn)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("隠された眼光", "Piercing Eyes",new List<Cost>() , new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if(GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
                {
                    if(card.UnitContainingThisCharacter() != null)
                    {
                        if (card.Owner.GetFrontUnits().Contains(card.UnitContainingThisCharacter()))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner != card.Owner,
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: false,
                    CanEndNotMax: false,
                    SelectUnitCoroutine: (unit) => SelectUnitCoroutine(unit),
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Custom,
                    cardEffect:activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));

                IEnumerator SelectUnitCoroutine(Unit unit)
                {
                    PowerModifyClass powerUpClass = new PowerModifyClass();
                    powerUpClass.SetUpPowerUpClass((_unit, Power) => Power - 10, (_unit) => _unit == unit, true);
                    unit.UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);

                    ChangePlayCostClass changePlayCostClass = new ChangePlayCostClass();
                    changePlayCostClass.SetUpChangeCCCostClass((cardSource,PlayCost) => 1,　(cardSource) => cardSource == unit.Character,false);
                    unit.UntilEachTurnEndUnitEffects.Add((_timing) => changePlayCostClass);

                    yield return null;
                }
            }
        }

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("圧倒する槍技", (enemyUnit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit != enemyUnit.Character.Owner.Lord && enemyUnit.Character.PlayCost <= 2, PowerUpByEnemy.Mode.Attacking, card);
        cardEffects.Add(powerUpByEnemy);

        return cardEffects;
    }
}
