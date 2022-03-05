using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Kagerou_RoyalKunoichi : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        CanNotAttackClass canNotAttackClass1 = new CanNotAttackClass();
        canNotAttackClass1.SetUpICardEffect("忍法車返し", null, new List<Func<Hashtable, bool>>(), -1, false);
        canNotAttackClass1.SetUpCanNotAttackClass((AttackingUnit) => (AttackingUnit.Weapons.Contains(Weapon.Bow)|| AttackingUnit.Weapons.Contains(Weapon.DarkWeapon)) && AttackingUnit.Character.Owner != this.card.Owner && AttackingUnit.Character.Owner.GetBackUnits().Contains(AttackingUnit), (DefendingUnit) => DefendingUnit == this.card.UnitContainingThisCharacter());
        cardEffects.Add(canNotAttackClass1);

        CanNotAttackClass canNotAttackClass2 = new CanNotAttackClass();
        canNotAttackClass2.SetUpICardEffect("忍法車返し", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
        canNotAttackClass2.SetUpCanNotAttackClass((AttackingUnit) => (AttackingUnit.Weapons.Contains(Weapon.Bow) || AttackingUnit.Weapons.Contains(Weapon.DarkWeapon)) && AttackingUnit.Character.Owner != this.card.Owner && AttackingUnit.Character.Owner.GetFrontUnits().Contains(AttackingUnit), (DefendingUnit) => DefendingUnit == this.card.UnitContainingThisCharacter());
        cardEffects.Add(canNotAttackClass2);

        bool CanUseCondition(Hashtable hashtable)
        {
            if (card != null)
            {
                if (card.UnitContainingThisCharacter() != null)
                {
                    if(card.UnitContainingThisCharacter().IsClassChanged())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("針手裏剣", (enemyUnit, Power) => Power + 20, (unit) => unit == this.card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Armor), PowerUpByEnemy.Mode.Attacking);
        cardEffects.Add(powerUpByEnemy);

        return cardEffects;
    }
}
