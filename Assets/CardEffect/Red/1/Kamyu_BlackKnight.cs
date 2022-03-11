using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Kamyu_BlackKnight : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("神槍 グラディウス", "Gradivus, the Divine Lance",new List<Cost>() {new ReverseCost(4, (cardSource) => true) }, new List<Func<Hashtable, bool>>(), -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                int DestroyCount = 0;

                List<Unit> EnemyUnits = new List<Unit>();
                List<IDestroyUnit> destroyUnits = new List<IDestroyUnit>();

                foreach (Unit unit in card.Owner.Enemy.FieldUnit)
                {
                    EnemyUnits.Add(unit);
                }

                
                foreach (Unit unit in EnemyUnits)
                {
                    if(unit.Character.PlayCost <= 2)
                    {
                        Hashtable hashtable = new Hashtable();
                        hashtable.Add("cardEffect", activateClass);
                        hashtable.Add("Unit", new Unit(unit.Characters));
                        IDestroyUnit destroyUnit = new IDestroyUnit(unit, 1, BreakOrbMode.Hand, hashtable);
                        destroyUnits.Add(destroyUnit);
                    }
                }

                foreach(IDestroyUnit destroyUnit in destroyUnits)
                {
                    yield return ContinuousController.instance.StartCoroutine(destroyUnit.Destroy());
                }

                DestroyCount = destroyUnits.Count((destroyUnit) => destroyUnit.Destroyed);

                if (DestroyCount > 0)
                {
                    RangeUpClass rangeUpClass = new RangeUpClass();
                    rangeUpClass.SetUpRangeUpClass((unit, Range) => { Range.Add(1); Range.Add(2); return Range; },(unit) => unit == card.UnitContainingThisCharacter());
                    card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => rangeUpClass);

                    PowerModifyClass powerUpClass = new PowerModifyClass();
                    powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10 * DestroyCount, (unit) => unit == card.UnitContainingThisCharacter(), true);
                    card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);
                }
                
            }
        }

        return cardEffects;
    }
}

