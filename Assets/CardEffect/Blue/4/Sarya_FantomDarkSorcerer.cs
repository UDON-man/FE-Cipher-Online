using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class Sarya_FantomDarkSorcerer : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnAttackAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("オーバーフリーズ", "Overfreeze",new List<Cost>() { new TapCost() }, new List<Func<Hashtable, bool>>() { CanUseCondition1 }, -1, true,card);
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
                            if (GManager.instance.turnStateMachine.AttackingUnit.Character.UnitNames.Contains("黒乃霧亜"))
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
                if(card.Owner.Enemy.HandCards.Count > 0)
                {
                    SelectHandEffect selectHandEffect = GetComponent<SelectHandEffect>();

                    selectHandEffect.SetUp(
                            SelectPlayer: card.Owner.Enemy,
                            CanTargetCondition: (cardSource) => cardSource.Owner.HandCards.Contains(cardSource),
                            CanTargetCondition_ByPreSelecetedList: null,
                            CanEndSelectCondition: null,
                            MaxCount: 1,
                            CanNoSelect: false,
                            CanEndNotMax: false,
                            isShowOpponent: true,
                            SelectCardCoroutine: null,
                            AfterSelectCardCoroutine: null,
                            mode: SelectHandEffect.Mode.Custom,
                            cardEffect: activateClass);

                    yield return ContinuousController.instance.StartCoroutine(selectHandEffect.Activate(null));
                }
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
                    if (card.Owner.GetFrontUnits().Count((unit) => unit.Character.UnitNames.Contains("黒乃霧亜")) > 0)
                    {
                        return true;
                    }
                }

                else if (card.Owner.GetBackUnits().Contains(card.UnitContainingThisCharacter()))
                {
                    if (card.Owner.GetBackUnits().Count((unit) => unit.Character.UnitNames.Contains("黒乃霧亜")) > 0)
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
        string _masterUnitName = "黒乃霧亜";

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
