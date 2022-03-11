using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Riz_SaintKingdomPrincess : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDestroyDuringBattleAlly)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("闘う修道女", "Cleric in Combat",new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
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
                SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                selectCardEffect.SetUp(
                    CanTargetCondition: (cardSource) => !cardSource.UnitNames.Contains("リズ") && cardSource.PlayCost <= 3,
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
                    cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));
            }
         }

        return cardEffects;
    }
}




