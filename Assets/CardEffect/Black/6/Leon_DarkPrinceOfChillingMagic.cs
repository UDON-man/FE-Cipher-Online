using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Leon_DarkPrinceOfChillingMagic : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("畏怖", "Terrorizing Fear", new List<Cost>() { new TapCost(), new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (card.Owner.OrbCards.Count((cardSource) => cardSource.IsReverse) > 0)
                {
                    return true;
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                if (card.Owner.OrbCards.Count((cardSource) => cardSource.IsReverse) > 0)
                {
                    SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                    selectCardEffect.SetUp(
                        CanTargetCondition: (cardSource) => cardSource.Owner == card.Owner && cardSource.IsReverse,
                        CanTargetCondition_ByPreSelecetedList: null,
                        CanEndSelectCondition: null,
                        CanNoSelect: () => false,
                        SelectCardCoroutine: null,
                        AfterSelectCardCoroutine: null,
                        Message: "Select a orb card to turn face up.",
                        MaxCount: 1,
                        CanEndNotMax: false,
                        isShowOpponent: true,
                        mode: SelectCardEffect.Mode.SetFace,
                        root: SelectCardEffect.Root.Orb,
                        CustomRootCardList: null,
                        CanLookReverseCard: false,
                        SelectPlayer: card.Owner,
                        cardEffect: activateClass);

                    yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));

                    SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                    selectUnitEffect.SetUp(
                        SelectPlayer: card.Owner,
                        CanTargetCondition: (unit) => unit.Character.Owner != card.Owner && unit != unit.Character.Owner.Lord,
                        CanTargetCondition_ByPreSelecetedList: null,
                        CanEndSelectCondition: null,
                        MaxCount: 1,
                        CanNoSelect: true,
                        CanEndNotMax: false,
                        SelectUnitCoroutine: (unit) => SelectUnitCoroutine(unit),
                        AfterSelectUnitCoroutine: null,
                        mode: SelectUnitEffect.Mode.Custom,
                        cardEffect: activateClass);

                    yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));

                    IEnumerator SelectUnitCoroutine(Unit targetUnit)
                    {
                        if(targetUnit != null)
                        {
                            if(targetUnit.Character != null)
                            {
                                List<string> DestroyUnitNames = new List<string>();

                                foreach (string UnitName in targetUnit.Character.UnitNames)
                                {
                                    DestroyUnitNames.Add(UnitName);
                                }

                                Hashtable hashtable = new Hashtable();
                                hashtable.Add("cardEffect", selectUnitEffect);
                                hashtable.Add("Unit", new Unit(targetUnit.Characters));
                                IDestroyUnit destroyUnit = new IDestroyUnit(targetUnit, 1, BreakOrbMode.Hand, hashtable);

                                yield return ContinuousController.instance.StartCoroutine(destroyUnit.Destroy());

                                if (destroyUnit.Destroyed)
                                {
                                    CanNotPlayClass canNotPlayClass = new CanNotPlayClass();
                                    canNotPlayClass.SetUpCanNotPlayClass(CanNotPlayCondition);
                                    card.Owner.UntilOpponentTurnEndEffects.Add((_timing) => canNotPlayClass);

                                    bool CanNotPlayCondition(CardSource cardSource)
                                    {
                                        foreach (string UnitName in DestroyUnitNames)
                                        {
                                            if (cardSource.UnitNames.Contains(UnitName))
                                            {
                                                return true;
                                            }
                                        }

                                        return false;
                                    }
                                }
                            }
                        }
                        
                    }
                }

                yield return null;
            }
        }

        else if (timing == EffectTiming.OnEndTurn)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("冷たき瘴気", "Chiiling Miasma", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if(IsExistOnField(null,card))
                {
                    if (card.Owner.OrbCards.Count((cardSource) => !cardSource.IsReverse) > 0)
                    {
                        if(GManager.instance.turnStateMachine.gameContext.Players_ForTurnPlayer.Count((player) => player.HandCards.Count >= 5) > 0)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                foreach(Player player in GManager.instance.turnStateMachine.gameContext.Players_ForTurnPlayer)
                {
                    if (player.HandCards.Count >= 5)
                    {
                        yield return new WaitForSeconds(0.7f);

                        SelectHandEffect selectHandEffect = GetComponent<SelectHandEffect>();

                        selectHandEffect.SetUp(
                            SelectPlayer: player,
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
                            cardEffect: activateClass);

                        yield return StartCoroutine(selectHandEffect.Activate(null));
                    }
                }
            }
        }

        return cardEffects;
    }
}
