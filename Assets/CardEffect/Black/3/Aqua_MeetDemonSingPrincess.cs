using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Aqua_MeetDemonSingPrincess : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("滅びの予言詩", "Prophecy of Ruin", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>(), -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                List<IDestroyUnit> destroyUnits = new List<IDestroyUnit>();

                List<Unit> targetUnits = new List<Unit>();

                foreach (Unit unit in card.Owner.Enemy.FieldUnit)
                {
                    targetUnits.Add(unit);
                }

                foreach (Unit unit in targetUnits)
                {
                    if(unit != card.Owner.Enemy.Lord)
                    {
                        if(unit.Weapons.Contains(Weapon.DragonStone))
                        {
                            Hashtable hashtable = new Hashtable();
                            hashtable.Add("cardEffect", activateClass);
                            hashtable.Add("Unit", new Unit(unit.Characters));
                            IDestroyUnit destroyUnit = new IDestroyUnit(unit, 1, BreakOrbMode.Hand, hashtable);
                            destroyUnits.Add(destroyUnit);

                            yield return ContinuousController.instance.StartCoroutine(destroyUnit.Destroy());
                        }
                    }
                }

                int discardCount = destroyUnits.Count((destroyUnit) => destroyUnit.Destroyed);

                if(card.Owner.Enemy.HandCards.Count < discardCount)
                {
                    discardCount = card.Owner.Enemy.HandCards.Count;
                }

                if(discardCount > 0)
                {
                    SelectHandEffect selectHandEffect = GetComponent<SelectHandEffect>();

                    selectHandEffect.SetUp(
                                    SelectPlayer: card.Owner.Enemy,
                                    CanTargetCondition: (cardSource) => cardSource.Owner.HandCards.Contains(cardSource),
                                    CanTargetCondition_ByPreSelecetedList: null,
                                    CanEndSelectCondition: null,
                                    MaxCount: discardCount,
                                    CanNoSelect: false,
                                    CanEndNotMax: false,
                                    isShowOpponent: true,
                                    SelectCardCoroutine: null,
                                    AfterSelectCardCoroutine: null,
                                    mode: SelectHandEffect.Mode.Discard,
                                    cardEffect: activateClass);

                    yield return ContinuousController.instance.StartCoroutine(selectHandEffect.Activate(null));
                }
            }
        }

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("異邦の王女", "",new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition1 }, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter(), true);
        cardEffects.Add(powerUpClass);

        bool CanUseCondition1(Hashtable hashtable)
        {
            if (card.Owner.BondCards.Count((cardSource) => cardSource.IsReverse) >= 2)
            {
                return true;
            }

            return false;
        }

        AddHasCCClass addHasCCClass = new AddHasCCClass();
        addHasCCClass.SetUpICardEffect("逢魔への暗道", "",new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition2 }, -1, false,card);
        addHasCCClass.SetUpAddHasCCClass((cardSource) => cardSource == card, (unit) => true);
        cardEffects.Add(addHasCCClass);

        ChangeCCCostClass changeCCCostClass = new ChangeCCCostClass();
        changeCCCostClass.SetUpICardEffect("逢魔への暗道","", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition2 }, -1, false,card);
        changeCCCostClass.SetUpChangeCCCostClass((cardSource, unit, CCCost) => 2, (cardSource) => cardSource == card);
        cardEffects.Add(changeCCCostClass);

        bool CanUseCondition2(Hashtable hashtable)
        {
            if (card != null)
            {
                if (card.Owner.BondCards.Count((cardSource) => !cardSource.IsReverse && cardSource.cardColors.Contains(CardColor.White)) >= 1)
                {
                    return true;
                }
            }

            return false;
        }

        return cardEffects;
    }
}

