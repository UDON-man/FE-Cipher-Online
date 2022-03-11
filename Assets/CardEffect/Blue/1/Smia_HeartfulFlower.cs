using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Smia_HeartfulFlower : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("天空を翔ける者", "Angelic Flight", new List<Cost>(), new List<Func<Hashtable, bool>>() { (hash) => !card.UnitContainingThisCharacter().IsTapped }, 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                Hashtable hashtable = new Hashtable();
                hashtable.Add("cardEffect", activateClass);
                yield return ContinuousController.instance.StartCoroutine(new IMoveUnit(new List<Unit>() { card.UnitContainingThisCharacter() }, true,hashtable).MoveUnits());
            }
        }

        SupportPowerUpClass supportPowerUpClass = new SupportPowerUpClass();
        supportPowerUpClass.SetUpICardEffect("天馬の叫び","",null,null,-1,false,card);
        supportPowerUpClass.SetUpSupportPowerUpClass((cardSource,SupportPower) => SupportPower + 10, ChangeSupportPowerCondition);
        supportPowerUpClass.SetCCS(card.UnitContainingThisCharacter());
        cardEffects.Add(supportPowerUpClass);

        bool ChangeSupportPowerCondition(CardSource cardSource)
        {
            if(GManager.instance.turnStateMachine.AttackingUnit != null && GManager.instance.turnStateMachine.DefendingUnit != null)
            {
                if(GManager.instance.turnStateMachine.AttackingUnit.Character.Owner == card.Owner || GManager.instance.turnStateMachine.DefendingUnit.Character.Owner == card.Owner)
                {
                    if (card.UnitContainingThisCharacter() != GManager.instance.turnStateMachine.AttackingUnit && card.UnitContainingThisCharacter() != GManager.instance.turnStateMachine.DefendingUnit)
                    {
                        if (cardSource.Weapons.Contains(Weapon.Wing))
                        {
                            if (cardSource.Owner == card.Owner)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            

            return false;
        }

        return cardEffects;
    }
}