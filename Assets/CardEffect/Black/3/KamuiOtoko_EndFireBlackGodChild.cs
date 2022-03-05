using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;


public class KamuiOtoko_EndFireBlackGodChild : CEntity_Effect
{
    
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[0].SetUpICardEffect("闇と光の炎刃", new List<Cost>() { new TapCost() }, new List<Func<Hashtable, bool>>() { CanUseCondition1 }, 1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Flaming Blade of Darkness and Light";
            }

            bool CanUseCondition1(Hashtable hashtable)
            {
                if (card.Owner.FieldUnit.Count((unit) => unit != card.UnitContainingThisCharacter() && !unit.IsTapped) >= 2)
                {
                    return true;
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                List<Unit> TappedUnits = new List<Unit>();

                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner == card.Owner && unit != card.UnitContainingThisCharacter() && !unit.IsTapped,
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 2,
                    CanNoSelect: false,
                    CanEndNotMax: false,
                    SelectUnitCoroutine: (unit) => SelectUnitCoroutine(unit),
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Custom);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));

                IEnumerator SelectUnitCoroutine(Unit unit)
                {
                    TappedUnits.Add(unit);
                    yield return ContinuousController.instance.StartCoroutine(unit.Tap());
                    yield return new WaitForSeconds(0.2f);
                }

                if (TappedUnits.Count == 2)
                {
                    selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner != card.Owner && unit != card.Owner.Enemy.Lord,
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: false,
                    CanEndNotMax: false,
                    SelectUnitCoroutine: null,
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Destroy);

                    yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));

                    if(TappedUnits.Count == TappedUnits.Count((unit) => unit.Character.cardColors.Contains(CardColor.White)))
                    {
                        activateClass[1].SetUpICardEffect("", new List<Cost>() { new ReverseCost(1, (_cardSource) => true) }, null, -1, true);
                        activateClass[1].SetUpActivateClass((hashtable) => ActivateCoroutine1());

                        IEnumerator ActivateCoroutine1()
                        {
                            Hashtable hashtable = new Hashtable();
                            hashtable.Add("cardEffect", activateClass[0]);
                            yield return ContinuousController.instance.StartCoroutine(card.UnitContainingThisCharacter().UnTap(hashtable));
                        }

                        if (activateClass[1].CanUse(null))
                        {
                            yield return ContinuousController.instance.StartCoroutine(activateClass[1].Activate_Optional_Cost_Execute(null, "Do you pay cost?"));
                        }
                    }
                }
            }
        }

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpICardEffect("暗夜の夜刀神・終夜", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("暗夜の夜刀神・終夜", (enemyUnit, Power) => Power + 40, (unit) => unit == this.card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Dragon), PowerUpByEnemy.Mode.Attacking);
        cardEffects.Add(powerUpByEnemy);

        bool CanUseCondition(Hashtable hashtable)
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
