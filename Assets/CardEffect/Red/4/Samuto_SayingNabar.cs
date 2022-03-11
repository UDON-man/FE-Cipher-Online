using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Samuto_SayingNabar : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("剣闘士の絆","", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition1 }, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter(), true);
        cardEffects.Add(powerUpClass);

        bool CanUseCondition1(Hashtable hashtable)
        {
            if(GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
            {
                if (card.Owner.FieldUnit.Count((_unit) => _unit.Character.UnitNames.Contains("オグマ")) > 0)
                {
                    return true;
                }
            }
            

            return false;
        }

        CanLevelUpToThisCardClass canLevelUpToThisCardClass = new CanLevelUpToThisCardClass();
        canLevelUpToThisCardClass.SetUpICardEffect("ウソ!…ウソですよ","", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
        canLevelUpToThisCardClass.SetUpCanLevelUpToThisCardClass((cardSource) => cardSource == card && card.Owner.FieldUnit.Count((unit) => unit.Character.UnitNames.Contains("サムトー")) == 0, (unit) => unit.Character.Owner == card.Owner && unit.Character.UnitNames.Contains("ナバール"));
        cardEffects.Add(canLevelUpToThisCardClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if (card.Owner.FieldUnit.Count((unit) => unit.Character.UnitNames.Contains("サムトー")) == 0)
            {
                return true;
            }

            return false;
        }


        return cardEffects;
    }

    #region 攻撃の紋章
    public override List<ICardEffect> SupportEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> supportEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnSetSupport)
        {
            ActivateClass activateClass_Support = new ActivateClass();
            activateClass_Support.SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass_Support.SetUpICardEffect("攻撃の紋章", "Attack Emblem", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false, card);
            supportEffects.Add(activateClass_Support);

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
                PowerModifyClass powerUpClass = new PowerModifyClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, (unit) => unit == GManager.instance.turnStateMachine.AttackingUnit && unit.Character.Owner == card.Owner, true);
                GManager.instance.turnStateMachine.AttackingUnit.UntilEndBattleEffects.Add((_timing) => powerUpClass);

                yield return null;
            }
        }

        return supportEffects;
    }
    #endregion
}
