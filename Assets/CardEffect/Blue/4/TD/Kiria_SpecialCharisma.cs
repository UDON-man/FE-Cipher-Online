using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Kiria_SpecialCharisma : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[0].SetUpICardEffect("Reincarnation", new List<Cost>() { new DiscardHandCost(1, (cardSource) => cardSource.Weapons.Contains(Weapon.Sharp)) }, null, 1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            IEnumerator ActivateCoroutine()
            {
                PowerUpClass powerUpClass = new PowerUpClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter());
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add(powerUpClass);

                yield return null;
            }
        }

        else if (timing == EffectTiming.OnEnterFieldAnyone)
        {
            activateClass[1].SetUpICardEffect("Give Me!!", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, true);
            activateClass[1].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[1]);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (hashtable != null)
                {
                    if (hashtable.ContainsKey("Unit"))
                    {
                        if (hashtable["Unit"] is Unit)
                        {
                            Unit Unit = (Unit)hashtable["Unit"];

                            if (Unit != null)
                            {
                                if (Unit.Character != null)
                                {
                                    if (Unit.Character.Owner == this.card.Owner)
                                    {
                                        if(hashtable.ContainsKey("cardEffect"))
                                        {
                                            if (hashtable["cardEffect"] is ICardEffect)
                                            {
                                                ICardEffect cardEffect = (ICardEffect)hashtable["cardEffect"];

                                                if(cardEffect != null)
                                                {
                                                    if(cardEffect.EffectName == "幻影の紋章"|| cardEffect.EffectName == "Mirage Emblem")
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
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                yield return ContinuousController.instance.StartCoroutine(new IDraw(card.Owner, 1).Draw());
            }
        }


        return cardEffects;
    }
}
