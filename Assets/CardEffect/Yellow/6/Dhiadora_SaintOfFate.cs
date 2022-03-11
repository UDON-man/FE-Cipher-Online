using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Dhiadora_SaintOfFate : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnEndTurn)
        {
            if (card.Owner.BondCards.Count((cardSource) => cardSource.IsReverse)> 0)
            {
                ActivateClass activateClass = new ActivateClass();
                activateClass.SetUpICardEffect("禁断を超える愛", "Taboo Surpassing Love",new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true,card);
                activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
                cardEffects.Add(activateClass);

                bool CanUseCondition(Hashtable hashtable)
                {
                    if(IsExistOnField(hashtable, card))
                    {
                        if(GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
                        {
                            if (card.Owner.BondCards.Count((cardSource) => cardSource.IsReverse) > 0)
                            {
                                return true;
                            }
                        }
                    }

                    return false;
                }

                IEnumerator ActivateCoroutine()
                {
                    SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                    selectCardEffect.SetUp(
                        CanTargetCondition: (cardSource) => cardSource.Owner == card.Owner && cardSource.IsReverse,
                        CanTargetCondition_ByPreSelecetedList: null,
                        CanEndSelectCondition: null,
                        CanNoSelect: () => false,
                        SelectCardCoroutine: null,
                        AfterSelectCardCoroutine: null,
                        Message: "Select a card to turn face up.",
                        MaxCount: 1,
                        CanEndNotMax: true,
                        isShowOpponent: true,
                        mode: SelectCardEffect.Mode.SetFace,
                        root: SelectCardEffect.Root.Bond,
                        CustomRootCardList: null,
                        CanLookReverseCard: true,
                        SelectPlayer: card.Owner,
                        cardEffect: activateClass);

                    yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));
                }
            }
        }

        CanNotUntapOnStartTurnClass canNotUntapOnStartTurnClass = new CanNotUntapOnStartTurnClass();
        canNotUntapOnStartTurnClass.SetUpICardEffect("ナーガの印","", new List<Cost>(), new List<Func<Hashtable, bool>>() { (hashtable) => IsExistOnField(hashtable,card) }, -1, false,card);
        canNotUntapOnStartTurnClass.SetUpCanNotUntapOnStartTurnClass(CanNotUnTapCondition);
        cardEffects.Add(canNotUntapOnStartTurnClass);

        bool CanNotUnTapCondition(Unit unit)
        {
            if(unit.Character != null)
            {
                if(unit != unit.Character.Owner.Lord && unit.Weapons.Contains(Weapon.DragonStone))
                {
                    return true;
                }
            }

            return false;
        }

        return cardEffects;
    }
}