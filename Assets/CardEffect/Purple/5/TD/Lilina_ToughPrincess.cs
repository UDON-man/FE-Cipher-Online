using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Lilina_ToughPrincess : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnEnterFieldAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("オスティアの盟主", "Leader of Ostia", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (hashtable != null)
                {
                    if (hashtable.ContainsKey("Unit"))
                    {
                        if (hashtable["Unit"] is Unit)
                        {
                            Unit Unit = (Unit)hashtable["Unit"];

                            if(Unit.Character != null)
                            {
                                if (Unit.Character.Owner == card.Owner)
                                {
                                    if (Unit.Character.cEntity_EffectController.GetAllSupportEffects().Count((cardEffect) => !cardEffect.IsInvalidate) > 0)
                                    {
                                        return true;
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

        else if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("溢れる魔力", "Overflowing Magic Power",new List<Cost>() { new ReverseCost(2, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { (hash) => IsExistOnField(hash,card) }, 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                yield return ContinuousController.instance.StartCoroutine(new IDraw(card.Owner, 1).Draw());
            }
        }

        return cardEffects;
    }
}

