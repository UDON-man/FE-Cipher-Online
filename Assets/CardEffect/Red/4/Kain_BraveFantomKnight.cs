using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Kain_BraveFantomKnight : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[0].SetUpICardEffect("爆速のエクスタシー", new List<Cost>() { new TapCost(), new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { IsExistOnField }, -1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Terminal Velocity";
            }

            IEnumerator ActivateCoroutine()
            {
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner == card.Owner && unit.Character.UnitNames.Contains("赤城斗馬"),
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: false,
                    CanEndNotMax: false,
                    SelectUnitCoroutine: (unit) => SelectUnitCoroutine(unit),
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Custom);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));

                IEnumerator SelectUnitCoroutine(Unit unit)
                {
                    StrikeUpClass strikeUpClass = new StrikeUpClass();
                    strikeUpClass.SetUpStrikeUpClass((_unit, Strike) => 2, (_unit) => _unit == unit);
                    unit.UntilEachTurnEndUnitEffects.Add(strikeUpClass);

                    yield return null;
                }
            }

        }

        CanNotDestroyedByBattleClass canNotDestroyedByBattleClass = new CanNotDestroyedByBattleClass();
        canNotDestroyedByBattleClass.SetUpICardEffect("カルネージフォーム", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
        canNotDestroyedByBattleClass.SetUpCanNotDestroyedByBattleClass((AttackingUnit) => AttackingUnit.Character.Owner == card.Owner.Enemy, (DefendingUnit) => DefendingUnit == card.UnitContainingThisCharacter());
        canNotDestroyedByBattleClass.SetCF();
        cardEffects.Add(canNotDestroyedByBattleClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if(IsExistOnField(hashtable))
            {
                if (card.UnitContainingThisCharacter() != null)
                {
                    if (card.Owner.GetFrontUnits().Contains(card.UnitContainingThisCharacter()))
                    {
                        if (card.Owner.GetFrontUnits().Count((unit) => unit.Character.UnitNames.Contains("赤城斗馬")) > 0)
                        {
                            return true;
                        }
                    }

                    else if (card.Owner.GetBackUnits().Contains(card.UnitContainingThisCharacter()))
                    {
                        if (card.Owner.GetBackUnits().Count((unit) => unit.Character.UnitNames.Contains("赤城斗馬")) > 0)
                        {
                            return true;
                        }
                    }
                }
            }
            

            return false;
        }

        return cardEffects;
    }

    #region 幻影の紋章
    public override List<ICardEffect> SupportEffects(EffectTiming timing)
    {
        string _masterUnitName = "赤城斗馬";

        List<ICardEffect> supportEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDiscardSuppot)
        {
            activateClass_Support[0].SetUpICardEffect("幻影の紋章", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true);
            activateClass_Support[0].SetUpActivateClass((hashtable) => ActivateCoroutine(_masterUnitName));
            supportEffects.Add(activateClass_Support[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass_Support[0].EffectName = "Mirage Emblem";
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
                    hashtable.Add("cardEffect", activateClass_Support[0]);
                    yield return StartCoroutine(new IPlayUnit(card, null, isFront, true, hashtable, false).PlayUnit());
                    card.Owner.SupportCards = new List<CardSource>();
                }
            }
        }

        return supportEffects;
    }

    #endregion
}
