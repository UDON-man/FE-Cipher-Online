using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KannnaOnnna_WhiteGodDragon : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpClass powerUpClass = new PowerUpClass();
        powerUpClass.SetUpICardEffect("神祖竜の血族", null, null, -1, false);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 30, CanPowerUpCondition);
        cardEffects.Add(powerUpClass);

        bool CanPowerUpCondition(Unit unit)
        {
            if (card.UnitContainingThisCharacter() == unit)
            {
                if (card.Owner.BondCards.Count >= 6)
                {
                    return true;
                }
            }

            return false;
        }

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[1].SetUpICardEffect("暴走する力", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, null, -1, false);
            activateClass[1].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[1]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[1].EffectName = "Reckless Power";
            }

            IEnumerator ActivateCoroutine()
            {
                foreach(CardSource cardSource in card.Owner.BondCards)
                {
                    cardSource.SetReverse();
                }

                if(card.Owner.BondCards.Count >= 6)
                {
                    StrikeUpClass strikeUpClass = new StrikeUpClass();
                    strikeUpClass.SetUpStrikeUpClass((unit, Strike) => 2, (unit) => unit == card.UnitContainingThisCharacter());
                    card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add(strikeUpClass);
                }
                
                yield return null;
            }
        }

        return cardEffects;
    }
}