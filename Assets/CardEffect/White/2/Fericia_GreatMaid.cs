using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Fericia_GreatMaid : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[0].SetUpICardEffect("いっきますよー!", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine0());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Here I go!";
            }

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
                hashtable.Add("cardEffect", activateClass[0]);
                yield return ContinuousController.instance.StartCoroutine(card.UnitContainingThisCharacter().UnTap(hashtable));
                yield return new WaitForSeconds(0.2f);
            }

            activateClass[1].SetUpICardEffect("容赦しませーん!", new List<Cost>() { new DiscardHandCost(1, CanTargetCondition) }, null, -1, false);
            activateClass[1].SetUpActivateClass((hashtable) => ActivateCoroutine1());
            cardEffects.Add(activateClass[1]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[1].EffectName = "I won't go easy!";
            }

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
                PowerUpClass powerUpClass = new PowerUpClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power * 2, (unit) => unit == card.UnitContainingThisCharacter());
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add(powerUpClass);

                yield return null;
            }
        }

        return cardEffects;
    }
}