using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Suzukaze_GuardKazenin : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        SupportPowerUpClass supportPowerUpClass = new SupportPowerUpClass();
        supportPowerUpClass.SetUpICardEffect("影の務め", null, null, -1, false);
        supportPowerUpClass.SetUpSupportPowerUpClass((cardSource, SupportPower) => SupportPower + 10, ChangeSupportPowerCondition);
        supportPowerUpClass.SetCCS(card.UnitContainingThisCharacter());
        cardEffects.Add(supportPowerUpClass);

        bool ChangeSupportPowerCondition(CardSource cardSource)
        {
            if (GManager.instance.turnStateMachine.AttackingUnit != null && GManager.instance.turnStateMachine.DefendingUnit != null)
            {
                if (GManager.instance.turnStateMachine.AttackingUnit.Character.Owner == card.Owner || GManager.instance.turnStateMachine.DefendingUnit.Character.Owner == card.Owner)
                {
                    if (card.UnitContainingThisCharacter() != GManager.instance.turnStateMachine.AttackingUnit && card.UnitContainingThisCharacter() != GManager.instance.turnStateMachine.DefendingUnit)
                    {
                        if (cardSource.Weapons.Contains(Weapon.DarkWeapon))
                        {
                            if (cardSource.Owner == card.Owner)
                            {
                                return true;
                            }
                        }
                    }
                }
            }


            return false;
        }

        if (timing == EffectTiming.OnEnterFieldAnyone)
        {
            activateClass[0].SetUpICardEffect("疾風の陣", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Gale Formation";
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

                            if (Unit != null)
                            {
                                if (Unit.Character != null)
                                {
                                    if (Unit.Character.Owner == this.card.Owner)
                                    {
                                        if (Unit == card.UnitContainingThisCharacter()||Unit.Weapons.Contains(Weapon.DarkWeapon))
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
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                SelectPlayer: card.Owner,
                CanTargetCondition: (unit) => unit.Character.Owner == this.card.Owner,
                CanTargetCondition_ByPreSelecetedList: null,
                CanEndSelectCondition: null,
                MaxCount: card.Owner.FieldUnit.Count,
                CanNoSelect: true,
                CanEndNotMax: true,
                SelectUnitCoroutine: null,
                AfterSelectUnitCoroutine: null,
                mode: SelectUnitEffect.Mode.Move);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));
            }
        }

        return cardEffects;
    }
}