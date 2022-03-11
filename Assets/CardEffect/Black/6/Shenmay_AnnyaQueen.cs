using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Shenmay_AnnyaQueen : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("交錯する記憶", "Jumbled Memories", new List<Cost>() { new TapCost() }, null, -1, false, card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                CardSource TopCard = null;

                yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(card.Owner));

                if(card.Owner.LibraryCards.Count > 0)
                {
                    TopCard = card.Owner.LibraryCards[0];

                    ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(new List<CardSource>() { TopCard }, "Deck Top Card", false));
                    yield return new WaitForSeconds(1f);
                    GManager.instance.GetComponent<Effects>().OffShowCard();

                    yield return ContinuousController.instance.StartCoroutine(new IDraw(card.Owner, 1).Draw());

                    if(!TopCard.cardColors.Contains(CardColor.White))
                    {
                        if (card.Owner.HandCards.Count >= 1)
                        {
                            SelectHandEffect selectHandEffect = GetComponent<SelectHandEffect>();

                            selectHandEffect.SetUp(
                                            SelectPlayer: card.Owner,
                                            CanTargetCondition: (cardSource) => cardSource.Owner.HandCards.Contains(cardSource),
                                            CanTargetCondition_ByPreSelecetedList: null,
                                            CanEndSelectCondition: null,
                                            MaxCount: 1,
                                            CanNoSelect: false,
                                            CanEndNotMax: false,
                                            isShowOpponent: true,
                                            SelectCardCoroutine: null,
                                            AfterSelectCardCoroutine: null,
                                            mode: SelectHandEffect.Mode.Discard,
                                            cardEffect: activateClass);

                            yield return ContinuousController.instance.StartCoroutine(selectHandEffect.Activate(null));
                        }
                    }
                }
            }
        }

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("透魔の眷属", "", null, null, -1, false, card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, PowerUpCondition, true);
        cardEffects.Add(powerUpClass);

        bool PowerUpCondition(Unit unit)
        {
            if (unit == card.UnitContainingThisCharacter())
            {
                if(card.Owner.BondCards.Count == card.Owner.BondCards.Count((cardSource) => cardSource.IsReverse))
                {
                    return true;
                }
            }

            return false;
        }

        return cardEffects;
    }
}
