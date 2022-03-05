using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class WeaponChangeClass : ICardEffect, IChangeWeaponEffect
{
    public void SetUpWeaponChangeClass(Func<CardSource, List<Weapon>, List<Weapon>> ChangeWeapons, Func<CardSource, bool> CanWeaponChangeCondition)
    {
        this.ChangeWeapons = ChangeWeapons;
        this.CanWeaponChangeCondition = CanWeaponChangeCondition;
    }

    Func<CardSource, List<Weapon>, List<Weapon>> ChangeWeapons { get; set; }
    Func<CardSource, bool> CanWeaponChangeCondition;


    public List<Weapon> GetWeapon(List<Weapon> Weapons, CardSource cardSource)
    {
        if (CanWeaponChangeCondition(cardSource))
        {
            Weapons = ChangeWeapons(cardSource, Weapons);
        }

        return Weapons;
    }
}