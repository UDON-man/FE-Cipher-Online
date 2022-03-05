using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

public abstract class CEntity_Effect : MonoBehaviourPunCallbacks
{
    public CardSource card
    {
        get;set;
    }

    public virtual List<ICardEffect> CardEffects(EffectTiming timing)
    {
        return new List<ICardEffect>();
    }

    public virtual List<ICardEffect> SupportEffects(EffectTiming timing)
    {
        return new List<ICardEffect>();
    }

    public List<ActivateClass> activateClass { get; set; } = new List<ActivateClass>();
    public List<ActivateClass> activateClass_Support { get; set; } = new List<ActivateClass>();
    public void AttachICardEffectComponent()
    {
        for(int i=0; i<8;i++)
        {
            ActivateClass _activateClass = gameObject.AddComponent<ActivateClass>();
            _activateClass._card = this.card;
            activateClass.Add(_activateClass);
        }

        for (int i = 0; i < 6; i++)
        {
            ActivateClass _activateClass = gameObject.AddComponent<ActivateClass>();
            _activateClass._card = this.card;
            activateClass_Support.Add(_activateClass);
        }

        SelectUnitEffect selectUnitEffect = gameObject.AddComponent<SelectUnitEffect>();
        selectUnitEffect._card = this.card;

        SelectCardEffect selectCardEffect = gameObject.AddComponent<SelectCardEffect>();
        selectCardEffect._card = this.card;

        SelectHandEffect selectHandEffect = gameObject.AddComponent<SelectHandEffect>();
        selectHandEffect._card = this.card;
    }

    public List<ICardEffect> GetCardEffects(EffectTiming timing)
    {
        List<ICardEffect> _GetCardEffects = new List<ICardEffect>();

        foreach (ICardEffect cardEffect in CardEffects(timing))
        {
            cardEffect._card = this.card;
            _GetCardEffects.Add(cardEffect);
        }

        return _GetCardEffects;
    }

    public List<ICardEffect> GetSupportEffects(EffectTiming timing)
    {
        List<ICardEffect> _GetSupportEffects = new List<ICardEffect>();

        foreach (ICardEffect cardEffect in SupportEffects(timing))
        {
            cardEffect._card = this.card;
            cardEffect.isSupportSkill = true;
            _GetSupportEffects.Add(cardEffect);
        }

        return _GetSupportEffects;
    }

    public List<ICardEffect> GetAllSupportEffects()
    {
        List<ICardEffect> GetAllSupportEffects = new List<ICardEffect>();

        foreach (EffectTiming timing in Enum.GetValues(typeof(EffectTiming)))
        {
            foreach (ICardEffect cardEffect in GetSupportEffects(timing))
            {
                GetAllSupportEffects.Add(cardEffect);
            }
        }

        return GetAllSupportEffects;
    }

    public List<ICardEffect> GetAllCardEffects()
    {
        List<ICardEffect> GetAllCardEffects = new List<ICardEffect>();

        foreach (EffectTiming timing in Enum.GetValues(typeof(EffectTiming)))
        {
            foreach (ICardEffect cardEffect in GetCardEffects(timing))
            {
                GetAllCardEffects.Add(cardEffect);
            }
        }

        foreach (EffectTiming timing in Enum.GetValues(typeof(EffectTiming)))
        {
            foreach (ICardEffect cardEffect in GetSupportEffects(timing))
            {
                GetAllCardEffects.Add(cardEffect);
            }
        }

        return GetAllCardEffects;
    }

    public void ResetUsedCountThisTurn()
    {
        foreach(ICardEffect cardEffect in GetAllCardEffects())
        {
            cardEffect.UseCountThisTurn = 0;
        }
    }

    public bool IsExistOnField(Hashtable hashtable)
    {
        if (card.UnitContainingThisCharacter() != null)
        {
            if (card.Owner.FieldUnit.Contains(card.UnitContainingThisCharacter()))
            {
                return true;
            }
        }

        return false;
    }

    public virtual void Init()
    {
        ResetUsedCountThisTurn();
    }
}