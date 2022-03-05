using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Erese_SunnyBrightStart : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnStartTurn)
        {
            activateClass[0].SetUpICardEffect("エリーゼの出番!", new List<Cost>() { new ReverseCost(2, (cardSource) => true),new BreakOrbCost(card.Owner,1,BreakOrbMode.Hand) }, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1,true);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "It's my turn!";
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if(GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
                {
                    return true;
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                selectCardEffect.SetUp(
                    CanTargetCondition: (cardSource) => !cardSource.UnitNames.Contains("エリーゼ"),
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    CanNoSelect: () => false,
                    SelectCardCoroutine: null,
                    AfterSelectCardCoroutine: null,
                    Message: "Select cards to add to hand.",
                    MaxCount: 2,
                    CanEndNotMax: true,
                    isShowOpponent: true,
                    mode: SelectCardEffect.Mode.AddHand,
                    root: SelectCardEffect.Root.Trash,
                    CustomRootCardList: null,
                    CanLookReverseCard: true);

                yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));

                
            }
        }

        else if(timing == EffectTiming.OnDeclaration)
        {
            activateClass[1].SetUpICardEffect("エリーゼにお任せ!", new List<Cost>() {new DiscardHandCost(1, (cardSource) => cardSource.UnitNames.Contains("エリーゼ")) }, null, 1, false);
            activateClass[1].SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass[1].SetCCS(card.UnitContainingThisCharacter());
            cardEffects.Add(activateClass[1]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[1].EffectName = "Leave it to me!";
            }

            IEnumerator ActivateCoroutine()
            {
                if(card.Owner.OrbCards.Count == 0)
                {
                    yield return StartCoroutine(new IAddOrbFromLibrary(card.Owner, 1).AddOrb());
                }
            }
        }

        return cardEffects;
    }
}

