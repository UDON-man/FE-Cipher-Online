using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Rutger_RedFurySwordman : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDestroyDuringBattleAlly)
        {
            activateClass[0].SetUpICardEffect("血旋風", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Blood Whirlwind";

            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable))
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                    {
                        if(hashtable.ContainsKey("Defender"))
                        {
                            if(hashtable["Defender"] is Unit)
                            {
                                Unit unit = (Unit)hashtable["Defender"];

                                if(unit != null)
                                {
                                    if(unit.Character != null)
                                    {
                                        if(unit.Character != card.Owner.Enemy.Lord.Character)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                        
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                Hashtable hashtable = new Hashtable();
                hashtable.Add("cardEffect", activateClass[0]);
                yield return ContinuousController.instance.StartCoroutine(card.UnitContainingThisCharacter().UnTap(hashtable));

                yield return new WaitForSeconds(0.2f);
            }
        }

        else if (timing == EffectTiming.OnUnTappedAnyone)
        {
            activateClass[1].SetUpICardEffect("死神の誘い", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true);
            activateClass[1].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[1]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[1].EffectName = "Death Temptation";
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if (hashtable != null)
                {
                    if (hashtable.ContainsKey("Unit"))
                    {
                        if (hashtable["Unit"] is Unit)
                        {
                            Unit Unit = (Unit)hashtable["Unit"];

                            if (Unit.Character != null)
                            {
                                if(Unit.Character == this.card)
                                {
                                    if(hashtable.ContainsKey("cardEffect"))
                                    {
                                        if(hashtable["cardEffect"] is ICardEffect)
                                        {
                                            ICardEffect cardEffect = (ICardEffect)hashtable["cardEffect"];

                                            if(cardEffect != null)
                                            {
                                                if(cardEffect.card() == this.card)
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

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner != this.card.Owner && unit.Character.Owner.GetBackUnits().Contains(unit) && unit.Power >= 70,
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: false,
                    CanEndNotMax: false,
                    SelectUnitCoroutine: null,
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Move);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));
            }
        }

        return cardEffects;
    }
}




