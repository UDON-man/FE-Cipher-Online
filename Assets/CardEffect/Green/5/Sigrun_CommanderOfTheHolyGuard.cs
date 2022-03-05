using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Sigrun_CommanderOfTheHolyGuard : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpClass powerUpClass = new PowerUpClass();
        powerUpClass.SetUpICardEffect("銀翼の将", null, null, -1, false);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10 * (card.UnitContainingThisCharacter().Characters.Count - 1), CanPowerUpCondition);
        cardEffects.Add(powerUpClass);

        bool CanPowerUpCondition(Unit unit)
        {
            if (IsExistOnField(null))
            {
                if (card.UnitContainingThisCharacter() != unit && unit.Weapons.Contains(Weapon.Wing) && unit.Character.Owner == card.Owner)
                {
                    if (GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
                    {
                        return true;
                    }
                }
            }


            return false;
        }

        if (timing == EffectTiming.OnEvadeAnyone)
        {
            activateClass[1].SetUpICardEffect("優美なる翼", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true);
            activateClass[1].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[1]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[1].EffectName = "Graceful Wing";
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable))
                {
                    if (GManager.instance.turnStateMachine.DefendingUnit != null)
                    {
                        if(GManager.instance.turnStateMachine.DefendingUnit.Character != null)
                        {
                            if (GManager.instance.turnStateMachine.DefendingUnit == card.UnitContainingThisCharacter())
                            {
                                bool canFromTrash = card.Owner.TrashCards.Count((cardSource) => cardSource.UnitNames.Contains("シグルーン")) > 0;

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
                    CanTargetCondition: (cardSource) => cardSource.UnitNames.Contains("シグルーン"),
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

