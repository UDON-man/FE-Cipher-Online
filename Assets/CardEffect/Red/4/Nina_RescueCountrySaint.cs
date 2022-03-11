using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Nina_RescueCountrySaint : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnEnterFieldAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("アカネイアの白薔薇", "Whtie Rose of Archanea", new List<Cost>() { new ReverseCost(5, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable,card))
                {
                    if (hashtable != null)
                    {
                        if (hashtable.ContainsKey("Unit"))
                        {
                            if (hashtable["Unit"] is Unit)
                            {
                                Unit Unit = (Unit)hashtable["Unit"];

                                if (Unit == card.UnitContainingThisCharacter())
                                {
                                    return true;
                                }
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
                    CanTargetCondition: (cardSource) => cardSource.cardColors.Contains(CardColor.Red),
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
                    cardEffect:activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));

                yield return ContinuousController.instance.StartCoroutine(new IDraw(card.Owner, 1).Draw());

                yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(card.Owner));

                yield return new WaitForSeconds(1f);

                if (card.Owner.LibraryCards.Count > 0)
                {
                    CardSource cardSource = card.Owner.LibraryCards[0];

                    if (cardSource.CanSetBondThisCard)
                    {
                        ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(new List<CardSource>() { cardSource }, "Added Bond Card", true));
                        card.Owner.LibraryCards.Remove(cardSource);
                        yield return StartCoroutine(new ISetBondCard(cardSource, true).SetBond());
                        yield return StartCoroutine(cardSource.Owner.bondObject.SetBond_Skill(cardSource.Owner));
                    }

                    else
                    {
                        ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(new List<CardSource>() { cardSource }, "Deck Top Card", true));
                    }
                }

                if (card.Owner.OrbCount < card.Owner.Enemy.OrbCount)
                {
                    yield return StartCoroutine(new IAddOrbFromLibrary(card.Owner, 1).AddOrb());
                }
            }
        }

        return cardEffects;
    }
}
