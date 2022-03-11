using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Leon_DarknessPrince : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("神器 ブリュンヒルデ", "Brynhildr", new List<Cost>() { new ReverseCost(2, (cardSource) => true), new DiscardHandCost(1, (cardSource) => cardSource.UnitNames.Contains("レオン")) }, null, 1, false, card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                if(card.Owner.Enemy.HandCards.Count == 0)
                {
                    yield break;
                }

                #region 捨てる手札をランダムに決定
                int discardCount = 2;

                if(card.Owner.Enemy.HandCards.Count < discardCount)
                {
                    discardCount = card.Owner.Enemy.HandCards.Count;
                }

                List<CardSource> DiscardCards = new List<CardSource>();

                while(DiscardCards.Count < discardCount)
                {
                    CardSource cardSource = card.Owner.Enemy.HandCards[UnityEngine.Random.Range(0, card.Owner.Enemy.HandCards.Count)];

                    if(!DiscardCards.Contains(cardSource))
                    {
                        DiscardCards.Add(cardSource);
                    }
                }
                #endregion

                ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(DiscardCards, "Discarded Cards", true));

                yield return new WaitForSeconds(0.5f);

                foreach(CardSource cardSource in DiscardCards)
                {
                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("cardEffect", activateClass);

                    yield return StartCoroutine(cardSource.cardOperation.DiscardFromHand(hashtable));
                }
            }
        }

        else if(timing == EffectTiming.OnDiscardHand)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("神蝕む闇", "Soulcrushing Darkness",new List<Cost>() ,new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false, card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine(hashtable));
            activateClass.SetCCS(card.UnitContainingThisCharacter());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if(card.UnitContainingThisCharacter() != null)
                {
                    if(card.Owner.FieldUnit.Contains(card.UnitContainingThisCharacter()))
                    {
                        if(hashtable != null)
                        {
                            if (hashtable.ContainsKey("cardEffect"))
                            {
                                if (hashtable["cardEffect"] is ICardEffect)
                                {
                                    ICardEffect cardEffect = (ICardEffect)hashtable["cardEffect"];

                                    if (cardEffect.card() != null)
                                    {
                                        if (cardEffect.card() == card)
                                        {
                                            if (hashtable.ContainsKey("Card"))
                                            {
                                                if (hashtable["Card"] is CardSource)
                                                {
                                                    CardSource cardSource = (CardSource)hashtable["Card"];

                                                    if (cardSource != null)
                                                    {
                                                        foreach(Unit unit in card.Owner.Enemy.FieldUnit)
                                                        {
                                                            foreach(string UnitName in unit.Character.UnitNames)
                                                            {
                                                                if(cardSource.UnitNames.Contains(UnitName))
                                                                {
                                                                    return true;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine(Hashtable hashtable)
            {
                if (hashtable != null)
                {
                    if (hashtable.ContainsKey("Card"))
                    {
                        if (hashtable["Card"] is CardSource)
                        {
                            CardSource cardSource = (CardSource)hashtable["Card"];

                            if(cardSource != null)
                            {
                                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                                selectUnitEffect.SetUp(
                                    SelectPlayer: card.Owner,
                                    CanTargetCondition: CanTargetCondtion,
                                    CanTargetCondition_ByPreSelecetedList: null,
                                    CanEndSelectCondition: null,
                                    MaxCount: 1,
                                    CanNoSelect: false,
                                    CanEndNotMax: false,
                                    SelectUnitCoroutine: (unit) => SelectUnitCoroutine(unit),
                                    AfterSelectUnitCoroutine: null,
                                    mode: SelectUnitEffect.Mode.Custom,
                                    cardEffect: activateClass);

                                yield return StartCoroutine(selectUnitEffect.Activate(null));

                                bool CanTargetCondtion(Unit unit)
                                {
                                    if (unit.Character.Owner != card.Owner)
                                    {
                                        foreach (string UnitName in unit.Character.UnitNames)
                                        {
                                            if (cardSource.UnitNames.Contains(UnitName))
                                            {
                                                return true;
                                            }
                                        }
                                    }

                                    return false;
                                }

                                IEnumerator SelectUnitCoroutine(Unit unit)
                                {
                                    PowerModifyClass powerUpClass = new PowerModifyClass();
                                    powerUpClass.SetUpPowerUpClass((_unit, Power) => Power - 20, (_unit) => _unit == unit, true);
                                    unit.UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);

                                    yield return null;
                                }
                            }
                        }
                    }
                }
            }
        }

        return cardEffects;
    }
}