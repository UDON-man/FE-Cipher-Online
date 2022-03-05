using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Julian_SearchForSaint : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[0].SetUpICardEffect("すり抜け", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Pass";
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if(card.Owner.Enemy.GetFrontUnits().Count <= 2)
                {
                    return true;
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                CanAttackTargetUnitRegardlessRangeClass canAttackTargetUnitRegardlessRangeClass = new CanAttackTargetUnitRegardlessRangeClass();
                canAttackTargetUnitRegardlessRangeClass.SetUpCanAttackTargetUnitRegardlessRangeClass((AttackingUnit) => AttackingUnit == card.UnitContainingThisCharacter() && card.Owner.GetFrontUnits().Contains(AttackingUnit), (DefendingUnit) => DefendingUnit.Character.Owner.GetBackUnits().Contains(DefendingUnit));
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add(canAttackTargetUnitRegardlessRangeClass);

                yield return null;
            }
        }

        else if(timing == EffectTiming.OnDestroyDuringBattleAlly)
        {
            activateClass[1].SetUpICardEffect("義賊のお宝", new List<Cost>() , new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
            activateClass[1].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[1]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[1].EffectName = "The Just Theif's Treasure";
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable))
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                    {
                        if (GetCardEffects(EffectTiming.OnDeclaration).Count > 0)
                        {
                            ICardEffect cardEffect = GetCardEffects(EffectTiming.OnDeclaration)[0];

                            if (cardEffect.EffectName == "すり抜け"|| cardEffect.EffectName == "Pass")
                            {
                                if (cardEffect.UseCountThisTurn > 0)
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
                if(card.Owner.Enemy.HandCards.Count > 0)
                {
                    SelectHandEffect selectHandEffect = GetComponent<SelectHandEffect>();

                    selectHandEffect.SetUp(
                            SelectPlayer: card.Owner.Enemy,
                            CanTargetCondition: (cardSource) => cardSource.Owner.HandCards.Contains(cardSource),
                            CanTargetCondition_ByPreSelecetedList: null,
                            CanEndSelectCondition: null,
                            MaxCount: 1,
                            CanNoSelect: true,
                            CanEndNotMax: false,
                            isShowOpponent: true,
                            SelectCardCoroutine: null,
                            AfterSelectCardCoroutine: (targetCards) => AfterSelectCardCoroutine(targetCards),
                            mode: SelectHandEffect.Mode.Custom);

                    yield return ContinuousController.instance.StartCoroutine(selectHandEffect.Activate(null));

                    IEnumerator AfterSelectCardCoroutine(List<CardSource> targetCards)
                    {
                        if(targetCards.Count > 0)
                        {
                            foreach (CardSource cardSource in targetCards)
                            {
                                Hashtable hashtable = new Hashtable();
                                hashtable.Add("cardEffect", selectHandEffect);
                                yield return StartCoroutine(cardSource.cardOperation.DiscardFromHand(hashtable));
                            }
                        }

                        else
                        {
                            yield return ContinuousController.instance.StartCoroutine(new IDraw(card.Owner, 1).Draw());
                        }
                    }
                }

                else
                {
                    yield return ContinuousController.instance.StartCoroutine(new IDraw(card.Owner, 1).Draw());
                }

            }
        }

        return cardEffects;
    }
}

