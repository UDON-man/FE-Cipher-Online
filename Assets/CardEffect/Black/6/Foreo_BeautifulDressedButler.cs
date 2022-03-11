using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Foreo_BeautifulDressedButler : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("王族の衣がえ", "Royal Fashionista", new List<Cost>() { new TapCost(), new DiscardHandCost(1, (cardSource) => true) }, null, -1, false, card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                if(card.Owner.OrbCards.Count > 0)
                {
                    SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                    selectCardEffect.SetUp(
                        CanTargetCondition: (cardSource) => true,
                        CanTargetCondition_ByPreSelecetedList: null,
                        CanEndSelectCondition: null,
                        CanNoSelect: () => true,
                        SelectCardCoroutine: (cardSource) => SelectCardCoroutine(cardSource),
                        AfterSelectCardCoroutine: (targetCards) => AfterSelectCardCoroutine(targetCards),
                        Message: "Select a broken orb.",
                        MaxCount: 1,
                        CanEndNotMax: false,
                        isShowOpponent: false,
                        mode: SelectCardEffect.Mode.Custom,
                        root: SelectCardEffect.Root.Orb,
                        CustomRootCardList: null,
                        CanLookReverseCard: false,
                        SelectPlayer: card.Owner,
                        cardEffect: activateClass);

                    yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));

                    IEnumerator SelectCardCoroutine(CardSource cardSource)
                    {
                        cardSource.Owner.OrbCards.Remove(cardSource);
                        yield return ContinuousController.instance.StartCoroutine(CardObjectController.AddHandCard(cardSource, false));
                    }

                    IEnumerator AfterSelectCardCoroutine(List<CardSource> targetCards)
                    {
                        if(targetCards.Count > 0)
                        {
                            yield return StartCoroutine(new IAddOrbFromLibrary(card.Owner, 1).AddOrb());
                        }
                    }
                }
            }
        }

        CanNotMoveBySkillClass canNotMoveBySkillClass = new CanNotMoveBySkillClass();
        canNotMoveBySkillClass.SetUpICardEffect("執事のおもてなし", "", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false, card);
        canNotMoveBySkillClass.SetUpCanNotMoveBySkillClass(CanNotMoveCondition);
        cardEffects.Add(canNotMoveBySkillClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if (GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner.Enemy)
            {
                return true;
            }

            return false;
        }

        bool CanNotMoveCondition(Unit unit)
        {
            if(unit != null && IsExistOnField(null,card))
            {
                if(unit.Character != null)
                {
                    if(unit == card.UnitContainingThisCharacter()||unit == card.Owner.Lord)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        return cardEffects;
    }
}
