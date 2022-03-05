using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Mikaya_SilverHairGirl : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[0].SetUpICardEffect("癒しの手", new List<Cost>() { new TapCost(), new DestroySelfCost() }, null, -1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Sacrifice";
            }

            IEnumerator ActivateCoroutine()
            {
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner == card.Owner && unit != card.UnitContainingThisCharacter(),
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: false,
                    CanEndNotMax: false,
                    SelectUnitCoroutine: (unit) => SelectUnitCoroutine(unit),
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Custom);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));

                IEnumerator SelectUnitCoroutine(Unit unit)
                {
                    PowerUpClass powerUpClass = new PowerUpClass();
                    powerUpClass.SetUpPowerUpClass((_unit, Power) => Power + 10, (_unit) => _unit == unit);
                    unit.UntilOpponentTurnEndEffects.Add(powerUpClass);

                    yield return null;
                }
            }
        }

        return cardEffects;
    }


    #region 祈りの紋章
    public override List<ICardEffect> SupportEffects(EffectTiming timing)
    {
        List<ICardEffect> supportEffects = new List<ICardEffect>();

        activateClass_Support[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
        activateClass_Support[0].SetUpICardEffect("祈りの紋章", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
        supportEffects.Add(activateClass_Support[0]);

        if (ContinuousController.instance.language == Language.ENG)
        {
            activateClass_Support[0].EffectName = "Miracle Emblem";
        }

        bool CanUseCondition(Hashtable hashtable)
        {
            if (card.Owner.SupportCards.Contains(card))
            {
                if (GManager.instance.turnStateMachine.gameContext.NonTurnPlayer == card.Owner)
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit != null && GManager.instance.turnStateMachine.DefendingUnit != null)
                    {
                        if (GManager.instance.turnStateMachine.DefendingUnit.Character != null)
                        {
                            if (GManager.instance.turnStateMachine.DefendingUnit.Character.Owner == card.Owner)
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
            CanNotCriticalClass canNotCriticalClass = new CanNotCriticalClass();
            canNotCriticalClass.SetUpCanNotCriticalClass((unit) => unit == GManager.instance.turnStateMachine.AttackingUnit && unit.Character.Owner != card.Owner);
            GManager.instance.turnStateMachine.AttackingUnit.UntilEndBattleEffects.Add(canNotCriticalClass);

            yield return null;
        }

        return supportEffects;
    }
    #endregion
}
