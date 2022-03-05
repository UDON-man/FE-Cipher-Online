using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Ema_OccaisonallyDarkHorseKnight : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[0].SetUpICardEffect("トライアングルアタック?", new List<Cost>() , new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Triangle Attack?";
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if(card.Owner.HandCards.Count((cardSource) => cardSource.UnitNames.Contains("エマ")) >= 2)
                {
                    return false;
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                if (card.Owner.HandCards.Count((cardSource) => cardSource.UnitNames.Contains("エマ")) >= 2)
                {
                    SelectHandEffect selectHandEffect = GetComponent<SelectHandEffect>();

                    selectHandEffect.SetUp(
                        SelectPlayer: card.Owner,
                        CanTargetCondition: (cardSource) => cardSource.Owner.HandCards.Contains(cardSource) && cardSource.UnitNames.Contains("エマ"),
                        CanTargetCondition_ByPreSelecetedList: null,
                        CanEndSelectCondition: null,
                        MaxCount: 2,
                        CanNoSelect: false,
                        CanEndNotMax: false,
                        isShowOpponent: true,
                        SelectCardCoroutine: null,
                        AfterSelectCardCoroutine: null,
                        mode: SelectHandEffect.Mode.Custom);

                    yield return StartCoroutine(selectHandEffect.Activate(null));

                    PowerUpClass powerUpClass = new PowerUpClass();
                    powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter());
                    card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add(powerUpClass);
                }
            }
        }

        return cardEffects;
    }
}