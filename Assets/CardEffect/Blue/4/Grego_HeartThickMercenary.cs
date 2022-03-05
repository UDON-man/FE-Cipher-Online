using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Grego_HeartThickMercenary : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("飛行特効", (enemyUnit, Power) => Power + 30, (unit) => unit == this.card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Wing), PowerUpByEnemy.Mode.Attacking);
        cardEffects.Add(powerUpByEnemy);

        if (timing == EffectTiming.OnCCAnyone)
        {
            activateClass[0].SetUpICardEffect("戦場のくわせ者", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Imposter on the Battlefield";
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if(card.UnitContainingThisCharacter() != null)
                {
                    if (hashtable != null)
                    {
                        if (hashtable.ContainsKey("Unit"))
                        {
                            if (hashtable["Unit"] is Unit)
                            {
                                Unit Unit = (Unit)hashtable["Unit"];

                                if (Unit.Character != null)
                                {
                                    return Unit.Character == this.card;
                                }

                            }
                        }
                    }
                }
                

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                SelectPlayer: card.Owner,
                CanTargetCondition: CanTargetCondition,
                CanTargetCondition_ByPreSelecetedList: null,
                CanEndSelectCondition: null,
                MaxCount: 1,
                CanNoSelect: true,
                CanEndNotMax: false,
                SelectUnitCoroutine: (unit) => SelectUnitCoroutine(unit),
                AfterSelectUnitCoroutine: null,
                mode: SelectUnitEffect.Mode.Custom);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));

                bool CanTargetCondition(Unit unit)
                {
                    if(unit.Character.Owner == card.Owner)
                    {
                        if(card.Owner.GetFrontUnits().Contains(card.UnitContainingThisCharacter()))
                        {
                            if(card.Owner.GetBackUnits().Contains(unit))
                            {
                                return true;
                            }
                        }

                        else if (card.Owner.GetBackUnits().Contains(card.UnitContainingThisCharacter()))
                        {
                            if (card.Owner.GetFrontUnits().Contains(unit))
                            {
                                return true;
                            }
                        }
                    }

                    return false;
                }

                IEnumerator SelectUnitCoroutine(Unit unit)
                {
                    if (unit != null)
                    {
                        Hashtable hashtable = new Hashtable();
                        hashtable.Add("cardEffect", activateClass[0]);
                        yield return ContinuousController.instance.StartCoroutine(new IMoveUnit(new List<Unit>() { unit, card.UnitContainingThisCharacter() }, true, hashtable).MoveUnits());
                    }
                }
            }
        }

        return cardEffects;
    }
}
