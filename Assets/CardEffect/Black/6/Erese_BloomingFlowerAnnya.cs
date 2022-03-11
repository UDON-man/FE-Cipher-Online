using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Erese_BloomingFlowerAnnya : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("写し身", "Replicate",new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition1 }, 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition1(Hashtable hashtable)
            {
                if (card.Owner.OrbCards.Count((cardSource) => cardSource.IsReverse) > 0)
                {
                    return true;
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                if (card.Owner.OrbCards.Count((cardSource) => cardSource.IsReverse) > 0)
                {
                    SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                    selectCardEffect.SetUp(
                        CanTargetCondition: (cardSource) => cardSource.Owner == card.Owner && cardSource.IsReverse,
                        CanTargetCondition_ByPreSelecetedList: null,
                        CanEndSelectCondition: null,
                        CanNoSelect: () => false,
                        SelectCardCoroutine: null,
                        AfterSelectCardCoroutine: null,
                        Message: "Select a orb card to turn face up.",
                        MaxCount: 1,
                        CanEndNotMax: false,
                        isShowOpponent: true,
                        mode: SelectCardEffect.Mode.SetFace,
                        root: SelectCardEffect.Root.Orb,
                        CustomRootCardList: null,
                        CanLookReverseCard: false,
                        SelectPlayer: card.Owner,
                        cardEffect: activateClass);

                    yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));

                    PowerModifyClass powerUpClass = new PowerModifyClass();
                    powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 30, (unit) => unit == card.UnitContainingThisCharacter(), true);
                    card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);

                    if(card.Owner.OrbCards.Count <= card.Owner.Enemy.OrbCards.Count)
                    {
                        Hashtable hashtable = new Hashtable();
                        hashtable.Add("cardEffect", activateClass);
                        yield return ContinuousController.instance.StartCoroutine(card.UnitContainingThisCharacter().UnTap(hashtable));
                    }
                }

                yield return null;
            }
        }

        CanNotCriticalClass canNotCriticalClass = new CanNotCriticalClass();
        canNotCriticalClass.SetUpICardEffect("祈りの輝光","",new List<Cost>(),new List<Func<Hashtable, bool>>() { CanUseCondition },-1,false,card);
        canNotCriticalClass.SetUpCanNotCriticalClass(UnitCondition);
        cardEffects.Add(canNotCriticalClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if (IsExistOnField(hashtable,card))
            {
                if (card.Owner.OrbCards.Count((cardSource) => !cardSource.IsReverse) > 0)
                {
                    return true;
                }
            }

            return false;
        }

        bool UnitCondition(Unit unit)
        {
            if(unit.Character != null)
            {
                if(unit.Character.PlayCost <= 2)
                {
                    return true;
                }
            }

            return false;
        }

        return cardEffects;
    }
}
