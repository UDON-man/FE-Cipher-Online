using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class LufureOtoko_SaintKingKamiGunshi : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[0].SetUpICardEffect("七色の叫び", new List<Cost>() { new ReverseCost(2, (cardSource) => true), new DiscardHandCost(1, (cardSource) => cardSource.UnitNames.Contains("ルフレ(男)")) }, null, -1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Rally Spectrum";
            }

            IEnumerator ActivateCoroutine()
            {
                foreach (Unit unit in card.Owner.FieldUnit)
                {
                    PowerUpClass powerUpClass = new PowerUpClass();
                    powerUpClass.SetUpPowerUpClass((_unit, Power) => Power + 10, (_unit) => _unit == unit);
                    unit.UntilEachTurnEndUnitEffects.Add(powerUpClass);
                }

                if(card.Owner.OrbCards.Count < card.Owner.Enemy.OrbCards.Count)
                {
                    if (card.Owner.HandCards.Count > 0)
                    {
                        SelectHandEffect selectHandEffect = GetComponent<SelectHandEffect>();

                        selectHandEffect.SetUp(
                            SelectPlayer: card.Owner,
                            CanTargetCondition: (cardSource) => cardSource.Owner.HandCards.Contains(cardSource),
                            CanTargetCondition_ByPreSelecetedList: null,
                            CanEndSelectCondition: null,
                            MaxCount: 1,
                            CanNoSelect: true,
                            CanEndNotMax: false,
                            isShowOpponent: false,
                            SelectCardCoroutine: (cardSource) => SelectCardCoroutine(cardSource),
                            AfterSelectCardCoroutine: null,
                            mode: SelectHandEffect.Mode.Custom);

                        yield return StartCoroutine(selectHandEffect.Activate(null));
                        
                        IEnumerator SelectCardCoroutine(CardSource cardSource)
                        {
                            yield return ContinuousController.instance.StartCoroutine(cardSource.cardOperation.PutOrbFromHand());
                        }
                    }
                }

                yield return null;
            }
        }

        else if(timing == EffectTiming.OnDestroyDuringBattleAlly)
        {
            activateClass[1].SetUpICardEffect("次なる一手", new List<Cost>() { new BreakOrbCost(card.Owner,1,BreakOrbMode.Hand)}, new List<Func<Hashtable, bool>>() { CanUseCondition } , -1, true);
            activateClass[1].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[1]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[1].EffectName = "Two Steps Ahead";
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable))
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                    {
                        return true;
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
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
                        isShowOpponent: false,
                        SelectCardCoroutine: (cardSource) => SelectCardCoroutine(cardSource),
                        AfterSelectCardCoroutine: null,
                        mode: SelectHandEffect.Mode.Custom);

                    yield return StartCoroutine(selectHandEffect.Activate(null));

                    IEnumerator SelectCardCoroutine(CardSource cardSource)
                    {
                        yield return ContinuousController.instance.StartCoroutine(cardSource.cardOperation.PutOrbFromHand());
                    }
                }
            }

        }



        return cardEffects;
    }
}
