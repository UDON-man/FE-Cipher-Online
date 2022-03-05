using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Edy_CheerfulSwordman : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpClass powerUpClass1 = new PowerUpClass();
        powerUpClass1.SetUpICardEffect("子供扱いすんなよ!", null, null, -1, false);
        powerUpClass1.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter() && GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner);
        powerUpClass1.SetLvS(card.UnitContainingThisCharacter(), 2);
        cardEffects.Add(powerUpClass1);

        PowerUpClass powerUpClass2 = new PowerUpClass();
        powerUpClass2.SetUpICardEffect("これが安全な戦い方だよな?", null, null, -1, false);
        powerUpClass2.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter() && GManager.instance.turnStateMachine.gameContext.NonTurnPlayer == card.Owner);
        powerUpClass2.SetLvS(card.UnitContainingThisCharacter(), 3);
        cardEffects.Add(powerUpClass2);

        PowerUpClass powerUpClass3 = new PowerUpClass();
        powerUpClass3.SetUpICardEffect("カラドボルグ", null, null, -1, false);
        powerUpClass3.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter());
        powerUpClass3.SetLvS(card.UnitContainingThisCharacter(), 4);
        cardEffects.Add(powerUpClass3);

        return cardEffects;
    }

}
