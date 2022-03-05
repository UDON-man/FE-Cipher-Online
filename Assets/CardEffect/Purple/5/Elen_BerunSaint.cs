using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Elen_BerunSaint : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[0].SetUpICardEffect("リザーブ", new List<Cost>() { new TapCost(), new ReverseCost(3, (cardSource) => true) }, new List<Func<Hashtable, bool>>(), -1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Fortify";
            }

            IEnumerator ActivateCoroutine()
            {
                SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                selectCardEffect.SetUp(
                    CanTargetCondition: (cardSource) => !cardSource.UnitNames.Contains("エレン"),
                    CanTargetCondition_ByPreSelecetedList: CanTargetCondition_ByPreSelecetedList,
                    CanEndSelectCondition: CanEndSelectCondition,
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

                bool CanTargetCondition_ByPreSelecetedList(List<CardSource> PreSelectedList, CardSource cardSource)
                {
                    foreach (string UnitName in cardSource.UnitNames)
                    {
                        if (PreSelectedList.Count((_cardSource) => _cardSource.UnitNames.Contains(UnitName)) > 0)
                        {
                            return false;
                        }
                    }

                    return true;
                }

                bool CanEndSelectCondition(List<CardSource> PreSelectedList)
                {
                    foreach (CardSource cardSource in PreSelectedList)
                    {
                        foreach (string UnitName in cardSource.UnitNames)
                        {
                            if (PreSelectedList.Count((_cardSource) => _cardSource.UnitNames.Contains(UnitName) && _cardSource != cardSource) > 0)
                            {
                                return false;
                            }
                        }
                    }

                    return true;
                }
            }
        }

        return cardEffects;
    }
}

