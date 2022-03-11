using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Henley_BrokenHeart : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("赤の呪い","", null, null, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter() && card.Owner.Enemy.HandCards.Count <= 4, true);
        cardEffects.Add(powerUpClass);

        return cardEffects;
    }

    #region 暗闇の紋章
    public override List<ICardEffect> SupportEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> supportEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnSetSupport)
        {
            ActivateClass activateClass_Support = new ActivateClass();
            activateClass_Support.SetUpICardEffect("暗闇の紋章", "Darkness Emblem", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false, card);
            activateClass_Support.SetUpActivateClass((hashtable) => ActivateCoroutine());
            supportEffects.Add(activateClass_Support);

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
                if (card.Owner.Enemy.HandCards.Count >= 5)
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
                                    mode: SelectHandEffect.Mode.Discard,
                                    cardEffect: activateClass_Support);

                    yield return ContinuousController.instance.StartCoroutine(selectHandEffect.Activate(null));
                }
            }
        }

        return supportEffects;
    }
    #endregion
}
