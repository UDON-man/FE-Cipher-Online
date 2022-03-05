using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;


public class Krtnaga_PrincessOfGrdra : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDestroyDuringBattleAlly)
        {
            activateClass[0].SetUpICardEffect("竜王の後継者", new List<Cost>() , new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Successor of the Dragon King";
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable))
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                    {
                        return true;
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                Unit targetUnit = null;

                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner == card.Owner && unit.Weapons.Contains(Weapon.Beast),
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
                    targetUnit = unit;

                    yield return null;
                }

                if (targetUnit != null)
                {
                    SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                    selectCardEffect.SetUp(
                        CanTargetCondition: CanTargetCondition,
                        CanTargetCondition_ByPreSelecetedList: null,
                        CanEndSelectCondition: null,
                        CanNoSelect: () => true,
                        SelectCardCoroutine: (cardSource) => SelectCardCoroutine(cardSource),
                        AfterSelectCardCoroutine: null,
                        Message: "Select a card to stack on top.",
                        MaxCount: 1,
                        CanEndNotMax: false,
                        isShowOpponent: true,
                        mode: SelectCardEffect.Mode.Custom,
                        root: SelectCardEffect.Root.Library,
                        CustomRootCardList: null,
                        CanLookReverseCard: true);

                    yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));

                    bool CanTargetCondition(CardSource cardSource)
                    {
                        foreach (string UnitName in targetUnit.Character.UnitNames)
                        {
                            if (cardSource.UnitNames.Contains(UnitName))
                            {
                                return true;
                            }
                        }

                        return false;
                    }

                    IEnumerator SelectCardCoroutine(CardSource cardSource)
                    {
                        Hashtable hashtable = new Hashtable();
                        hashtable.Add("cardEffect", activateClass[0]);

                        yield return ContinuousController.instance.StartCoroutine(new IPlayUnit(cardSource, targetUnit, false, true, hashtable, false).PlayUnit());
                    }
                }
            }
        }

        else if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[1].SetUpICardEffect("黒焔のブレス", new List<Cost>() { new ReverseCost(2, (cardSource) => true) }, new List<Func<Hashtable, bool>>(), -1, false);
            activateClass[1].SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass[1].SetLvS(card.UnitContainingThisCharacter(),3);
            cardEffects.Add(activateClass[1]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[1].EffectName = "Black Dragon Breath";
            }

            IEnumerator ActivateCoroutine()
            {
                List<Unit> EnemyUnits = new List<Unit>();

                foreach (Unit unit in card.Owner.Enemy.FieldUnit)
                {
                    EnemyUnits.Add(unit);
                }

                Hashtable hashtable = new Hashtable();
                hashtable.Add("cardEffect", activateClass[1]);

                foreach (Unit unit in EnemyUnits)
                {
                    if (unit != unit.Character.Owner.Lord)
                    {
                        yield return ContinuousController.instance.StartCoroutine(new IDestroyUnit(unit, 1, BreakOrbMode.Hand, hashtable).Destroy());
                    }
                }
            }
        }

        return cardEffects;
    }
}



