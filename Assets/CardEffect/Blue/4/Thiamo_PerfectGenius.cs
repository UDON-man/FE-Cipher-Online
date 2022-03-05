using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Thiamo_PerfectGenius : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnEndTurn)
        {
            activateClass[0].SetUpICardEffect("完璧な采配", new List<Cost>(), null, -1, true);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Intermezzo";
            }

            IEnumerator ActivateCoroutine()
            {
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner == card.Owner,
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: true,
                    CanEndNotMax: false,
                    SelectUnitCoroutine: null,
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Move);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));
            }
        }

        PowerUpClass powerUpClass = new PowerUpClass();
        powerUpClass.SetUpICardEffect("蒼穹の女神", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition1 }, -1, false);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, PowerUpCondition);
        powerUpClass.SetCCS(card.UnitContainingThisCharacter());
        cardEffects.Add(powerUpClass);

        RangeUpClass rangeUpClass = new RangeUpClass();
        rangeUpClass.SetUpICardEffect("蒼穹の女神", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition2 }, -1, false);
        rangeUpClass.SetUpRangeUpClass((unit, Range) => { Range.Add(1); Range.Add(2); return Range; }, RangeUpCondition);
        rangeUpClass.SetCCS(card.UnitContainingThisCharacter());
        cardEffects.Add(rangeUpClass);

        bool CanUseCondition1(Hashtable hashtable)
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

        bool PowerUpCondition(Unit unit)
        {
            if(unit.Weapons.Contains(Weapon.Lance))
            {
                if (card.Owner.GetFrontUnits().Contains(unit))
                {
                    if (unit.Character.Owner == card.Owner)
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }

        bool CanUseCondition2(Hashtable hashtable)
        {
            if (GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
            {
                if (card.UnitContainingThisCharacter() != null)
                {
                    if (card.Owner.GetBackUnits().Contains(card.UnitContainingThisCharacter()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        bool RangeUpCondition(Unit unit)
        {
            if (unit.Weapons.Contains(Weapon.Lance))
            {
                if (card.Owner.GetBackUnits().Contains(unit))
                {
                    if (unit.Character.Owner == card.Owner)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        return cardEffects;
    }
}
