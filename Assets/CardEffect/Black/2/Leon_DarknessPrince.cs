using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Leon_DarknessPrince : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[0].SetUpICardEffect("神器 ブリュンヒルデ", new List<Cost>() { new ReverseCost(2, (cardSource) => true), new DiscardHandCost(1, (cardSource) => cardSource.UnitNames.Contains("レオン")) }, null, 1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Brynhildr";
            }

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
                    hashtable.Add("cardEffect", activateClass[0]);

                    yield return StartCoroutine(cardSource.cardOperation.DiscardFromHand(hashtable));
                }
            }
        }

        else if(timing == EffectTiming.OnDiscardHand)
        {
            activateClass[1].SetUpICardEffect("神蝕む闇", new List<Cost>() ,new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
            activateClass[1].SetUpActivateClass((hashtable) => ActivateCoroutine(hashtable));
            activateClass[1].SetCCS(card.UnitContainingThisCharacter());
            cardEffects.Add(activateClass[1]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[1].EffectName = "Soulcrushing Darkness";
            }

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
                                        if (cardEffect.card() == this.card)
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
                                    mode: SelectUnitEffect.Mode.Custom);

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
                                    PowerUpClass powerUpClass = new PowerUpClass();
                                    powerUpClass.SetUpPowerUpClass((_unit, Power) => Power - 20, (_unit) => _unit == unit);
                                    unit.UntilEachTurnEndUnitEffects.Add(powerUpClass);

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