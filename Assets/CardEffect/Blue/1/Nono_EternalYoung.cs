using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class Nono_EternalYoung : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("長寿な竜一族","", null, null, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 30, CanPowerUpCondition, true);
        cardEffects.Add(powerUpClass);

        bool CanPowerUpCondition(Unit unit)
        {
            if (card.UnitContainingThisCharacter() == unit)
            {
                if (card.Owner.BondCards.Count >= 6)
                {
                    return true;
                }
            }

            return false;
        }

        if(timing == EffectTiming.OnEndTurn)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("幼き竜", "Immature Dragon", new List<Cost>(), new List<Func<Hashtable, bool>>() { (hashtable) => GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner }, -1, false, card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                if (card.Owner.BondCards.Count > 0)
                {
                    SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                    selectCardEffect.SetUp(
                        CanTargetCondition: (cardSource) => cardSource.Owner == card.Owner,
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
                        root: SelectCardEffect.Root.Bond,
                        CustomRootCardList: null,
                        CanLookReverseCard: true,
                        SelectPlayer: card.Owner,
                        cardEffect: activateClass);

                    yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));
                }
            }
        }

        return cardEffects;
    }
}