using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Samuto_CriticalSwordUser : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[0].SetUpICardEffect("今宵の必殺剣はよく切れる…", new List<Cost>() { new ReverseCost(2, (cardSource) => true) }, null, -1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "My Killing Edge is sharp this evening...";
            }

            IEnumerator ActivateCoroutine()
            {
                CanNotBeEvadedClass canNotBeEvadedClass = new CanNotBeEvadedClass();
                canNotBeEvadedClass.SetUpCanNotBeEvadedClass((AttackingUnit) => AttackingUnit == this.card.UnitContainingThisCharacter(), (DefendingUnit) => DefendingUnit.Character.PlayCost <= 3);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add(canNotBeEvadedClass);

                yield return null;
            }
        }

        CanLevelUpToThisCardClass canLevelUpToThisCardClass = new CanLevelUpToThisCardClass();
        canLevelUpToThisCardClass.SetUpICardEffect("チッばれちまったか",new List<Cost>(),new List<Func<Hashtable, bool>>() { CanUseCondition },-1,false);
        canLevelUpToThisCardClass.SetUpCanLevelUpToThisCardClass((cardSource) =>  cardSource == this.card && card.Owner.FieldUnit.Count((unit) => unit.Character.UnitNames.Contains("サムトー")) == 0, (unit) => unit.Character.Owner == card.Owner && unit.Character.UnitNames.Contains("ナバール"));
        cardEffects.Add(canLevelUpToThisCardClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if(card.Owner.FieldUnit.Count((unit) => unit.Character.UnitNames.Contains("サムトー")) == 0)
            {
                return true;
            }

            return false;
        }


        return cardEffects;
    }
}
