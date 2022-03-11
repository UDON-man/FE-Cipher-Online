using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Doga_SolidFantomKnight : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnAttackedAlly)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("慈愛の盾", "Compassionate Shield",new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition1 }, -1, true,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition1(Hashtable hashtable)
            {
                if(card.UnitContainingThisCharacter() != null)
                {
                    if(card.Owner.FieldUnit.Contains(card.UnitContainingThisCharacter()))
                    {
                        if (GManager.instance.turnStateMachine.DefendingUnit != null)
                        {
                            if (GManager.instance.turnStateMachine.DefendingUnit.Character != null)
                            {
                                if (GManager.instance.turnStateMachine.DefendingUnit.Character.Owner == card.Owner)
                                {
                                    if (!GManager.instance.turnStateMachine.DefendingUnit.Character.UnitNames.Contains("源まもり"))
                                    {
                                        return true;
                                    }
                                }
                            }

                        }
                    }
                }
                

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                Unit masterUnit = null;

                foreach (Unit unit in card.Owner.FieldUnit)
                {
                    if (unit.Character.UnitNames.Contains("源まもり"))
                    {
                        masterUnit = unit;
                        break;
                    }
                }

                if (masterUnit != null)
                {
                    #region 旧防御ユニットのエフェクトを削除
                    GManager.instance.turnStateMachine.DefendingUnit.ShowingFieldUnitCard.OffAttackerDefenderEffect();
                    GManager.instance.OffTargetArrow();
                    #endregion

                    //防御ユニットを更新
                    GManager.instance.turnStateMachine.DefendingUnit = masterUnit;

                    #region 新防御ユニットのエフェクトを表示
                    GManager.instance.turnStateMachine.DefendingUnit.ShowingFieldUnitCard.SetDefenderEffect();
                    yield return GManager.instance.OnTargetArrow(
                    GManager.instance.turnStateMachine.AttackingUnit.ShowingFieldUnitCard.GetLocalCanvasPosition(),
                    GManager.instance.turnStateMachine.DefendingUnit.ShowingFieldUnitCard.GetLocalCanvasPosition(),
                    GManager.instance.turnStateMachine.AttackingUnit.ShowingFieldUnitCard,
                    GManager.instance.turnStateMachine.DefendingUnit.ShowingFieldUnitCard);
                    #endregion
                }

                yield return null;
            }
        }

        CanNotDestroyedByBattleClass canNotDestroyedByBattleClass = new CanNotDestroyedByBattleClass();
        canNotDestroyedByBattleClass.SetUpICardEffect("カルネージフォーム","", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
        canNotDestroyedByBattleClass.SetUpCanNotDestroyedByBattleClass((AttackingUnit) => AttackingUnit.Character.Owner == card.Owner.Enemy, (DefendingUnit) => DefendingUnit == card.UnitContainingThisCharacter());
        canNotDestroyedByBattleClass.SetCF();
        cardEffects.Add(canNotDestroyedByBattleClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if (card.UnitContainingThisCharacter() != null)
            {
                if (card.Owner.GetFrontUnits().Contains(card.UnitContainingThisCharacter()))
                {
                    if (card.Owner.GetFrontUnits().Count((unit) => unit.Character.UnitNames.Contains("源まもり")) > 0)
                    {
                        return true;
                    }
                }

                else if (card.Owner.GetBackUnits().Contains(card.UnitContainingThisCharacter()))
                {
                    if (card.Owner.GetBackUnits().Count((unit) => unit.Character.UnitNames.Contains("源まもり")) > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        return cardEffects;
    }

    #region 幻影の紋章
    public override List<ICardEffect> SupportEffects(EffectTiming timing, CardSource card)
    {
        string _masterUnitName = "源まもり";

        List<ICardEffect> supportEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDiscardSuppot)
        {
            ActivateClass activateClass_Support = new ActivateClass();
            activateClass_Support.SetUpICardEffect("幻影の紋章", "Mirage Emblem", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true, card);
            activateClass_Support.SetUpActivateClass((hashtable) => ActivateCoroutine(_masterUnitName));
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
                                if (card.CanPlayAsNewUnit())
                                {
                                    if (card.Owner.FieldUnit.Count((unit) => unit.Character.UnitNames.Contains(_masterUnitName)) > 0)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine(string masterUnitName)
            {
                Unit masterUnit = null;
                bool isFront = false;

                foreach (Unit unit in card.Owner.FieldUnit)
                {
                    if (unit.Character.UnitNames.Contains(masterUnitName))
                    {
                        masterUnit = unit;
                        break;
                    }
                }

                if (masterUnit != null)
                {
                    if (card.Owner.GetFrontUnits().Contains(masterUnit))
                    {
                        isFront = true;
                    }

                    else if (card.Owner.GetBackUnits().Contains(masterUnit))
                    {
                        isFront = false;
                    }

                    card.Owner.SupportHandCard.gameObject.SetActive(false);
                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("cardEffect", activateClass_Support);
                    yield return StartCoroutine(new IPlayUnit(card, null, isFront, true, hashtable, false).PlayUnit());
                    card.Owner.SupportCards = new List<CardSource>();
                }
            }
        }

        return supportEffects;
    }

    #endregion
}
