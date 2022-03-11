using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Serena_PursueLonging : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("斧殺し", (enemyUnit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Axe), PowerUpByEnemy.Mode.Both,card);
        cardEffects.Add(powerUpByEnemy);

        SupportPowerUpClass supportPowerUpClass = new SupportPowerUpClass();
        supportPowerUpClass.SetUpICardEffect("あざとい一面", "",null, null, -1, false,card);
        supportPowerUpClass.SetUpSupportPowerUpClass((cardSource, SupportPower) => SupportPower + 10, ChangeSupportPowerCondition);
        supportPowerUpClass.SetCCS(card.UnitContainingThisCharacter());
        cardEffects.Add(supportPowerUpClass);

        bool ChangeSupportPowerCondition(CardSource cardSource)
        {
            if (GManager.instance.turnStateMachine.AttackingUnit != null && GManager.instance.turnStateMachine.DefendingUnit != null)
            {
                if (GManager.instance.turnStateMachine.AttackingUnit.Character.Owner == card.Owner || GManager.instance.turnStateMachine.DefendingUnit.Character.Owner == card.Owner)
                {
                    if (card.UnitContainingThisCharacter() == GManager.instance.turnStateMachine.AttackingUnit || card.UnitContainingThisCharacter() == GManager.instance.turnStateMachine.DefendingUnit)
                    {
                        if (cardSource.sex.Contains(Sex.male))
                        {
                            if (cardSource.Owner == card.Owner)
                            {
                                return true;
                            }
                        }
                    }
                }
            }


            return false;
        }

        return cardEffects;
    }
}

