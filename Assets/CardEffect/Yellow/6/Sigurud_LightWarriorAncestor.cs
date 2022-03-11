using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Sigurud_LightWarriorAncestor : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnCriticalAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("闇を照らす戦士", "Darkness-Illuminating Warrior",new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition1 }, 1, true,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition1(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable,card))
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit != null)
                    {
                        if (GManager.instance.turnStateMachine.AttackingUnit.Character != null)
                        {
                            if (GManager.instance.turnStateMachine.AttackingUnit.Character.Owner == card.Owner)
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
                    CanTargetCondition: CanTargetCondition,
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    CanNoSelect: () => false,
                    SelectCardCoroutine: null,
                    AfterSelectCardCoroutine: null,
                    Message: "Select a card in your retreat area to add to bond.",
                    MaxCount: 1,
                    CanEndNotMax: false,
                    isShowOpponent: true,
                    mode: SelectCardEffect.Mode.SetFaceBond,
                    root: SelectCardEffect.Root.Trash,
                    CustomRootCardList: null,
                    CanLookReverseCard: true,
                    SelectPlayer: card.Owner,
                    cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));

                bool CanTargetCondition(CardSource cardSource)
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit != null)
                    {
                        if (GManager.instance.turnStateMachine.AttackingUnit.Character != null)
                        {
                            if (GManager.instance.turnStateMachine.AttackingUnit.Character.Owner == card.Owner)
                            {
                                foreach(string UnitName in GManager.instance.turnStateMachine.AttackingUnit.Character.UnitNames)
                                {
                                    if(cardSource.UnitNames.Contains(UnitName))
                                    {
                                        if(cardSource.CanSetBondThisCard)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    return false;
                }
            }
        }

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("聖剣 ティルフィング", "",new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter(), true);
        cardEffects.Add(powerUpClass);

        StrikeModifyClass strikeModifyClass = new StrikeModifyClass();
        strikeModifyClass.SetUpICardEffect("聖剣 ティルフィング", "", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false, card);
        strikeModifyClass.SetUpStrikeModifyClass((unit, Strike) => 2, (unit) => unit == card.UnitContainingThisCharacter(),false);
        cardEffects.Add(strikeModifyClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if (card.Owner.BondCards.Count >= 8)
            {
                return true;
            }

            return false;
        }

        return cardEffects;
    }
}
