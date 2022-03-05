using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Sanaki_BegnionEmperor : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnEnterFieldAnyone)
        {
            activateClass[0].SetUpICardEffect("皇帝の檄", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, true);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine(hashtable));
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "The Empress's Exhortation";
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable))
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
                                        if (Unit.Character.Owner == this.card.Owner)
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
                            CanLookReverseCard: true);

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

        PowerUpClass powerUpClass = new PowerUpClass();
        powerUpClass.SetUpICardEffect("頂に立つ者", null, null, -1, false);
        powerUpClass.SetUpPowerUpClass((unit,Power) => Power + 10 * card.Owner.FieldUnit.Count((_unit) => _unit != card.UnitContainingThisCharacter() && _unit.IsLevelUp()),(unit) => unit == card.UnitContainingThisCharacter());
        powerUpClass.SetCCS(card.UnitContainingThisCharacter());
        cardEffects.Add(powerUpClass);

        return cardEffects;
    }
}
