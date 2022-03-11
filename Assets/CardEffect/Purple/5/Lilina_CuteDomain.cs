using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Lilina_CuteDomain : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("業火の理 フォルブレイズ", "Infernal Element: Forblaze", new List<Cost>() {new DiscardHandCost(1, (cardSource) => cardSource.UnitNames.Contains("リリーナ")) }, null, 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine1());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine1()
            {
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner != card.Owner && unit != unit.Character.Owner.Lord && unit.Weapons.Contains(Weapon.Dragon),
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: true,
                    CanEndNotMax: true,
                    SelectUnitCoroutine: null,
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Destroy,
                    cardEffect:activateClass);

                yield return StartCoroutine(selectUnitEffect.Activate(null));

                PowerModifyClass powerUpClass = new PowerModifyClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 60, (unit) => unit == card.UnitContainingThisCharacter(), true);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);
            }
        }

        else if (timing == EffectTiming.OnDiscardHand)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("大魔導士の素質", "The Making of a Great Sorcerer", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, true,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (card.UnitContainingThisCharacter() != null)
                {
                    if (card.Owner.FieldUnit.Contains(card.UnitContainingThisCharacter()))
                    {
                        if (hashtable != null)
                        {
                            #region 自分が手札を捨てた場合
                            if (hashtable.Contains("Card"))
                            {
                                if (hashtable["Card"] is CardSource)
                                {
                                    CardSource cardSource = (CardSource)hashtable["Card"];

                                    if (cardSource != null)
                                    {
                                        if (cardSource.Owner == card.Owner)
                                        {
                                            #region スキルの効果で捨てられた場合
                                            if (hashtable.ContainsKey("cardEffect"))
                                            {
                                                if (hashtable["cardEffect"] is ICardEffect)
                                                {
                                                    ICardEffect cardEffect = (ICardEffect)hashtable["cardEffect"];

                                                    if (cardEffect != null)
                                                    {
                                                        return true;
                                                    }

                                                }
                                            }
                                            #endregion

                                            #region コストで捨てられた場合
                                            if (hashtable.Contains("isCost"))
                                            {
                                                if (hashtable["isCost"] is bool)
                                                {
                                                    bool isCost = (bool)hashtable["isCost"];

                                                    if (isCost)
                                                    {
                                                        return true;
                                                    }
                                                }
                                            }
                                            #endregion
                                        }
                                    }
                                }
                            }
                            #endregion

                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                yield return ContinuousController.instance.StartCoroutine(new IDraw(card.Owner, 1).Draw());
            }
        }

        return cardEffects;
    }
}