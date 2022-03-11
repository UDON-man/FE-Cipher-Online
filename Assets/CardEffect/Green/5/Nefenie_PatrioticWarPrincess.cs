using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Nefenie_PatrioticWarPrincess : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("怒髪天衝", "Furious Point",new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable,card))
                {
                    if (card.Owner.TrashCards.Count((cardSource) => cardSource.UnitNames.Contains("ネフェニー")) >= 1)
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
                    CanTargetCondition: (cardSource) => cardSource.UnitNames.Contains("ネフェニー"),
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    CanNoSelect: () => false,
                    SelectCardCoroutine: null,
                    AfterSelectCardCoroutine: AfterSelectCardCoroutine,
                    Message: "Select cards to stack down.",
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

                IEnumerator AfterSelectCardCoroutine(List<CardSource> targetCards)
                {
                    if (targetCards.Count == 1)
                    {
                        yield return ContinuousController.instance.StartCoroutine(new IGrow(card.UnitContainingThisCharacter(), targetCards).Grow());

                        PowerModifyClass powerUpClass = new PowerModifyClass();
                        powerUpClass.SetUpPowerUpClass((unit, Power) => Power * 2, (unit) => unit == card.UnitContainingThisCharacter(), false);
                        card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);

                        yield return null;
                    }
                }
            }

            ActivateClass activateClass1 = new ActivateClass();
            activateClass1.SetUpICardEffect("手槍", "Javelin", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, null, -1, false,card);
            activateClass1.SetUpActivateClass((hashtable) => ActivateCoroutine1());
            cardEffects.Add(activateClass1);

            IEnumerator ActivateCoroutine1()
            {
                RangeUpClass rangeUpClass = new RangeUpClass();
                rangeUpClass.SetUpRangeUpClass((unit, Range) => { Range.Add(1); Range.Add(2); return Range; }, (unit) => unit == card.UnitContainingThisCharacter());
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => rangeUpClass);

                yield return null;
            }
        }

        return cardEffects;
    }
}