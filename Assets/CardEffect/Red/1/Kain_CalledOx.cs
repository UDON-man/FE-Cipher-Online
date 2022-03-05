using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Kain_CalledOx : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        CanNotAttackClass canNotAttackClass = new CanNotAttackClass();
        canNotAttackClass.SetUpICardEffect("聖騎士の加護", null, null, -1, false);
        canNotAttackClass.SetUpCanNotAttackClass((AttackingUnit) => AttackingUnit.Character.Owner != this.card.Owner && AttackingUnit.Character.Owner.GetBackUnits().Contains(AttackingUnit), (DefendingUnit) => DefendingUnit.Character.Owner == this.card.Owner && (DefendingUnit == this.card.UnitContainingThisCharacter() || DefendingUnit.Character.cEntity_Base.PlayCost <= 2));

        cardEffects.Add(canNotAttackClass);

        if (timing == EffectTiming.OnAttackAnyone)
        {
            SelectAllyCost selectAllyCost = new SelectAllyCost(
                SelectPlayer: card.Owner,
                CanTargetCondition: (unit) => unit.Character.Owner == card.Owner && unit.Character.UnitNames.Contains("アベル") && !unit.IsTapped,
                CanTargetCondition_ByPreSelecetedList: null,
                CanEndSelectCondition: null,
                MaxCount: 1,
                CanNoSelect: false,
                CanEndNotMax: false,
                SelectUnitCoroutine: null,
                AfterSelectUnitCoroutine: null,
                mode: SelectUnitEffect.Mode.Tap);

            activateClass[0].SetUpICardEffect("赤緑の双撃", new List<Cost>() { selectAllyCost }, new List<Func<Hashtable, bool>>() { CanUseCondition } , -1, true);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Red-Green Twin Strike";
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable))
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                    {
                        return true;
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                PowerUpClass powerUpClass = new PowerUpClass();
                Unit targetUnit = card.UnitContainingThisCharacter();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 40, (unit) => unit == targetUnit);

                targetUnit.UntilEndBattleEffects.Add(powerUpClass);

                yield return null;
            }
        }

        return cardEffects;
    }
}
