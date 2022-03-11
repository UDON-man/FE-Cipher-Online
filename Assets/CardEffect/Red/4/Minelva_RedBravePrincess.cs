using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Minelva_RedBravePrincess : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("天覆う翼", "Eclipsing Wings", new List<Cost>() { new ReverseCost(3, (cardSource) => true), new DiscardHandCost(1, (cardSource) => cardSource.UnitNames.Contains("ミネルバ")) }, null, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                SelectPlayer: card.Owner,
                CanTargetCondition: (unit) => unit.Character.Owner == card.Owner && unit.Weapons.Contains(Weapon.Wing),
                CanTargetCondition_ByPreSelecetedList: null,
                CanEndSelectCondition: null,
                MaxCount: card.Owner.FieldUnit.Count((unit) => unit.Weapons.Contains(Weapon.Wing)),
                CanNoSelect: true,
                CanEndNotMax: true,
                SelectUnitCoroutine: null,
                AfterSelectUnitCoroutine: null,
                mode: SelectUnitEffect.Mode.Move,
                cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));

                card.Owner.UntilOpponentTurnEndEffects.Add((_timing) => new AllyPowerUp(new List<Func<Unit, bool>>() { PowerUpConditon }, PlusPower));

                bool PowerUpConditon(Unit unit)
                {
                    if (unit.Character != null)
                    {
                        if (card.Owner.FieldUnit.Contains(unit))
                        {
                            if(unit.Weapons.Contains(Weapon.Wing))
                            {
                                return true;
                            }
                        }
                    }

                    return false;
                }

                int PlusPower(Unit unit, int Power)
                {
                    return Power + 30;
                }

                yield return null;
            }
        }

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("竜騎の将","",new List<Cost>(),new List<Func<Hashtable, bool>>() { CanUseCondition },-1,false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter(), true);
        cardEffects.Add(powerUpClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if(card.Owner.FieldUnit.Count((unit) => unit != card.UnitContainingThisCharacter() && unit.Weapons.Contains(Weapon.Wing)) >= 2)
            {
                return true;
            }

            return false;
        }

        return cardEffects;
    }
}