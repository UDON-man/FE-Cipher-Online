using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Hinoka_RedPrincess : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
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

            activateClass[0].SetUpICardEffect("叱咤激励", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, null, maxCount, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Rallying Cry";
            }

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
                    mode: SelectUnitEffect.Mode.Custom);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));

                IEnumerator SelectUnitCoroutine(Unit unit)
                {
                    PowerUpClass powerUpClass = new PowerUpClass();
                    powerUpClass.SetUpPowerUpClass((_unit, Power) => Power + 30, (_unit) => _unit == unit);
                    unit.UntilEachTurnEndUnitEffects.Add(powerUpClass);

                    yield return null;
                }
            }
        }

        ChangeCCCostClass changeCCCostClass = new ChangeCCCostClass();
        changeCCCostClass.SetUpICardEffect("護りの騎手", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
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
