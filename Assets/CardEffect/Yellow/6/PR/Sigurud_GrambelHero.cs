using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Sigurud_GrambelHero : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("誉れ高き騎士", "", null, null, -1, false, card);
        powerUpClass.SetUpPowerUpClass(ChangePower, CanPowerUpCondition, true);
        cardEffects.Add(powerUpClass);

        int ChangePower(Unit unit,int Power)
        {
            Power += 10 * card.Owner.BondCards.Count((cardSource) => !cardSource.IsReverse && cardSource.cEntity_EffectController.GetAllBSCardEffects().Count((cardEffect) => !cardEffect.IsInvalidate) > 0);
            return Power;
        }

        bool CanPowerUpCondition(Unit unit)
        {
            if (card.UnitContainingThisCharacter() == unit)
            {
                if (GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
                {
                    return true;
                }
            }

            return false;
        }
        
        return cardEffects;
    }
}
