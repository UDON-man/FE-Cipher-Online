using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Ayra_BladePrincessAstra : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnEvadeAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("流星剣", "Astra", new List<Cost>() { new ReverseCost(1, (cardSource) => cardSource.UnitNames.Contains("アイラ")) }, new List<Func<Hashtable, bool>>() { CanUseCondition1 }, -1, true,card);
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
                Hashtable hashtable = new Hashtable();
                hashtable.Add("cardEffect", activateClass);
                yield return ContinuousController.instance.StartCoroutine(card.UnitContainingThisCharacter().UnTap(hashtable));

                yield return new WaitForSeconds(0.2f);
            }
        }

        else if (timing == EffectTiming.OnDestroyDuringBattleAlly)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("刻命の瞬き", "Shredder", new List<Cost>() , new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable,card))
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                    {
                        return true;
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                List<CardSource> TopCards = new List<CardSource>();

                for (int i = 0; i < 5; i++)
                {
                    yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(card.Owner));

                    if (card.Owner.LibraryCards.Count > 0)
                    {
                        CardSource topCard = card.Owner.LibraryCards[0];
                        TopCards.Add(topCard);
                        CardObjectController.AddTrashCard(topCard);
                        card.Owner.LibraryCards.Remove(topCard);
                    }

                    yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(card.Owner));
                }

                if (TopCards.Count > 0)
                {
                    ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(TopCards, "Revealed Card", true));
                }

                yield return new WaitForSeconds(1f);

                int destroyCount = TopCards.Count((cardSource) => cardSource.UnitNames.Contains("アイラ"));

                if(destroyCount > card.Owner.Enemy.FieldUnit.Count((unit) => unit != unit.Character.Owner.Lord))
                {
                    destroyCount = card.Owner.Enemy.FieldUnit.Count((unit) => unit != unit.Character.Owner.Lord);
                }

                if(destroyCount > 0)
                {
                    SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                    selectUnitEffect.SetUp(
                        SelectPlayer: card.Owner,
                        CanTargetCondition: (unit) => unit.Character.Owner.Lord != unit && unit.Character.Owner != card.Owner,
                        CanTargetCondition_ByPreSelecetedList: null,
                        CanEndSelectCondition: null,
                        MaxCount: destroyCount,
                        CanNoSelect: false,
                        CanEndNotMax: false,
                        SelectUnitCoroutine: null,
                        AfterSelectUnitCoroutine: null,
                        SelectUnitEffect.Mode.Destroy,
                        cardEffect: activateClass);

                    yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));
                }
            }
        }

        return cardEffects;
    }
}

