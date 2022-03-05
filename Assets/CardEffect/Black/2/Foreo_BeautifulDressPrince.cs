using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Foreo_BeautifulDressPrince : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[0].SetUpICardEffect("ドロー", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, null, 1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass[0].SetCCS(card.UnitContainingThisCharacter());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Deadlock";
            }

            IEnumerator ActivateCoroutine()
            {
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner != card.Owner && unit.Character.Owner.GetBackUnits().Contains(unit),
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: false,
                    CanEndNotMax: false,
                    SelectUnitCoroutine: null,
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Move);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));
            }
        }

        CanNotMoveBySkillClass canNotMoveBySkillClass = new CanNotMoveBySkillClass();
        canNotMoveBySkillClass.SetUpICardEffect("新たな戦術",new List<Cost>(),new List<Func<Hashtable, bool>>() { CanUseCondition },-1,false);
        canNotMoveBySkillClass.SetUpCanNotMoveBySkillClass((unit) => card.Owner.FieldUnit.Contains(unit));
        cardEffects.Add(canNotMoveBySkillClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if(GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner.Enemy)
            {
                if(!card.UnitContainingThisCharacter().IsTapped)
                {
                    return true;
                }
            }

            return false;
        }

        return cardEffects;
    }
}
