using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Cilus_AuthBestFriend : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        ChangeCardColorsClass changeCardColorsClass = new ChangeCardColorsClass();
        changeCardColorsClass.SetUpICardEffect("変わらぬ友情","", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition1 }, -1, false,card);
        changeCardColorsClass.SetUpCardColorChangeClass(ChangeCardColors, CanCardColorChangeCondition);
        cardEffects.Add(changeCardColorsClass);

        bool CanUseCondition1(Hashtable hashtable)
        {
            if (card != null)
            {
                if (card.UnitContainingThisCharacter() != null)
                {
                    if(card.Owner.FieldUnit.Contains(card.UnitContainingThisCharacter()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        List<CardColor> ChangeCardColors(CardSource cardSource,List<CardColor> cardColors)
        {
            if(cardColors.Contains(CardColor.Black))
            {
                cardColors.Add(CardColor.White);
            }

            return cardColors;
        }

        bool CanCardColorChangeCondition(CardSource cardSource)
        {
            if(cardSource.UnitContainingThisCharacter() != null)
            {
                if(card.Owner.FieldUnit.Contains(cardSource.UnitContainingThisCharacter()))
                {
                    return true;
                }
            }

            if(card.Owner.TrashCards.Contains(cardSource))
            {
                return true;
            }

            return false;
        }

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("友情の誓い","", null, null, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, PowerUpCondition, true);
        cardEffects.Add(powerUpClass);

        bool PowerUpCondition(Unit unit)
        {
            if (unit == card.UnitContainingThisCharacter())
            {
                if (GManager.instance.turnStateMachine.AttackingUnit != null && GManager.instance.turnStateMachine.DefendingUnit != null)
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit == unit || GManager.instance.turnStateMachine.DefendingUnit == unit)
                    {
                        if (card.UnitContainingThisCharacter() == GManager.instance.turnStateMachine.AttackingUnit || card.UnitContainingThisCharacter() == GManager.instance.turnStateMachine.DefendingUnit)
                        {
                            if (card.Owner.SupportCards.Count((cardSource) => cardSource.UnitNames.Contains("カムイ(男)") || cardSource.UnitNames.Contains("カムイ(女)")) > 0)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        return cardEffects;
    }
}

