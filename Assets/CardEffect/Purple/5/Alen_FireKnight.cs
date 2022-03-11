using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Alen_FireKnight : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("忠騎の絆","", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter(), true);
        cardEffects.Add(powerUpClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if (card.Owner.FieldUnit.Count((_unit) => _unit.Character.UnitNames.Contains("ランス")) > 0)
            {
                return true;
            }

            return false;
        }

        if (timing == EffectTiming.OnSetSupport)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("猛炎の双撃", "Fierce Flame Twin Strile",new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition1 }, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition1(Hashtable hashtable)
            {
                if(IsExistOnField(hashtable,card))
                {
                    if(GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter()|| GManager.instance.turnStateMachine.DefendingUnit == card.UnitContainingThisCharacter())
                    {
                        if(card.Owner.SupportCards.Count((cardSource) => cardSource.UnitNames.Contains("ランス")) > 0)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                PowerModifyClass _powerUpClass = new PowerModifyClass();
                _powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter(), true);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => _powerUpClass);

                StrikeModifyClass strikeModifyClass = new StrikeModifyClass();
                strikeModifyClass.SetUpStrikeModifyClass((unit, Strike) => 2, (unit) => unit == card.UnitContainingThisCharacter(), false);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => strikeModifyClass);

                yield return null;
            }
        }

        return cardEffects;
    }
}