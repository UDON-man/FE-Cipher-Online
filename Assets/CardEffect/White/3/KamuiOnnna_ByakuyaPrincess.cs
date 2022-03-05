using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class KamuiOnnna_ByakuyaPrincess : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpClass powerUpClass = new PowerUpClass();
        powerUpClass.SetUpICardEffect("夜刀神・幻夜", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition1 }, -1, false);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter());
        cardEffects.Add(powerUpClass);

        bool CanUseCondition1(Hashtable hashtable)
        {
            if (card != null)
            {
                if (GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
                {
                    if (card.Owner.BondCards.Count((cardSource) => !cardSource.IsReverse && cardSource.cardColors.Contains(CardColor.White)) >= 1 && card.Owner.BondCards.Count((cardSource) => !cardSource.IsReverse && cardSource.cardColors.Contains(CardColor.Black)) >= 1)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        return cardEffects;
    }

    #region 英雄の紋章
    public override List<ICardEffect> SupportEffects(EffectTiming timing)
    {
        List<ICardEffect> supportEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnSetSupport)
        {
            activateClass_Support[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass_Support[0].SetUpICardEffect("英雄の紋章", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
            supportEffects.Add(activateClass_Support[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass_Support[0].EffectName = "Hero Emblem";
            }

            IEnumerator ActivateCoroutine()
            {
                StrikeUpClass strikeUpClass = new StrikeUpClass();
                strikeUpClass.SetUpStrikeUpClass((unit, Strike) => 2, (unit) => unit == GManager.instance.turnStateMachine.AttackingUnit && unit.Character.Owner == card.Owner);
                GManager.instance.turnStateMachine.AttackingUnit.UntilEndBattleEffects.Add(strikeUpClass);

                yield return null;
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
                                if (GManager.instance.turnStateMachine.AttackingUnit.Character.cardColors.Contains(CardColor.White))
                                {
                                    return true;
                                }
                            }

                        }
                    }
                }

                return false;
            }
        }

        return supportEffects;
    }
    #endregion
}

