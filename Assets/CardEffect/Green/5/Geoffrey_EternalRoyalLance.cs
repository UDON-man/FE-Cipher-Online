using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Geoffrey_EternalRoyalLance : CEntity_Effect
{
    List<Unit> LevelUpGrownUnits = new List<Unit>();
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnLevelUpAnyone || timing == EffectTiming.OnGrowAnyone)
        {
            activateClass[0].SetUpICardEffect("王宮騎士団長", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, true);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass[0].SetCCS(card.UnitContainingThisCharacter());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[1].EffectName = "Commander of the Royal Guards";
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

                            if (Unit.Character.Owner == this.card.Owner)
                            {
                                if (Unit != this.card.UnitContainingThisCharacter())
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
                    CanTargetCondition: (unit) => unit.Character.Owner != this.card.Owner && unit.Character.Owner.GetBackUnits().Contains(unit),
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

            activateClass[1].SetUpICardEffect("", new List<Cost>(), new List<Func<Hashtable, bool>>(), -1, false);
            activateClass[1].SetUpActivateClass((hashtable) => ActivateCoroutine1(hashtable));
            activateClass[1].isNotCheck_Effect = true;
            cardEffects.Add(activateClass[1]);

            IEnumerator ActivateCoroutine1(Hashtable hashtable)
            {
                if (hashtable != null)
                {
                    if (hashtable.ContainsKey("Unit"))
                    {
                        if (hashtable["Unit"] is Unit)
                        {
                            Unit Unit = (Unit)hashtable["Unit"];

                            if (Unit.Character.Owner == this.card.Owner)
                            {
                                if (Unit != this.card.UnitContainingThisCharacter())
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
            activateClass[2].SetUpICardEffect("", new List<Cost>(), new List<Func<Hashtable, bool>>() , -1, false);
            activateClass[2].SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass[2].isNotCheck_Effect = true;
            cardEffects.Add(activateClass[2]);

            IEnumerator ActivateCoroutine()
            {
                LevelUpGrownUnits = new List<Unit>();
                yield return null;
            }
        }

        PowerUpClass powerUpClass = new PowerUpClass();
        powerUpClass.SetUpICardEffect("女王に栄光あれ!", new List<Cost>(), new List<Func<Hashtable, bool>>(), -1, false);
        powerUpClass.SetUpPowerUpClass(GetPower,(unit) => card.Owner.GetFrontUnits().Contains(unit));
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
        base.Init();
        LevelUpGrownUnits = new List<Unit>();
    }
}