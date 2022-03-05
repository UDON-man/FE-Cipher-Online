using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tsubasa_PegasusIdol : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[0].SetUpICardEffect("目指せ一番乗り!", new List<Cost>(), new List<Func<Hashtable, bool>>() { (hash) => !card.UnitContainingThisCharacter().IsTapped }, 1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "First on the Scene";
            }

            IEnumerator ActivateCoroutine()
            {
                Hashtable hashtable = new Hashtable();
                hashtable.Add("cardEffect", activateClass[0]);
                yield return ContinuousController.instance.StartCoroutine(new IMoveUnit(new List<Unit>() { card.UnitContainingThisCharacter() }, true, hashtable).MoveUnits());
            }
        }

        PowerUpClass powerUpClass = new PowerUpClass();
        powerUpClass.SetUpICardEffect("ツバサに夢chu♪", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition  }, -1, false);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, CanPowerUpCondition);
        cardEffects.Add(powerUpClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if(card.UnitContainingThisCharacter() != null)
            {
                if(card.UnitContainingThisCharacter().DoneMoveThisTurn)
                {
                    return true;
                }
            }

            return false;
        }

        bool CanPowerUpCondition(Unit unit)
        {
            if (card.UnitContainingThisCharacter() != null && unit.Character != null)
            {
                if(unit != card.UnitContainingThisCharacter() && unit.Character.Owner == card.Owner)
                {
                    if(card.Owner.GetFrontUnits().Contains(card.UnitContainingThisCharacter()))
                    {
                        if(card.Owner.GetFrontUnits().Contains(unit))
                        {
                            return true;
                        }
                    }

                    else if (card.Owner.GetBackUnits().Contains(card.UnitContainingThisCharacter()))
                    {
                        if (card.Owner.GetBackUnits().Contains(unit))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        

        return cardEffects;
    }
}



