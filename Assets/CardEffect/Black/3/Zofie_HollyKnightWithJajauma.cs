using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Zofie_HollyKnightWithJajauma : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[0].SetUpICardEffect("人馬一体", new List<Cost>(), new List<Func<Hashtable, bool>>() { (hashtable) => !card.UnitContainingThisCharacter().IsTapped }, 1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass[0].SetCCS(card.UnitContainingThisCharacter());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Rider and Steed United";
            }

            IEnumerator ActivateCoroutine()
            {
                Hashtable hashtable = new Hashtable();
                hashtable.Add("cardEffect", activateClass[0]);
                yield return ContinuousController.instance.StartCoroutine(new IMoveUnit(new List<Unit>() { card.UnitContainingThisCharacter() }, true,hashtable).MoveUnits());

                PowerUpClass powerUpClass = new PowerUpClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter());
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add(powerUpClass);

            }
        }

        else if (timing == EffectTiming.OnEndAttackAnyone)
        {
            activateClass[1].SetUpICardEffect("再移動", new List<Cost>() , new List<Func<Hashtable, bool>>() { (hashtable) => GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter() }, -1, true);
            activateClass[1].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[1]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[1].EffectName = "Canto";
            }

            IEnumerator ActivateCoroutine()
            {
                Hashtable hashtable = new Hashtable();
                hashtable.Add("cardEffect", activateClass[1]);
                yield return ContinuousController.instance.StartCoroutine(new IMoveUnit(new List<Unit>() { card.UnitContainingThisCharacter() }, true,hashtable).MoveUnits());
            }
        }

        return cardEffects;
    }
}



