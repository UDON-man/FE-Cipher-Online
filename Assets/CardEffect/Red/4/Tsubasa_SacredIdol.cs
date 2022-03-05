using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Tsubasa_SacredIdol : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[0].SetUpICardEffect("グランドフィナーレ", new List<Cost>() { new ReverseCost(3, (cardSource) => true), new DiscardHandCost(1, (cardSource) => cardSource.UnitNames.Contains("織部つばさ")|| cardSource.UnitNames.Contains("シーダ")) }, null, 1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Grand Finale";
            }

            IEnumerator ActivateCoroutine()
            {
                foreach(Unit unit in card.Owner.FieldUnit)
                {
                    if(unit != card.UnitContainingThisCharacter())
                    {
                        if (unit.IsTapped)
                        {
                            Hashtable hashtable = new Hashtable();
                            hashtable.Add("cardEffect", activateClass[0]);
                            yield return ContinuousController.instance.StartCoroutine(unit.UnTap(hashtable));
                        }
                    }
                }
            }
        }

        else if(timing == EffectTiming.OnMovedAnyone)
        {
            activateClass[1].SetUpICardEffect("Fly～君という風～", new List<Cost>() , new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, false);
            activateClass[1].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[1]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[1].EffectName = "Fly: Your're My Wind";
            }

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
                PowerUpClass powerUpClass = new PowerUpClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter());
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add(powerUpClass);

                yield return null;
            }
        }

        return cardEffects;
    }
}
