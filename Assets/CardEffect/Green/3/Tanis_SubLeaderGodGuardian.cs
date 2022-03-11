using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Tanis_SubLeaderGodGuardian : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("鬼の副長", "",new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter(), true);
        cardEffects.Add(powerUpClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if (card.Owner.FieldUnit.Count((_unit) => _unit != card.UnitContainingThisCharacter() && _unit.Weapons.Contains(Weapon.Wing)) >= 2)
            {
                return true;
            }

            return false;
        }

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("援軍", "Reinforce", new List<Cost>() { new TapCost() }, null, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine1());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine1()
            {
                if (card.Owner.HandCards.Count > 0)
                {
                    SelectHandEffect selectHandEffect = GetComponent<SelectHandEffect>();

                    selectHandEffect.SetUp(
                        SelectPlayer: card.Owner,
                        CanTargetCondition: CanSelectCondition,
                        CanTargetCondition_ByPreSelecetedList: null,
                        CanEndSelectCondition: null,
                        MaxCount: 1,
                        CanNoSelect: true,
                        CanEndNotMax: false,
                        isShowOpponent: true,
                        SelectCardCoroutine: null,
                        AfterSelectCardCoroutine: null,
                        mode: SelectHandEffect.Mode.Deploy,
                        cardEffect: activateClass);

                    bool CanSelectCondition(CardSource cardSource)
                    {
                        if (cardSource.Owner.HandCards.Contains(cardSource))
                        {
                            if (cardSource.CanPlayAsNewUnit())
                            {
                                if (cardSource.PlayCost <= 2)
                                {
                                    if(cardSource.Weapons.Contains(Weapon.Wing))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }

                        return false;
                    }

                    yield return StartCoroutine(selectHandEffect.Activate(null));
                }
            }
        }

        return cardEffects;
    }

}