using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Hardin_OreruanJuniorKing : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpClass powerUpClass = new PowerUpClass();
        powerUpClass.SetUpICardEffect("オレルアンの絆", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition1 }, -1, false);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter());
        cardEffects.Add(powerUpClass);

        bool CanUseCondition1(Hashtable hashtable)
        {
            if (GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
            {
                if (card.Owner.FieldUnit.Count((_unit) => _unit.Character.UnitNames.Contains("ニーナ")) > 0)
                {
                    return true;
                }

                if (card.Owner.FieldUnit.Count((_unit) => _unit.Character.UnitNames.Contains("ウルフ")) > 0)
                {
                    return true;
                }

                if (card.Owner.FieldUnit.Count((_unit) => _unit.Character.UnitNames.Contains("ザガロ")) > 0)
                {
                    return true;
                }

                if (card.Owner.FieldUnit.Count((_unit) => _unit.Character.UnitNames.Contains("ロシェ")) > 0)
                {
                    return true;
                }

                if (card.Owner.FieldUnit.Count((_unit) => _unit.Character.UnitNames.Contains("ビラク")) > 0)
                {
                    return true;
                }
            }


            return false;
        }


        return cardEffects;
    }

    #region 攻撃の紋章
    public override List<ICardEffect> SupportEffects(EffectTiming timing)
    {
        List<ICardEffect> supportEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnSetSupport)
        {
            activateClass_Support[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass_Support[0].SetUpICardEffect("攻撃の紋章", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
            supportEffects.Add(activateClass_Support[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass_Support[0].EffectName = "Attack Emblem";
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
                                return true;
                            }
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                PowerUpClass powerUpClass = new PowerUpClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, (unit) => unit == GManager.instance.turnStateMachine.AttackingUnit && unit.Character.Owner == card.Owner);
                GManager.instance.turnStateMachine.AttackingUnit.UntilEndBattleEffects.Add(powerUpClass);

                yield return null;
            }
        }

        return supportEffects;
    }
    #endregion
}
