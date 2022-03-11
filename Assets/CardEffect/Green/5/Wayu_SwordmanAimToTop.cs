using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Wayu_SwordmanAimToTop : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDestroyDuringBattleAlly)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("より高く! より強く!", "Even higher! Even stronger!", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition1 }, -1, true,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition1(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable,card))
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit != null)
                    {
                        if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                List<CardSource> TopCards = new List<CardSource>();

                for (int i = 0; i < 4; i++)
                {
                    yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(card.Owner));

                    if (card.Owner.LibraryCards.Count > 0)
                    {
                        CardSource topCard = card.Owner.LibraryCards[0];
                        TopCards.Add(topCard);
                        card.Owner.LibraryCards.Remove(topCard);
                    }

                    yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(card.Owner));
                }

                SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                selectCardEffect.SetUp(
                    CanTargetCondition: (cardSource) => cardSource.PlayCost >= 4,
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    CanNoSelect: CanNoSelect,
                    SelectCardCoroutine: null,
                    AfterSelectCardCoroutine: (targetCards) => AfterSelectCardCoroutine(targetCards),
                    Message: "Select a card to add to hand.",
                    MaxCount: 1,
                    CanEndNotMax: false,
                    isShowOpponent: true,
                    mode: SelectCardEffect.Mode.AddHand,
                    root: SelectCardEffect.Root.Custom,
                    CustomRootCardList: TopCards,
                    CanLookReverseCard: true,
                    SelectPlayer: card.Owner,
                    cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));

                bool CanNoSelect()
                {
                    if(TopCards.Count((cardSource) => cardSource.PlayCost >= 4) == 0)
                    {
                        return true;
                    }

                    return false;
                }

                IEnumerator AfterSelectCardCoroutine(List<CardSource> targetCards)
                {
                    foreach (CardSource cardSource in TopCards)
                    {
                        if (!targetCards.Contains(cardSource))
                        {
                            CardObjectController.AddTrashCard(cardSource);
                        }
                    }

                    yield return null;
                }
            }
        }

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("相手に不足なし!", (enemyUnit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter(), enemyUnitCondition, PowerUpByEnemy.Mode.Both,card);
        cardEffects.Add(powerUpByEnemy);

        bool enemyUnitCondition(Unit enemyUnit)
        {
            if(enemyUnit.Character != null)
            {
                if(enemyUnit.Character.PlayCost >= 4)
                {
                    return true;
                }
            }

            return false;
        }

        return cardEffects;
    }
}

