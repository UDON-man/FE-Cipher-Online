using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Lukina_FutureKnower : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("裏剣 ファルシオン", (enemyUnit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Dragon), PowerUpByEnemy.Mode.Attacking,card);
        cardEffects.Add(powerUpByEnemy);

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("運命に抗う者", "Defiant of Destiny",new List<Cost>() { new ReverseCost(1, (cardSource) => true), new DiscardHandCost(1, (cardSource) => cardSource.UnitNames.Contains("ルキナ")) }, null, 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                int drawCount = 2;
                int returnCount = 1;

                if(card.UnitContainingThisCharacter() != null)
                {
                    if (card.UnitContainingThisCharacter().IsClassChanged())
                    {
                        drawCount = 3;
                        returnCount = 2;
                    }
                }
               
                yield return ContinuousController.instance.StartCoroutine(new IDraw(card.Owner, drawCount).Draw());

                if (card.Owner.HandCards.Count >= returnCount)
                {
                    SelectHandEffect selectHandEffect = GetComponent<SelectHandEffect>();

                    selectHandEffect.SetUp(
                        SelectPlayer: card.Owner,
                        CanTargetCondition: (cardSource) => cardSource.Owner.HandCards.Contains(cardSource),
                        CanTargetCondition_ByPreSelecetedList: null,
                        CanEndSelectCondition: null,
                        MaxCount: returnCount,
                        CanNoSelect: false,
                        CanEndNotMax: false,
                        isShowOpponent: false,
                        SelectCardCoroutine: null,
                        AfterSelectCardCoroutine: null,
                        mode: SelectHandEffect.Mode.PutLibraryTop,
                        cardEffect:activateClass);

                    yield return StartCoroutine(selectHandEffect.Activate(null));
                }
            }
        }

        return cardEffects;
    }
}