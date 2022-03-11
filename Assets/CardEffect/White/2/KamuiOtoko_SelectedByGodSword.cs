using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class KamuiOtoko_SelectedByGodSword : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("光の御旗", "Banner of Light", new List<Cost>() { new ReverseCost(3, (cardSource) => true), new DiscardHandCost(1, (cardSource) => cardSource.UnitNames.Contains("カムイ(男)")) }, null, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine1());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine1()
            {
                if (card.Owner.HandCards.Count > 0)
                {
                    SelectHandEffect selectHandEffect = GetComponent<SelectHandEffect>();

                    selectHandEffect.SetUp(
                        SelectPlayer: card.Owner,
                        CanTargetCondition: CanSelectCondition,
                        CanTargetCondition_ByPreSelecetedList: null,
                        CanEndSelectCondition: null,
                        MaxCount: 1,
                        CanNoSelect: true,
                        CanEndNotMax: false,
                        isShowOpponent: true,
                        SelectCardCoroutine: null,
                        AfterSelectCardCoroutine: null,
                        mode: SelectHandEffect.Mode.Deploy,
                        cardEffect: activateClass);

                    bool CanSelectCondition(CardSource cardSource)
                    {
                        if(cardSource.Owner.HandCards.Contains(cardSource))
                        {
                            if(cardSource.CanPlayAsNewUnit())
                            {
                                if(cardSource.PlayCost <= 2)
                                {
                                    return true;
                                }
                            }
                        }

                        return false;
                    }

                    yield return StartCoroutine(selectHandEffect.Activate(null));
                }

                foreach (Unit unit in card.Owner.FieldUnit)
                {
                    PowerModifyClass powerUpClass = new PowerModifyClass();
                    powerUpClass.SetUpPowerUpClass((_unit, Power) => Power + 30, (_unit) => _unit == unit, true);
                    unit.UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);
                }
            }

            ActivateClass activateClass1 = new ActivateClass();
            activateClass1.SetUpICardEffect("汚れなき白刃", "Untarnished White Sword", new List<Cost>() { new ReverseCost(3, (cardSource) => true), new DiscardHandCost(1, (cardSource) => cardSource.UnitNames.Contains("カムイ(男)")) }, null, -1, false,card);
            activateClass1.SetUpActivateClass((hashtable) => ActivateCoroutine2());
            cardEffects.Add(activateClass1);

            IEnumerator ActivateCoroutine2()
            {
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner != card.Owner && unit != unit.Character.Owner.Lord,
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: false,
                    CanEndNotMax: false,
                    SelectUnitCoroutine: null,
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Destroy,
                    cardEffect: activateClass);

                yield return StartCoroutine(selectUnitEffect.Activate(null));

                SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                selectCardEffect.SetUp(
                    CanTargetCondition: (cardSource) => !cardSource.UnitNames.Contains("カムイ(男)") && cardSource.cardColors.Contains(CardColor.White),
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

                yield return StartCoroutine(selectCardEffect.Activate(null));
            }

        }

        return cardEffects;
    }
}