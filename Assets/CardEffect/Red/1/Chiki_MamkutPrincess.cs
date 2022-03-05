using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Chiki_MamkutPrincess : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("神竜石", (enemyUnit, Power) => Power + 20, (unit) => unit == this.card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Dragon), PowerUpByEnemy.Mode.Attacking);
        cardEffects.Add(powerUpByEnemy);

        PowerUpClass powerUpClass = new PowerUpClass();
        powerUpClass.SetUpICardEffect("長寿な竜一族", null, null, -1, false);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 30, CanPowerUpCondition);
        cardEffects.Add(powerUpClass);

        bool CanPowerUpCondition(Unit unit)
        {
            if (card.UnitContainingThisCharacter() == unit)
            {
                if (card.Owner.BondCards.Count >= 8)
                {
                    return true;
                }
            }

            return false;
        }

        if(timing == EffectTiming.OnDiscardSuppot)
        {
            activateClass[0].SetUpICardEffect("竜姫の微笑み",null,new List<Func<Hashtable, bool>>() { CanUseCondition },-1,true);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "The Dragon Scion's Smile";
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if(GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                    {
                        if(card.Owner.SupportCards.Count((cardSource) => cardSource.CanSetBondThisCard) > 0)
                        {
                            return true;
                        }
                    }
                }
                
                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                List<CardSource> SupportCards = new List<CardSource>();

                foreach (CardSource cardSource in card.Owner.SupportCards)
                {
                    SupportCards.Add(cardSource);
                }

                foreach (CardSource cardSource in SupportCards)
                {
                    if(cardSource.CanSetBondThisCard)
                    {
                        card.Owner.SupportHandCard.gameObject.SetActive(false);
                        card.Owner.SupportCards.Remove(cardSource);
                        yield return StartCoroutine(new ISetBondCard(cardSource, true).SetBond());
                        yield return StartCoroutine(cardSource.Owner.bondObject.SetBond_Skill(cardSource.Owner));
                        ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(new List<CardSource>() { cardSource }, "Added Bond Card", true));
                    }
                }
            }
        }

        return cardEffects;
    }
}