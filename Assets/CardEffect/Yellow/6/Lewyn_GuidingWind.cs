using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Lewyn_GuidingWind : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("シレジアの王子", "Prince of Silesea", null, new List<Func<Hashtable, bool>>() { (hashtable) => GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner}, -1, false, card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10 * card.Owner.FieldUnit.Count((_unit) => _unit != unit && _unit.Weapons.Contains(Weapon.Wing)), (unit) => unit == card.UnitContainingThisCharacter(), true);
        powerUpClass.SetCCS(card.UnitContainingThisCharacter());
        cardEffects.Add(powerUpClass);

        WeaponChangeClass weaponChangeClass = new WeaponChangeClass();
        weaponChangeClass.SetUpICardEffect("風魔法 フォルセティ","Wind Magic, Forseti",null, new List<Func<Hashtable, bool>>() { (hashtable) => GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner }, -1, false, card);
        weaponChangeClass.SetUpWeaponChangeClass(ChangeWeapons, (cardSource) => true);
        weaponChangeClass.SetCCS(card.UnitContainingThisCharacter());
        cardEffects.Add(weaponChangeClass);

        List<Weapon> ChangeWeapons(CardSource cardSource, List<Weapon> Weapons)
        {
            if (cardSource.UnitContainingThisCharacter() != null)
            {
                if (cardSource.UnitContainingThisCharacter().Character == cardSource)
                {
                    if (cardSource.Owner == card.Owner)
                    {
                        Weapons.Add(Weapon.Wing);
                    }
                }
            }

            return Weapons;
        }

        AddSkillClass addSkillClass = new AddSkillClass();
        addSkillClass.SetUpICardEffect("風の申し子","Heavenly Child",new List<Cost>(),new List<Func<Hashtable, bool>>(),-1,false,card);
        addSkillClass.SetUpAddSkillClass((cardSource) => true, GetEffects);
        cardEffects.Add(addSkillClass);

        List<ICardEffect> GetEffects(CardSource cardSource, List<ICardEffect> GetCardEffects,EffectTiming _timing)
        {
            if (_timing == EffectTiming.OnDeclaration)
            {
                if (cardSource.UnitContainingThisCharacter() != null)
                {
                    if (cardSource.UnitContainingThisCharacter().Character == cardSource)
                    {
                        if (cardSource.Owner == card.Owner)
                        {
                            if (cardSource.Weapons.Contains(Weapon.Wing))
                            {
                                if (GetCardEffects.Count((cardEffect) => cardEffect.EffectName == "天空の運び手") == 0)
                                {
                                    ActivateClass activateClass = new ActivateClass();
                                    activateClass.SetUpICardEffect("天空の運び手", "Winged Deliverer", new List<Cost>() { new TapCost() }, null, -1, false, cardSource);
                                    activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
                                    GetCardEffects.Add(activateClass);

                                    IEnumerator ActivateCoroutine()
                                    {
                                        SelectUnitEffect selectUnitEffect = cardSource.GetComponent<SelectUnitEffect>();

                                        selectUnitEffect.SetUp(
                                            SelectPlayer: cardSource.Owner,
                                            CanTargetCondition: (unit) => unit.Character.Owner == cardSource.Owner && unit != cardSource.UnitContainingThisCharacter(),
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
                                }
                            }
                        }
                    }
                }
            }
                
            
            return GetCardEffects;
        }

        return cardEffects;
    }
}
