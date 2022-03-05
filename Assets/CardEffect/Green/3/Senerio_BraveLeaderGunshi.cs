using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Senerio_BraveLeaderGunshi : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnStartTurn)
        {
            activateClass[0].SetUpICardEffect("寡兵での戦い", new List<Cost>() , new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine1());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Battlefield Efficiency";
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if(GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
                {
                    if(card.Owner.HandCards.Count <= 3)
                    {
                        return true;
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine1()
            {
                yield return ContinuousController.instance.StartCoroutine(new IDraw(card.Owner, 1).Draw());
            }
        }

        else if(timing == EffectTiming.OnDeclaration)
        {
            activateClass[1].SetUpICardEffect("闇衣の参謀", new List<Cost>() { new DiscardHandCost(1, (cardSource) => cardSource.UnitNames.Contains("セネリオ")) }, null, -1, false);
            activateClass[1].SetUpActivateClass((hashtable) => ActivateCoroutine2());
            cardEffects.Add(activateClass[1]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[1].EffectName = "Espionage Scenario";
            }

            IEnumerator ActivateCoroutine2()
            {
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: CanTargetCondition,
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: card.Owner.Enemy.FieldUnit.Count((unit) => CanTargetCondition(unit)),
                    CanNoSelect: true,
                    CanEndNotMax: true,
                    SelectUnitCoroutine: (unit) => SelectUnitCoroutine(unit),
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Custom);

                yield return StartCoroutine(selectUnitEffect.Activate(null));

                bool CanTargetCondition(Unit unit)
                {
                    if(unit.Character.Owner != card.Owner)
                    {
                        if(unit.Power >= 80)
                        {
                            return true;
                        }
                    }

                    return false;
                }

                IEnumerator SelectUnitCoroutine(Unit unit)
                {
                    CanNotSupportClass canNotSupportClass = new CanNotSupportClass();
                    canNotSupportClass.SetUpCanNotSupportClass((cardSource) => cardSource.Owner == card.Owner.Enemy, (_unit) => _unit == unit);
                    unit.UntilEachTurnEndUnitEffects.Add(canNotSupportClass);

                    yield return null;
                }
            }
        }

        return cardEffects;
    }
}
