using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Elfie_KnightOfMight : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnCCAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("剛腕旋風", "Strongarm Whirwind",new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition1 }, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition1(Hashtable hashtable)
            {
                if (hashtable != null)
                {
                    if (hashtable.ContainsKey("Unit"))
                    {
                        if (hashtable["Unit"] is Unit)
                        {
                            Unit Unit = (Unit)hashtable["Unit"];

                            if (Unit.Character != null)
                            {
                                if (Unit.Character == card)
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
                List<Unit> DestroyUnit = new List<Unit>();

                foreach (Unit unit in card.Owner.Enemy.FieldUnit)
                {
                    if (unit != card.Owner.Enemy.Lord && unit.Character.PlayCost <= 2)
                    {
                        DestroyUnit.Add(unit);
                    }
                }

                foreach (Unit unit in DestroyUnit)
                {
                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("cardEffect", activateClass);
                    hashtable.Add("Unit",new Unit(unit.Characters));
                    yield return ContinuousController.instance.StartCoroutine(new IDestroyUnit(unit, 1, BreakOrbMode.Hand, hashtable).Destroy());
                }
            }
        }

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("重装の騎士","", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter(), true);
        cardEffects.Add(powerUpClass);

        CanNotMoveBySkillClass canNotMoveBySkillClass = new CanNotMoveBySkillClass();
        canNotMoveBySkillClass.SetUpICardEffect("重装の騎士", "", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false, card);
        canNotMoveBySkillClass.SetUpCanNotMoveBySkillClass((unit) => card.Owner.FieldUnit.Contains(unit));
        cardEffects.Add(canNotMoveBySkillClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if (GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner.Enemy)
            {
                return true;
            }

            return false;
        }

        return cardEffects;
    }
}
