using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Unit 
{
    public Unit(List<CardSource> _Characters)
    {
        Characters = new List<CardSource>();

        foreach (CardSource _soul in _Characters)
        {
            Characters.Add(_soul);
        }
    }

    #region 重なっているカード
    public List<CardSource> Characters = new List<CardSource>();
    #endregion

    #region 一番上のカード
    public CardSource Character
    {
        get
        {
            if (Characters.Count > 0)
            {
                return Characters[0];
            }

            return null;
        }
    }
    #endregion

    #region タップされているか
    public bool OldIsTapped;
    public bool IsTapped;
    #endregion

    #region 自分のターン終了時にリセットされる効果
    public List<ICardEffect> UntilOwnerTurnEndUnitEffects = new List<ICardEffect>();
    #endregion

    #region お互いのターン終了時にリセットされる効果
    public List<ICardEffect> UntilEachTurnEndUnitEffects = new List<ICardEffect>();
    #endregion

    #region 相手ターン終了時にリセットされる効果
    public List<ICardEffect> UntilOpponentTurnEndEffects = new List<ICardEffect>();
    #endregion

    #region 攻撃終了時にリセットされる効果
    public List<ICardEffect> UntilEndBattleEffects = new List<ICardEffect>();
    #endregion

    #region 武器
    public List<Weapon> Weapons
    {
        get
        {
            List<Weapon> Weapons = new List<Weapon>();

            if(Character != null)
            {
                foreach (Weapon weapon in Character.Weapons)
                {
                    Weapons.Add(weapon);
                }
            }

            return Weapons;
        }
    }
    #endregion

    #region ユニットのパワー
    public int Power
    {
        get
        {
            int power = 0;

            if(Character != null)
            {
                power = Character.cEntity_Base.Power;

                #region ターン終了時までの効果
                foreach(Player player in GManager.instance.turnStateMachine.gameContext.Players)
                {
                    foreach(Unit unit in player.FieldUnit)
                    {
                        foreach (ICardEffect cardEffect in unit.UntilEachTurnEndUnitEffects)
                        {
                            if (cardEffect is IChangePowerCardEffect)
                            {
                                if (cardEffect.CanUse(null))
                                {
                                    power = ((IChangePowerCardEffect)cardEffect).GetPower(power, this);
                                }

                            }
                        }
                    }
                }

                #endregion

                #region 相手ターン終了時までの効果
                foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players)
                {
                    foreach (Unit unit in player.FieldUnit)
                    {
                        foreach (ICardEffect cardEffect in unit.UntilOpponentTurnEndEffects)
                        {
                            if (cardEffect is IChangePowerCardEffect)
                            {
                                if (cardEffect.CanUse(null))
                                {
                                    power = ((IChangePowerCardEffect)cardEffect).GetPower(power, this);
                                }
                            }
                        }
                    }
                }
                
                #endregion

                #region 支援
                if (GManager.instance != null)
                {
                    if(GManager.instance.turnStateMachine != null)
                    {
                        if(GManager.instance.turnStateMachine.AttackingUnit == this || GManager.instance.turnStateMachine.DefendingUnit == this)
                        {
                            foreach (CardSource SupportCard in Character.Owner.SupportCards)
                            {
                                if (!this.CanNotSupportThisUnit(SupportCard))
                                {
                                    power += SupportCard.SupportPower;
                                }
                            }
                        }
                    }
                }
                #endregion

                #region 支援スキル
                if (GManager.instance != null)
                {
                    if (GManager.instance.turnStateMachine != null)
                    {
                        if (GManager.instance.turnStateMachine.AttackingUnit == this || GManager.instance.turnStateMachine.DefendingUnit == this)
                        {
                            foreach (CardSource SupportCard in Character.Owner.SupportCards)
                            {
                                if (!this.CanNotSupportThisUnit(SupportCard))
                                {
                                    foreach(ICardEffect cardEffect in SupportCard.cEntity_EffectController.GetSupportEffects(EffectTiming.None))
                                    {
                                        if (cardEffect is IChangePowerCardEffect)
                                        {
                                            if (cardEffect.CanUse(null))
                                            {
                                                power = ((IChangePowerCardEffect)cardEffect).GetPower(power, this);
                                            }
                                                
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                #region プレイヤーにかかる効果
                foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players)
                {
                    foreach(ICardEffect cardEffect in player.PlayerEffects)
                    {
                        if (cardEffect is IChangePowerCardEffect)
                        {
                            if (cardEffect.CanUse(null))
                            {
                                power = ((IChangePowerCardEffect)cardEffect).GetPower(power, this);
                            }
                                
                        }
                    }
                }
                #endregion

                #region スキルの効果
                foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players)
                {
                    foreach(Unit unit in player.FieldUnit)
                    {
                        foreach(ICardEffect cardEffect in unit.Character.cEntity_EffectController.GetCardEffects(EffectTiming.None))
                        {
                            if (cardEffect is IChangePowerCardEffect)
                            {
                                if(cardEffect.CanUse(null))
                                {
                                    power = ((IChangePowerCardEffect)cardEffect).GetPower(power, this);
                                }
                            }
                        }
                    }
                }
                #endregion

                #region バトル終了時までの効果(必殺攻撃含む)
                foreach (ICardEffect cardEffect in UntilEndBattleEffects)
                {
                    if (cardEffect is IChangePowerCardEffect)
                    {
                        if (cardEffect.CanUse(null))
                        {
                            power = ((IChangePowerCardEffect)cardEffect).GetPower(power, this);
                        }
                           
                    }
                }
                #endregion
            }

            if(power < 0)
            {
                power = 0;
            }

            return power;
        }
    }
    #endregion

    #region このユニットが必殺攻撃できるか
    public bool CanCritical
    {
        get
        {
            #region 敵の支援カードの効果
            foreach (CardSource cardSource in Character.Owner.Enemy.SupportCards)
            {
                foreach(ICardEffect cardEffect in cardSource.cEntity_EffectController.GetSupportEffects(EffectTiming.None))
                {
                    if (cardEffect is ICanNotCriticalCardEffect)
                    {
                        if (cardEffect.CanUse(null))
                        {
                            if (((ICanNotCriticalCardEffect)cardEffect).CanNotCritical(this))
                            {
                                return false;
                            }
                        }

                    }
                }
            }
            #endregion

            #region 自身の効果
            foreach (ICardEffect cardEffect in EffectList(EffectTiming.None))
            {
                if (cardEffect is ICanNotCriticalCardEffect)
                {
                    if (cardEffect.CanUse(null))
                    {
                        if (((ICanNotCriticalCardEffect)cardEffect).CanNotCritical(this))
                        {
                            return false;
                        }
                    }
                }
            }
            #endregion

            return true;
        }
    }
    #endregion

    #region 必殺攻撃倍率
    public int CriricalMagnification
    {
        get
        {
            int CriticalMagnification = 2;

            #region スキルの効果
            foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players)
            {
                foreach (Unit unit in player.FieldUnit)
                {
                    foreach (ICardEffect cardEffect in unit.EffectList(EffectTiming.None))
                    {
                        if (cardEffect is IChangeCriticalMagnificationEffect)
                        {
                            if (cardEffect.CanUse(null))
                            {
                                CriticalMagnification = ((IChangeCriticalMagnificationEffect)cardEffect).GetCriticalMagnification(this,CriticalMagnification);
                            }
                        }
                    }
                }
            }
            #endregion

            return CriticalMagnification;
        }
    }
    #endregion

    #region このユニットの攻撃を対象の防御ユニットが神速回避出来るか
    public bool CanBeEvaded(Unit DefendingUnit)
    {
        foreach (ICardEffect cardEffect in EffectList(EffectTiming.None))
        {
            if (cardEffect is ICanNotBeEvadedCardEffect)
            {
                if (cardEffect.CanUse(null))
                {
                    if (((ICanNotBeEvadedCardEffect)cardEffect).CanNotBeEvaded(this, DefendingUnit))
                    {
                        return false;
                    }
                }

            }
        }

        return true;
    }
    #endregion

    #region 1回の攻撃で与えるダメージ
    public int Strike
    {
        get
        {
            int strike = 1;

            foreach (ICardEffect effect in EffectList(EffectTiming.None))
            {
                if (effect is IChangeDamageCardEffect)
                {
                    if(effect.CanUse(null))
                    {
                        strike = ((IChangeDamageCardEffect)effect).GetDamage(strike, this);
                    }
                    
                }
            }

            #region 支援スキル
            if (GManager.instance != null)
            {
                if (GManager.instance.turnStateMachine != null)
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit == this || GManager.instance.turnStateMachine.DefendingUnit == this)
                    {
                        foreach (CardSource SupportCard in Character.Owner.SupportCards)
                        {
                            if (!this.CanNotSupportThisUnit(SupportCard))
                            {
                                foreach (ICardEffect cardEffect in SupportCard.cEntity_EffectController.GetSupportEffects(EffectTiming.None))
                                {
                                    if (cardEffect is IChangeDamageCardEffect)
                                    {
                                        if(cardEffect.CanUse(null))
                                        {
                                            strike = ((IChangeDamageCardEffect)cardEffect).GetDamage(strike, this);
                                        }
                                        
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            return strike;
        }
    }
    #endregion

    #region このマジンを表示している場のカードオブジェクト
    public FieldUnitCard ShowingFieldUnitCard { get; set; }
    #endregion

    #region そのユニットにかかる効果全て
    public List<ICardEffect> EffectList(EffectTiming timing)
    {
        List<ICardEffect> _EffectList = new List<ICardEffect>();

        if (Character != null)
        {
            foreach (ICardEffect cardEffect in UntilOwnerTurnEndUnitEffects)
            {
                _EffectList.Add(cardEffect);
            }

            foreach (ICardEffect cardEffect in UntilEachTurnEndUnitEffects)
            {
                _EffectList.Add(cardEffect);
            }

            foreach (ICardEffect cardEffect in UntilEndBattleEffects)
            {
                _EffectList.Add(cardEffect);
            }

            foreach (ICardEffect cardEffect in UntilOpponentTurnEndEffects)
            {
                _EffectList.Add(cardEffect);
            }

            foreach (ICardEffect cardEffect in Character.cEntity_EffectController.GetCardEffects(timing))
            {
                _EffectList.Add(cardEffect);
            }
        }

        return _EffectList;
    }
    #endregion

    #region スキルの効果で移動できるか
    public bool CanMoveBySkill
    {
        get
        {
            foreach(Player player in GManager.instance.turnStateMachine.gameContext.Players)
            {
                foreach(Unit unit in player.FieldUnit)
                {
                    foreach(ICardEffect cardEffect in unit.EffectList(EffectTiming.None))
                    {
                        if (cardEffect is ICanNotMoveBySkillEffect)
                        {
                            if (cardEffect.CanUse(null))
                            {
                                if (((ICanNotMoveBySkillEffect)cardEffect).CanNotMoveBySkill(this))
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }
    }
    #endregion

    #region 行動フェイズ中に移動できるか
    public bool CanMoveDurinAction
    {
        get
        {
            if(this.IsTapped)
            {
                return false;
            }

            return true;
        }
    }
    #endregion

    #region このユニットは対象のユニットを射程に関係なく攻撃できるか
    public bool CanAttackTargetUnitRegardlessRange(Unit targetUnit)
    {
        foreach(Player player in GManager.instance.turnStateMachine.gameContext.Players)
        {
            foreach(Unit unit in player.FieldUnit)
            {
                foreach(ICardEffect cardEffect in unit.EffectList(EffectTiming.None))
                {
                    if(cardEffect is ICanAttackTargetUnitRegardlessRangeEffect)
                    {
                        if(cardEffect.CanUse(null))
                        {
                            if(((ICanAttackTargetUnitRegardlessRangeEffect)cardEffect).CanAttackTargetUnitRegardlessRange(this,targetUnit))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }
    #endregion

    #region 攻撃できるか
    #region このユニットが攻撃できるか
    public bool CanAttack
    {
        get
        {
            if (Character != null)
            {
                return Character.Owner.Enemy.FieldUnit.Count((unit) => CanAttachTargetUnit(unit)) > 0;
            }
                
            return false;
        }
    }
    #endregion

    #region このユニットが対象のユニットを攻撃できるか
    public bool CanAttachTargetUnit(Unit targetUnit)
    {
        if (Character != null)
        {
            if (this.IsTapped)
            {
                return false;
            }

            if (!new CheckRange(this, targetUnit).CanReachRange() && !CanAttackTargetUnitRegardlessRange(targetUnit))
            {
                return false;
            }

            if (GManager.instance.turnStateMachine.isFirstPlayerFirstTurn)
            {
                return false;
            }

            #region 攻撃できないを付与されていたら攻撃できない
            foreach(Player player in GManager.instance.turnStateMachine.gameContext.Players)
            {
                foreach(Unit _unit in player.FieldUnit)
                {
                    if(_unit.Character != null)
                    {
                        foreach (ICardEffect cardEffect in _unit.EffectList(EffectTiming.None))
                        {
                            if(cardEffect is ICanNotAttackTargetUnitCardEffect)
                            {
                                if(cardEffect.CanUse(null))
                                {
                                    if (((ICanNotAttackTargetUnitCardEffect)cardEffect).CanNotAttackTargetUnit(this, targetUnit))
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            #endregion

            return true;
        }

        return false;
    }
    #endregion

    #endregion

    #region 起動効果を宣言できる
    #region 宣言できる起動効果が一つでもある
    public bool CanDeclareSkill(Hashtable hash)
    {
        return CanDeclareSkillList(hash).Count > 0;
    }
    #endregion

    #region その起動効果を宣言できる
    public bool CanDeclareThisSkill(ICardEffect cardEffect,Hashtable hash)
    {
        if (Character != null)
        {
            foreach (ICardEffect _cardEffect in Character.cEntity_EffectController.GetCardEffects(EffectTiming.OnDeclaration))
            {
                if (_cardEffect is ActivateICardEffect)
                {
                    if(_cardEffect == cardEffect)
                    {
                        if (cardEffect.CanUse(hash))
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }
    #endregion

    #region 宣言できる起動効果リスト
    public List<ICardEffect> CanDeclareSkillList(Hashtable hash)
    {
        List<ICardEffect> CanDeclareSkillList = new List<ICardEffect>();

        if (Character != null)
        {
            foreach (ICardEffect _cardEffect in Character.cEntity_EffectController.GetCardEffects(EffectTiming.OnDeclaration))
            {
                if (CanDeclareThisSkill(_cardEffect,hash))
                {
                    CanDeclareSkillList.Add(_cardEffect);
                }
            }
        }

        return CanDeclareSkillList;
    }
    #endregion
    #endregion

    #region 対象の支援カードがこのユニットの支援に失敗するか
    public bool CanNotSupportThisUnit(CardSource SupportCard)
    {
        #region スキルの効果で必ず支援に成功する場合は成功
        foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players)
        {
            foreach (Unit unit in player.FieldUnit)
            {
                if (unit.Character != null)
                {
                    foreach (ICardEffect cardEffect in EffectList(EffectTiming.None))
                    {
                        if (cardEffect is ISuccessSupportTargetUnitCardEffect)
                        {
                            if (cardEffect.CanUse(null))
                            {
                                if (((ISuccessSupportTargetUnitCardEffect)cardEffect).SuccessSupporTargetUnit(SupportCard, this))
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        if (this.Character != null)
        {
            if (CardSource.IsSameUnitName(this.Character, SupportCard))
            {
                return true;
            }
        }

        #region スキルの効果で支援に失敗する場合
        foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players)
        {
            foreach(Unit unit in player.FieldUnit)
            {
                if(unit.Character != null)
                {
                    foreach(ICardEffect cardEffect in EffectList(EffectTiming.None))
                    {
                        if (cardEffect is ICanNotSupportEffect)
                        {
                            if (cardEffect.CanUse(null))
                            {
                                if (((ICanNotSupportEffect)cardEffect).CanNotSupporTargetUnit(SupportCard, this))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        return false;
    }
    #endregion

    #region 射程
    public List<int> Range
    {
        get
        {
            List<int> Range = new List<int>();

            foreach(int range in Character.Range)
            {
                if(!Range.Contains(range))
                {
                    Range.Add(range);
                }
            }

            foreach(Player player in GManager.instance.turnStateMachine.gameContext.Players)
            {
                foreach(Unit unit in player.FieldUnit)
                {
                    foreach(ICardEffect cardEffect in unit.EffectList(EffectTiming.None))
                    {
                        if(cardEffect is IChangeRangeEffect)
                        {
                            if(cardEffect.CanUse(null))
                            {
                                Range = ((IChangeRangeEffect)cardEffect).GetRange(Range, this);
                            }
                        }
                    }
                }
            }

            Range = Range.Distinct().ToList();
            Range.Sort();

            return Range;
        }
    }
    #endregion

    #region クラスチェンジしているか
    public bool IsClassChanged()
    {
        if (Characters.Count > 1)
        {
            if (Character.cEntity_Base.HasCC)
            {
                return true;
            }
        }

        return false;
    }
    #endregion

    #region レベルアップしているか
    public bool IsLevelUp()
    {
        if (Characters.Count > 1)
        {
            return true;
        }

        return false;
    }
    #endregion

    #region このターンに攻撃しているか
    public bool DoneAttackThisTurn { get; set; } = false;
    #endregion

    #region このターンに移動しているか
    public bool DoneMoveThisTurn { get; set; } = false;
    #endregion

    #region タップする
    public IEnumerator Tap()
    {
        IsTapped = true;

        yield return null;
    }
    #endregion

    #region アンタップする
    public IEnumerator UnTap(Hashtable hashtable)
    {
        IsTapped = false;

        #region アンタップ時の効果
        List<SkillInfo> skillInfos = new List<SkillInfo>();
        foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players)
        {
            foreach (Unit unit in player.FieldUnit)
            {
                foreach(ICardEffect cardEffect in unit.EffectList(EffectTiming.OnUnTappedAnyone))
                {
                    if(cardEffect is ActivateICardEffect)
                    {
                        #region Hashtableを設定
                        Hashtable _hashtable = new Hashtable();
                        _hashtable.Add("Unit", this);

                        if(hashtable != null)
                        {
                            if(hashtable.ContainsKey("cardEffect"))
                            {
                                if(hashtable["cardEffect"] is ICardEffect)
                                {
                                    ICardEffect cardEffect1 = (ICardEffect)hashtable["cardEffect"];

                                    if(cardEffect1 != null)
                                    {
                                        _hashtable.Add("cardEffect", cardEffect1);
                                    }
                                }
                            }
                        }
                        #endregion

                        if (cardEffect.CanUse(_hashtable))
                        {
                            skillInfos.Add(new SkillInfo(cardEffect,_hashtable));
                        }
                    }
                }
            }
        }
        yield return ContinuousController.instance.StartCoroutine(GManager.instance.availableMultipleSkills.ActivateMultipleSkills(skillInfos));
        #endregion
    }
    #endregion

    #region このユニットはレベルアップできるか
    public bool CanLevelUp
    {
        get
        {
            #region スキルの効果でレベルアップできない効果を付与されている場合
            foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players)
            {
                foreach (Unit unit in player.FieldUnit)
                {
                    if (unit.Character != null)
                    {
                        foreach (ICardEffect cardEffect in EffectList(EffectTiming.None))
                        {
                            if (cardEffect is ICanNotLevelUpEffect)
                            {
                                if (cardEffect.CanUse(null))
                                {
                                    if (((ICanNotLevelUpEffect)cardEffect).CanNotLevelUp(this))
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            return true;
        }
    }
    #endregion

    #region このユニットが対象の攻撃ユニットからの攻撃で撃破されないか
    public bool CanNotDestroyedByBattle(Unit Attacker)
    {
        bool CanNotDestroyed = false;

        foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players)
        {
            foreach (Unit unit in player.FieldUnit)
            {
                foreach (ICardEffect cardEffect in unit.EffectList(EffectTiming.None))
                {
                    if (cardEffect is ICanNotDestroyedByBattle)
                    {
                        if (cardEffect.CanUse(null))
                        {
                            bool _CanNotDestroy = ((ICanNotDestroyedByBattle)cardEffect).CanNotDestroyedByBattle(Attacker, this);

                            if (_CanNotDestroy)
                            {
                                CanNotDestroyed = _CanNotDestroy;
                            }
                        }
                    }
                }
            }
        }

        return CanNotDestroyed;
    }
    #endregion

    #region このユニットが対象の効果で撃破されないか
    public bool CanNotDestroyedBySkill(ICardEffect skill)
    {
        bool CanNotDestroyed = false;

        foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players)
        {
            foreach (Unit unit in player.FieldUnit)
            {
                foreach (ICardEffect cardEffect in unit.EffectList(EffectTiming.None))
                {
                    if (cardEffect is ICanNotDestroyedBySkill)
                    {
                        if (cardEffect.CanUse(null))
                        {
                            bool _CanNotDestroy = ((ICanNotDestroyedBySkill)cardEffect).CanNotDestroyedBySkill(unit,skill);

                            if (_CanNotDestroy)
                            {
                                CanNotDestroyed = _CanNotDestroy;
                            }
                        }
                    }
                }
            }
        }

        return CanNotDestroyed;
    }
    #endregion
}

#region 射程の計算
public class CheckRange
{
    public CheckRange(Unit Attacker, Unit Defender)
    {
        this.Attacker = Attacker;
        this.Defender = Defender;
    }

    Unit Attacker { get; set; }
    Unit Defender { get; set; }
    public bool CanReachRange()
    {
        if(Attacker.Character != null && Defender.Character != null)
        {
            if(Attacker.Character.Owner != Defender.Character.Owner)
            {
                if(!Attacker.Range.Contains(Distance()))
                {
                    return false;
                }

                return true;
            }
        }

        return false;
    }

    public int Distance()
    {
        return Mathf.Abs(UnitLineIndex(Attacker) - UnitLineIndex(Defender));
    }

    int UnitLineIndex(Unit unit)
    {
        if (unit.Character != null)
        {
            if (unit.Character.Owner.isYou)
            {
                if (unit.Character.Owner.GetBackUnits().Contains(unit))
                {
                    return 0;
                }

                else if (unit.Character.Owner.GetFrontUnits().Contains(unit))
                {
                    return 1;
                }
            }

            else
            {
                if (unit.Character.Owner.GetFrontUnits().Contains(unit))
                {
                    return 2;
                }

                else if (unit.Character.Owner.GetBackUnits().Contains(unit))
                {
                    return 3;
                }
            }
        }

        return 114514;
    }
}
#endregion