using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Oliver_TanasDomain : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnAttackedAlly)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("美の守護者", "Champion of Beauty",new List<Cost>() { new ReverseCost(2, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition1 }, -1, true,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition1(Hashtable hashtable)
            {
                if (GManager.instance.turnStateMachine.DefendingUnit != null)
                {
                    if(GManager.instance.turnStateMachine.DefendingUnit.Character != null)
                    {
                        if (GManager.instance.turnStateMachine.DefendingUnit.Character.Owner == card.Owner)
                        {
                            if(GManager.instance.turnStateMachine.DefendingUnit.Character.UnitNames.Contains("リュシオン")|| GManager.instance.turnStateMachine.DefendingUnit.Character.UnitNames.Contains("リアーネ"))
                            {
                                if (card.Owner.GetFrontUnits().Contains(card.UnitContainingThisCharacter()))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                #region 旧防御ユニットのエフェクトを削除
                GManager.instance.turnStateMachine.DefendingUnit.ShowingFieldUnitCard.OffAttackerDefenderEffect();
                GManager.instance.OffTargetArrow();
                #endregion

                //防御ユニットを更新
                GManager.instance.turnStateMachine.DefendingUnit = card.UnitContainingThisCharacter();

                #region 新防御ユニットのエフェクトを表示
                GManager.instance.turnStateMachine.DefendingUnit.ShowingFieldUnitCard.SetDefenderEffect();
                yield return GManager.instance.OnTargetArrow(
                    GManager.instance.turnStateMachine.AttackingUnit.ShowingFieldUnitCard.GetLocalCanvasPosition(),
                    GManager.instance.turnStateMachine.DefendingUnit.ShowingFieldUnitCard.GetLocalCanvasPosition(),
                    GManager.instance.turnStateMachine.AttackingUnit.ShowingFieldUnitCard,
                    GManager.instance.turnStateMachine.DefendingUnit.ShowingFieldUnitCard);
                #endregion

                yield return null;
            }
        }

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("麗しき私の小鳥","", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter(), true);
        cardEffects.Add(powerUpClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if (card.Owner.FieldUnit.Count((_unit) => _unit.Character.UnitNames.Contains("リュシオン")|| _unit.Character.UnitNames.Contains("リアーネ")) > 0)
            {
                return true;
            }

            return false;
        }

        return cardEffects;
    }
}
