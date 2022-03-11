using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class KamuiOtoko_ByakuyaPrince : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("夜刀神・空夜","", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition1 }, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter(), true);
        cardEffects.Add(powerUpClass);

        bool CanUseCondition1(Hashtable hashtable)
        {
            if (card != null)
            {
                if (GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
                {
                    if (card.Owner.BondCards.Count((cardSource) => !cardSource.IsReverse && cardSource.cardColors.Contains(CardColor.White)) >= 2)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        ChangeCardColorsClass changeCardColorsClass = new ChangeCardColorsClass();
        changeCardColorsClass.SetUpICardEffect("暗夜の心","", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition2 }, -1, false,card);
        changeCardColorsClass.SetUpCardColorChangeClass((cardSource, cardColors) => { cardColors.Add(CardColor.Black); return cardColors; }, (cardSource) => cardSource == card);
        cardEffects.Add(changeCardColorsClass);

        bool CanUseCondition2(Hashtable hashtable)
        {
            if(card != null)
            {
                if(card.UnitContainingThisCharacter() != null)
                {
                    return true;
                }
            }

            return false;
        }

        return cardEffects;
    }

    #region 英雄の紋章
    public override List<ICardEffect> SupportEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> supportEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnSetSupport)
        {
            ActivateClass activateClass_Support = new ActivateClass();
            activateClass_Support.SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass_Support.SetUpICardEffect("英雄の紋章", "Hero Emblem", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false, card);
            supportEffects.Add(activateClass_Support);

            IEnumerator ActivateCoroutine()
            {
                StrikeModifyClass strikeModifyClass = new StrikeModifyClass();
                strikeModifyClass.SetUpStrikeModifyClass((unit, Strike) => 2, CanStrikeModifyCondition, false);
                GManager.instance.turnStateMachine.AttackingUnit.UntilEndBattleEffects.Add((_timing) => strikeModifyClass);

                bool CanStrikeModifyCondition(Unit unit)
                {
                    if (unit == GManager.instance.turnStateMachine.AttackingUnit && unit.Character != null)
                    {
                        if (unit.Character.Owner == card.Owner)
                        {
                            return true;
                        }
                    }

                    return false;
                }

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

