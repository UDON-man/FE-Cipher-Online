using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Sanaki_BegnionEmperor : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnEnterFieldAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("皇帝の檄", "The Empress's Exhortation",new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, true,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine(hashtable));
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

                                if (Unit != null)
                                {
                                    if (Unit.Character != null)
                                    {
                                        if (Unit.Character.Owner == card.Owner)
                                        {
                                            if (Unit != card.UnitContainingThisCharacter())
                                            {
                                                if (Unit.Character.cardColors.Contains(CardColor.Green))
                                                {
                                                    return true;
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }
                    }
                }  

                return false;
            }

            IEnumerator ActivateCoroutine(Hashtable hashtable)
            {
                Unit Unit = (Unit)hashtable["Unit"];

                if(Unit != null)
                {
                    if(Unit.Character != null)
                    {
                        SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                        selectCardEffect.SetUp(
                            CanTargetCondition: CanTargetCondition,
                            CanTargetCondition_ByPreSelecetedList: null,
                            CanEndSelectCondition: null,
                            CanNoSelect: () => true,
                            SelectCardCoroutine: null,
                            AfterSelectCardCoroutine: AfterSelectCardCoroutine,
                            Message: "Select a card to stack down.",
                            MaxCount: card.Owner.TrashCards.Count((cardSource) => CanTargetCondition(cardSource)),
                            CanEndNotMax: true,
                            isShowOpponent: true,
                            mode: SelectCardEffect.Mode.Custom,
                            root: SelectCardEffect.Root.Trash,
                            CustomRootCardList: null,
                            CanLookReverseCard: true,
                            SelectPlayer: card.Owner,
                            cardEffect: activateClass);

                        bool CanTargetCondition(CardSource cardSource)
                        {
                            foreach (string UnitName in Unit.Character.UnitNames)
                            {
                                if (cardSource.UnitNames.Contains(UnitName))
                                {
                                    return true;
                                }
                            }

                            return false;
                        }

                        IEnumerator AfterSelectCardCoroutine(List<CardSource> cardSources)
                        {
                            yield return ContinuousController.instance.StartCoroutine(new IGrow(Unit, cardSources).Grow());
                        }

                        yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));
                    }
                }
            }
        }

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("頂に立つ者", "",null, null, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit,Power) => Power + 10 * card.Owner.FieldUnit.Count((_unit) => _unit != card.UnitContainingThisCharacter() && _unit.IsLevelUp()),(unit) => unit == card.UnitContainingThisCharacter(), true);
        powerUpClass.SetCCS(card.UnitContainingThisCharacter());
        cardEffects.Add(powerUpClass);

        return cardEffects;
    }
}
