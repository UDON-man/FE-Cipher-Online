using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Land_WanderingTwinSwordKnight : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        ChangeCardColorsClass changeCardColorsClass = new ChangeCardColorsClass();
        changeCardColorsClass.SetUpICardEffect("今日から仲間ってことで", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition2 }, -1, false);
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
    public override List<ICardEffect> SupportEffects(EffectTiming timing)
    {
        List<ICardEffect> supportEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnSetSupport)
        {
            activateClass_Support[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass_Support[0].SetUpICardEffect("攻撃の紋章", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
            supportEffects.Add(activateClass_Support[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass_Support[0].EffectName = "Attack Emblem";
            }

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
                PowerUpClass powerUpClass = new PowerUpClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, (unit) => unit == GManager.instance.turnStateMachine.AttackingUnit && unit.Character.Owner == card.Owner);
                GManager.instance.turnStateMachine.AttackingUnit.UntilEndBattleEffects.Add(powerUpClass);

                yield return null;
            }
        }

        return supportEffects;
    }
    #endregion
}
