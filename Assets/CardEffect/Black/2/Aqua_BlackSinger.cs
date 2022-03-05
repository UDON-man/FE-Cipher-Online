using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Aqua_BlackSinger : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[0].SetUpICardEffect("暗夜の聖歌", new List<Cost>() { new TapCost(), new ReverseCost(2, (cardSource) => true) }, null, -1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Anthem oh Nohr";
            }

            IEnumerator ActivateCoroutine()
            {
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                SelectPlayer: card.Owner,
                CanTargetCondition: (unit) => unit.Character.Owner == card.Owner && unit.DoneAttackThisTurn && card.cardColors.Contains(CardColor.Black),
                CanTargetCondition_ByPreSelecetedList: null,
                CanEndSelectCondition: null,
                MaxCount: 1,
                CanNoSelect: false,
                CanEndNotMax: false,
                SelectUnitCoroutine: (unit) => SelectUnitCoroutine(unit),
                AfterSelectUnitCoroutine: null,
                mode: SelectUnitEffect.Mode.Custom);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));

                IEnumerator SelectUnitCoroutine(Unit unit)
                {
                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("cardEffect", activateClass[0]);
                    yield return ContinuousController.instance.StartCoroutine(unit.UnTap(hashtable));

                    yield return new WaitForSeconds(0.2f);

                    PowerUpClass powerUpClass = new PowerUpClass();
                    powerUpClass.SetUpPowerUpClass((_unit, Power) => Power + 20, (_unit) => _unit == unit);
                    unit.UntilEachTurnEndUnitEffects.Add(powerUpClass);

                    yield return null;
                }
            }
        }

        return cardEffects;
    }
}