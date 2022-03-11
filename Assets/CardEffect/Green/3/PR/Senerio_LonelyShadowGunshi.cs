using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Senerio_LonelyShadowGunshi : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDestroyDuringBattleAlly)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("影の知略", "Convert Operations", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (GManager.instance.turnStateMachine.AttackingUnit != null)
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit.Character != null)
                    {
                        if (GManager.instance.turnStateMachine.AttackingUnit.Character.Owner == card.Owner)
                        {
                            if (GManager.instance.turnStateMachine.AttackingUnit == card.Owner.Lord)
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                selectCardEffect.SetUp(
                    CanTargetCondition: CanTargetCondition,
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    CanNoSelect: () => false,
                    SelectCardCoroutine: null,
                    AfterSelectCardCoroutine: AfterSelectCardCoroutine,
                    Message: "Select a card to stack down.",
                    MaxCount: 1,
                    CanEndNotMax: false,
                    isShowOpponent: true,
                    mode: SelectCardEffect.Mode.Custom,
                    root: SelectCardEffect.Root.Trash,
                    CustomRootCardList: null,
                    CanLookReverseCard: true,
                    SelectPlayer: card.Owner,
                    cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));

                bool CanTargetCondition(CardSource cardSource)
                {
                    foreach (string UnitName in card.Owner.Lord.Character.UnitNames)
                    {
                        if (cardSource.UnitNames.Contains(UnitName))
                        {
                            return true;
                        }
                    }

                    return false;
                }

                IEnumerator AfterSelectCardCoroutine(List<CardSource> cardSources)
                {
                    yield return ContinuousController.instance.StartCoroutine(new IGrow(card.Owner.Lord, cardSources).Grow());
                }
            }
        }

        return cardEffects;
    }

   
}

