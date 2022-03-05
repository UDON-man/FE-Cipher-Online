using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Esto_YoungWingWhiteKnight : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            SelectAllyCost selectAllyCost = new SelectAllyCost(
                 SelectPlayer: card.Owner,
                 CanTargetCondition: (unit) => unit.Character.Owner == card.Owner && !unit.IsTapped && (unit.Character.UnitNames.Contains("パオラ") || unit.Character.UnitNames.Contains("カチュア")),
                 CanTargetCondition_ByPreSelecetedList: null,
                 CanEndSelectCondition: null,
                 MaxCount: 1,
                 CanNoSelect: false,
                 CanEndNotMax: false,
                 SelectUnitCoroutine: null,
                 AfterSelectUnitCoroutine: null,
                 mode: SelectUnitEffect.Mode.Tap);

            activateClass[0].SetUpICardEffect("エストの風笛", new List<Cost>() { new TapCost(), new ReverseCost(1, (cardSource) => true), selectAllyCost }, null, -1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Est's Bagpipes";
            }

            IEnumerator ActivateCoroutine()
            {
                SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                selectCardEffect.SetUp(
                CanTargetCondition: CanTargetCondition,
                CanTargetCondition_ByPreSelecetedList: null,
                CanEndSelectCondition: null,
                CanNoSelect: () => true,
                SelectCardCoroutine: null,
                AfterSelectCardCoroutine: null,
                Message: "Select a card to deploy.",
                MaxCount: 1,
                CanEndNotMax: false,
                isShowOpponent: true,
                mode: SelectCardEffect.Mode.Deploy,
                root: SelectCardEffect.Root.Library,
                CustomRootCardList: null,
                CanLookReverseCard: true);

                yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));

                bool CanTargetCondition(CardSource cardSource)
                {
                    if (cardSource.Owner == card.Owner)
                    {
                        if (cardSource.cEntity_Base.PlayCost <= 2)
                        {
                            if (cardSource.cardColors.Contains(CardColor.Red))
                            {
                                if(cardSource.Weapons.Contains(Weapon.Wing))
                                {
                                    if (cardSource.CanPlayAsNewUnit())
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }

                    return false;
                }
            }
        }

        return cardEffects;
    }
}