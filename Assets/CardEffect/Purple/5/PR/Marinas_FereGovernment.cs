using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Marinas_FereGovernment : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDestroyedDuringBattleAlly)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("て、撤退じゃあ!", "Tch, time to withdraw!", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (hashtable != null)
                {
                    if (hashtable.ContainsKey("Defender"))
                    {
                        if (hashtable["Defender"] is Unit)
                        {
                            Unit unit = (Unit)hashtable["Defender"];

                            if (unit != null)
                            {
                                if (unit.Character != null)
                                {
                                    if (unit.Character == card)
                                    {
                                        if(card.Owner.TrashCards.Contains(card))
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
                yield return ContinuousController.instance.StartCoroutine(new IAddHandCardFromTrash(card, null).AddHandCardFromTrash());
            }
        }

        CanNotAttackClass canNotAttackClass = new CanNotAttackClass();
        canNotAttackClass.SetUpICardEffect("非戦闘員","",new List<Cost>(),new List<Func<Hashtable, bool>>(),-1,false,card);
        canNotAttackClass.SetUpCanNotAttackClass((Attacker) => Attacker == card.UnitContainingThisCharacter() , (Defender) => true);
        cardEffects.Add(canNotAttackClass);

        CanNotBeEvadedClass canNotBeEvadedClass = new CanNotBeEvadedClass();
        canNotBeEvadedClass.SetUpICardEffect("非戦闘員", "",new List<Cost>(), new List<Func<Hashtable, bool>>(), -1, false,card);
        canNotBeEvadedClass.SetUpCanNotBeEvadedClass((Attacker) => true, (Defender) => Defender == card.UnitContainingThisCharacter());
        cardEffects.Add(canNotBeEvadedClass);

        return cardEffects;
    }

}
