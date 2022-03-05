using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Sarya_ShadowDarkUser : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpClass powerUpClass = new PowerUpClass();
        powerUpClass.SetUpICardEffect("呪い", null, null, -1, false);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter() && GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner && card.Owner.Enemy.HandCards.Count < card.Owner.HandCards.Count);
        cardEffects.Add(powerUpClass);

        return cardEffects;
    }

    #region 暗闇の紋章
    public override List<ICardEffect> SupportEffects(EffectTiming timing)
    {
        List<ICardEffect> supportEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnSetSupport)
        {
            activateClass_Support[0].SetUpICardEffect("暗闇の紋章", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
            activateClass_Support[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            supportEffects.Add(activateClass_Support[0]);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (card.Owner.SupportCards.Contains(card))
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit != null)
                    {
                        if (GManager.instance.turnStateMachine.AttackingUnit.Character != null)
                        {
                            if (GManager.instance.turnStateMachine.AttackingUnit.Character.Owner == card.Owner)
                            {
                                if (card.Owner.Enemy.HandCards.Count >= 5)
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
                SelectHandEffect selectHandEffect = GetComponent<SelectHandEffect>();

                selectHandEffect.SetUp(
                                SelectPlayer: card.Owner.Enemy,
                                CanTargetCondition: (cardSource) => cardSource.Owner.HandCards.Contains(cardSource),
                                CanTargetCondition_ByPreSelecetedList: null,
                                CanEndSelectCondition: null,
                                MaxCount: 1,
                                CanNoSelect: false,
                                CanEndNotMax: false,
                                isShowOpponent: true,
                                SelectCardCoroutine: null,
                                AfterSelectCardCoroutine: null,
                                mode: SelectHandEffect.Mode.Discard);

                yield return ContinuousController.instance.StartCoroutine(selectHandEffect.Activate(null));
            }
        }

        return supportEffects;
    }
    #endregion
}