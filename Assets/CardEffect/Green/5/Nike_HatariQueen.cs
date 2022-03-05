using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Nike_HatariQueen : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnAttackAnyone)
        {
            if (card.Owner.Enemy.HandCards.Count > 0)
            {
                activateClass[0].SetUpICardEffect("邪眼", new List<Cost>() , new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
                activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
                cardEffects.Add(activateClass[0]);

                if (ContinuousController.instance.language == Language.ENG)
                {
                    activateClass[0].EffectName = "Glare";
                }

                bool CanUseCondition(Hashtable hashtable)
                {
                    if(IsExistOnField(hashtable))
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
                            mode: SelectHandEffect.Mode.Discard);

                    yield return ContinuousController.instance.StartCoroutine(selectHandEffect.Activate(null));
                }

            }
        }

        CanNotAttackClass canNotAttackClass = new CanNotAttackClass();
        canNotAttackClass.SetUpICardEffect("威風", null, new List<Func<Hashtable, bool>>() { CanUseCondition1 }, -1, false);
        canNotAttackClass.SetUpCanNotAttackClass((AttackingUnit) => AttackingUnit.Character.Owner != this.card.Owner && AttackingUnit.Character.Owner.GetBackUnits().Contains(AttackingUnit), (DefendingUnit) => DefendingUnit == this.card.UnitContainingThisCharacter());
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

        return cardEffects;
    }
}