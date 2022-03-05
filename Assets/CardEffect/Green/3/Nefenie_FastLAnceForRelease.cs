using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Photon.Pun;

public class Nefenie_FastLAnceForRelease : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpClass powerUpClass = new PowerUpClass();
        powerUpClass.SetUpICardEffect("怒れる槍姫", null, null, -1, false);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10 * (card.UnitContainingThisCharacter().Characters.Count - 1), CanPowerUpCondition);
        cardEffects.Add(powerUpClass);

        bool CanPowerUpCondition(Unit unit)
        {
            if(card.UnitContainingThisCharacter() != null)
            {
                if (card.UnitContainingThisCharacter() == unit)
                {
                    if (GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
                    {
                        return true;
                    }
                }
            }
            

            return false;
        }

        if (timing == EffectTiming.OnCriticalAnyone)
        {
            activateClass[1].SetUpICardEffect("秘めた激情", new List<Cost>() , new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true);
            activateClass[1].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[1]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[1].EffectName = "Tempered Wrath";
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable))
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit != null)
                    {
                        if (GManager.instance.turnStateMachine.AttackingUnit.Character != null)
                        {
                            if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                            {
                                bool canFromTrash = card.Owner.TrashCards.Count((cardSource) => cardSource.UnitNames.Contains("ネフェニー")) > 0;

                                if (canFromTrash)
                                {
                                    return true;
                                }
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
                    CanTargetCondition: (cardSource) => cardSource.UnitNames.Contains("ネフェニー"),
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    CanNoSelect: () => true,
                    SelectCardCoroutine: null,
                    AfterSelectCardCoroutine: AfterSelectCardCoroutine,
                    Message: "Select a card to stack down.",
                    MaxCount: 1,
                    CanEndNotMax: false,
                    isShowOpponent: true,
                    mode: SelectCardEffect.Mode.Custom,
                    root: SelectCardEffect.Root.Trash,
                    CustomRootCardList: null,
                    CanLookReverseCard: true);

                IEnumerator AfterSelectCardCoroutine(List<CardSource> cardSources)
                {
                    yield return ContinuousController.instance.StartCoroutine(new IGrow(card.UnitContainingThisCharacter(), cardSources).Grow());
                }

                yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));
            }
        }

        return cardEffects;
    }
}



