using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Atena_ForeignSwordman : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        CanNotPlayClass canNotPlayClass = new CanNotPlayClass();
        canNotPlayClass.SetUpICardEffect("少女剣士の恩返し", "",null, null,-1, false,card);
        canNotPlayClass.SetUpCanNotPlayClass((cardSource) => card.Owner.TrashCards.Count < 5);
        cardEffects.Add(canNotPlayClass);

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("サンダーソード", "Levin Sword", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, null, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                WeaponChangeClass weaponChangeClass = new WeaponChangeClass();
                weaponChangeClass.SetUpWeaponChangeClass((CardSource, Weapons) => { Weapons.Add(Weapon.MagicBook); return Weapons; }, CanWeaponChangeCondition);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => weaponChangeClass);

                bool CanWeaponChangeCondition(CardSource cardSource)
                {
                    if (card.UnitContainingThisCharacter() != null)
                    {
                        if (cardSource == card.UnitContainingThisCharacter().Character)
                        {
                            return true;
                        }
                    }

                    return false;
                }

                RangeUpClass rangeUpClass = new RangeUpClass();
                rangeUpClass.SetUpRangeUpClass((unit, Range) => { Range.Add(1); Range.Add(2); return Range; }, (unit) => unit == card.UnitContainingThisCharacter());
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => rangeUpClass);

                PowerModifyClass powerUpClass = new PowerModifyClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power - 10, (unit) => unit == card.UnitContainingThisCharacter(), true);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);

                yield return null;
            }
        }

        return cardEffects;
    }
}
