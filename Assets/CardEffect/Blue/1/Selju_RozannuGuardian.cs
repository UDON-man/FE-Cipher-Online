using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;


public class Selju_RozannuGuardian : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("天空を翔ける者", "Angelic Flight", new List<Cost>(), new List<Func<Hashtable, bool>>() { (hash) => !card.UnitContainingThisCharacter().IsTapped }, 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                Hashtable hashtable = new Hashtable();
                hashtable.Add("cardEffect", activateClass);
                yield return ContinuousController.instance.StartCoroutine(new IMoveUnit(new List<Unit>() { card.UnitContainingThisCharacter() }, true,hashtable).MoveUnits());
            }
        }

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("飛竜の叫び","", null, null, -1, false,card);
        powerUpClass.SetUpPowerUpClass((cardSource, Power) => Power + 10, PowerUpCondition, true);
        powerUpClass.SetCCS(card.UnitContainingThisCharacter());
        cardEffects.Add(powerUpClass);

        bool PowerUpCondition(Unit unit)
        {
            if (card.UnitContainingThisCharacter() != unit)
            {
                if (unit.Weapons.Contains(Weapon.Wing))
                {
                    if (unit.Character.Owner == card.Owner)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        return cardEffects;
    }
}