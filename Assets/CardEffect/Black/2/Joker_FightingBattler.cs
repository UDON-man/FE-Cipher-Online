using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Joker_FightingBattler : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("どうぞこちらへ", "By your leave.", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>(), 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine0());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine0()
            {
                Hashtable hashtable = new Hashtable();
                hashtable.Add("cardEffect", activateClass);

                yield return ContinuousController.instance.StartCoroutine(new IMoveUnit(new List<Unit>() { card.Owner.Lord }, true,hashtable).MoveUnits());
            }

            ActivateClass activateClass1 = new ActivateClass();
            activateClass1.SetUpICardEffect("すぐに手当てを", "Are you hurt?", new List<Cost>() { new ReverseCost(2, (cardSource) => true) }, new List<Func<Hashtable, bool>>(), 1, false,card);
            activateClass1.SetUpActivateClass((hashtable) => ActivateCoroutine1());
            cardEffects.Add(activateClass1);

            IEnumerator ActivateCoroutine1()
            {
                SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                selectCardEffect.SetUp(
                    CanTargetCondition: CanTargetCondition,
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

                bool CanTargetCondition(CardSource cardSource)
                {
                    foreach(string UnitName in cardSource.UnitNames)
                    {
                        if(card.Owner.Lord.Character.UnitNames.Contains(UnitName))
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }

            ActivateClass activateClass2 = new ActivateClass();
            activateClass2.SetUpICardEffect("貴方のために", "After you.", new List<Cost>() { new ReverseCost(3, (cardSource) => true) }, new List<Func<Hashtable, bool>>(), 1, false,card);
            activateClass2.SetUpActivateClass((hashtable) => ActivateCoroutine2());
            cardEffects.Add(activateClass2);

            IEnumerator ActivateCoroutine2()
            {
               if(card.Owner.Lord.DoneAttackThisTurn)
               {
                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("cardEffect", activateClass2);
                    yield return ContinuousController.instance.StartCoroutine(card.Owner.Lord.UnTap(hashtable));
                    yield return new WaitForSeconds(0.2f);
               }
            }
        }

        return cardEffects;
    }

    
}
