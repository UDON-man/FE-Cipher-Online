using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class Lilina_OstiaPrincess : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("秘めた炎","", null, null, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, CanPowerUpCondition, true);
        cardEffects.Add(powerUpClass);

        bool CanPowerUpCondition(Unit unit)
        {
            if(IsExistOnField(null,card))
            {
                if (card.UnitContainingThisCharacter() == unit)
                {
                    if (GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
                    {
                        if (card.Owner.FieldUnit.Count((_unit) => _unit != unit && _unit.Character.cEntity_EffectController.GetAllSupportEffects().Count((cardEffect) => !cardEffect.IsInvalidate) > 0) >= 2)
                        {
                            return true;
                        }
                    }
                }
            }
            
            return false;
        }

        return cardEffects;
    }

    #region 才姫の魔術
    public override List<ICardEffect> SupportEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> supportEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnSetSupport)
        {
            ActivateClass activateClass_Support = new ActivateClass();
            activateClass_Support.SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass_Support.SetUpICardEffect("才姫の魔術","Talented Princess's Magic", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
            supportEffects.Add(activateClass_Support);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (card.Owner.SupportCards.Contains(card))
                {
                    if (GManager.instance.turnStateMachine.DefendingUnit != null)
                    {
                        if (GManager.instance.turnStateMachine.DefendingUnit.Character != null)
                        {
                            if (GManager.instance.turnStateMachine.DefendingUnit.Character.Owner == card.Owner)
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                yield return ContinuousController.instance.StartCoroutine(new IDraw(card.Owner, 1).Draw());

                if (card.Owner.HandCards.Count > 0)
                {
                    SelectHandEffect selectHandEffect = GetComponent<SelectHandEffect>();

                    selectHandEffect.SetUp(
                        SelectPlayer: card.Owner,
                        CanTargetCondition: (cardSource) => cardSource.Owner.HandCards.Contains(cardSource),
                        CanTargetCondition_ByPreSelecetedList: null,
                        CanEndSelectCondition: null,
                        MaxCount: 1,
                        CanNoSelect: false,
                        CanEndNotMax: false,
                        isShowOpponent: true,
                        SelectCardCoroutine: null,
                        AfterSelectCardCoroutine: null,
                        mode: SelectHandEffect.Mode.Discard,
                        cardEffect: activateClass_Support);

                    yield return StartCoroutine(selectHandEffect.Activate(null));
                }
            }
        }


        return supportEffects;
    }
    #endregion
}