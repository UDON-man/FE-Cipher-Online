using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Marks_AnnyaStrongestKnight : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDestroyedDuringBattleAlly)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("黒き覇軍", "Black Leadership", new List<Cost>(){ new ReverseCost(2, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (hashtable != null)
                {
                    if (hashtable.ContainsKey("Defender"))
                    {
                        if (hashtable["Defender"] is Unit)
                        {
                            Unit unit = (Unit)hashtable["Defender"];

                            if (unit != null)
                            {
                                if (unit.Character != null)
                                {
                                    if (unit.Character.Owner == card.Owner)
                                    {
                                        if(unit.Character == card || IsExistOnField(hashtable,card))
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                foreach(Unit unit in card.Owner.FieldUnit)
                {
                    PowerModifyClass powerUpClass = new PowerModifyClass();
                    powerUpClass.SetUpPowerUpClass((_unit, Power) => Power + 10, (_unit) => _unit == unit, true);
                    unit.UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);
                }

                yield return null;
            }
        }

        else if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("暗黒剣 ジークフリート", "Dark Sword, Siegfried", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, null, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                RangeUpClass rangeUpClass = new RangeUpClass();
                rangeUpClass.SetUpRangeUpClass((unit, Range) => { Range.Add(1); Range.Add(2); return Range; }, (unit) => unit == card.UnitContainingThisCharacter());
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => rangeUpClass);

                yield return null;
            }
        }

        return cardEffects;
    }

}
