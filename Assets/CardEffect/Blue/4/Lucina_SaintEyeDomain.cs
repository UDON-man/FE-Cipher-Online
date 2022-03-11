using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Lucina_SaintEyeDomain : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnStartTurn)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("運命を変えます", "Hands of Fate",new List<Cost>() , new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if(GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
                {
                    if(card.UnitContainingThisCharacter() != null)
                    {
                        if(card.Owner.FieldUnit.Contains(card.UnitContainingThisCharacter()))
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
                List<CardSource> LeftCards = new List<CardSource>();

                for (int i = 0; i < 2; i++)
                {
                    yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(card.Owner));

                    if(card.Owner.LibraryCards.Count > 0)
                    {
                        CardSource topCard = card.Owner.LibraryCards[0];
                        TopCards.Add(topCard);
                        card.Owner.LibraryCards.Remove(topCard);
                    }
                    
                    yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(card.Owner));
                }

                if(TopCards.Count > 0)
                {
                    SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                    selectCardEffect.SetUp(
                        CanTargetCondition: (cardSource) => true,
                        CanTargetCondition_ByPreSelecetedList: null,
                        CanEndSelectCondition: null,
                        CanNoSelect: () => false,
                        SelectCardCoroutine: (cardSource) => SelectCardCoroutine(cardSource),
                        AfterSelectCardCoroutine: (targetCards) => AfterSelectCardCoroutine(targetCards),
                        Message: "Select a card to discard.",
                        MaxCount: 1,
                        CanEndNotMax: false,
                        isShowOpponent: true,
                        mode: SelectCardEffect.Mode.Custom,
                        root: SelectCardEffect.Root.Custom,
                        CustomRootCardList: TopCards,
                        CanLookReverseCard: true,
                        SelectPlayer: card.Owner,
                        cardEffect: activateClass);

                    yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));

                    IEnumerator SelectCardCoroutine(CardSource cardSource)
                    {
                        CardObjectController.AddTrashCard(cardSource);
                        yield return null;
                    }

                    IEnumerator AfterSelectCardCoroutine(List<CardSource> targetCards)
                    {
                        foreach (CardSource cardSource in TopCards)
                        {
                            if (!targetCards.Contains(cardSource))
                            {
                                LeftCards.Add(cardSource);
                            }
                        }

                        foreach(CardSource cardSource in LeftCards)
                        {
                            card.Owner.LibraryCards.Insert(0, cardSource);
                        }

                        yield return null;
                    }
                }
            }


        }

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("聖なる剣光", "",new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition1 }, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, PowerUpCondition, true);
        powerUpClass.SetCCS(card.UnitContainingThisCharacter());
        cardEffects.Add(powerUpClass);

        bool CanUseCondition1(Hashtable hashtable)
        {
            if(card.UnitContainingThisCharacter() != null)
            {
                if (card.Owner.GetFrontUnits().Contains(card.UnitContainingThisCharacter()))
                {
                    return true;
                }
            }
            
            return false;
        }

        bool PowerUpCondition(Unit unit)
        {
            if(unit.Character.Owner == card.Owner)
            {
                if(unit != card.UnitContainingThisCharacter())
                {
                    if(unit.IsClassChanged())
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

