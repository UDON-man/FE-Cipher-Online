using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Matoi_PerfectPersueWing : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDestroyDuringBattleAlly)
        {
            activateClass[0].SetUpICardEffect("速さの叫び", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass[0].SetCCS(card.UnitContainingThisCharacter());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Speed of Sound";
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if(GManager.instance.turnStateMachine.AttackingUnit != null)
                {
                    if(GManager.instance.turnStateMachine.AttackingUnit.Character != null)
                    {
                        if(GManager.instance.turnStateMachine.AttackingUnit.Character.Owner == card.Owner)
                        {
                            if(GManager.instance.turnStateMachine.AttackingUnit.Weapons.Contains(Weapon.Wing))
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
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner == this.card.Owner,
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

        else if(timing == EffectTiming.OnDeclaration)
        {
            activateClass[1].SetUpICardEffect("武器輸送", new List<Cost>() { new ReverseCost(1,(cardSource) => true)}, new List<Func<Hashtable, bool>>(), -1, false);
            activateClass[1].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[1]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[1].EffectName = "Weapon Shift";
            }

            IEnumerator ActivateCoroutine()
            {
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner == this.card.Owner && unit.Weapons.Contains(Weapon.Wing),
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
                    RangeUpClass rangeUpClass = new RangeUpClass();
                    rangeUpClass.SetUpRangeUpClass((_unit, Range) => { Range.Add(1); Range.Add(2); return Range; }, (_unit) => _unit == unit);
                    unit.UntilEachTurnEndUnitEffects.Add(rangeUpClass);

                    yield return null;
                }
            }
        }

        return cardEffects;
    }
}
