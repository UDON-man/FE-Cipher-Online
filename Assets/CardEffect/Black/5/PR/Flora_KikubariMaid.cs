using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Flora_KikubariMaid : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("こちらです", "Please don't slip!", new List<Cost>() { new TapCost() }, new List<Func<Hashtable, bool>>(), -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine0());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine0()
            {
                Hashtable hashtable = new Hashtable();
                hashtable.Add("cardEffect", activateClass);
                yield return ContinuousController.instance.StartCoroutine(new IMoveUnit(new List<Unit>() { card.Owner.Lord }, true, hashtable).MoveUnits());
            }

            ActivateClass activateClass1 = new ActivateClass();
            activateClass1.SetUpICardEffect("お召し物をどうぞ", "Let me assist you.", new List<Cost>() { new TapCost() }, new List<Func<Hashtable, bool>>(), -1, false,card);
            activateClass1.SetUpActivateClass((hashtable) => ActivateCoroutine1());
            cardEffects.Add(activateClass1);

            IEnumerator ActivateCoroutine1()
            {
                PowerModifyClass powerUpClass = new PowerModifyClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.Owner.Lord, true);
                card.Owner.Lord.UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);

                yield return null;
            }
        }

        return cardEffects;
    }
}
