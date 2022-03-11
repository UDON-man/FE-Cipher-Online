using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Nike_HatariQueen : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnAttackAnyone)
        {
            if (card.Owner.Enemy.HandCards.Count > 0)
            {
                ActivateClass activateClass = new ActivateClass();
                activateClass.SetUpICardEffect("邪眼", "Glare", new List<Cost>() , new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
                activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
                cardEffects.Add(activateClass);

                bool CanUseCondition(Hashtable hashtable)
                {
                    if(IsExistOnField(hashtable,card))
                    {
                        if(GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                        {
                            if(card.Owner.Enemy.HandCards.Count >= 5)
                            {
                                return true;
                            }
                        }
                    }

                    return false;
                }

                IEnumerator ActivateCoroutine()
                {
                    SelectHandEffect selectHandEffect = GetComponent<SelectHandEffect>();

                    selectHandEffect.SetUp(
                        SelectPlayer: card.Owner.Enemy,
                        CanTargetCondition: (cardSource) => cardSource.Owner.HandCards.Contains(cardSource),
                        CanTargetCondition_ByPreSelecetedList: null,
                        CanEndSelectCondition: null,
                        MaxCount: 1,
                        CanNoSelect: false,
                        CanEndNotMax: false,
                        isShowOpponent: true,
                        SelectCardCoroutine: null,
                        AfterSelectCardCoroutine: null,
                        mode: SelectHandEffect.Mode.Discard,
                        cardEffect: activateClass);

                    yield return ContinuousController.instance.StartCoroutine(selectHandEffect.Activate(null));
                }

            }
        }

        CanNotAttackClass canNotAttackClass = new CanNotAttackClass();
        canNotAttackClass.SetUpICardEffect("威風","", null, new List<Func<Hashtable, bool>>() { CanUseCondition1 }, -1, false,card);
        canNotAttackClass.SetUpCanNotAttackClass(AttackingCondition, DefendingCondition);
        cardEffects.Add(canNotAttackClass);

        bool CanUseCondition1(Hashtable hashtable)
        {
            if (GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
            {
                if (card.Owner.FieldUnit.Count((_unit) => _unit != card.UnitContainingThisCharacter() && _unit.Weapons.Contains(Weapon.Beast)) >= 2)
                {
                    return true;
                }
            }

            return false;
        }

        bool AttackingCondition(Unit AttackingUnit)
        {
            if(AttackingUnit != null)
            {
                if(AttackingUnit.Character != null)
                {
                    if(AttackingUnit.Character.Owner != card.Owner && AttackingUnit.Character.Owner.GetBackUnits().Contains(AttackingUnit))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        bool DefendingCondition(Unit DefendingUnit)
        {
            if (DefendingUnit != null)
            {
                if (DefendingUnit.Character != null)
                {
                    if (DefendingUnit == card.UnitContainingThisCharacter())
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