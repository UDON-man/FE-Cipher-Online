using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Shara_DarkFireOnmyoushi : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("魔術の才能", "Magical Talents",new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, null, 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine0());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine0()
            {
                PowerModifyClass powerUpClass = new PowerModifyClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter(), true);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);

                yield return null;
            }

            ActivateClass activateClass1 = new ActivateClass();
            activateClass1.SetUpICardEffect("秘術の研究", "Studying Numerology", new List<Cost>() { new DiscardHandCost(1, (cardSource) => cardSource.Weapons.Contains(Weapon.MagicBook)) }, null, 1, false,card);
            activateClass1.SetUpActivateClass((hashtable) => ActivateCoroutine1());
            cardEffects.Add(activateClass1);

            IEnumerator ActivateCoroutine1()
            {
                yield return ContinuousController.instance.StartCoroutine(new IDraw(card.Owner, 1).Draw());
            }
        }

        return cardEffects;
    }
}
