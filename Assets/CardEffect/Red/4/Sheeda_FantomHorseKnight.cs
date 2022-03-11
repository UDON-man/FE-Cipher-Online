using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Sheeda_FantomHorseKnight : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("ツバサと共に", "Harmonious Wings", new List<Cost>() { new TapCost() }, null, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner == card.Owner && unit != card.UnitContainingThisCharacter(),
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: false,
                    CanEndNotMax: false,
                    SelectUnitCoroutine: (unit) => SelectUnitCoroutine(unit),
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Custom,
                    cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));

                IEnumerator SelectUnitCoroutine(Unit unit)
                {
                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("cardEffect", selectUnitEffect);
                    yield return ContinuousController.instance.StartCoroutine(new IMoveUnit(new List<Unit>() { unit }, true, hashtable).MoveUnits());

                    if(unit.Character.UnitNames.Contains("織部つばさ"))
                    {
                        ActivateClass activateClass1 = new ActivateClass();
                        activateClass1.SetUpICardEffect("","", new List<Cost>() , new List<Func<Hashtable, bool>>(), -1, true,card);
                        activateClass1.SetUpActivateClass((_hashtable) => ActivateCoroutine1());

                        IEnumerator ActivateCoroutine1()
                        {
                            Hashtable _hashtable = new Hashtable();
                            _hashtable.Add("cardEffect", activateClass1);
                            yield return ContinuousController.instance.StartCoroutine(new IMoveUnit(new List<Unit>() { card.UnitContainingThisCharacter() }, true, _hashtable).MoveUnits());
                        }

                        if (activateClass1.CanUse(null))
                        {
                            yield return ContinuousController.instance.StartCoroutine(activateClass1.Activate_Optional_Cost_Execute(null, "Do you move the unit?"));
                        }
                    }
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
            if(card.UnitContainingThisCharacter() != null)
            {
                if(card.Owner.GetFrontUnits().Contains(card.UnitContainingThisCharacter()))
                {
                    if(card.Owner.GetFrontUnits().Count((unit) => unit.Character.UnitNames.Contains("織部つばさ")) > 0)
                    {
                        return true;
                    }
                }

                else if (card.Owner.GetBackUnits().Contains(card.UnitContainingThisCharacter()))
                {
                    if (card.Owner.GetBackUnits().Count((unit) => unit.Character.UnitNames.Contains("織部つばさ")) > 0)
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
        string _masterUnitName = "織部つばさ";

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
