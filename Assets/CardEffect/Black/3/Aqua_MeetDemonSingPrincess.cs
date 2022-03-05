using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Aqua_MeetDemonSingPrincess : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[0].SetUpICardEffect("滅びの予言詩", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>(), -1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Prophecy of Ruin";
            }

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
                            hashtable.Add("cardEffect", activateClass[0]);

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
                                    mode: SelectHandEffect.Mode.Discard);

                    yield return ContinuousController.instance.StartCoroutine(selectHandEffect.Activate(null));
                }
            }
        }

        PowerUpClass powerUpClass = new PowerUpClass();
        powerUpClass.SetUpICardEffect("異邦の王女", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition1 }, -1, false);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter());
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
        addHasCCClass.SetUpICardEffect("逢魔への暗道", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition2 }, -1, false);
        addHasCCClass.SetUpAddHasCCClass((cardSource) => cardSource == this.card, (unit) => true);
        cardEffects.Add(addHasCCClass);

        ChangeCCCostClass changeCCCostClass = new ChangeCCCostClass();
        changeCCCostClass.SetUpICardEffect("逢魔への暗道", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition2 }, -1, false);
        changeCCCostClass.SetUpChangeCCCostClass((cardSource, unit, CCCost) => 2, (cardSource) => cardSource == this.card);
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

