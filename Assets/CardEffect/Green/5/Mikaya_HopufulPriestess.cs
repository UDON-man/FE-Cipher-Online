using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class Mikaya_HopufulPriestess : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass1 = new PowerModifyClass();
        powerUpClass1.SetUpICardEffect("暁の巫女", "",null, null, -1, false,card);
        powerUpClass1.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner && unit != card.UnitContainingThisCharacter() && unit.Character.Owner == card.Owner, true);
        powerUpClass1.SetLvS(card.UnitContainingThisCharacter(), 5);
        cardEffects.Add(powerUpClass1);

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("シャイン", "Shine", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if(IsExistOnField(hashtable,card))
                {
                    if (card.Owner.TrashCards.Count((cardSource) => cardSource.UnitNames.Contains("ミカヤ")) >= 2)
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
                    CanTargetCondition: (cardSource) => cardSource.UnitNames.Contains("ミカヤ"),
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    CanNoSelect: () => false,
                    SelectCardCoroutine: null,
                    AfterSelectCardCoroutine: AfterSelectCardCoroutine,
                    Message: "Select cards to stack down.",
                    MaxCount: 2,
                    CanEndNotMax: false,
                    isShowOpponent: true,
                    mode: SelectCardEffect.Mode.Custom,
                    root: SelectCardEffect.Root.Trash,
                    CustomRootCardList: null,
                    CanLookReverseCard: true,
                    SelectPlayer: card.Owner,
                    cardEffect:activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));

                IEnumerator AfterSelectCardCoroutine(List<CardSource> targetCards)
                {
                    if (targetCards.Count == 2)
                    {
                        yield return ContinuousController.instance.StartCoroutine(new IGrow(card.UnitContainingThisCharacter(), targetCards).Grow());
                        yield return ContinuousController.instance.StartCoroutine(new IDraw(card.Owner, 1).Draw());
                    }
                }
            }
        }

        return cardEffects;
    }
}