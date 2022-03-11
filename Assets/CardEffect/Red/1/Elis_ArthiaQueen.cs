using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Elis_ArthiaQueen : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("オーム", "Aum",new List<Cost>() { new TapCost(), new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { (hashtable) => !card.Owner.DoneUseOrm }, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

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
                    Message: "Select a card in your retreat area to deploy.",
                    MaxCount: 1,
                    CanEndNotMax: false,
                    isShowOpponent: true,
                    mode: SelectCardEffect.Mode.Deploy,
                    root: SelectCardEffect.Root.Trash,
                    CustomRootCardList: null,
                    CanLookReverseCard: true,
                    SelectPlayer: card.Owner,
                    cardEffect: activateClass);

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

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit.Character.Owner == card.Owner && (unit.Character.UnitNames.Contains("マルス") || unit.Character.UnitNames.Contains("マリク")), true);
        powerUpClass.SetUpICardEffect("エリスの想い","", null, null, -1, false,card);
        cardEffects.Add(powerUpClass);

        return cardEffects;
    }
}

