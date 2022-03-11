using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Fae_DragonCalledGod : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("純血なる神竜", "",null, null, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 30, CanPowerUpCondition, true);
        cardEffects.Add(powerUpClass);

        bool CanPowerUpCondition(Unit unit)
        {
            if (card.UnitContainingThisCharacter() == unit)
            {
                if (card.Owner.BondCards.Count >= 6)
                {
                    return true;
                }
            }

            return false;
        }

        if (timing == EffectTiming.OnDeclaration)
        {
            if (card.Owner.BondCards.Count > 0)
            {
                ActivateClass activateClass = new ActivateClass();
                activateClass.SetUpICardEffect("永遠の幼子", "The Eternal Child", new List<Cost>(), new List<Func<Hashtable, bool>>() { (hashtable) => IsExistOnField(hashtable,card) }, 1, false,card);
                activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
                cardEffects.Add(activateClass);

                IEnumerator ActivateCoroutine()
                {
                    SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                    selectCardEffect.SetUp(
                        CanTargetCondition: (cardSource) => cardSource.Owner == card.Owner,
                        CanTargetCondition_ByPreSelecetedList: null,
                        CanEndSelectCondition: null,
                        CanNoSelect: () => false,
                        SelectCardCoroutine: null,
                        AfterSelectCardCoroutine: null,
                        Message: "Select a card to place on top of deck.",
                        MaxCount: 1,
                        CanEndNotMax: false,
                        isShowOpponent: true,
                        mode: SelectCardEffect.Mode.PutLibraryTop,
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