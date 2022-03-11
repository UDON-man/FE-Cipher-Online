using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardOperation : MonoBehaviour
{
    CardSource card
    {
        get
        {
            return GetComponent<CardSource>();
        }
    }

    #region 使用可能な絆を減らす
    public void ReduceBond(int unitIndex)
    {
        Unit unit = null;

        if(0 <= unitIndex && unitIndex < card.Owner.FieldUnit.Count)
        {
            unit = card.Owner.FieldUnit[unitIndex];
        }

        card.Owner.Bond -= card.Cost(unit);
        card.Owner.BondConsumed += card.Cost(unit);
    }
    #endregion

    #region 絆ゾーンに置く

    public IEnumerator SetBondFromHand(bool isFace)
    {
        yield return StartCoroutine(GManager.instance.GetComponent<Effects>().DeleteHandCardEffectCoroutine(card));

        if(!GManager.instance.GetComponent<Effects>().ShowCardParent.transform.parent.gameObject.activeSelf)
        {
            ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(new List<CardSource>() { card }, "Added Bond Card", true));
        }

        yield return StartCoroutine(new ISetBondCard(card,isFace).SetBond());
    }
    #endregion

    #region 手札からユニットをプレイ
    public IEnumerator PlayUnit(int _targetUnitIndex,bool isFront)
    {
        Unit targetUnit = null;

        if (0 <= _targetUnitIndex && _targetUnitIndex < card.Owner.FieldUnit.Count)
        {
            targetUnit = card.Owner.FieldUnit[_targetUnitIndex];
        }

        yield return StartCoroutine(GManager.instance.GetComponent<Effects>().DeleteHandCardEffectCoroutine(card));

        yield return StartCoroutine(GManager.instance.GetComponent<Effects>().ShowUseHandCardEffect_PlayUnit(card));

        yield return StartCoroutine(new IPlayUnit(card, targetUnit,isFront,true,null,false).PlayUnit());

        yield return ContinuousController.instance.StartCoroutine(GManager.instance.turnStateMachine.AddUseHandCard());
        GManager.instance.turnStateMachine.IsSelecting = false;
        GManager.instance.turnStateMachine.isSync = false;
    }
    #endregion

    #region 手札からオーブに置く
    public IEnumerator PutOrbFromHand()
    {
        yield return StartCoroutine(GManager.instance.GetComponent<Effects>().DeleteHandCardEffectCoroutine(card));

        card.Owner.HandCards.Remove(card);

        yield return ContinuousController.instance.StartCoroutine(CardObjectController.AddOrbCard(card, false));
    }
    #endregion

    #region 手札から捨てる
    public IEnumerator DiscardFromHand(Hashtable hashtable)
    {
        Vector3 handPosition = Vector3.zero;

        if (card.Owner.isYou)
        {
            handPosition = new Vector3(0, 0, -40);
        }

        else
        {
            handPosition = new Vector3(0, 0, 40);
        }

        yield return StartCoroutine(GManager.instance.GetComponent<Effects>().DeleteHandCardEffectCoroutine(card));

        //yield return StartCoroutine(GManager.instance.GetComponent<Effects>().ShowSelectCardEffect(card, Effects.Target.Hand, true, true));

        //yield return StartCoroutine(GManager.instance.GetComponent<Effects>().LightBallSoulEffect(handPosition, card.Owner, Effects.Target.Trash));

        card.Owner.HandCards.Remove(card);

        CardObjectController.AddTrashCard(card);

        #region 「手札が捨てられた時」効果
        #region 捨てられたカード+捨てた効果を引数に載せる
        Hashtable _hashtable = new Hashtable();
        _hashtable.Add("Card", card);

        if(hashtable != null)
        {
            if(hashtable.ContainsKey("cardEffect"))
            {
                if (hashtable["cardEffect"] is ICardEffect)
                {
                    ICardEffect cardEffect = (ICardEffect)hashtable["cardEffect"];

                    _hashtable.Add("cardEffect", cardEffect);
                }
            }

            if(hashtable.Contains("isCost"))
            {
                if(hashtable["isCost"] is bool)
                {
                    bool isCost = (bool)hashtable["isCost"];

                    _hashtable.Add("isCost",isCost);
                }
            }
        }
        #endregion

        List<SkillInfo> skillInfos = new List<SkillInfo>();
        foreach (ICardEffect cardEffect in card.cEntity_EffectController.GetCardEffects(EffectTiming.OnDiscardHand))
        {
            if (cardEffect is ActivateICardEffect)
            {
                if (cardEffect.CanUse(_hashtable))
                {
                    skillInfos.Add(new SkillInfo(cardEffect, _hashtable));
                    //yield return ContinuousController.instance.StartCoroutine(((ActivateICardEffect)cardEffect).Activate_Effect_Optional_Cost_Execute(_hashtable));
                }
            }
        }

        foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players_ForTurnPlayer)
        {
            foreach (Unit unit in player.FieldUnit)
            {
                foreach (ICardEffect cardEffect in unit.EffectList(EffectTiming.OnDiscardHand))
                {
                    if (cardEffect is ActivateICardEffect)
                    {
                        if (cardEffect.CanUse(_hashtable))
                        {
                            skillInfos.Add(new SkillInfo(cardEffect, _hashtable));
                            //yield return ContinuousController.instance.StartCoroutine(((ActivateICardEffect)cardEffect).Activate_Effect_Optional_Cost_Execute(_hashtable));
                        }
                    }
                }
            }
        }
        yield return ContinuousController.instance.StartCoroutine(GManager.instance.availableMultipleSkills.ActivateMultipleSkills(skillInfos));

        #endregion
    }
    #endregion

    #region 山札から捨てる
    public IEnumerator DiscardFromLibrary()
    {
        ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(new List<CardSource>() { card }, "Trash Card", true));

        yield return new WaitForSeconds(0.1f);

        card.Owner.LibraryCards.Remove(card);

        CardObjectController.AddTrashCard(card);

        yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(card.Owner));
    }
    #endregion

    #region 手札から山札の上に置く
    public IEnumerator PutLibraryTopFromHand()
    {
        Vector3 handPosition = Vector3.zero;

        if (card.Owner.isYou)
        {
            handPosition = new Vector3(0, 0, -40);
        }

        else
        {
            handPosition = new Vector3(0, 0, 40);
        }

        yield return StartCoroutine(GManager.instance.GetComponent<Effects>().DeleteHandCardEffectCoroutine(card));

        //yield return StartCoroutine(GManager.instance.GetComponent<Effects>().ShowSelectCardEffect(card, Effects.Target.Hand, true, true));

        //yield return StartCoroutine(GManager.instance.GetComponent<Effects>().LightBallSoulEffect(handPosition, card.Owner, Effects.Target.Trash));

        card.Owner.HandCards.Remove(card);

        card.Owner.LibraryCards.Insert(0, card);
    }
    #endregion
}
