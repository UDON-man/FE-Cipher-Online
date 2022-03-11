using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Shade_DarkRoadInvitor : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("草原を駆ける者", "Plains Galloper",new List<Cost>(), new List<Func<Hashtable, bool>>() { (hash) => !card.UnitContainingThisCharacter().IsTapped }, 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                Hashtable hashtable = new Hashtable();
                hashtable.Add("cardEffect", activateClass);
                yield return ContinuousController.instance.StartCoroutine(new IMoveUnit(new List<Unit>() { card.UnitContainingThisCharacter() }, true, hashtable).MoveUnits());
            }
        }

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("死の口づけ","", null, null, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter() && card.Owner.Enemy.HandCards.Count == 4 && GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner, true);
        cardEffects.Add(powerUpClass);

        return cardEffects;
    }
}



