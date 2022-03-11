using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Edy_CheerfulSwordman : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass1 = new PowerModifyClass();
        powerUpClass1.SetUpICardEffect("子供扱いすんなよ!","", null, null, -1, false,card);
        powerUpClass1.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter() && GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner, true);
        powerUpClass1.SetLvS(card.UnitContainingThisCharacter(), 2);
        cardEffects.Add(powerUpClass1);

        PowerModifyClass powerUpClass2 = new PowerModifyClass();
        powerUpClass2.SetUpICardEffect("これが安全な戦い方だよな?","", null, null, -1, false,card);
        powerUpClass2.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter() && GManager.instance.turnStateMachine.gameContext.NonTurnPlayer == card.Owner, true);
        powerUpClass2.SetLvS(card.UnitContainingThisCharacter(), 3);
        cardEffects.Add(powerUpClass2);

        PowerModifyClass powerUpClass3 = new PowerModifyClass();
        powerUpClass3.SetUpICardEffect("カラドボルグ","", null, null, -1, false,card);
        powerUpClass3.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter(), true);
        powerUpClass3.SetLvS(card.UnitContainingThisCharacter(), 4);
        cardEffects.Add(powerUpClass3);

        return cardEffects;
    }

}
