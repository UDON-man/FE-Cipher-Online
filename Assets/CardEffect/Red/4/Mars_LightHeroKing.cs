using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class Mars_LightHeroKing : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("ファルシオン", (enemyUnit, Power) => Power + 20, (unit) => unit == this.card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Dragon), PowerUpByEnemy.Mode.Attacking);
        cardEffects.Add(powerUpByEnemy);

        if (timing == EffectTiming.OnDiscardSuppot)
        {
            activateClass[0].SetUpICardEffect("群の英雄", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Leader of the Ragtag";
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if (GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                    {
                        if (card.Owner.SupportCards.Count((cardSource) => cardSource.cEntity_Base.PlayCost <= 2) > 0)
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
                    if (cardSource.cEntity_Base.PlayCost <= 2)
                    {
                        card.Owner.SupportHandCard.gameObject.SetActive(false);
                        card.Owner.SupportCards.Remove(cardSource);
                        yield return ContinuousController.instance.StartCoroutine(CardObjectController.AddHandCard(cardSource, false));
                        ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(new List<CardSource>() { cardSource }, "Added Hand Card", true));
                    }
                }
            }
        }

        return cardEffects;
    }
}