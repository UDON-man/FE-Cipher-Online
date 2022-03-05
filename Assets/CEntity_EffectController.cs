using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CEntity_EffectController : MonoBehaviour
{
    public CardSource card;

    List<CEntity_Effect> cEntity_Effects = new List<CEntity_Effect>();

    #region CEntity_Effectのリスト
    public List<CEntity_Effect> GetCEntity_Effects
    {
        get
        {
            List<CEntity_Effect> GetCEntity_Effects = new List<CEntity_Effect>();

            foreach(CEntity_Effect cEntity_Effect in cEntity_Effects)
            {
                if(cEntity_Effect != null)
                {
                    if(cEntity_Effect.enabled)
                    {
                        GetCEntity_Effects.Add(cEntity_Effect);
                    }
                }
            }

            return GetCEntity_Effects;
        }
    }
    #endregion

    #region スキルリスト
    public List<ICardEffect> GetCardEffects(EffectTiming timing)
    {
        List<ICardEffect> GetCardEffects = new List<ICardEffect>();

        foreach(CEntity_Effect cEntity_Effect in GetCEntity_Effects)
        {
            foreach(ICardEffect cardEffect in cEntity_Effect.GetCardEffects(timing))
            {
                GetCardEffects.Add(cardEffect);
            }
        }

        return GetCardEffects;
    }
    #endregion

    #region 支援スキルリスト
    public List<ICardEffect> GetSupportEffects(EffectTiming timing)
    {
        List<ICardEffect> GetSupportCardEffects = new List<ICardEffect>();

        foreach (CEntity_Effect cEntity_Effect in GetCEntity_Effects)
        {
            foreach (ICardEffect cardEffect in cEntity_Effect.GetSupportEffects(timing))
            {
                cardEffect._card = this.card;
                GetSupportCardEffects.Add(cardEffect);
            }
        }

        return GetSupportCardEffects;
    }
    #endregion

    #region BSスキルリスト
    public List<ICardEffect> GetBSCardEffect(EffectTiming timing)
    {
        List<ICardEffect> GetBSCardEffect = new List<ICardEffect>();

        foreach(ICardEffect cardEffect in GetCardEffects(timing))
        {
            if(cardEffect.isBS)
            {
                GetBSCardEffect.Add(cardEffect);
            }
        }

        return GetBSCardEffect;
    }
    #endregion

    #region 全ての支援スキルリスト
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
    #endregion

    #region スキル+支援スキルリスト
    public List<ICardEffect> GetAllCardEffects()
    {
        List<ICardEffect> GetAllCardEffects = new List<ICardEffect>();

        foreach (CEntity_Effect cEntity_Effect in GetCEntity_Effects)
        {
            foreach (ICardEffect cardEffect in cEntity_Effect.GetAllCardEffects())
            {
                GetAllCardEffects.Add(cardEffect);
            }
        }

        return GetAllCardEffects;
    }
    #endregion

    #region スキルをリセット
    public void Init()
    {
        foreach(CEntity_Effect cEntity_Effect in cEntity_Effects)
        {
            cEntity_Effect.Init();
        }
    }
    #endregion

    #region カード効果をセット
    bool first = true;
    public void AddCardEffect(string ClassName)
    {
        #region カード効果クラスのインスタンスを生成して登録
        if (!string.IsNullOrEmpty(ClassName))
        {
            bool CanAttachEffectComponent()
            {
                Type t = null;

                if (!string.IsNullOrEmpty(ClassName))
                {
                    t = Type.GetType(ClassName);

                    if (t != null)
                    {
                        return true;
                    }
                }

                return false;
            }

            CEntity_Effect cEntity_Effect = null;

            if (CanAttachEffectComponent())
            {
                Type t = Type.GetType(ClassName);

                cEntity_Effect = (CEntity_Effect)(this.gameObject.AddComponent(t));
            }

            else
            {
                cEntity_Effect = this.gameObject.AddComponent<EmptyCEntity_Effect>();
            }

            cEntity_Effect.card = this.card;
            cEntity_Effects.Add(cEntity_Effect);

            if (first)
            {
                cEntity_Effect.AttachICardEffectComponent();
                first = false;
            }
        }
        #endregion
    }
    #endregion
}
