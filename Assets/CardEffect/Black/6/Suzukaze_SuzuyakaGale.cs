using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Suzukaze_SuzuyakaGale : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("スズカゼの疾風針", "Kaze's Needle", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, null, 1, false, card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                PowerModifyClass powerUpClass = new PowerModifyClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter(), true);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);

                yield return null;
            }
        }

        ChangePlayCostClass  changePlayCostClass = new ChangePlayCostClass();
        changePlayCostClass.SetUpICardEffect("影縫い","Shadow Weaving", new List<Cost>(), new List<Func<Hashtable, bool>>(),-1,false,card);
        changePlayCostClass.SetUpChangeCCCostClass((cardSource,PlayCost) => PlayCost + 1,(cardSource) => cardSource.Owner == card.Owner.Enemy && cardSource.Owner.TrashCards.Contains(cardSource),true);
        cardEffects.Add(changePlayCostClass);

        return cardEffects;
    }
}

