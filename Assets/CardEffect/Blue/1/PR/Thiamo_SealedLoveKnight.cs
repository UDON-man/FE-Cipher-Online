using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thiamo_SealedLoveKnight : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[0].SetUpICardEffect("手槍", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, null, -1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Javelin";
            }

            IEnumerator ActivateCoroutine()
            {
                RangeUpClass rangeUpClass = new RangeUpClass();
                rangeUpClass.SetUpRangeUpClass((unit, Range) => { Range.Add(1); Range.Add(2); return Range; }, (unit) => unit == card.UnitContainingThisCharacter());
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add(rangeUpClass);

                yield return null;
            }
        }

        else if(timing == EffectTiming.OnStartTurn)
        {
            activateClass[1].SetUpICardEffect("リフレッシュ", new List<Cost>(), new List<System.Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
            activateClass[1].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[1]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[1].EffectName = "Relief";
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if(card.UnitContainingThisCharacter() != null)
                {
                    if (card.Owner.GetFrontUnits().Contains(card.UnitContainingThisCharacter()))
                    {
                        if (card.Owner.GetFrontUnits().Count == 1)
                        {
                            return true;
                        }
                    }

                    else if (card.Owner.GetBackUnits().Contains(card.UnitContainingThisCharacter()))
                    {
                        if (card.Owner.GetBackUnits().Count == 1)
                        {
                            return true;
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
