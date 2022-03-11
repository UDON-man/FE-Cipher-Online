using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Rivela_BeautifulPriest : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnAddHandCardFromTrash)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("僧侶の戦斧", "War Monk's Axe",new List<Cost>(), new List<Func<Hashtable, bool>>() , 1, false,card);
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


        return cardEffects;
    }
}
