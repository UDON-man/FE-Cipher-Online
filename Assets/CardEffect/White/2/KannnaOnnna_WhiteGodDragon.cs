using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KannnaOnnna_WhiteGodDragon : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("神祖竜の血族","", null, null, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 30, CanPowerUpCondition, true);
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
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("暴走する力", "Reckless Power", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, null, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                foreach(CardSource cardSource in card.Owner.BondCards)
                {
                    cardSource.SetReverse();
                }

                if(card.Owner.BondCards.Count >= 6)
                {
                    StrikeModifyClass strikeModifyClass = new StrikeModifyClass();
                    strikeModifyClass.SetUpStrikeModifyClass((unit, Strike) => 2, (unit) => unit == card.UnitContainingThisCharacter(), false);
                    card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => strikeModifyClass);
                }
                
                yield return null;
            }
        }

        return cardEffects;
    }
}