using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Nefenie_PatrioticWarPrincess : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[0].SetUpICardEffect("怒髪天衝", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Furious Point";
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable))
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
                    CanLookReverseCard: true);

                yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));

                IEnumerator AfterSelectCardCoroutine(List<CardSource> targetCards)
                {
                    if (targetCards.Count == 1)
                    {
                        yield return ContinuousController.instance.StartCoroutine(new IGrow(card.UnitContainingThisCharacter(), targetCards).Grow());

                        PowerUpClass powerUpClass = new PowerUpClass();
                        powerUpClass.SetUpPowerUpClass((unit, Power) => Power * 2, (unit) => unit == card.UnitContainingThisCharacter());
                        card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add(powerUpClass);

                        yield return null;
                    }
                }
            }

            activateClass[1].SetUpICardEffect("手槍", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, null, -1, false);
            activateClass[1].SetUpActivateClass((hashtable) => ActivateCoroutine1());
            cardEffects.Add(activateClass[1]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Javelin";
            }

            IEnumerator ActivateCoroutine1()
            {
                RangeUpClass rangeUpClass = new RangeUpClass();
                rangeUpClass.SetUpRangeUpClass((unit, Range) => { Range.Add(1); Range.Add(2); return Range; }, (unit) => unit == card.UnitContainingThisCharacter());
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add(rangeUpClass);

                yield return null;
            }
        }

        return cardEffects;
    }
}