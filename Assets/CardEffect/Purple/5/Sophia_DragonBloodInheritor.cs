using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Sophia_DragonBloodInheritor : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            if (card.Owner.Enemy.HandCards.Count > 0)
            {
                activateClass[0].SetUpICardEffect("神竜の巫術", new List<Cost>() { new ReverseCost(3, (cardSource) => true), new PutHandLibraryTopCost(1, (cardSource) => cardSource.UnitNames.Contains("ソフィーヤ"),true) }, null, 1, false);
                activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
                cardEffects.Add(activateClass[0]);

                if (ContinuousController.instance.language == Language.ENG)
                {
                    activateClass[0].EffectName = "Divination of the Divine Dragon";
                }

                IEnumerator ActivateCoroutine()
                {
                    SelectHandEffect selectHandEffect = GetComponent<SelectHandEffect>();

                    int maxCount = 2;

                    if (card.Owner.Enemy.HandCards.Count < maxCount)
                    {
                        maxCount = card.Owner.Enemy.HandCards.Count;
                    }

                    selectHandEffect.SetUp(
                            SelectPlayer: card.Owner.Enemy,
                            CanTargetCondition: (cardSource) => cardSource.Owner.HandCards.Contains(cardSource),
                            CanTargetCondition_ByPreSelecetedList: null,
                            CanEndSelectCondition: null,
                            MaxCount: maxCount,
                            CanNoSelect: false,
                            CanEndNotMax: false,
                            isShowOpponent: true,
                            SelectCardCoroutine: null,
                            AfterSelectCardCoroutine: null,
                            mode: SelectHandEffect.Mode.Discard);

                    yield return ContinuousController.instance.StartCoroutine(selectHandEffect.Activate(null));
                }

            }
        }

        return cardEffects;
    }

    #region 竜血の脈動
    public override List<ICardEffect> SupportEffects(EffectTiming timing)
    {
        List<ICardEffect> supportEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnSetSupport)
        {
            activateClass_Support[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass_Support[0].SetUpICardEffect("竜血の脈動", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
            supportEffects.Add(activateClass_Support[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass_Support[0].EffectName = "Coursing Dragon Blood";
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if (card.Owner.SupportCards.Contains(card))
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit != null)
                    {
                        if (GManager.instance.turnStateMachine.AttackingUnit.Character != null)
                        {
                            if (GManager.instance.turnStateMachine.AttackingUnit.Character.Owner == card.Owner)
                            {
                                if (card.Owner.Enemy.HandCards.Count <= 3)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                PowerUpClass powerUpClass = new PowerUpClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, (unit) => unit == GManager.instance.turnStateMachine.AttackingUnit && unit.Character.Owner == card.Owner);
                GManager.instance.turnStateMachine.AttackingUnit.UntilEndBattleEffects.Add(powerUpClass);

                yield return null;
            }
        }

        return supportEffects;
    }
    #endregion
}