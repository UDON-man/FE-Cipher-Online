using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Edain_LadyOfYngvi : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("ライブ", "Heal", new List<Cost>() { new TapCost(), new ReverseCost(2, (cardSource) => true) }, new List<Func<Hashtable, bool>>(), -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                selectCardEffect.SetUp(
                    CanTargetCondition: (cardSource) => !cardSource.UnitNames.Contains("エーディン"),
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    CanNoSelect: () => false,
                    SelectCardCoroutine: null,
                    AfterSelectCardCoroutine: null,
                    Message: "Select a card to add to hand.",
                    MaxCount: 1,
                    CanEndNotMax: false,
                    isShowOpponent: true,
                    mode: SelectCardEffect.Mode.AddHand,
                    root: SelectCardEffect.Root.Trash,
                    CustomRootCardList: null,
                    CanLookReverseCard: true,
                    SelectPlayer: card.Owner,
                    cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));
            }
        }

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, PowerUpCondition, true);
        powerUpClass.SetUpICardEffect("美しき公女","", null, null, -1, false,card);
        cardEffects.Add(powerUpClass);

        bool PowerUpCondition(Unit unit)
        {
            if (unit.Character != null)
            {
                if (unit.Character.Owner == card.Owner)
                {
                    if (unit.Character.UnitNames.Contains("アゼル")　|| unit.Character.UnitNames.Contains("ミデェール") || unit.Character.UnitNames.Contains("ジャムカ"))
                    {
                        if (GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        return cardEffects;
    }

    #region 祈りの紋章
    public override List<ICardEffect> SupportEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> supportEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnSetSupport)
        {
            ActivateClass activateClass_Support = new ActivateClass();
            activateClass_Support.SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass_Support.SetUpICardEffect("祈りの紋章", "Miracle Emblem", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false, card);
            supportEffects.Add(activateClass_Support);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (card.Owner.SupportCards.Contains(card))
                {
                    if (GManager.instance.turnStateMachine.gameContext.NonTurnPlayer == card.Owner)
                    {
                        if (GManager.instance.turnStateMachine.AttackingUnit != null && GManager.instance.turnStateMachine.DefendingUnit != null)
                        {
                            if (GManager.instance.turnStateMachine.DefendingUnit.Character != null)
                            {
                                if (GManager.instance.turnStateMachine.DefendingUnit.Character.Owner == card.Owner)
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
                CanNotCriticalClass canNotCriticalClass = new CanNotCriticalClass();
                canNotCriticalClass.SetUpCanNotCriticalClass((unit) => unit == GManager.instance.turnStateMachine.AttackingUnit && unit.Character.Owner != card.Owner);
                GManager.instance.turnStateMachine.AttackingUnit.UntilEndBattleEffects.Add((_timing) => canNotCriticalClass);

                yield return null;
            }
        }

        return supportEffects;
    }
    #endregion
}
