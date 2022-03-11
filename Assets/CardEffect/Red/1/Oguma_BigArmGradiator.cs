using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oguma_BigArmGradiator : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        CanNotSupportClass canNotSupportClass = new CanNotSupportClass();
        canNotSupportClass.SetUpCanNotSupportClass((cardSource) => !cardSource.UnitNames.Contains("シーダ") && !cardSource.UnitNames.Contains("ナバール"), (unit) => unit == card.UnitContainingThisCharacter());
        canNotSupportClass.SetUpICardEffect("宿命の好敵手","", null, null, -1, false,card);
        cardEffects.Add(canNotSupportClass);

        return cardEffects;
    }
}