using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Miriel_ChallengeToDeepKnowledge : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
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

            activateClass[0].SetUpICardEffect("智者の魔法陣", new List<Cost>() { selectAllyCost }, null, -1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Sage of Arcane Circles";
            }

            IEnumerator ActivateCoroutine()
            {
                PowerUpClass powerUpClass = new PowerUpClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 100, (unit) => unit == card.UnitContainingThisCharacter());
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add(powerUpClass);

                yield return null;
            }
        }

        return cardEffects;
    }
}