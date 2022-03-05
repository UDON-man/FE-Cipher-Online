using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Bledhi_ProudBlood : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnStartTurn)
        {
            activateClass[0].SetUpICardEffect("ケガはねぇか?", new List<Cost>() { new ReverseCost(2, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCodition }, -1, true);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Doesn't it hurt?";
            }

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
                    CanLookReverseCard: true);

                yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));
            }
        }

        else if(timing == EffectTiming.OnAddHandCardFromTrash)
        {
            activateClass[1].SetUpICardEffect("英才教育", new List<Cost>() , new List<Func<Hashtable, bool>>() { CanUseCodition }, -1, false);
            activateClass[1].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[1]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[1].EffectName = "Higher Education";
            }

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
                                    if(cardEffect.card() == this.card)
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
                PowerUpClass powerUpClass = new PowerUpClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter());
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add(powerUpClass);

                yield return null;
            }
        }

        return cardEffects;
    }
}

