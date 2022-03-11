using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Nabar_SilentRedFantom : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnAttackAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("怨敵調伏", "Exorcise the Profane",new List<Cost>() { new TapCost(),new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition1 }, -1, true,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition1(Hashtable hashtable)
            {
                if (GManager.instance.turnStateMachine.AttackingUnit != null && GManager.instance.turnStateMachine.DefendingUnit != null)
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit.Character != null && GManager.instance.turnStateMachine.DefendingUnit.Character != null)
                    {
                        if (GManager.instance.turnStateMachine.AttackingUnit.Character.Owner == card.Owner)
                        {
                            if (GManager.instance.turnStateMachine.AttackingUnit.Character.UnitNames.Contains("剣弥代"))
                            {
                                if(GManager.instance.turnStateMachine.DefendingUnit != card.Owner.Enemy.Lord)
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
                CanNotBeEvadedClass canNotBeEvadedClass = new CanNotBeEvadedClass();
                canNotBeEvadedClass.SetUpCanNotBeEvadedClass((AttackingUnit) => AttackingUnit == GManager.instance.turnStateMachine.AttackingUnit, (DefendingUnit) => true);
                GManager.instance.turnStateMachine.AttackingUnit.UntilEndBattleEffects.Add((_timing) => canNotBeEvadedClass);

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
                    if (card.Owner.GetFrontUnits().Count((unit) => unit.Character.UnitNames.Contains("剣弥代")) > 0)
                    {
                        return true;
                    }
                }

                else if (card.Owner.GetBackUnits().Contains(card.UnitContainingThisCharacter()))
                {
                    if (card.Owner.GetBackUnits().Count((unit) => unit.Character.UnitNames.Contains("剣弥代")) > 0)
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
        string _masterUnitName = "剣弥代";

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
