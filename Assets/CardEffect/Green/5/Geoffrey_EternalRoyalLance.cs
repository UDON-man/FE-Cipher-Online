using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Geoffrey_EternalRoyalLance : CEntity_Effect
{
    List<Unit> LevelUpGrownUnits = new List<Unit>();
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnLevelUpAnyone || timing == EffectTiming.OnGrowAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("王宮騎士団長", "Commander of the Royal Guards", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, true,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass.SetCCS(card.UnitContainingThisCharacter());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (hashtable != null)
                {
                    if (hashtable.ContainsKey("Unit"))
                    {
                        if (hashtable["Unit"] is Unit)
                        {
                            Unit Unit = (Unit)hashtable["Unit"];

                            if (Unit.Character.Owner == card.Owner)
                            {
                                if (Unit != card.UnitContainingThisCharacter())
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
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner != card.Owner && unit.Character.Owner.GetBackUnits().Contains(unit),
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: false,
                    CanEndNotMax: false,
                    SelectUnitCoroutine: null,
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Move,
                    cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));
            }

            ActivateClass activateClass1 = new ActivateClass();
            activateClass1.SetUpICardEffect("","", new List<Cost>(), new List<Func<Hashtable, bool>>(), -1, false,card);
            activateClass1.SetUpActivateClass((hashtable) => ActivateCoroutine1(hashtable));
            activateClass1.isNotCheck_Effect = true;
            cardEffects.Add(activateClass1);

            IEnumerator ActivateCoroutine1(Hashtable hashtable)
            {
                if (hashtable != null)
                {
                    if (hashtable.ContainsKey("Unit"))
                    {
                        if (hashtable["Unit"] is Unit)
                        {
                            Unit Unit = (Unit)hashtable["Unit"];

                            if (Unit.Character.Owner == card.Owner)
                            {
                                if (Unit != card.UnitContainingThisCharacter())
                                {
                                    if(!LevelUpGrownUnits.Contains(Unit))
                                    {
                                        LevelUpGrownUnits.Add(Unit);
                                    }
                                }
                            }
                        }
                    }
                }

                yield return null;
            }
        }

        else if(timing == EffectTiming.OnEndTurn)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("","", new List<Cost>(), new List<Func<Hashtable, bool>>() , -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass.isNotCheck_Effect = true;
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                LevelUpGrownUnits = new List<Unit>();
                yield return null;
            }
        }

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("女王に栄光あれ!","", new List<Cost>(), new List<Func<Hashtable, bool>>(), -1, false,card);
        powerUpClass.SetUpPowerUpClass(GetPower,(unit) => card.Owner.GetFrontUnits().Contains(unit), true);
        cardEffects.Add(powerUpClass);

        int GetPower(Unit unit,int Power)
        {
            LevelUpGrownUnits = LevelUpGrownUnits.Distinct().ToList();

            Power += 10 * LevelUpGrownUnits.Count((_unit) => card.Owner.FieldUnit.Contains(_unit));

            return Power;
        }

        return cardEffects;
    }

    public override void Init()
    {
        LevelUpGrownUnits = new List<Unit>();
    }
}