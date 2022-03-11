using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Doni_GrownVillager : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnAttackAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("才能の開花", "Blossoming Aptitude", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition } , -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass.SetCCS(card.UnitContainingThisCharacter());
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
                PowerModifyClass powerUpClass = new PowerModifyClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10 * card.Owner.TrashCards.Count((cardSource) => cardSource.UnitNames.Contains("ドニ")), (unit) => unit == card.UnitContainingThisCharacter(), true);
                card.UnitContainingThisCharacter().UntilEndBattleEffects.Add((_timing) => powerUpClass);
                yield return null;
            }
        }

        else if (timing == EffectTiming.OnStartTurn)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("畑に水やるべ!", "Water the Fields!", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
                {
                    return true;
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                List<CardSource> TopCards = new List<CardSource>();

                for (int i = 0; i < 3; i++)
                {
                    yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(card.Owner));

                    if(card.Owner.LibraryCards.Count > 0)
                    {
                        CardSource topCard = card.Owner.LibraryCards[0];
                        TopCards.Add(topCard);
                        CardObjectController.AddTrashCard(topCard);
                        card.Owner.LibraryCards.Remove(topCard);
                    }
                    
                    yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(card.Owner));
                }

                if(TopCards.Count > 0)
                {
                    ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(TopCards, "Trash Card", true));
                }
            }
        }


        return cardEffects;
    }
}