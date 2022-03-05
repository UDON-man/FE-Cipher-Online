using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Flora_KikubariMaid : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[0].SetUpICardEffect("こちらです", new List<Cost>() { new TapCost() }, new List<Func<Hashtable, bool>>(), -1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine0());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Please don't slip!";
            }

            IEnumerator ActivateCoroutine0()
            {
                Hashtable hashtable = new Hashtable();
                hashtable.Add("cardEffect", activateClass[0]);
                yield return ContinuousController.instance.StartCoroutine(new IMoveUnit(new List<Unit>() { card.Owner.Lord }, true, hashtable).MoveUnits());
            }


            activateClass[1].SetUpICardEffect("お召し物をどうぞ", new List<Cost>() { new TapCost() }, new List<Func<Hashtable, bool>>(), -1, false);
            activateClass[1].SetUpActivateClass((hashtable) => ActivateCoroutine1());
            cardEffects.Add(activateClass[1]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[1].EffectName = "Let me assist you.";
            }

            IEnumerator ActivateCoroutine1()
            {
                PowerUpClass powerUpClass = new PowerUpClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.Owner.Lord);
                card.Owner.Lord.UntilEachTurnEndUnitEffects.Add(powerUpClass);

                yield return null;
            }
        }

        return cardEffects;
    }


}
