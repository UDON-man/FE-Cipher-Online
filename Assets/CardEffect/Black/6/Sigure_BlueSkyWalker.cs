using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Sigure_BlueSkyWalker : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("天空の運び手", "Winged Deliverer", new List<Cost>() { new TapCost() }, null, -1, false, card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner == card.Owner && unit != card.UnitContainingThisCharacter(),
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: false,
                    CanEndNotMax: false,
                    SelectUnitCoroutine: null,
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Move,
                    cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));
            }

            ActivateClass activateClass1 = new ActivateClass();
            activateClass1.SetUpICardEffect("竜脈の発見", "Dragon Vein Discovery", new List<Cost>() { new DestroySelfCost() }, null, -1, false, card);
            activateClass1.SetUpActivateClass((hashtable) => ActivateCoroutine1());
            cardEffects.Add(activateClass1);

            IEnumerator ActivateCoroutine1()
            {
                SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                selectCardEffect.SetUp(
                    CanTargetCondition: (cardSource) => cardSource.Owner == card.Owner,
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    CanNoSelect: () => true,
                    SelectCardCoroutine: (cardSource) => SelectCardCoroutine(cardSource),
                    AfterSelectCardCoroutine: null,
                    Message: "Select a card to flip the front and back.",
                    MaxCount: 1,
                    CanEndNotMax: false,
                    isShowOpponent: true,
                    mode: SelectCardEffect.Mode.Custom,
                    root: SelectCardEffect.Root.Orb,
                    CustomRootCardList: null,
                    CanLookReverseCard: false,
                    SelectPlayer: card.Owner,
                    cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));

                IEnumerator SelectCardCoroutine(CardSource cardSource)
                {
                    cardSource.IsReverse = !cardSource.IsReverse;
                    yield return null;
                }
            }
        }

        return cardEffects;
    }
}
