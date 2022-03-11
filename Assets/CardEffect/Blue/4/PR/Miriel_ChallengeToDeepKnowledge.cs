using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Miriel_ChallengeToDeepKnowledge : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            SelectAllyCost selectAllyCost = new SelectAllyCost(
                 SelectPlayer: card.Owner,
                 CanTargetCondition: (unit) => unit.Character.Owner == card.Owner && unit != card.UnitContainingThisCharacter() && unit.Weapons.Contains(Weapon.MagicBook) && !unit.IsTapped,
                 CanTargetCondition_ByPreSelecetedList: null,
                 CanEndSelectCondition: null,
                 MaxCount: 4,
                 CanNoSelect: false,
                 CanEndNotMax: false,
                 SelectUnitCoroutine: null,
                 AfterSelectUnitCoroutine: null,
                 mode: SelectUnitEffect.Mode.Tap);

            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("智者の魔法陣", "Sage of Arcane Circles", new List<Cost>() { selectAllyCost }, null, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                PowerModifyClass powerUpClass = new PowerModifyClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 100, (unit) => unit == card.UnitContainingThisCharacter(), true);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);

                yield return null;
            }
        }

        return cardEffects;
    }
}