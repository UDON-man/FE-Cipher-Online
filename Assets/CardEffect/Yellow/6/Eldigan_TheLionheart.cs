using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Eldigan_TheLionheart : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass.SetUpICardEffect("魔剣 ミストルティン", "Mystletainn, the Demon Blade", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>(), 1, false,card);
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                if (card.Owner.HandCards.Count((cardSource) => cardSource.CanSetBondThisCard && cardSource.UnitNames.Contains("エルトシャン")) > 0)
                {
                    SelectHandEffect selectHandEffect = GetComponent<SelectHandEffect>();

                    selectHandEffect.SetUp(
                        SelectPlayer: card.Owner,
                        CanTargetCondition: (cardSource) => cardSource.Owner.HandCards.Contains(cardSource) && cardSource.CanSetBondThisCard && cardSource.UnitNames.Contains("エルトシャン"),
                        CanTargetCondition_ByPreSelecetedList: null,
                        CanEndSelectCondition: null,
                        MaxCount: 1,
                        CanNoSelect: false,
                        CanEndNotMax: false,
                        isShowOpponent: true,
                        SelectCardCoroutine: null,
                        AfterSelectCardCoroutine: null,
                        mode: SelectHandEffect.Mode.SetFaceBond,
                        cardEffect: activateClass);

                    yield return StartCoroutine(selectHandEffect.Activate(null));
                }
            }
        }

        CanNotCriticalClass canNotCriticalClass = new CanNotCriticalClass();
        canNotCriticalClass.SetUpICardEffect("ヘズルの加護","",new List<Cost>(),new List<Func<Hashtable, bool>>() { CanUseCondition },-1,false,card);
        canNotCriticalClass.SetUpCanNotCriticalClass(UnitCondition);
        cardEffects.Add(canNotCriticalClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if(card.Owner.BondCards.Count((cardSource) => !cardSource.IsReverse && cardSource.UnitNames.Contains("エルトシャン")) > 0)
            {
                return true;
            }

            return false;
        }

        bool UnitCondition(Unit unit)
        {
            if(unit != null)
            {
                if(unit.Character != null)
                {
                    if(unit.Character.Owner == card.Owner.Enemy)
                    {
                        if(unit.Character.Owner.GetBackUnits().Contains(unit))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        return cardEffects;
    }
}