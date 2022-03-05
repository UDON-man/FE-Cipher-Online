using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class KamuiOtoko_SelectedFuture : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnAttackAnyone)
        {
            SelectAllyCost selectAllyCost = new SelectAllyCost(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner == card.Owner && !unit.IsTapped &&(unit.Character.UnitNames.Contains("リョウマ")|| unit.Character.UnitNames.Contains("ヒノカ")|| unit.Character.UnitNames.Contains("タクミ")|| unit.Character.UnitNames.Contains("サクラ")),
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: false,
                    CanEndNotMax: false,
                    SelectUnitCoroutine: null,
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Tap);

            activateClass[0].SetUpICardEffect("白夜に捧ぐ剣", new List<Cost>() { selectAllyCost }, new List<Func<Hashtable, bool>>() { CanUseCondition1 }, -1,true);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Consecrated Sword of Hoshido";
            }

            bool CanUseCondition1(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable))
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                    {
                        if (GManager.instance.turnStateMachine.DefendingUnit != null)
                        {
                            if (GManager.instance.turnStateMachine.DefendingUnit.Character.cardColors.Contains(CardColor.Black))
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                PowerUpClass powerUpClass1 = new PowerUpClass();
                powerUpClass1.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter());
                card.UnitContainingThisCharacter().UntilEndBattleEffects.Add(powerUpClass1);
                yield return null;
            }
        }

        PowerUpClass powerUpClass = new PowerUpClass();
        powerUpClass.SetUpICardEffect("連なる想い", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit != card.UnitContainingThisCharacter() && unit.Character.Owner == card.Owner);
        cardEffects.Add(powerUpClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if(GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
            {
                if(card.Owner.GetFrontUnits().Contains(card.UnitContainingThisCharacter()))
                {
                    if (card.Owner.FieldUnit.Count((unit) => unit != card.UnitContainingThisCharacter()) >= 3)
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