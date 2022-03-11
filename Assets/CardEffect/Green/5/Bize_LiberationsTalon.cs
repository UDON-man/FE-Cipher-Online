using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Bize_LiberationsTalon : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("黒翼の運び手", "Black Wings",new List<Cost>() { new TapCost() }, null, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner == card.Owner && unit != card.UnitContainingThisCharacter(),
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: MaxCount(),
                    CanNoSelect: false,
                    CanEndNotMax: CanEndNotMax(),
                    SelectUnitCoroutine: null,
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Move,
                    cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));

                bool CanEndNotMax()
                {
                    if (card.UnitContainingThisCharacter() != null)
                    {
                        if (card.UnitContainingThisCharacter().IsLevelUp())
                        {
                            return true;
                        }
                    }

                    return false;
                }

                int MaxCount()
                {
                    if(card.UnitContainingThisCharacter() != null)
                    {
                        if(card.UnitContainingThisCharacter().IsLevelUp())
                        {
                            return card.Owner.FieldUnit.Count((unit) => unit != card.UnitContainingThisCharacter());
                        }
                    }

                    return 1;
                }
            }

        }

        return cardEffects;
    }
}