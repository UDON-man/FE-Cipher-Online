using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class MarkOnnna_GirlFromAnotherWorld : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        WeaponChangeClass weaponChangeClass = new WeaponChangeClass();
        weaponChangeClass.SetUpICardEffect("すべての可能性", new List<Cost>(), new List<Func<Hashtable, bool>>() , -1, false);
        weaponChangeClass.SetUpWeaponChangeClass(ChangeWeapon, (cardSource) => cardSource == card);
        cardEffects.Add(weaponChangeClass);

        List<Weapon> ChangeWeapon(CardSource cardSource, List<Weapon> Weapons)
        {
            if(cardSource == card)
            {
                Weapons.Add(Weapon.Lance);
                Weapons.Add(Weapon.Axe);
                Weapons.Add(Weapon.Bow);
                Weapons.Add(Weapon.MagicBook);
                Weapons.Add(Weapon.Rod);
                Weapons.Add(Weapon.DragonStone);
                Weapons.Add(Weapon.Beast);
            }

            Weapons = Weapons.Distinct().ToList();

            return Weapons;
        }

        
        return cardEffects;
    }

    #region 計略の紋章
    public override List<ICardEffect> SupportEffects(EffectTiming timing)
    {
        List<ICardEffect> supportEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnSetSupport)
        {
            activateClass_Support[0].SetUpICardEffect("計略の紋章", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true);
            activateClass_Support[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            supportEffects.Add(activateClass_Support[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass_Support[0].EffectName = "Tactical Emblem";
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
                                if (GManager.instance.turnStateMachine.AttackingUnit.Character.cardColors.Contains(CardColor.Blue))
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
                    CanTargetCondition: (unit) => unit.Character.Owner != card.Owner && unit != GManager.instance.turnStateMachine.DefendingUnit,
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: true,
                    CanEndNotMax: false,
                    SelectUnitCoroutine: null,
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Move);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));
            }
        }

        return supportEffects;
    }
    #endregion
}
