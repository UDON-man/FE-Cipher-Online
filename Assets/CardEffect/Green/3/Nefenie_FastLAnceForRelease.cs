using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Photon.Pun;

public class Nefenie_FastLAnceForRelease : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("怒れる槍姫","", null, null, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10 * (card.UnitContainingThisCharacter().Characters.Count - 1), CanPowerUpCondition, true);
        cardEffects.Add(powerUpClass);

        bool CanPowerUpCondition(Unit unit)
        {
            if(card.UnitContainingThisCharacter() != null)
            {
                if (card.UnitContainingThisCharacter() == unit)
                {
                    if (GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
                    {
                        return true;
                    }
                }
            }
            

            return false;
        }

        if (timing == EffectTiming.OnCriticalAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("秘めた激情", "Tempered Wrath",new List<Cost>() , new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable,card))
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit != null)
                    {
                        if (GManager.instance.turnStateMachine.AttackingUnit.Character != null)
                        {
                            if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                            {
                                bool canFromTrash = card.Owner.TrashCards.Count((cardSource) => cardSource.UnitNames.Contains("ネフェニー")) > 0;

                                if (canFromTrash)
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
                    CanTargetCondition: (cardSource) => cardSource.UnitNames.Contains("ネフェニー"),
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    CanNoSelect: () => true,
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

                IEnumerator AfterSelectCardCoroutine(List<CardSource> cardSources)
                {
                    yield return ContinuousController.instance.StartCoroutine(new IGrow(card.UnitContainingThisCharacter(), cardSources).Grow());
                }

                yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));
            }
        }

        return cardEffects;
    }
}



