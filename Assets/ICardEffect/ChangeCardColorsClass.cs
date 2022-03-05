using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ChangeCardColorsClass : ICardEffect, IChangeCardColorsEffect
{
    public void SetUpCardColorChangeClass(Func<CardSource, List<CardColor>, List<CardColor>> ChangeCardColors, Func<CardSource, bool> CanCardColorChangeCondition)
    {
        this.ChangeCardColors = ChangeCardColors;
        this.CanCardColorChangeCondition = CanCardColorChangeCondition;
    }

    Func<CardSource, List<CardColor>, List<CardColor>> ChangeCardColors { get; set; }
    Func<CardSource, bool> CanCardColorChangeCondition;

    public List<CardColor> GetCardColors(List<CardColor> CardColors, CardSource cardSource)
    {
        if (CanCardColorChangeCondition(cardSource))
        {
            CardColors = ChangeCardColors(cardSource, CardColors);
        }

        return CardColors;
    }
}