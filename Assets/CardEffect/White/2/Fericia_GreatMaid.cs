using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Fericia_GreatMaid : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("いっきますよー!", "Here I go!",new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine0());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if(!card.Owner.Lord.IsTapped)
                {
                    return true;
                }

                return false;
            }

            IEnumerator ActivateCoroutine0()
            {
                yield return ContinuousController.instance.StartCoroutine(card.Owner.Lord.Tap());
                yield return new WaitForSeconds(0.2f);
                Hashtable hashtable = new Hashtable();
                hashtable.Add("cardEffect", activateClass);
                yield return ContinuousController.instance.StartCoroutine(card.UnitContainingThisCharacter().UnTap(hashtable));
                yield return new WaitForSeconds(0.2f);
            }

            ActivateClass activateClass1 = new ActivateClass();
            activateClass1.SetUpICardEffect("容赦しませーん!", "I won't go easy!", new List<Cost>() { new DiscardHandCost(1, CanTargetCondition) }, null, -1, false,card);
            activateClass1.SetUpActivateClass((hashtable) => ActivateCoroutine1());
            cardEffects.Add(activateClass1);

            bool CanTargetCondition(CardSource cardSource)
            {
                foreach(string UnitName in cardSource.UnitNames)
                {
                    if(card.Owner.Lord.Character.UnitNames.Count((_UnitName) => UnitName == _UnitName) > 0)
                    {
                        return true;
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine1()
            {
                PowerModifyClass powerUpClass = new PowerModifyClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power * 2, (unit) => unit == card.UnitContainingThisCharacter(), false);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);

                yield return null;
            }
        }

        return cardEffects;
    }
}