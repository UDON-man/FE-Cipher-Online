using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Sakura_HeartfulBattleMiko : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("雪割りの桜矢", "Cascading Blossoms of Restoration",new List<Cost>() { new TapCost(), new ReverseCost(4, (cardSource) => true) }, new List<Func<Hashtable, bool>>(), -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                selectCardEffect.SetUp(
                    CanTargetCondition: (cardSource) => !cardSource.UnitNames.Contains("サクラ"),
                    CanTargetCondition_ByPreSelecetedList: CanTargetCondition_ByPreSelecetedList,
                    CanEndSelectCondition: CanEndSelectCondition,
                    CanNoSelect: () => false,
                    SelectCardCoroutine: null,
                    AfterSelectCardCoroutine: null,
                    Message: "Select cards to add to hand.",
                    MaxCount: 3,
                    CanEndNotMax: true,
                    isShowOpponent: true,
                    mode: SelectCardEffect.Mode.AddHand,
                    root: SelectCardEffect.Root.Trash,
                    CustomRootCardList: null,
                    CanLookReverseCard: true,
                    SelectPlayer: card.Owner,
                    cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));

                bool CanTargetCondition_ByPreSelecetedList(List<CardSource> PreSelectedList, CardSource cardSource)
                {
                    foreach (string UnitName in cardSource.UnitNames)
                    {
                        if (PreSelectedList.Count((_cardSource) => _cardSource.UnitNames.Contains(UnitName)) > 0)
                        {
                            return false;
                        }
                    }

                    return true;
                }

                bool CanEndSelectCondition(List<CardSource> PreSelectedList)
                {
                    foreach (CardSource cardSource in PreSelectedList)
                    {
                        foreach (string UnitName in cardSource.UnitNames)
                        {
                            if (PreSelectedList.Count((_cardSource) => _cardSource.UnitNames.Contains(UnitName) && _cardSource != cardSource) > 0)
                            {
                                return false;
                            }
                        }
                    }

                    return true;
                }
            }
        }

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("飛行特効", (enemyUnit, Power) => Power + 30, (unit) => unit == card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Wing), PowerUpByEnemy.Mode.Attacking,card);
        cardEffects.Add(powerUpByEnemy);

        return cardEffects;
    }
}
