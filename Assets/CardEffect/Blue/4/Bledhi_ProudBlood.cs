using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Bledhi_ProudBlood : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnStartTurn)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("ケガはねぇか?", "Doesn't it hurt?", new List<Cost>() { new ReverseCost(2, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCodition }, -1, true,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCodition(Hashtable hashtable)
            {
                if(GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
                {
                    return true;
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                selectCardEffect.SetUp(
                    CanTargetCondition: (cardSource) => !cardSource.UnitNames.Contains("ブレディ"),
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    CanNoSelect: () => false,
                    SelectCardCoroutine: null,
                    AfterSelectCardCoroutine: null,
                    Message: "Select a card to add to hand.",
                    MaxCount: 1,
                    CanEndNotMax: false,
                    isShowOpponent: true,
                    mode: SelectCardEffect.Mode.AddHand,
                    root: SelectCardEffect.Root.Trash,
                    CustomRootCardList: null,
                    CanLookReverseCard: true,
                    SelectPlayer: card.Owner,
                    cardEffect:activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));
            }
        }

        else if(timing == EffectTiming.OnAddHandCardFromTrash)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("英才教育", "Higher Education",new List<Cost>() , new List<Func<Hashtable, bool>>() { CanUseCodition }, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCodition(Hashtable hashtable)
            {
                if(hashtable != null)
                {
                    if(hashtable.ContainsKey("cardEffect") && hashtable.ContainsKey("Card"))
                    {
                        if(hashtable["cardEffect"] is ICardEffect && hashtable["Card"] is CardSource)
                        {
                            ICardEffect cardEffect = (ICardEffect)hashtable["cardEffect"];
                            CardSource cardSource = (CardSource)hashtable["Card"];

                            if(cardEffect != null && cardSource != null)
                            {
                                if(cardEffect.card() != null)
                                {
                                    if(cardEffect.card() == card)
                                    {
                                        if (cardSource.Weapons.Contains(Weapon.MagicBook) || cardSource.Weapons.Contains(Weapon.Rod))
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                PowerModifyClass powerUpClass = new PowerModifyClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter(), true);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);

                yield return null;
            }
        }

        return cardEffects;
    }
}

