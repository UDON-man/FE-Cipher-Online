using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CEntity_EffectController : MonoBehaviour
{
    public CardSource card;

    List<CEntity_Effect> cEntity_Effects = new List<CEntity_Effect>();

    public Hashtable UseCountsThisTurn = new Hashtable();

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

    #region 実行スキルリストを取得
    public List<ICardEffect> GetCardEffects(EffectTiming timing)
    {
        List<ICardEffect> GetCardEffects = new List<ICardEffect>();

        foreach(CEntity_Effect cEntity_Effect in GetCEntity_Effects)
        {
            foreach(ICardEffect cardEffect in cEntity_Effect.GetCardEffects(timing, card))
            {
                GetCardEffects.Add(cardEffect);
            }
        }

        #region 場のユニットの効果によってスキルを追加される
        foreach(Player player in GManager.instance.turnStateMachine.gameContext.Players)
        {
            foreach(Unit unit in player.FieldUnit)
            {
                foreach (CEntity_Effect cEntity_Effect in unit.Character.cEntity_EffectController.GetCEntity_Effects)
                {
                    foreach (ICardEffect cardEffect in cEntity_Effect.GetCardEffects(EffectTiming.None, unit.Character))
                    {
                        if(cardEffect is IAddSkillEffect)
                        {
                            if(cardEffect.CanUse(null))
                            {
                                GetCardEffects = ((IAddSkillEffect)cardEffect).GetCardEffect(this.card, GetCardEffects, timing);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        return GetCardEffects;
    }
    #endregion

    #region 支援スキルリスト
    public List<ICardEffect> GetSupportEffects(EffectTiming timing)
    {
        List<ICardEffect> GetSupportCardEffects = new List<ICardEffect>();

        foreach (CEntity_Effect cEntity_Effect in GetCEntity_Effects)
        {
            foreach (ICardEffect cardEffect in cEntity_Effect.GetSupportEffects(timing, card))
            {
                cardEffect._card = this.card;
                GetSupportCardEffects.Add(cardEffect);
            }
        }

        return GetSupportCardEffects;
    }
    #endregion

    #region 全てのBSスキルリスト
    public List<ICardEffect> GetAllBSCardEffects()
    {
        List<ICardEffect> GetAllBSCardEffects = new List<ICardEffect>();

        foreach (EffectTiming timing in Enum.GetValues(typeof(EffectTiming)))
        {
            foreach (ICardEffect cardEffect in GetBSCardEffect(timing))
            {
                GetAllBSCardEffects.Add(cardEffect);
            }
        }

        return GetAllBSCardEffects;
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
            foreach (ICardEffect cardEffect in cEntity_Effect.GetAllCardEffects(card))
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
        UseCountsThisTurn = new Hashtable();
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

            //cEntity_Effect.card = this.card;
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

    #region 対象のスキルをこのターンに使用した回数を取得
    public int GetUseCountThisTurn(string EffectName)
    {
        int useCount = 0;

        if (UseCountsThisTurn.ContainsKey(EffectName))
        {
            if (UseCountsThisTurn[EffectName] is int)
            {
                int UseCountThisTurn = (int)UseCountsThisTurn[EffectName];

                useCount = UseCountThisTurn;
            }
        }

        //Debug.Log($"{EffectName}のこのターンの使用回数:{useCount}回");
        return useCount;
    }
    #endregion

    #region 対象のスキルがこのターンの使用回数上限を超えているかチェック
    public bool IsOverMaxCountPerTurn(string EffectName,int MaxCountPerTurn)
    {
        //Debug.Log($"{EffectName}の最大使用回数:{MaxCountPerTurn}回");
        return GetUseCountThisTurn(EffectName) >= MaxCountPerTurn;
    }
    #endregion

    #region 対象のスキルをこのターン中に使用した回数をカウントアップ
    public void CountUpUseCountsThisTurn(ICardEffect cardEffect)
    {
        if (UseCountsThisTurn.ContainsKey(cardEffect.EffectName))
        {
            if (UseCountsThisTurn[cardEffect.EffectName] is int)
            {
                UseCountsThisTurn[cardEffect.EffectName] = (int)UseCountsThisTurn[cardEffect.EffectName] + 1;
            }
        }

        else
        {
            UseCountsThisTurn.Add(cardEffect.EffectName,1);
        }
    }
    #endregion
}
