using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class KamuiOtoko_SealedDragon : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnEnterFieldAnyone)
        {
            activateClass[0].SetUpICardEffect("竜の咆哮", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Dragon's Roar";
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

                                if (Unit == card.UnitContainingThisCharacter())
                                {
                                    if (card.Owner.BondCards.Count((cardSource) => !cardSource.IsReverse) > 0)
                                    {
                                        if (card.Owner.FieldUnit.Count((unit) => unit.Character.UnitNames.Contains("アクア")) == 0)
                                        {
                                            return true;
                                        }
                                    }
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
                CanTargetCondition: (cardSource) => !cardSource.IsReverse,
                CanTargetCondition_ByPreSelecetedList: null,
                CanEndSelectCondition: null,
                CanNoSelect: () => false,
                SelectCardCoroutine: null,
                AfterSelectCardCoroutine: null,
                Message: "Select a card to reverse.",
                MaxCount: 1,
                CanEndNotMax: false,
                isShowOpponent: true,
                mode: SelectCardEffect.Mode.Reverse,
                root: SelectCardEffect.Root.Bond,
                CustomRootCardList: null,
                CanLookReverseCard: true);

                yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));
            }
        }

        return cardEffects;
    }
}

