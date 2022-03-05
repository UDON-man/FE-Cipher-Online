using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Reflection;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon;
using Photon.Pun;
using System.Linq;

public class CardSource : MonoBehaviour
{
    #region カード操作に関するクラス
    public CardOperation cardOperation
    {
        get
        {
            return GetComponent<CardOperation>();
        }
    }
    #endregion

    #region カード情報
    //カードの基本情報
    public CEntity_Base cEntity_Base
    {
        get; set;
    }

    /*
    public CEntity_Effect cEntity_Effect
    {
        get; set;
    }
    */
    #endregion

    #region PhotonView
    public PhotonView photonView { get; set; }
    #endregion

    #region カードの持ち主
    public Player Owner
    {
        get; set;
    }
    #endregion

    #region シーン中のカード番号
    public int cardIndex
    {
        get; set;
    }
    #endregion

    #region カードの基本情報・持ち主を初期登録
    public void SetBaseData(CEntity_Base _cEntity_Base, Player _owner)
    {
        //カードの基本情報を登録
        cEntity_Base = _cEntity_Base;

        //カードの所有者を登録
        Owner = _owner;

        //オブジェクト名を変更
        this.gameObject.name = cEntity_Base.CardName;

        SetFace();
    }
    #endregion

    #region カード効果
    public CEntity_EffectController cEntity_EffectController;
    /*
    bool first = true;
    public void SetUpCardEffect()
    {
        #region カード効果クラスのインスタンスを生成して登録
        if (!string.IsNullOrEmpty(cEntity_Base.ClassName))
        {
            if (cEntity_Effect == null)
            {
                bool CanAttachEffectComponent(string ClassName)
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

                if(CanAttachEffectComponent(cEntity_Base.ClassName))
                {
                    Type t = null;

                    if (!string.IsNullOrEmpty(cEntity_Base.ClassName))
                    {
                        t = Type.GetType(cEntity_Base.ClassName);

                        if (t != null)
                        {
                            cEntity_Effect = (CEntity_Effect)(this.gameObject.AddComponent(t));
                        }
                    }
                }

                else
                {
                    cEntity_Effect = this.gameObject.AddComponent<EmptyCEntity_Effect>();
                }

                //cEntity_Effect = GetComponent<CEntity_Effect>();
                cEntity_Effect.card = this;

                if (first)
                {
                    cEntity_Effect.AttachICardEffectComponent();
                }
            }
        }
        #endregion

        first = false;
    }
    */
    #endregion

    #region 表・裏の切り替え
    #region 表にする
    public void SetFace()
    {
        this.IsReverse = false;
    }
    #endregion

    #region 裏にする
    public void SetReverse()
    {
        this.IsReverse = true;
    }
    #endregion
    #endregion

    #region 裏かどうか
    public bool IsReverse { get; set; }
    #endregion

    #region カードIDのセットアップ
    public void SetUpCardIndex(int _cardIndex)
    {
        PhotonView _PhotonView = GetComponent<PhotonView>();

        if (_PhotonView == null)
        {
            _PhotonView = this.gameObject.AddComponent<PhotonView>();
        }

        cardIndex = _cardIndex;

        _PhotonView.ViewID = cardIndex + 60;

        photonView = _PhotonView;
    }
    #endregion

    #region AddComponentString
    public static void AddComponentString(GameObject attachObject, string ComponentName)
    {
        Type t = null;

        if(!string.IsNullOrEmpty(ComponentName))
        {
            t = Type.GetType(ComponentName);

            if(t != null)
            {
                MethodInfo mi = typeof(GameObject).GetMethod(
            "AddComponent",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new Type[0],
            null
        );
                MethodInfo bound = mi.MakeGenericMethod(t);
                bound.Invoke(attachObject, null);
            }
        }

        
    }
    #endregion

    #region 支援力
    public int SupportPower
    {
        get
        {
            int SupportPower = cEntity_Base.SupportPower;

            #region スキルの効果
            foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players)
            {
                foreach (Unit unit in player.FieldUnit)
                {
                    foreach (ICardEffect cardEffect in unit.EffectList(EffectTiming.None))
                    {
                        if (cardEffect is IChangeSupportPowerCardEffect)
                        {
                            if (cardEffect.CanUse(null))
                            {
                                SupportPower = ((IChangeSupportPowerCardEffect)cardEffect).GetSupportPower(SupportPower, this);
                            }
                        }
                    }
                }
            }
            #endregion

            return SupportPower;
        }
    }
    #endregion

    #region 射程
    public List<int> Range
    {
        get
        {
            List<int> Range = new List<int>();

            foreach(int range in cEntity_Base.Ranges)
            {
                if(!Range.Contains(range))
                {
                    Range.Add(range);
                }
            }

            Range.Sort();

            return Range;
        }
    }
    #endregion

    #region このカードを絆エリアに置くことが出来るか
    public bool CanSetBondThisCard
    {
        get
        {
            foreach(ICardEffect cardEffect in cEntity_EffectController.GetCardEffects(EffectTiming.None))
            {
                if(cardEffect is ICanNotSetBondCardEffect)
                {
                    if(((ICanNotSetBondCardEffect)cardEffect).CanNotSetBond(this))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
    #endregion

    #region プレイコスト
    public int PlayCost
    {
        get
        {
            int _PlayCost = cEntity_Base.PlayCost;

            #region スキルの効果
            foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players)
            {
                foreach (Unit _unit in player.FieldUnit)
                {
                    foreach (ICardEffect cardEffect in _unit.EffectList(EffectTiming.None))
                    {
                        if (cardEffect is IChangePlayCost)
                        {
                            if (cardEffect.CanUse(null))
                            {
                                _PlayCost = ((IChangePlayCost)cardEffect).GetPlayCost(_PlayCost,  this);
                            }
                        }
                    }
                }
            }


            foreach (ICardEffect cardEffect in cEntity_EffectController.GetCardEffects(EffectTiming.None))
            {
                if (cardEffect is IChangePlayCost)
                {
                    if (cardEffect.CanUse(null))
                    {
                        _PlayCost = ((IChangePlayCost)cardEffect).GetPlayCost(_PlayCost, this);
                    }
                }

            }
            #endregion

            return _PlayCost;
        }
    }
    #endregion

    #region CCコスト
    public int CCCost(Unit targetUnit)
    {
        int _CCCost = cEntity_Base.CCCost;

        #region スキルの効果
        foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players)
        {
            foreach (Unit _unit in player.FieldUnit)
            {
                foreach (ICardEffect cardEffect in _unit.EffectList(EffectTiming.None))
                {
                    if (cardEffect is IChangeCCCostEffect)
                    {
                        if (cardEffect.CanUse(null))
                        {
                            _CCCost = ((IChangeCCCostEffect)cardEffect).GetCCCost(_CCCost, targetUnit, this);
                        }
                    }
                }
            }
        }
       

        foreach (ICardEffect cardEffect in cEntity_EffectController.GetCardEffects(EffectTiming.None))
        {
            if (cardEffect is IChangeCCCostEffect)
            {
                if (cardEffect.CanUse(null))
                {
                    _CCCost = ((IChangeCCCostEffect)cardEffect).GetCCCost(_CCCost, targetUnit, this);
                }
            }
                
        }
        #endregion

        return _CCCost;
    }
    #endregion

    #region CCをもつか
    public bool HasCC(Unit targetUnit)
    {
        foreach (ICardEffect cardEffect in cEntity_EffectController.GetCardEffects(EffectTiming.None))
        {
            if (cardEffect is IAddHasCCEffect)
            {
                if (cardEffect.CanUse(null))
                {
                    bool _HasCC = ((IAddHasCCEffect)cardEffect).HasCC(this,targetUnit);

                    if(_HasCC)
                    {
                        return true;
                    }
                }
            }

        }

        return cEntity_Base.HasCC;
    }
    #endregion

    #region プレイするのに必要なコスト
    public int Cost(Unit unit)
    {
        int Cost = 114514;

        if (unit == null)
        {
            Cost = PlayCost;
        }

        else
        {
            //レベルアップできるなら
            if (CanLevelUpFromTargetUnit(unit))
            {
                Cost = PlayCost;
            }

            //CCできるなら
            if (CanCCFromTargetUnit(unit))
            {
                Cost = CCCost(unit);
            }
        }

        return Cost;
    }
    #endregion

    #region レベルアップ対象のユニット
    #region 手札のこのカードを重ねてレベルアップできるユニット
    public List<Unit> CanLevelUpUnits
    {
        get
        {
            List<Unit> CanLevelUpUnits = new List<Unit>();

            foreach(Unit unit in Owner.FieldUnit)
            {
                if(CanLevelUpFromTargetUnit(unit))
                {
                    CanLevelUpUnits.Add(unit);
                }
            }

            return CanLevelUpUnits;
        }
    }
    #endregion

    #region 対象のユニットの上に手札のこのカードを重ねてレベルアップできるか
    public bool CanLevelUpFromTargetUnit(Unit targetUnit)
    {
        if (targetUnit.Character != null)
        {
            if(!targetUnit.CanLevelUp)
            {
                return false;
            }

            foreach (string UnitName in this.UnitNames)
            {
                if (targetUnit.Character.UnitNames.Contains(UnitName))
                {
                    return true;
                }
            }

            foreach (ICardEffect cardEffect in cEntity_EffectController.GetCardEffects(EffectTiming.None))
            {
                if (cardEffect is ICanLevelUpToThisCardEffect)
                {
                    if (cardEffect.CanUse(null))
                    {
                        if (((ICanLevelUpToThisCardEffect)cardEffect).CanLevelUpToThisCard(this, targetUnit))
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
    #endregion

    #region CC対象のユニット
    #region 手札のこのカードを重ねてCCできるユニット
    public List<Unit> CanCCUnits
    {
        get
        {
            List<Unit> CanCCUnits = new List<Unit>();

            foreach (Unit unit in Owner.FieldUnit)
            {
                if (CanCCFromTargetUnit(unit))
                {
                    CanCCUnits.Add(unit);
                }
            }

            return CanCCUnits;
        }
    }
    #endregion

    #region 対象のユニットの上に手札のこのカードを重ねてCCできるか
    public bool CanCCFromTargetUnit(Unit targetUnit)
    {
        if (targetUnit.Character != null)
        {
            if (!targetUnit.CanLevelUp)
            {
                return false;
            }

            //他の特別な条件も
            if (HasCC(targetUnit))
            {
                foreach (string UnitName in this.UnitNames)
                {
                    if (targetUnit.Character.UnitNames.Contains(UnitName))
                    {
                        return true;
                    }
                }

                foreach(ICardEffect cardEffect in cEntity_EffectController.GetCardEffects(EffectTiming.None))
                {
                    if(cardEffect is ICanLevelUpToThisCardEffect)
                    {
                        if(cardEffect.CanUse(null))
                        {
                            if(((ICanLevelUpToThisCardEffect)cardEffect).CanLevelUpToThisCard(this,targetUnit))
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
    #endregion

    #region ユニット名
    public List<string> UnitNames
    {
        get
        {
            List<string> UnitNames = new List<string>();

            UnitNames.Add(cEntity_Base.UnitName);

            foreach (ICardEffect cardEffect in cEntity_EffectController.GetCardEffects(EffectTiming.None))
            {
                if (cardEffect is IChangeUnitNameEffect)
                {
                    if (cardEffect.CanUse(null))
                    {
                        UnitNames = ((IChangeUnitNameEffect)cardEffect).GetUnitNames(UnitNames, this);
                    }
                }
            }

            return UnitNames;
        }
    }
    #endregion

    #region 新規プレイができるか
    public bool CanPlayAsNewUnit()
    {
        bool Exist = false;

        foreach(Unit unit in Owner.FieldUnit)
        {
            foreach(string UnitName in this.UnitNames)
            {
                if (unit.Character.UnitNames.Contains(UnitName))
                {
                    Exist = true;
                }
            }
        }

        if(Exist)
        {
            foreach (ICardEffect cardEffect in cEntity_EffectController.GetCardEffects(EffectTiming.None))
            {
                if (cardEffect is ICanPlayEvenIfExistSameUnit)
                {
                    if (cardEffect.CanUse(null))
                    {
                        if (((ICanPlayEvenIfExistSameUnit)cardEffect).CanPlayEvenIfExistSameUnit(this))
                        {
                            return true;
                        }
                    }
                        
                }
            }

            return false;
        }

        else
        {
            foreach (ICardEffect cardEffect in cEntity_EffectController.GetCardEffects(EffectTiming.None))
            {
                if (cardEffect is ICanNotPlayCardEffect)
                {
                    if(cardEffect.CanUse(null))
                    {
                        if (((ICanNotPlayCardEffect)cardEffect).CanNotPlay(this))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
    #endregion

    #region 使用可能かどうか
    public bool CanUseCard()
    {
        foreach(Unit unit in Owner.FieldUnit)
        {
            if(CanUseTargetUnit(unit))
            {
                return true;
            }
        }

        if (CanUseTargetUnit(null))
        {
            return true;
        }

        return false;
    }

    public bool CanUseTargetUnit(Unit unit)
    {
        #region 色条件
        bool MatchColor()
        {
            foreach (CardColor cardColor in cardColors)
            {
                if (cardColor != CardColor.Colorless)
                {
                    if (Owner.BondCards.Count((cardSource) => !cardSource.IsReverse && cardSource.cardColors.Contains(cardColor)) == 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        #endregion

        #region コスト条件
        bool MatchCost()
        {
            return Owner.BondCards.Count - Owner.BondConsumed >= Cost(unit);
        }
        #endregion

        if (!MatchColor())
        {
            return false;
        }

        if (!MatchCost())
        {
            return false;
        }

        if (GManager.instance.turnStateMachine.gameContext.TurnPhase != GameContext.phase.Deploy)
        {
            return false;
        }

        if(unit == null)
        {
            if(!CanPlayAsNewUnit())
            {
                return false;
            }
        }

        return true;
    }
    #endregion

    #region このカードを表示している手札のカードオブジェクト
    public HandCard ShowingHandCard { get; set; }
    #endregion

    #region 一方のカードが他方のカードと同じユニット名か
    public static bool IsSameUnitName(CardSource card0,CardSource card1)
    {
        foreach(string UnitName in card0.UnitNames)
        {
            if(card1.UnitNames.Contains(UnitName))
            {
                return true;
            }
        }

        return false;
    }
    #endregion

    #region このカードを含むユニット
    public Unit UnitContainingThisCharacter()
    {
        Unit unit = null;

        foreach (Unit _unit in Owner.FieldUnit)
        {
            if (_unit.Characters.Contains(this))
            {
                return _unit;
            }
        }

        return unit;
    }
    #endregion

    #region カードを初期化
    public void Init()
    {
        cEntity_EffectController.Init();
        SetFace();
    }
    #endregion

    #region カードの色
    public List<CardColor> cardColors
    {
        get
        {
            List<CardColor> cardColors = new List<CardColor>();

            foreach(CardColor cardColor in cEntity_Base.cardColors)
            {
                cardColors.Add(cardColor);
            }

            foreach (ICardEffect cardEffect in cEntity_EffectController.GetCardEffects(EffectTiming.None))
            {
                if (cardEffect is IChangeCardColorsEffect)
                {
                    if (cardEffect.CanUse(null))
                    {
                        cardColors = ((IChangeCardColorsEffect)cardEffect).GetCardColors(cardColors, this);
                    }
                }
            }

            foreach(Player player in GManager.instance.turnStateMachine.gameContext.Players)
            {
                foreach(Unit unit in player.FieldUnit)
                {
                    foreach (ICardEffect cardEffect in unit.EffectList(EffectTiming.None))
                    {
                        if (cardEffect is IChangeCardColorsEffect)
                        {
                            if (cardEffect.CanUse(null))
                            {
                                cardColors = ((IChangeCardColorsEffect)cardEffect).GetCardColors(cardColors, this);
                            }
                        }
                    }
                }
            }

            return cardColors;
        }
    }
    #endregion

    #region 性別
    public List<Sex> sex
    {
        get
        {
            List<Sex> sex = new List<Sex>();

            if(cEntity_Base != null)
            {
                sex.Add(cEntity_Base.sex);
            }

            foreach (ICardEffect cardEffect in cEntity_EffectController.GetCardEffects(EffectTiming.None))
            {
                if (cardEffect is IChangeSexEffect)
                {
                    if (cardEffect.CanUse(null))
                    {
                        sex = ((IChangeSexEffect)cardEffect).GetSex(sex);
                    }
                }
            }

            return sex;
        }
    }
    #endregion

    #region 武器
    public List<Weapon> Weapons
    {
        get
        {
            List<Weapon> Weapons = new List<Weapon>();

            foreach (Weapon weapon in cEntity_Base.Weapons)
            {
                Weapons.Add(weapon);
            }

            foreach (ICardEffect cardEffect in cEntity_EffectController.GetCardEffects(EffectTiming.None))
            {
                if (cardEffect is IChangeWeaponEffect)
                {
                    if (cardEffect.CanUse(null))
                    {
                        Weapons = ((IChangeWeaponEffect)cardEffect).GetWeapon(Weapons, this);
                    }
                }
            }

            foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players)
            {
                foreach (Unit unit in player.FieldUnit)
                {
                    foreach (ICardEffect cardEffect in unit.EffectList(EffectTiming.None))
                    {
                        if (cardEffect is IChangeWeaponEffect)
                        {
                            if (cardEffect.CanUse(null))
                            {
                                Weapons = ((IChangeWeaponEffect)cardEffect).GetWeapon(Weapons, this);
                            }
                        }
                    }
                }
            }

            return Weapons;
        }
    }
    #endregion
}

public class EmptyCEntity_Effect : CEntity_Effect
{

}