using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Joker_FightingBattler : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[0].SetUpICardEffect("どうぞこちらへ", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>(), 1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine0());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "By your leave.";
            }

            IEnumerator ActivateCoroutine0()
            {
                Hashtable hashtable = new Hashtable();
                hashtable.Add("cardEffect", activateClass[0]);

                yield return ContinuousController.instance.StartCoroutine(new IMoveUnit(new List<Unit>() { card.Owner.Lord }, true,hashtable).MoveUnits());
            }


            activateClass[1].SetUpICardEffect("すぐに手当てを", new List<Cost>() { new ReverseCost(2, (cardSource) => true) }, new List<Func<Hashtable, bool>>(), 1, false);
            activateClass[1].SetUpActivateClass((hashtable) => ActivateCoroutine1());
            cardEffects.Add(activateClass[1]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[1].EffectName = "Are you hurt?";
            }

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
                    CanLookReverseCard: true);

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

            activateClass[2].SetUpICardEffect("貴方のために", new List<Cost>() { new ReverseCost(3, (cardSource) => true) }, new List<Func<Hashtable, bool>>(), 1, false);
            activateClass[2].SetUpActivateClass((hashtable) => ActivateCoroutine2());
            cardEffects.Add(activateClass[2]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[2].EffectName = "After you.";
            }

            IEnumerator ActivateCoroutine2()
            {
               if(card.Owner.Lord.DoneAttackThisTurn)
               {
                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("cardEffect", activateClass[2]);
                    yield return ContinuousController.instance.StartCoroutine(card.Owner.Lord.UnTap(hashtable));
                    yield return new WaitForSeconds(0.2f);
               }
            }
        }

        return cardEffects;
    }

    
}
