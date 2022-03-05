using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Elis_ArthiaQueen : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[0].SetUpICardEffect("オーム", new List<Cost>() { new TapCost(), new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { (hashtable) => !card.Owner.DoneUseOrm }, -1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Aum";
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
                    Message: "Select a card to deploy.",
                    MaxCount: 1,
                    CanEndNotMax: false,
                    isShowOpponent: true,
                    mode: SelectCardEffect.Mode.Deploy,
                    root: SelectCardEffect.Root.Trash,
                    CustomRootCardList: null,
                    CanLookReverseCard: true);

                yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));

                card.Owner.DoneUseOrm = true;

                bool CanTargetCondition(CardSource cardSource)
                {
                    if(cardSource.Owner == card.Owner)
                    {
                        if (cardSource.cardColors.Contains(CardColor.Red))
                        {
                            if (cardSource.cEntity_Base.PlayCost <= 2)
                            {
                                if (cardSource.CanPlayAsNewUnit())
                                {
                                    return true;
                                }
                            }
                        }
                    }

                    return false;
                }
            }
        }

        PowerUpClass powerUpClass = new PowerUpClass();
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit.Character.Owner == this.card.Owner && (unit.Character.UnitNames.Contains("マルス") || unit.Character.UnitNames.Contains("マリク")));
        powerUpClass.SetUpICardEffect("エリスの想い", null, null, -1, false);
        cardEffects.Add(powerUpClass);

        return cardEffects;
    }
}

