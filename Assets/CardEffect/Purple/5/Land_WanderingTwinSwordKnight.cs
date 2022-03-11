using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Land_WanderingTwinSwordKnight : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        ChangeCardColorsClass changeCardColorsClass = new ChangeCardColorsClass();
        changeCardColorsClass.SetUpICardEffect("今日から仲間ってことで","", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition2 }, -1, false,card);
        changeCardColorsClass.SetUpCardColorChangeClass(ChangeCardColors, (cardSource) => cardSource == card);
        cardEffects.Add(changeCardColorsClass);

        bool CanUseCondition2(Hashtable hashtable)
        {
            if (card != null)
            {
                if (card.UnitContainingThisCharacter() != null)
                {
                    return true;
                }
            }

            return false;
        }

        List<CardColor> ChangeCardColors(CardSource cardSource, List<CardColor> cardColors)
        {
            if (card != null && cardColors != null)
            {
                if (card.UnitContainingThisCharacter() != null)
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter() || GManager.instance.turnStateMachine.DefendingUnit == card.UnitContainingThisCharacter())
                    {
                        foreach(CardSource cardSource1 in card.Owner.SupportCards)
                        {
                            foreach(CardColor cardColor in cardSource1.cardColors)
                            {
                                cardColors.Add(cardColor);
                            }
                        }
                    }
                }
            }

            return cardColors;
        }

        return cardEffects;
    }

    #region 攻撃の紋章
    public override List<ICardEffect> SupportEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> supportEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnSetSupport)
        {
            ActivateClass activateClass_Support = new ActivateClass();
            activateClass_Support.SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass_Support.SetUpICardEffect("攻撃の紋章", "Attack Emblem", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false, card);
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
                                return true;
                            }
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                PowerModifyClass powerUpClass = new PowerModifyClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, (unit) => unit == GManager.instance.turnStateMachine.AttackingUnit && unit.Character.Owner == card.Owner, true);
                GManager.instance.turnStateMachine.AttackingUnit.UntilEndBattleEffects.Add((_timing) => powerUpClass);

                yield return null;
            }
        }

        return supportEffects;
    }
    #endregion
}
