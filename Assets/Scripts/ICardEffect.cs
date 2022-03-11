using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//カード効果
public abstract class ICardEffect 
{
    public CardSource _card { get; set; } = null;

    public CardSource card()
    {
        return _card;
    }

    #region 効果をセットアップ
    public void SetUpICardEffect(string EffectName,string EffectName_En,List<Cost> costs, List<Func<Hashtable, bool>> CanUseConditions,int MaxCountPerTurn,bool Optional,CardSource card)
    {
        this.EffectName = EffectName;
        this.EffecrName_En = EffectName_En;

        this.costs = new List<Cost>();
        if(costs != null)
        {
            foreach(Cost cost in costs)
            {
                this.costs.Add(cost);
            }
        }

        this.CanUseConditions = new List<Func<Hashtable,bool>>();
        if (CanUseConditions != null)
        {
            foreach(Func<Hashtable,bool> CanUseCondition in CanUseConditions)
            {
                this.CanUseConditions.Add(CanUseCondition);
            }
        }

        this.MaxCountPerTurn = 114514;
        if(MaxCountPerTurn > 0)
        {
            this.MaxCountPerTurn = MaxCountPerTurn;
        }

        this.Optional = Optional;
        this._card = card;

        isCCS = false;
        isLvS = false;
        isCF = false;
        isBS = false;
        isSupportSkill = false;
        isNotCheck_Effect = false;
    }
    #endregion

    #region 発動確認・エフェクト無しか
    public bool isNotCheck_Effect { get; set; }
    #endregion

    #region 一回のスキル発動で処理が実行される回数
    public int ActivateCount
    {
        get
        {
            int ActivateCount = 1;

            foreach(Player player in GManager.instance.turnStateMachine.gameContext.Players_ForTurnPlayer)
            {
                foreach(Unit unit in player.FieldUnit)
                {
                    foreach(ICardEffect cardEffect in unit.EffectList(EffectTiming.None))
                    {
                        if (cardEffect is IChangeSkillActivateCountEffect)
                        {
                            if (cardEffect.CanUse(null))
                            {
                                ActivateCount = ((IChangeSkillActivateCountEffect)cardEffect).GetActivateCount(this, ActivateCount);
                            }
                        }
                    }
                }
            }

            return ActivateCount;
        }
    }
    #endregion

    #region 1ターンに使える回数
    public int MaxCountPerTurn { get; set; } = 114514;
    #endregion

    #region このターンに使った回数
    //public int UseCountThisTurn { get; set; }
    #endregion

    #region 効果名

    #region 効果名日本語
    public virtual string EffectName { get; set; } = "";
    #endregion


    #region 効果名英語
    public virtual string EffecrName_En { get; set; } = "";
    #endregion

    public string GetEffectName()
    {
        if(ContinuousController.instance.language == Language.ENG)
        {
            return EffecrName_En;
        }

        else
        {
            return EffectName;
        }
    }
    #endregion

    #region 任意効果か
    public bool Optional { get; set; }
    #endregion

    #region 支援スキルか
    public bool isSupportSkill { get; set; }
    #endregion

    #region 任意効果を発動するか
    public bool UseOptional { get; set; }
    #endregion

    #region 無効化されているか
    public bool IsInvalidate
    {
        get
        {
            foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players)
            {
                foreach (Unit unit in player.FieldUnit)
                {
                    foreach(CEntity_Effect cEntity_Effect in unit.Character.cEntity_EffectController.GetCEntity_Effects)
                    {
                        foreach (ICardEffect cardEffect in cEntity_Effect.GetCardEffects(EffectTiming.None,unit.Character))
                        {
                            if (cardEffect is IInvalidationCardEffect)
                            {
                                if (((IInvalidationCardEffect)cardEffect).IsInvalidate(this))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    
                }

                foreach (ICardEffect cardEffect in player.PlayerEffects(EffectTiming.None))
                {
                    if (cardEffect is IInvalidationCardEffect)
                    {
                        if (((IInvalidationCardEffect)cardEffect).IsInvalidate(this))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
    #endregion

    #region コストの支払いが完了したか
    public bool DonePayCost { get; set; }
    #endregion

    #region 使用可能判定
    public List<Func<Hashtable,bool>> CanUseConditions = new List<Func<Hashtable,bool>>(); 
    public virtual bool CanUse(Hashtable hash)
    {
        #region コストによる使用可否判定
        foreach(Cost cost in costs)
        {
            if (cost != null && card() != null)
            {
                #region リバースコスト
                if (cost is ReverseCost)
                {
                    if (((ReverseCost)cost).ReverseCount > card().Owner.BondCards.Count((cardSource) => !cardSource.IsReverse && ((ReverseCost)cost).CanTargetCondition(cardSource)))
                    {
                        return false;
                    }
                }
                #endregion

                #region 自身をリバースするコスト
                else if (cost is ReverseSelfCost)
                {
                    if (card() == null)
                    {
                        return false;
                    }

                    if (!card().Owner.BondCards.Contains(card())||card().IsReverse)
                    {
                        return false;
                    }
                }
                #endregion

                #region タップコスト
                else if (cost is TapCost)
                {
                    if(card() == null)
                    {
                        return false;
                    }

                    if (card().UnitContainingThisCharacter() == null)
                    {
                        return false;
                    }

                    if (card().UnitContainingThisCharacter().IsTapped)
                    {
                        return false;
                    }
                }
                #endregion

                #region 味方を選択して支払うコスト
                else if (cost is SelectAllyCost)
                {
                    SelectUnitEffect selectUnitEffect = GManager.instance.GetComponent<SelectUnitEffect>();

                    selectUnitEffect.SetUp(
                        SelectPlayer: ((SelectAllyCost)cost).SelectPlayer,
                        CanTargetCondition: ((SelectAllyCost)cost).CanTargetCondition,
                        CanTargetCondition_ByPreSelecetedList: ((SelectAllyCost)cost).CanTargetCondition_ByPreSelecetedList,
                        CanEndSelectCondition: ((SelectAllyCost)cost).CanEndSelectCondition,
                        MaxCount: ((SelectAllyCost)cost).MaxCount,
                        CanNoSelect: ((SelectAllyCost)cost).CanNoSelect,
                        CanEndNotMax: ((SelectAllyCost)cost).CanEndNotMax,
                        SelectUnitCoroutine: ((SelectAllyCost)cost).SelectUnitCoroutine,
                        AfterSelectUnitCoroutine: ((SelectAllyCost)cost).AfterSelectUnitCoroutine,
                        mode: ((SelectAllyCost)cost).mode,
                        cardEffect:null);

                    if(!selectUnitEffect.active())
                    {
                        return false;
                    }
                }
                #endregion

                #region 手札を捨てるコスト
                else if (cost is DiscardHandCost)
                {
                    if (((DiscardHandCost)cost).DiscardCount > card().Owner.HandCards.Count((cardSource) => ((DiscardHandCost)cost).CanTargetCondition(cardSource)))
                    {
                        return false;
                    }
                }
                #endregion

                #region 手札を山札の上に置くコスト
                else if (cost is PutHandLibraryTopCost)
                {
                    if (((PutHandLibraryTopCost)cost).SelectCount > card().Owner.HandCards.Count((cardSource) => ((PutHandLibraryTopCost)cost).CanTargetCondition(cardSource)))
                    {
                        return false;
                    }
                }
                #endregion

                #region 自分のオーブを撃破するコスト
                else if (cost is BreakOrbCost)
                {
                    if (((BreakOrbCost)cost).BreakCount > ((BreakOrbCost)cost).player.OrbCards.Count)
                    {
                        return false;
                    }
                }
                #endregion

                #region 自身を撃破するコスト
                else if(cost is DestroySelfCost)
                {
                    if (card() == null)
                    {
                        return false;
                    }

                    if (card().UnitContainingThisCharacter() == null)
                    {
                        return false;
                    }
                }
                #endregion
            }
        }
        #endregion

        #region カード毎の使用可否判定
        foreach (Func<Hashtable,bool> CanUseCondition in CanUseConditions)
        {
            if(!CanUseCondition(hash))
            {
                return false;
            }
        }
        #endregion

        #region 使用回数による成否判定
        if (card().cEntity_EffectController.IsOverMaxCountPerTurn(EffectName,MaxCountPerTurn))
        {
            return false;
        }
        #endregion

        #region 無効化による成否判定
        if(IsInvalidate)
        {
            return false;
        }
        #endregion

        #region CCSによる成否判定
        if (isCCS)
        {
            bool isCC = false;

            if(CCUnit != null)
            {
                if (CCUnit.Character != null)
                {
                    if (CCUnit.IsClassChanged())
                    {
                        isCC = true;
                    }
                }
            }
            

            if(!isCC)
            {
                return false;
            }
        }
        #endregion

        #region LvSによる成否判定
        if(isLvS)
        {
            bool isLv = false;

            if (LvUnit != null)
            {
                if (LvUnit.Characters.Count >= Lv)
                {
                    isLv = true;
                }
            }


            if (!isLv)
            {
                return false;
            }
        }
        #endregion

        #region BSによる成否判定
        if(isBS)
        {
            if(card() != null)
            {
                if (!card().Owner.BondCards.Contains(card())||card().IsReverse)
                {
                    return false;
                }
            }
        }
        #endregion

        return true;
    }
    #endregion

    #region 選択が終了したか
    public bool endSelect { get; set; }
    #endregion

    #region コスト
    public List<Cost> costs = new List<Cost>();
    #endregion

    #region クラスチェンジスキル
    public bool isCCS { get; set; }
    public Unit CCUnit { get; set; }
    public void SetCCS(Unit CCUnit)
    {
        isCCS = true;
        this.CCUnit = CCUnit;
    }
    #endregion

    #region レベルスキル
    public bool isLvS { get; set; }
    public Unit LvUnit { get; set; }
    public int Lv { get; set; }
    public void SetLvS(Unit LvUnit,int Lv)
    {
        isLvS = true;
        this.LvUnit = LvUnit;
        this.Lv = Lv;
    }
    #endregion

    #region カルネージフォーム
    public bool isCF { get; set; }
    public void SetCF()
    {
        this.isCF = true;
    }
    #endregion

    #region BS
    public bool isBS { get; set; }
    public void SetBS()
    {
        this.isBS = true;
    }
    #endregion
}

#region スキルのコスト
public class Cost
{

}

public class ReverseCost : Cost
{
    public ReverseCost(int ReverseCount, Func<CardSource, bool> CanTargetCondition)
    {
        this.ReverseCount = ReverseCount;
        this.CanTargetCondition = CanTargetCondition;
    }

    public int ReverseCount { get; set; }
    public Func<CardSource, bool> CanTargetCondition;
}

public class ReverseSelfCost:Cost
{

}

public class TapCost : Cost
{

}

public class SelectAllyCost : Cost
{
    public SelectAllyCost(
        Player SelectPlayer,
        Func<Unit, bool> CanTargetCondition,
        Func<List<Unit>, Unit, bool> CanTargetCondition_ByPreSelecetedList,
        Func<List<Unit>, bool> CanEndSelectCondition,
        int MaxCount,
        bool CanNoSelect,
        bool CanEndNotMax,
        Func<Unit, IEnumerator> SelectUnitCoroutine,
        Func<List<Unit>, IEnumerator> AfterSelectUnitCoroutine,
        SelectUnitEffect.Mode mode)
    {
        this.SelectPlayer = SelectPlayer;
        this.CanTargetCondition = CanTargetCondition;
        this.CanTargetCondition_ByPreSelecetedList = CanTargetCondition_ByPreSelecetedList;
        this.CanEndSelectCondition = CanEndSelectCondition;
        this.MaxCount = MaxCount;
        this.CanNoSelect = CanNoSelect;
        this.CanEndNotMax = CanEndNotMax;
        this.SelectUnitCoroutine = SelectUnitCoroutine;
        this.AfterSelectUnitCoroutine = AfterSelectUnitCoroutine;
        this.mode = mode;
    }

    public Player SelectPlayer { get; set; }
    public Func<Unit, bool> CanTargetCondition { get; set; }
    public Func<List<Unit>, Unit, bool> CanTargetCondition_ByPreSelecetedList { get; set; }
    public Func<List<Unit>, bool> CanEndSelectCondition { get; set; }
    public int MaxCount { get; set; }
    public bool CanNoSelect { get; set; }
    public bool CanEndNotMax { get; set; }
    public Func<Unit, IEnumerator> SelectUnitCoroutine { get; set; }
    public Func<List<Unit>, IEnumerator> AfterSelectUnitCoroutine { get; set; }
    public SelectUnitEffect.Mode mode { get; set; }
}

public class BreakOrbCost:Cost
{
    public BreakOrbCost(Player player,int BreakCount, BreakOrbMode breakOrbMode)
    {
        this.player = player;
        this.BreakCount = BreakCount;
        this.breakOrbMode = breakOrbMode;
    }

    public Player player;
    public int BreakCount { get; set; }
    public BreakOrbMode breakOrbMode { get; set; }
}

public class DiscardHandCost : Cost
{
    public DiscardHandCost(int DiscardCount, Func<CardSource, bool> CanTargetCondition)
    {
        this.DiscardCount = DiscardCount;
        this.CanTargetCondition = CanTargetCondition;
    }

    public int DiscardCount { get; set; }
    public Func<CardSource, bool> CanTargetCondition;
}

public class PutHandLibraryTopCost : Cost
{
    public PutHandLibraryTopCost(int SelectCount, Func<CardSource, bool> CanTargetCondition,bool isShowOpponent)
    {
        this.SelectCount = SelectCount;
        this.CanTargetCondition = CanTargetCondition;
        this.isShowOpponent = isShowOpponent;
    }

    public int SelectCount { get; set; }
    public Func<CardSource, bool> CanTargetCondition;
    public bool isShowOpponent { get; set; }
}


public class DestroySelfCost:Cost
{

}

public class CustomCost:Cost
{
    public CustomCost(IEnumerator PayCostCoroutine)
    {
        this.PayCostCoroutine = PayCostCoroutine;
    }

    public IEnumerator PayCostCoroutine { get; set; }

}

#endregion

#region カード効果が発動するタイミング
public enum EffectTiming
{
    OnDeclaration,
    OnEnterFieldAnyone,
    OnStartDeployPhase,
    OnCCAnyone,
    OnLevelUpAnyone,
    OnGrowAnyone,
    OnDestroyDuringBattleAlly,
    OnDestroyedDuringBattleAlly,
    OnEndBattle,
    OnAttackAnyone,
    OnAttackedAlly,
    OnSetSupport,
    OnSetSupportBeforeSupportSkill,
    OnDiscardSuppot,
    OnDestroyedAnyone,
    OnEndTurn,
    OnStartTurn,
    OnMovedAnyone,
    OnEvadeAnyone,
    OnCriticalAnyone,
    OnEndAttackAnyone,
    OnDiscardHand,
    OnUnTappedAnyone,
    OnOpponentShowLibraryBySkill,
    OnAddHandCardFromTrash,
    BeforeDiscardCritical_EvasionCard,
    OnSetBond,
    None,
}
#endregion

#region その場で処理する効果
public interface ActivateICardEffect
{
    IEnumerator Activate(Hashtable hash);
}

public static class ActivateICardEffectExtensionClass
{
    #region エフェクト→任意効果発動選択→コスト→処理
    public static IEnumerator Activate_Effect_Optional_Cost_Execute(this ActivateICardEffect activateICardEffect, Hashtable hash)
    {
        GManager.instance.turnStateMachine.isExecuting = true;

        if (activateICardEffect is ICardEffect)
        {
            //エフェクト
            yield return ContinuousController.instance.StartCoroutine(Activate_Effect(activateICardEffect));

            //任意効果発動選択→コスト→処理
            yield return ContinuousController.instance.StartCoroutine(Activate_Optional_Cost_Execute(activateICardEffect, hash, null));
        }

        foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players)
        {
            player.TrashHandCard.gameObject.SetActive(false);
            player.TrashHandCard.SkillNameText.transform.parent.gameObject.SetActive(false);
            player.SupportHandCard.SkillNameText.transform.parent.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(Time.deltaTime * 2);

        CardSource card = ((ICardEffect)activateICardEffect).card();

        if (card != null)
        {
            if (card.UnitContainingThisCharacter() != null)
            {
                if (card.UnitContainingThisCharacter().ShowingFieldUnitCard != null)
                {
                    card.UnitContainingThisCharacter().ShowingFieldUnitCard.OffUsingSkillEffect();
                    card.UnitContainingThisCharacter().ShowingFieldUnitCard.OffSkillName();
                }
            }

            if (card.ShowingHandCard != null)
            {
                card.ShowingHandCard.SkillNameText.transform.parent.gameObject.SetActive(false);
            }
        }

        GManager.instance.turnStateMachine.isExecuting = false;
    }
    #endregion

    #region 任意効果発動選択→エフェクト→コスト→処理
    public static IEnumerator Activate_Optional_Effect_Cost_Execute(this ActivateICardEffect activateICardEffect, Hashtable hash)
    {
        GManager.instance.turnStateMachine.isExecuting = true;

        if (activateICardEffect is ICardEffect)
        {
            CardSource card = ((ICardEffect)activateICardEffect).card();

            if (card != null)
            {
                if (card.Owner.isYou)
                {
                    if (card.ShowingHandCard != null)
                    {
                        card.ShowingHandCard.Outline_Select.SetActive(true);
                        card.ShowingHandCard.SetOrangeOutline();
                    }
                }
            }

            //任意効果発動選択
            if (((ICardEffect)activateICardEffect).Optional)
            {
                yield return ContinuousController.instance.StartCoroutine(Activate_Optional(activateICardEffect, null, false));
            }

            if (((ICardEffect)activateICardEffect).UseOptional || !((ICardEffect)activateICardEffect).Optional)
            {
                //エフェクト
                yield return ContinuousController.instance.StartCoroutine(Activate_Effect(activateICardEffect));

                //コスト→処理
                yield return ContinuousController.instance.StartCoroutine(Activate_Cost_Execute(activateICardEffect, hash));
            }

            if (card != null)
            {
                if (card.ShowingHandCard != null)
                {
                    card.ShowingHandCard.Outline_Select.SetActive(false);
                    card.ShowingHandCard.ShowOpponent = false;
                }
            }
        }

        GManager.instance.turnStateMachine.isExecuting = false;
    }
    #endregion

    #region カード効果発動エフェクト表示
    public static IEnumerator Activate_Effect(this ActivateICardEffect activateICardEffect)
    {
        CardSource card = ((ICardEffect)activateICardEffect).card();

        if (card != null)
        {
            Unit unit = card.UnitContainingThisCharacter();

            if (unit != null)
            {
                if (unit.ShowingFieldUnitCard != null)
                {
                    yield return ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ActivateFieldUnitSkillEffect(unit, (ICardEffect)activateICardEffect));
                }
            }

            else if (card.Owner.SupportCards.Contains(card))
            {
                yield return ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ActivateSupportCardSkillEffect(card, (ICardEffect)activateICardEffect));
            }

            else if (card.Owner.HandCards.Contains(card))
            {
                if (card.ShowingHandCard != null)
                {
                    yield return ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ActivateHandCardSkillEffect(card, (ICardEffect)activateICardEffect));
                }
            }

            else if (card.Owner.TrashCards.Contains(card) || card.Owner.InfinityCards.Contains(card))
            {
                yield return ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ActivateTrashInfinityCardSkillEffect(card, (ICardEffect)activateICardEffect));
            }

            else if (card.Owner.BondCards.Contains(card))
            {
                yield return ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ActivateBondCardSkillEffect(card, (ICardEffect)activateICardEffect));
            }
        }
    }
    #endregion

    #region 任意効果発動選択
    public static IEnumerator Activate_Optional(this ActivateICardEffect activateICardEffect, string Message, bool ShowOpponentMessage)
    {
        if (((ICardEffect)activateICardEffect).Optional)
        {
            yield return ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<OptionalSkill>().SelectOptional((ICardEffect)activateICardEffect, Message, ShowOpponentMessage));
        }
    }
    #endregion

    #region コスト→処理
    public static IEnumerator Activate_Cost_Execute(this ActivateICardEffect activateICardEffect, Hashtable hash)
    {
        if (((ICardEffect)activateICardEffect).UseOptional || !((ICardEffect)activateICardEffect).Optional)
        {
            //使用回数をカウントアップ
            ((ICardEffect)activateICardEffect).card().cEntity_EffectController.CountUpUseCountsThisTurn((ICardEffect)activateICardEffect);

            //コストを処理
            if (activateICardEffect is ICardEffect)
            {
                ((ICardEffect)activateICardEffect).DonePayCost = false;

                yield return ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<PayCostClass>().PayCost(((ICardEffect)activateICardEffect).costs, ((ICardEffect)activateICardEffect).card(), (ICardEffect)activateICardEffect));
                
                if(!((ICardEffect)activateICardEffect).DonePayCost)
                {
                    yield break;
                }
            }

            //効果を処理
            yield return ContinuousController.instance.StartCoroutine(activateICardEffect.Activate(hash));
        }
    }
    #endregion

    #region 任意効果発動選択→コスト→処理
    public static IEnumerator Activate_Optional_Cost_Execute(this ActivateICardEffect activateICardEffect, Hashtable hash, string Message)
    {
        //任意効果発動選択
        if (((ICardEffect)activateICardEffect).Optional)
        {
            yield return ContinuousController.instance.StartCoroutine(Activate_Optional(activateICardEffect, Message, true));
        }

        //コスト→処理
        if (((ICardEffect)activateICardEffect).UseOptional || !((ICardEffect)activateICardEffect).Optional)
        {
            yield return ContinuousController.instance.StartCoroutine(Activate_Cost_Execute(activateICardEffect, hash));
        }
    }
    #endregion
}
#endregion

#region 「対象の攻撃ユニットは対象の防御ユニットを射程に関係なく攻撃できる」を付与する効果
public interface ICanAttackTargetUnitRegardlessRangeEffect
{
    bool CanAttackTargetUnitRegardlessRange(Unit AttackingUnit, Unit DefendingUnit);
}
#endregion

#region 「対象のスキルは無効化される」を付与する効果
public interface IInvalidationCardEffect
{
    bool IsInvalidate(ICardEffect cardEffect);
}
#endregion

#region 「対象の攻撃のユニットは対象の防御ユニットに神速回避されない」を付与する効果
public interface ICanNotBeEvadedCardEffect
{
    bool CanNotBeEvaded(Unit AttackingUnit,Unit DefendingUnit);
}
#endregion

#region 「対象のユニットは必殺攻撃できない」を付与する効果
public interface ICanNotCriticalCardEffect
{
    bool CanNotCritical(Unit unit);
}
#endregion

#region 「対象のスキルの発動回数を変化させる」効果
public interface IChangeSkillActivateCountEffect
{
    int GetActivateCount(ICardEffect cardEffect,int ActiveCount);
}
#endregion

#region 「対象のユニットはレベルアップできない」を付与する効果
public interface ICanNotLevelUpEffect
{
    bool CanNotLevelUp(Unit unit);
}
#endregion

#region 「対象のユニットはスキルで移動できない」を付与する効果
public interface ICanNotMoveBySkillEffect
{
    bool CanNotMoveBySkill(Unit unit);
}
#endregion

#region 「対象のユニットは移動できない」を付与する効果
public interface ICanNotMoveEffect
{
    bool CanNotMove(Unit unit);
}
#endregion

#region 「対象のユニットを攻撃できない」を付与する効果
public interface ICanNotAttackTargetUnitCardEffect
{
    bool CanNotAttackTargetUnit(Unit Attacker, Unit Defender);
}
#endregion

#region 「対象のカードは対象のユニットの支援に必ず成功する」効果
public interface ISuccessSupportTargetUnitCardEffect
{
    bool SuccessSupporTargetUnit(CardSource SupportCard, Unit TargetUnit);
}
#endregion

#region 「対象のカードは対象のユニットを支援できない」効果
public interface ICanNotSupportEffect
{
    bool CanNotSupporTargetUnit(CardSource SupportCard, Unit TargetUnit);
}
#endregion

#region 「このカードは絆に置けない」効果
public interface ICanNotSetBondCardEffect
{
    bool CanNotSetBond(CardSource cardSource);
}
#endregion

#region 「このカードはプレイできない」効果
public interface ICanNotPlayCardEffect
{
    bool CanNotPlay(CardSource cardSource);
}
#endregion

#region 「対象のカードは同名カードが既に場にあってもプレイできる」効果
public interface ICanPlayEvenIfExistSameUnit
{
    bool CanPlayEvenIfExistSameUnit(CardSource cardSource);
}
#endregion

#region オーブダメージを変化させる効果

public interface IStrikeModifyCardEffect
{
    int GetDamage(int Damage, Unit unit);
    bool isUpDown();
}

#endregion

#region 必殺攻撃倍率を変化させる効果
public interface IChangeCriticalMagnificationEffect
{
    int GetCriticalMagnification(Unit unit,int CriticalMagnification);
}
#endregion

#region パワーを変化させる効果
public interface IPowerModifyCardEffect
{
    int GetPower(int Power, Unit unit);
    bool isUpDown();
}
#endregion

#region 支援力を変化させる効果
public interface IChangeSupportPowerCardEffect
{
    int GetSupportPower(int SupportPower, CardSource cardSource);
}
#endregion

#region 「対象のカードを対象のユニットにCCさせる時のCCコスト」を変化させる効果
public interface IChangeCCCostEffect
{
    int GetCCCost(int CCCost,Unit targetUnit ,CardSource cardSource);
}
#endregion

#region 「対象のカードをプレイするコスト」を変化させる効果
public interface IChangePlayCost
{
    int GetPlayCost(int PlayCost, CardSource cardSource);
    bool isUpDown();
}
#endregion

#region 「対象のカードにクラスチェンジコストを持たせる」を付与する効果
public interface IAddHasCCEffect
{
    bool HasCC(CardSource cardSource, Unit targetUnit);
}
#endregion

#region 「対象のユニットの上に重ねられる」効果を対象のカードに付与する効果
public interface ICanLevelUpToThisCardEffect
{
    bool CanLevelUpToThisCard(CardSource cardSource, Unit targetUnit);
}
#endregion

#region 射程を変化させる効果
public interface IChangeRangeEffect
{
    List<int> GetRange(List<int> Range, Unit unit);
}
#endregion

#region カードの色を変化させる効果
public interface IChangeCardColorsEffect
{
    List<CardColor> GetCardColors(List<CardColor> CardColors, CardSource cardSource);
}
#endregion

#region カードの性別を変化させる効果
public interface IChangeSexEffect
{
    List<Sex> GetSex(List<Sex> sex);
}
#endregion

#region ユニット名を変化させる効果
public interface IChangeUnitNameEffect
{
    List<string> GetUnitNames(List<string> UnitNames, CardSource cardSource);
}
#endregion

#region 「対象の攻撃ユニットが対象のユニット(主人公以外)を撃破した時にを墓地以外の領域に送る」を付与する効果
public interface IChangePlaceDestroyedUnitEffect
{
    DestroyMode GetDestroyMode(Unit Defender);
}
#endregion

#region 武器を変化させる効果
public interface IChangeWeaponEffect
{
    List<Weapon> GetWeapon(List<Weapon> Weapons, CardSource cardSource);
}
#endregion

#region 「対象の防御ユニットは対象の攻撃ユニットの攻撃では撃破されない」を付与する効果
public interface ICanNotDestroyedByBattle
{
    bool CanNotDestroyedByBattle(Unit AttackingUnit, Unit DefendingUnit);
}
#endregion

#region 「対象のユニットはスキルで撃破されない」を付与する効果
public interface ICanNotDestroyedBySkill
{
    bool CanNotDestroyedBySkill(Unit unit, ICardEffect skill);
}
#endregion

#region 「対象のユニットはスキルのコストで撃破されない」を付与する効果
public interface ICanNotDestroyedByCost
{
    bool CanNotDestroyedByCost(Unit unit);
}
#endregion

#region 「対象のユニットはターン開始時にアンタップしない」を付与する効果
public interface ICanNotUntapOnStartTurnEffect
{
    bool CanNotUntapOnStartTurn(Unit unit);
}
#endregion

#region 「対象のカードに特定のスキルを与える」効果
public interface IAddSkillEffect
{
    List<ICardEffect> GetCardEffect(CardSource card, List<ICardEffect> GetCardEffects, EffectTiming timing);
}
#endregion


