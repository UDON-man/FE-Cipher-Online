using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eldigan_YoungKingNordion : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("闇を裂く剣", "The Sword that Cleaves through Darkness", new List<Cost>() { new ReverseSelfCost() }, null, -1, false, card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass.SetBS();
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
