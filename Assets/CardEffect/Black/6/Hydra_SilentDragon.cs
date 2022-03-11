using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Hydra_SilentDragon : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        CanLevelUpToThisCardClass canLevelUpToThisCardClass = new CanLevelUpToThisCardClass();
        canLevelUpToThisCardClass.SetUpICardEffect("竜の器", "", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false, card);
        canLevelUpToThisCardClass.SetUpCanLevelUpToThisCardClass((cardSource) => cardSource == card && card.Owner.FieldUnit.Count((unit) => unit.Character.UnitNames.Contains("ハイドラ")) == 0, (unit) => unit.Character.Owner == card.Owner && (unit.Character.UnitNames.Contains("ガロン")|| unit.Character.UnitNames.Contains("ギュンター")));
        cardEffects.Add(canLevelUpToThisCardClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if (card.Owner.FieldUnit.Count((unit) => unit.Character.UnitNames.Contains("ハイドラ")) == 0)
            {
                return true;
            }

            return false;
        }

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("蘇る透魔竜", "", null, null, -1, false, card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 80, PowerUpCondition, true);
        cardEffects.Add(powerUpClass);

        RangeUpClass rangeUpClass = new RangeUpClass();
        rangeUpClass.SetUpICardEffect("蘇る透魔竜", "", null, null, -1, false, card);
        rangeUpClass.SetUpRangeUpClass((unit, Range) => { Range.Add(1); Range.Add(2); return Range; }, PowerUpCondition);
        cardEffects.Add(rangeUpClass);

        bool PowerUpCondition(Unit unit)
        {
            if (unit == card.UnitContainingThisCharacter())
            {
                if (card.Owner.BondCards.Count((cardSource) => cardSource.IsReverse) >= 4)
                {
                    return true;
                }
            }

            return false;
        }

        if (timing == EffectTiming.OnDeclaration)
        {
            int maxCount = 1;

            if (card != null)
            {
                if (card.UnitContainingThisCharacter() != null)
                {
                    if (card.UnitContainingThisCharacter().IsLevelUp())
                    {
                        maxCount = 114514;
                    }
                }
            }

            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("黒き太陽", "Black Sun", new List<Cost>() , new List<Func<Hashtable, bool>>() { CanUseCondition1 }, maxCount, false, card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition1(Hashtable hashtable)
            {
                if (IsExistOnField(null, card))
                {
                    if (card.Owner.BondCards.Count((cardSource) => cardSource.IsReverse) > 0)
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
                    CanTargetCondition: (cardSource) => cardSource.IsReverse,
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    CanNoSelect: () => false,
                    SelectCardCoroutine: null,
                    AfterSelectCardCoroutine: null,
                    Message: "Select your bond card to discard.",
                    MaxCount: 1,
                    CanEndNotMax: false,
                    isShowOpponent: true,
                    mode: SelectCardEffect.Mode.DiscardFromBond,
                    root: SelectCardEffect.Root.Bond,
                    CustomRootCardList: null,
                    CanLookReverseCard: true,
                    SelectPlayer: card.Owner,
                    cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));
                yield return StartCoroutine(card.Owner.bondObject.SetBond_Skill(card.Owner));

                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner != card.Owner && unit != card.Owner.Enemy.Lord,
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: false,
                    CanEndNotMax: false,
                    SelectUnitCoroutine: null,
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Destroy,
                    cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));
            }
        }

        return cardEffects;
    }
}
