using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Topak_ReleaseLeader : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("ラグズ奴隷解放軍", "",new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter(), true);
        cardEffects.Add(powerUpClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if (GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
            {
                if (card.Owner.FieldUnit.Count((_unit) => _unit != card.UnitContainingThisCharacter() && _unit.Weapons.Contains(Weapon.Beast)) >= 2)
                {
                    return true;
                }
            }

            return false;
        }

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("俊足", "Celerity", new List<Cost>(), new List<Func<Hashtable, bool>>() { (hash) => !card.UnitContainingThisCharacter().IsTapped }, 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                Hashtable hashtable = new Hashtable();
                hashtable.Add("cardEffect", activateClass);
                yield return ContinuousController.instance.StartCoroutine(new IMoveUnit(new List<Unit>() { card.UnitContainingThisCharacter() }, true, hashtable).MoveUnits());
            }
        }

        return cardEffects;
    }
}