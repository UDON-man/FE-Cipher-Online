using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Flora_FrozenHeart : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if(timing == EffectTiming.OnDestroyedAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("…許しません", "...I won't forgive you", new List<Cost>() , new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, true, card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if(IsExistOnField(hashtable, card))
                {
                    if(hashtable != null)
                    {
                        if(hashtable.ContainsKey("Unit"))
                        {
                            if(hashtable["Unit"] is Unit)
                            {
                                Unit Unit = (Unit)hashtable["Unit"];

                                if(Unit != null)
                                {
                                    if(Unit.Character != null)
                                    {
                                        if(Unit.Character == card.Owner.Lord.Character)
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
                if(card.UnitContainingThisCharacter().IsTapped)
                {
                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("cardEffect", activateClass);
                    yield return ContinuousController.instance.StartCoroutine(card.UnitContainingThisCharacter().UnTap(hashtable));

                    PowerModifyClass powerUpClass = new PowerModifyClass();
                    powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter(), true);
                    card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);
                }
            }
        }

        else if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("氷の護壁", "Ice Ramparts", new List<Cost>() { new TapCost(), new ReverseCost(3, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { (hashtable) => IsExistOnField(hashtable,card)}, -1, false, card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                if(card.Owner.OrbCards.Count == 0)
                {
                    SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                    selectCardEffect.SetUp(
                        CanTargetCondition: CanTargetCondition,
                        CanTargetCondition_ByPreSelecetedList: null,
                        CanEndSelectCondition: null,
                        CanNoSelect: () => false,
                        SelectCardCoroutine: null,
                        AfterSelectCardCoroutine: null,
                        Message: "Select a card to add to orb.",
                        MaxCount: 1,
                        CanEndNotMax: false,
                        isShowOpponent: true,
                        mode: SelectCardEffect.Mode.AddOrb,
                        root: SelectCardEffect.Root.Trash,
                        CustomRootCardList: null,
                        CanLookReverseCard: true,
                        SelectPlayer: card.Owner,
                        cardEffect: activateClass);

                    yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));

                    bool CanTargetCondition(CardSource cardSource)
                    {
                        foreach (string UnitName in card.Owner.Lord.Character.UnitNames)
                        {
                            if (cardSource.UnitNames.Contains(UnitName))
                            {
                                return true;
                            }
                        }

                        return false;
                    }

                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("cardEffect", activateClass);
                    hashtable.Add("Unit", new Unit(card.UnitContainingThisCharacter().Characters));
                    yield return ContinuousController.instance.StartCoroutine(new IDestroyUnit(card.UnitContainingThisCharacter(), 1, BreakOrbMode.Hand, hashtable).Destroy());
                }
                
            }
        }

        return cardEffects;
    }
}

