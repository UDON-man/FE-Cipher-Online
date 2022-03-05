using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class Misto_HollyBattleGirl : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnCCAnyone)
        {
            activateClass[0].SetUpICardEffect("癒しの風", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Therapeutic Breeze";
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if (hashtable != null)
                {
                    if (hashtable.ContainsKey("Unit"))
                    {
                        if (hashtable["Unit"] is Unit)
                        {
                            Unit CCUnit = (Unit)hashtable["Unit"];

                            return CCUnit.Character == this.card;
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
                    CanLookReverseCard: true);

                yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));
            }
        }

        WeaponChangeClass weaponChangeClass = new WeaponChangeClass();
        weaponChangeClass.SetUpICardEffect("ソニックソード", null, null, -1, false);
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
        rangeUpClass.SetUpICardEffect("ソニックソード", null, null, -1, false);
        rangeUpClass.SetUpRangeUpClass((unit, Range) => { Range.Add(1); Range.Add(2); return Range; }, (unit) => unit == card.UnitContainingThisCharacter());
        rangeUpClass.SetLvS(card.UnitContainingThisCharacter(), 3);
        cardEffects.Add(rangeUpClass);

        return cardEffects;
    }
}


