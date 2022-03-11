using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Marks_UnifierOfAnnyaKingdom : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("神風", "Godly Wind",new List<Cost>() , new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if(card.Owner.OrbCards.Count((cardSource) => cardSource.IsReverse) > 0)
                {
                    return true;
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                if (card.Owner.OrbCards.Count((cardSource) => cardSource.IsReverse) > 0)
                {
                    SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                    selectCardEffect.SetUp(
                        CanTargetCondition: (cardSource) => cardSource.Owner == card.Owner && cardSource.IsReverse,
                        CanTargetCondition_ByPreSelecetedList: null,
                        CanEndSelectCondition: null,
                        CanNoSelect: () => false,
                        SelectCardCoroutine: null,
                        AfterSelectCardCoroutine: null,
                        Message: "Select a orb card to turn face up.",
                        MaxCount: 1,
                        CanEndNotMax: false,
                        isShowOpponent: true,
                        mode: SelectCardEffect.Mode.SetFace,
                        root: SelectCardEffect.Root.Orb,
                        CustomRootCardList: null,
                        CanLookReverseCard: false,
                        SelectPlayer: card.Owner,
                        cardEffect: activateClass);

                    yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));

                    foreach (Unit unit in card.Owner.FieldUnit)
                    {
                        CanAttackTargetUnitRegardlessRangeClass canAttackTargetUnitRegardlessRangeClass = new CanAttackTargetUnitRegardlessRangeClass();
                        canAttackTargetUnitRegardlessRangeClass.SetUpCanAttackTargetUnitRegardlessRangeClass((AttackingUnit) => AttackingUnit == unit, (DefendingUnit) => true);
                        unit.UntilEachTurnEndUnitEffects.Add((_timing) => canAttackTargetUnitRegardlessRangeClass);
                    }
                }

                yield return null;
            }
        }

        PowerModifyClass powerUpClass1 = new PowerModifyClass();
        powerUpClass1.SetUpICardEffect("一騎討ちの舞台","", null, null, -1, false,card);
        powerUpClass1.SetUpPowerUpClass((unit, Power) => Power + 30, CanPowerUpCondition, true);
        cardEffects.Add(powerUpClass1);

        bool CanPowerUpCondition(Unit unit)
        {
            if (card.Owner.OrbCards.Count((cardSource) => !cardSource.IsReverse) > 0)
            {
                if(IsExistOnField(null,card))
                {
                    if (unit.Character != null)
                    {
                        if (unit == card.UnitContainingThisCharacter())
                        {
                            return true;
                        }

                        if (unit == unit.Character.Owner.Lord && unit.Character.Owner == card.Owner.Enemy)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        ChangeCCCostClass changeCCCostClass = new ChangeCCCostClass();
        changeCCCostClass.SetUpICardEffect("竜の鼓動", "",new List<Cost>(), new List<Func<Hashtable, bool>>() , -1, false,card);
        changeCCCostClass.SetUpChangeCCCostClass(GetCCCost, (cardSource) => cardSource == card);
        cardEffects.Add(changeCCCostClass);

        int GetCCCost(CardSource cardSource,Unit targetUnit,int CCCost)
        {
            if(cardSource == card && card.Owner.HandCards.Contains(card))
            {
                CCCost -= card.Owner.OrbCards.Count((_cardSource) => !_cardSource.IsReverse);

                if(CCCost < 1)
                {
                    CCCost = 1;
                }
            }

            return CCCost;
        }

        return cardEffects;
    }
}
