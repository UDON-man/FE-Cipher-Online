using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;


public class Elincia_QueenOfDeadCountry : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("天空を翔ける者", "Angelic Flight", new List<Cost>(), new List<Func<Hashtable, bool>>() { (hash) => !card.UnitContainingThisCharacter().IsTapped }, 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                Hashtable hashtable = new Hashtable();
                hashtable.Add("cardEffect", activateClass);
                yield return ContinuousController.instance.StartCoroutine(new IMoveUnit(new List<Unit>() { card.UnitContainingThisCharacter() }, true,hashtable).MoveUnits());
            }
        }

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("白き戦装束","", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition1 }, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter(), true);
        cardEffects.Add(powerUpClass);

        bool CanUseCondition1(Hashtable hashtable)
        {
            if(GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
            {
                if (card.Owner.FieldUnit.Count((unit) => unit != card.UnitContainingThisCharacter() && unit.IsLevelUp()) >= 2)
                {
                    return true;
                }
            }
            
            return false;
        }

        return cardEffects;
    }
}


