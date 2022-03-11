using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Aqua_WhiteSinger : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("白夜の賛歌", "Hymn of Hoshido", new List<Cost>() { new TapCost(), new ReverseCost(2, (cardSource) => true) }, null, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                SelectPlayer: card.Owner,
                CanTargetCondition: (unit) => unit.Character.Owner == card.Owner && unit.DoneAttackThisTurn && card.cardColors.Contains(CardColor.White),
                CanTargetCondition_ByPreSelecetedList: null,
                CanEndSelectCondition: null,
                MaxCount: 1,
                CanNoSelect: false,
                CanEndNotMax: false,
                SelectUnitCoroutine: (unit) => SelectUnitCoroutine(unit),
                AfterSelectUnitCoroutine: null,
                mode: SelectUnitEffect.Mode.Custom,
                cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));

                IEnumerator SelectUnitCoroutine(Unit unit)
                {
                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("cardEffect", activateClass);
                    yield return ContinuousController.instance.StartCoroutine(unit.UnTap(hashtable));

                    yield return new WaitForSeconds(0.2f);

                    PowerModifyClass powerUpClass = new PowerModifyClass();
                    powerUpClass.SetUpPowerUpClass((_unit, Power) => Power + 20, (_unit) => _unit == unit, true);
                    unit.UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);

                    yield return null;
                }
            }
        }

        return cardEffects;
    }
}