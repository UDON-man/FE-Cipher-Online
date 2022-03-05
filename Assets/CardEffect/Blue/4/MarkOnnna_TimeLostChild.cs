using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class MarkOnnna_TimeLostChild : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        WeaponChangeClass weaponChangeClass = new WeaponChangeClass();
        weaponChangeClass.SetUpICardEffect("無限の可能性", new List<Cost>(), new List<Func<Hashtable, bool>>() { (hash) => GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner }, -1, false);
        weaponChangeClass.SetUpWeaponChangeClass(ChangeWeapon, (cardSource) => cardSource == card);
        cardEffects.Add(weaponChangeClass);

        List<Weapon> ChangeWeapon(CardSource cardSource,List<Weapon> Weapons)
        {
            List<Weapon> otherWeapons = new List<Weapon>();

            foreach(Unit _unit in card.Owner.FieldUnit)
            {
                if(!_unit.Character.UnitNames.Contains("マーク(女)"))
                {
                    foreach(Weapon weapon in _unit.Weapons)
                    {
                        if(!otherWeapons.Contains(weapon))
                        {
                            otherWeapons.Add(weapon);
                        }
                    }
                }
            }

            otherWeapons = otherWeapons.Distinct().ToList();

            foreach(Weapon weapon in otherWeapons)
            {
                if(!Weapons.Contains(weapon))
                {
                    Weapons.Add(weapon);
                }
            }

            Weapons = Weapons.Distinct().ToList();

            return Weapons;
        }

        RangeUpClass rangeUpClass = new RangeUpClass();
        rangeUpClass.SetUpICardEffect("無限の可能性", new List<Cost>(), new List<Func<Hashtable, bool>>() { (hash) => GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner && card.UnitContainingThisCharacter() != null }, -1, false);
        rangeUpClass.SetUpRangeUpClass(ChangeRange, (unit) => unit == card.UnitContainingThisCharacter());
        cardEffects.Add(rangeUpClass);

        List<int> ChangeRange(Unit unit, List<int> Range)
        {
            List<int> otherRange = new List<int>();

            foreach (Unit _unit in card.Owner.FieldUnit)
            {
                if (!_unit.Character.UnitNames.Contains("マーク(女)"))
                {
                    foreach (int range in _unit.Range)
                    {
                        if (!otherRange.Contains(range))
                        {
                            otherRange.Add(range);
                        }
                    }
                }
            }

            otherRange = otherRange.Distinct().ToList();

            foreach (int range in otherRange)
            {
                if (!Range.Contains(range))
                {
                    Range.Add(range);
                }
            }

            Range = Range.Distinct().ToList();

            return Range;
        }

        PowerUpClass powerUpClass = new PowerUpClass();
        powerUpClass.SetUpICardEffect("華炎", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter());
        powerUpClass.SetCCS(card.UnitContainingThisCharacter());
        cardEffects.Add(powerUpClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if(GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
            {
                if(card.UnitContainingThisCharacter() != null)
                {
                    if(card.UnitContainingThisCharacter().Weapons.Distinct().ToList().Count >= 3)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        return cardEffects;
    }
}
