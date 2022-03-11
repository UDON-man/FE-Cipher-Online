using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Hinoka_RedPrincess : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            int maxCount = 1;

            if (card != null)
            {
                if (card.UnitContainingThisCharacter() != null)
                {
                    if (card.UnitContainingThisCharacter().IsClassChanged())
                    {
                        maxCount = 114514;
                    }
                }
            }

            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("叱咤激励", "Rallying Cry", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, null, maxCount, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner == card.Owner && unit != card.UnitContainingThisCharacter() && unit.Power <= 30,
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: false,
                    CanEndNotMax: false,
                    SelectUnitCoroutine: (unit) => SelectUnitCoroutine(unit),
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Custom,
                    cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));

                IEnumerator SelectUnitCoroutine(Unit unit)
                {
                    PowerModifyClass powerUpClass = new PowerModifyClass();
                    powerUpClass.SetUpPowerUpClass((_unit, Power) => Power + 30, (_unit) => _unit == unit, true);
                    unit.UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);

                    yield return null;
                }
            }
        }

        ChangeCCCostClass changeCCCostClass = new ChangeCCCostClass();
        changeCCCostClass.SetUpICardEffect("護りの騎手", "",new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
        changeCCCostClass.SetUpChangeCCCostClass((cardSource,targetUnit,CCCost) => 2, (cardSource) => cardSource == card);
        cardEffects.Add(changeCCCostClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if(card.Owner.FieldUnit.Count((unit) => !unit.Character.UnitNames.Contains("ヒノカ")) >= 2)
            {
                return true;
            }

            return false;
        }

        return cardEffects;
    }
}
