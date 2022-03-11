using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

public abstract class CEntity_Effect : MonoBehaviourPunCallbacks
{
    public virtual List<ICardEffect> CardEffects(EffectTiming timing, CardSource cardSource)
    {
        return new List<ICardEffect>();
    }

    public virtual List<ICardEffect> SupportEffects(EffectTiming timing, CardSource cardSource)
    {
        return new List<ICardEffect>();
    }

    public CEntity_EffectController cEntity_EffectController
    {
        get
        {
            return GetComponent<CEntity_EffectController>();
        }
    }

    public void AttachICardEffectComponent()
    {
        SelectUnitEffect selectUnitEffect = gameObject.AddComponent<SelectUnitEffect>();
        SelectCardEffect selectCardEffect = gameObject.AddComponent<SelectCardEffect>();
        SelectHandEffect selectHandEffect = gameObject.AddComponent<SelectHandEffect>();
    }

    public List<ICardEffect> GetCardEffects(EffectTiming timing, CardSource cardSource)
    {
        List<ICardEffect> _GetCardEffects = new List<ICardEffect>();

        foreach (ICardEffect cardEffect in CardEffects(timing,cardSource))
        {
            _GetCardEffects.Add(cardEffect);
        }

        return _GetCardEffects;
    }

    public List<ICardEffect> GetSupportEffects(EffectTiming timing, CardSource cardSource)
    {
        List<ICardEffect> _GetSupportEffects = new List<ICardEffect>();

        foreach (ICardEffect cardEffect in SupportEffects(timing,cardSource))
        {
            cardEffect.isSupportSkill = true;
            _GetSupportEffects.Add(cardEffect);
        }

        return _GetSupportEffects;
    }

    public List<ICardEffect> GetAllSupportEffects(CardSource cardSource)
    {
        List<ICardEffect> GetAllSupportEffects = new List<ICardEffect>();

        foreach (EffectTiming timing in Enum.GetValues(typeof(EffectTiming)))
        {
            foreach (ICardEffect cardEffect in GetSupportEffects(timing,cardSource))
            {
                GetAllSupportEffects.Add(cardEffect);
            }
        }

        return GetAllSupportEffects;
    }

    public List<ICardEffect> GetAllCardEffects(CardSource cardSource)
    {
        List<ICardEffect> GetAllCardEffects = new List<ICardEffect>();

        foreach (EffectTiming timing in Enum.GetValues(typeof(EffectTiming)))
        {
            foreach (ICardEffect cardEffect in GetCardEffects(timing,cardSource))
            {
                GetAllCardEffects.Add(cardEffect);
            }
        }

        foreach (EffectTiming timing in Enum.GetValues(typeof(EffectTiming)))
        {
            foreach (ICardEffect cardEffect in GetSupportEffects(timing,cardSource))
            {
                GetAllCardEffects.Add(cardEffect);
            }
        }

        return GetAllCardEffects;
    }

    public bool IsExistOnField(Hashtable hashtable,CardSource card)
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

    }
}