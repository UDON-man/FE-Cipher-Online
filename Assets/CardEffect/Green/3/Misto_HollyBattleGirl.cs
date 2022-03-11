using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class Misto_HollyBattleGirl : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnCCAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("癒しの風", "Therapeutic Breeze", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
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

                            if(Unit.Character == card)
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                selectCardEffect.SetUp(
                    CanTargetCondition: (cardSource) => !cardSource.UnitNames.Contains("ミスト"),
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    CanNoSelect: () => false,
                    SelectCardCoroutine: null,
                    AfterSelectCardCoroutine: null,
                    Message: "Select a card to add to hand.",
                    MaxCount: 1,
                    CanEndNotMax: false,
                    isShowOpponent: true,
                    mode: SelectCardEffect.Mode.AddHand,
                    root: SelectCardEffect.Root.Trash,
                    CustomRootCardList: null,
                    CanLookReverseCard: true,
                    SelectPlayer: card.Owner,
                    cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));
            }
        }

        WeaponChangeClass weaponChangeClass = new WeaponChangeClass();
        weaponChangeClass.SetUpICardEffect("ソニックソード", "",null, null, -1, false,card);
        weaponChangeClass.SetUpWeaponChangeClass((cardSource, Weapons) => { Weapons.Add(Weapon.MagicBook); return Weapons; }, CanWeaponChangeCondition);
        weaponChangeClass.SetLvS(card.UnitContainingThisCharacter(),3);
        cardEffects.Add(weaponChangeClass);

        bool CanWeaponChangeCondition(CardSource cardSource)
        {
            if (card.UnitContainingThisCharacter() != null)
            {
                if (cardSource == card.UnitContainingThisCharacter().Character)
                {
                    return true;
                }
            }

            return false;
        }

        RangeUpClass rangeUpClass = new RangeUpClass();
        rangeUpClass.SetUpICardEffect("ソニックソード", "", null, null, -1, false, card);
        rangeUpClass.SetUpRangeUpClass((unit, Range) => { Range.Add(1); Range.Add(2); return Range; }, (unit) => unit == card.UnitContainingThisCharacter());
        rangeUpClass.SetLvS(card.UnitContainingThisCharacter(), 3);
        cardEffects.Add(rangeUpClass);

        return cardEffects;
    }
}


