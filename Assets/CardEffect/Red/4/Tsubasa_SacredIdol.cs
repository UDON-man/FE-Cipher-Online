using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Tsubasa_SacredIdol : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("グランドフィナーレ", "Grand Finale", new List<Cost>() { new ReverseCost(3, (cardSource) => true), new DiscardHandCost(1, (cardSource) => cardSource.UnitNames.Contains("織部つばさ")|| cardSource.UnitNames.Contains("シーダ")) }, null, 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                foreach(Unit unit in card.Owner.FieldUnit)
                {
                    if(unit != card.UnitContainingThisCharacter())
                    {
                        if (unit.IsTapped)
                        {
                            Hashtable hashtable = new Hashtable();
                            hashtable.Add("cardEffect", activateClass);
                            yield return ContinuousController.instance.StartCoroutine(unit.UnTap(hashtable));
                        }
                    }
                }
            }
        }

        else if(timing == EffectTiming.OnMovedAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("Fly～君という風～", "Fly: Your're My Wind",new List<Cost>() , new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if(hashtable != null)
                {
                    if(hashtable.ContainsKey("Unit"))
                    {
                        if(hashtable["Unit"] is Unit)
                        {
                            Unit unit = (Unit)hashtable["Unit"];

                            if(unit != null)
                            {
                                if(unit.Character != null)
                                {
                                    if(unit.Character.Owner == card.Owner)
                                    {
                                        if (hashtable.ContainsKey("cardEffect"))
                                        {
                                            if(hashtable["cardEffect"] is ICardEffect)
                                            {
                                                return true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                PowerModifyClass powerUpClass = new PowerModifyClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter(), true);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);

                yield return null;
            }
        }

        return cardEffects;
    }
}
