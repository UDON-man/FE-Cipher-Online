using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class KamuiOnnna_EndFireWhiteChild : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("光と闇の炎刃", "Flaming Blade of Light and Darkness",new List<Cost>() { new TapCost(), new ReverseCost(2, (cardSource) => true) }, new List<Func<Hashtable, bool>>() , 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                selectCardEffect.SetUp(
                    CanTargetCondition: CanTargetCondition,
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    CanNoSelect: () => false,
                    SelectCardCoroutine: null,
                    AfterSelectCardCoroutine: (targetCards) => AfterSelectCardCoroutine(targetCards),
                    Message: "Select a card to deploy.",
                    MaxCount: 1,
                    CanEndNotMax: false,
                    isShowOpponent: true,
                    mode: SelectCardEffect.Mode.Deploy,
                    root: SelectCardEffect.Root.Trash,
                    CustomRootCardList: null,
                    CanLookReverseCard: true,
                    SelectPlayer: card.Owner,
                    cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));

                bool CanTargetCondition(CardSource cardSource)
                {
                    if (cardSource.Owner == card.Owner)
                    {
                        if (cardSource.cEntity_Base.PlayCost <= 3)
                        {
                            if (cardSource.CanPlayAsNewUnit())
                            {
                                return true;
                            }
                        }
                    }

                    return false;
                }

                IEnumerator AfterSelectCardCoroutine(List<CardSource> targetCards)
                {
                    foreach(CardSource cardSource in targetCards)
                    {
                        if(cardSource.UnitContainingThisCharacter() != null)
                        {
                            if(cardSource.UnitContainingThisCharacter().Character != null)
                            {
                                if(cardSource.UnitContainingThisCharacter().Character.cardColors.Contains(CardColor.Black))
                                {
                                    Hashtable hashtable = new Hashtable();
                                    hashtable.Add("cardEffect", activateClass);
                                    yield return ContinuousController.instance.StartCoroutine(card.UnitContainingThisCharacter().UnTap(hashtable));
                                    yield break;
                                }
                            }
                        }
                    }
                }
            }
        }

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpICardEffect("白夜の夜刀神・終夜","",new List<Cost>(),new List<Func<Hashtable, bool>>() { CanUseCondition },-1,false,card);
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("白夜の夜刀神・終夜", (enemyUnit, Power) => Power + 40, (unit) => unit == card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Dragon), PowerUpByEnemy.Mode.Attacking,card);
        cardEffects.Add(powerUpByEnemy);

        bool CanUseCondition(Hashtable hashtable)
        {
            if (card != null)
            {
                if (card.Owner.BondCards.Count((cardSource) => !cardSource.IsReverse && cardSource.cardColors.Contains(CardColor.Black)) >= 1)
                {
                    return true;
                }
            }

            return false;
        }

        return cardEffects;
    }
}

