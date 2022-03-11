using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Uudo_SelectedByGod : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("ダメだ…! 力が…暴走する…!","", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false, card);
        powerUpClass.SetUpPowerUpClass((unit,Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter(), true);
        cardEffects.Add(powerUpClass);

        CanNotAttackClass canNotAttackClass = new CanNotAttackClass();
        canNotAttackClass.SetUpICardEffect("ダメだ…! 力が…暴走する…!","", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
        canNotAttackClass.SetUpCanNotAttackClass((AttackingUnit) => AttackingUnit == card.UnitContainingThisCharacter(),(DefendingUnit) => DefendingUnit != DefendingUnit.Character.Owner.Lord);
        cardEffects.Add(canNotAttackClass);

        CanAttackTargetUnitRegardlessRangeClass canAttackTargetUnitRegardlessRangeClass = new CanAttackTargetUnitRegardlessRangeClass();
        canAttackTargetUnitRegardlessRangeClass.SetUpICardEffect("ダメだ…! 力が…暴走する…!","", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
        canAttackTargetUnitRegardlessRangeClass.SetUpCanAttackTargetUnitRegardlessRangeClass((AttackingUnit) => AttackingUnit == card.UnitContainingThisCharacter(), (DefendingUnit) => DefendingUnit == DefendingUnit.Character.Owner.Lord);
        cardEffects.Add(canAttackTargetUnitRegardlessRangeClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if(GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
            {
                if(card.UnitContainingThisCharacter() != null)
                {
                    if(card.Owner.GetFrontUnits().Contains(card.UnitContainingThisCharacter()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        StrikeModifyClass strikeUpClass = new StrikeModifyClass();
        strikeUpClass.SetUpICardEffect("烈火剣 レイジングファイヤーソード","", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition1 }, -1, false,card);
        strikeUpClass.SetUpStrikeModifyClass((unit, Strike) => 2, (unit) => unit == card.UnitContainingThisCharacter(), false);
        cardEffects.Add(strikeUpClass);

        bool CanUseCondition1(Hashtable hashtable)
        {
            if (card.Owner.HandCards.Count == 0)
            {
                return true;
            }

            return false;
        }

        return cardEffects;
    }
}
