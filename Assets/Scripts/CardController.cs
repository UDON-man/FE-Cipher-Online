using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using AutoLayout3D;
using System.Linq;

#region 絆ゾーンにセットする(ISetCard)
public class ISetBondCard
{
    public ISetBondCard(CardSource _card,bool _isFace)
    {
        card = _card;
        isFace = _isFace;
    }
    CardSource card { get; set; }

    bool isFace { get; set; }

    public IEnumerator SetBond()
    {
        //表・裏でカードデータを書き換え
        new ISetFaceAndReverse(card, isFace).SetFaceAndReverse();

        //絆ゾーンにカードを追加
        yield return ContinuousController.instance.StartCoroutine(CardObjectController.SetBondCard(card));

        #region 絆ゾーンにカードが置かれた時の効果
        List<SkillInfo> skillInfos = new List<SkillInfo>();
        foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players_ForTurnPlayer)
        {
            foreach (Unit unit in player.FieldUnit)
            {
                foreach (ICardEffect cardEffect in unit.EffectList(EffectTiming.OnSetBond))
                {
                    if (cardEffect is ActivateICardEffect)
                    {
                        Hashtable hashtable = new Hashtable();
                        hashtable.Add("Card", card);
                        if (cardEffect.CanUse(hashtable))
                        {
                            skillInfos.Add(new SkillInfo(cardEffect, hashtable));
                        }
                    }
                }
            }
        }

        foreach (ICardEffect cardEffect in card.cEntity_EffectController.GetBSCardEffect(EffectTiming.OnSetBond))
        {
            if (cardEffect is ActivateICardEffect)
            {
                Hashtable hashtable = new Hashtable();
                hashtable.Add("Card", card);
                if (cardEffect.CanUse(hashtable))
                {
                    skillInfos.Add(new SkillInfo(cardEffect, hashtable));
                }
            }
        }
        yield return ContinuousController.instance.StartCoroutine(GManager.instance.availableMultipleSkills.ActivateMultipleSkills(skillInfos));
        #endregion
    }
}
#endregion

#region 表・裏にカードをセット
public class ISetFaceAndReverse
{
    public ISetFaceAndReverse(CardSource _card, bool _isFace)
    {
        card = _card;
        isFace = _isFace;
    }

    CardSource card { get; set; }
    bool isFace { get; set; }

    public void SetFaceAndReverse()
    {
        //表・裏でカードデータを書き換え
        if (isFace)
        {
            card.SetFace();
        }

        else
        {
            card.SetReverse();
        }
    }
}
#endregion

#region ユニットプレイする(IPlayUnit)
public class IPlayUnit
{
    public IPlayUnit(CardSource _card, Unit _targetUnit,bool _isFront,bool _isFace, Hashtable _hashtable,bool isTap)
    {
        card = _card;
        targetUnit = _targetUnit;
        isFront = _isFront;
        isFace = _isFace;
        this.hashtable = _hashtable;
        this.isTap = isTap;
    }

    CardSource card { get; set; }
    Unit targetUnit { get; set; }
    bool isFront { get; set; }
    bool isFace { get; set; }
    Hashtable hashtable { get; set; }
    bool isTap;

    public IEnumerator PlayUnit()
    {
        List<SkillInfo> skillInfos = new List<SkillInfo>();
        card.Init();

        GManager.instance.turnStateMachine.isSync = true;

        //カードを表か裏向きにする
        new ISetFaceAndReverse(card, isFace).SetFaceAndReverse();

        //新規にユニットをプレイ
        if (targetUnit == null)
        {
            Unit unit = new Unit(new List<CardSource> { card });

            #region HashTableを設定
            Hashtable _hashtable = new Hashtable();

            _hashtable.Add("Unit", unit);

            if (hashtable != null)
            {
                if (hashtable.ContainsKey("cardEffect"))
                {
                    if (hashtable["cardEffect"] is ICardEffect)
                    {
                        ICardEffect cardEffect = (ICardEffect)hashtable["cardEffect"];

                        _hashtable.Add("cardEffect", cardEffect);
                    }
                }
            }
            #endregion

            //表のカードを場に追加
            yield return ContinuousController.instance.StartCoroutine(CardObjectController.CreateNewUnit(unit,isFront));

            if (GManager.instance.turnStateMachine.DoseStartGame)
            {
                if (isFace)
                {
                    //エフェクト
                    //yield return ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().CreateFieldMajinCardEffect(unit.ShowingFieldMajinCard));

                    //エフェクト
                    yield return ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().CreateFieldUnitCardEffect(unit.ShowingFieldUnitCard));

                    if(isTap)
                    {
                        if(card.UnitContainingThisCharacter() != null)
                        {
                            yield return ContinuousController.instance.StartCoroutine(card.UnitContainingThisCharacter().Tap());
                        }
                    }

                    //「ユニットが出撃した時」効果を使用
                    skillInfos = new List<SkillInfo>();

                    foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players)
                    {
                        foreach (Unit _unit in player.FieldUnit)
                        {
                            foreach (ICardEffect cardEffect in _unit.Character.cEntity_EffectController.GetCardEffects(EffectTiming.OnEnterFieldAnyone))
                            {
                                if (cardEffect is ActivateICardEffect)
                                {
                                    if (cardEffect.CanUse(_hashtable))
                                    {
                                        skillInfos.Add(new SkillInfo(cardEffect,_hashtable));
                                        //yield return ContinuousController.instance.StartCoroutine(((ActivateICardEffect)effect).Activate_Effect_Optional_Cost_Execute(_hashtable));
                                    }
                                }
                            }
                        }
                    }

                    yield return ContinuousController.instance.StartCoroutine(GManager.instance.availableMultipleSkills.ActivateMultipleSkills(skillInfos));
                }
            }
                
        }

        //既に場に出ているユニットに重ねてプレイ
        else
        {
            #region ユニットの効果をリセット
            targetUnit.UntilEachTurnEndUnitEffects = new List<System.Func<EffectTiming, ICardEffect>>();
            targetUnit.UntilEndBattleEffects = new List<System.Func<EffectTiming, ICardEffect>>();
            targetUnit.UntilOpponentTurnEndEffects = new List<System.Func<EffectTiming, ICardEffect>>();
            targetUnit.UntilOwnerTurnEndUnitEffects = new List<System.Func<EffectTiming, ICardEffect>>();

            foreach (CardSource cardSource in targetUnit.Characters)
            {
                cardSource.Init();
            }
            #endregion

            #region HashTableを設定
            Hashtable _hashtable = new Hashtable();

            _hashtable.Add("Unit", targetUnit);

            if (hashtable != null)
            {
                if (hashtable.ContainsKey("cardEffect"))
                {
                    if (hashtable["cardEffect"] is ICardEffect)
                    {
                        ICardEffect cardEffect = (ICardEffect)hashtable["cardEffect"];

                        _hashtable.Add("cardEffect", cardEffect);
                    }
                }
            }
            #endregion

            if (isFace)
            {
                //エフェクト
                yield return ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().EvolutionFieldUnitCardEffect(card,targetUnit.ShowingFieldUnitCard));
            }

            bool HasCC = card.HasCC(targetUnit);

            //そのユニットのキャラクターリストの先頭にカードを追加
            targetUnit.Characters.Insert(0, card);

            targetUnit.ShowingFieldUnitCard.ShowUnitData();

            skillInfos = new List<SkillInfo>();

            if (GManager.instance.turnStateMachine.DoseStartGame)
            {
                if (HasCC)
                {
                    yield return new WaitForSeconds(0.1f);

                    #region CCボーナス
                    yield return ContinuousController.instance.StartCoroutine(new IDraw(targetUnit.Character.Owner, 1).Draw());
                    #endregion

                    #region 「ユニットがCCした時」効果
                    foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players)
                    {
                        foreach (Unit _unit in player.FieldUnit)
                        {
                            foreach (ICardEffect cardEffect in _unit.Character.cEntity_EffectController.GetCardEffects(EffectTiming.OnCCAnyone))
                            {
                                if (cardEffect is ActivateICardEffect)
                                {
                                    if (cardEffect.CanUse(_hashtable))
                                    {
                                        skillInfos.Add(new SkillInfo(cardEffect,_hashtable));
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }

                #region 「ユニットがレベルアップした時」効果
                foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players)
                {
                    foreach (Unit _unit in player.FieldUnit)
                    {
                        foreach (ICardEffect cardEffect in _unit.Character.cEntity_EffectController.GetCardEffects(EffectTiming.OnLevelUpAnyone))
                        {
                            if (cardEffect is ActivateICardEffect)
                            {
                                if (cardEffect.CanUse(_hashtable))
                                {
                                    if (cardEffect.isNotCheck_Effect)
                                    {
                                        yield return ContinuousController.instance.StartCoroutine(((ActivateICardEffect)cardEffect).Activate(_hashtable));
                                    }

                                    else
                                    {
                                        skillInfos.Add(new SkillInfo(cardEffect, _hashtable));
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                yield return ContinuousController.instance.StartCoroutine(GManager.instance.availableMultipleSkills.ActivateMultipleSkills(skillInfos));
            }
            
        }

        yield return GManager.instance.photonWaitController.StartWait("EndPlayUnit");
    }
}
#endregion

#region カードをドローする(IDraw)
public class IDraw
{
    public IDraw(Player _player, int _DrawCount)
    {
        player = _player;
        DrawCount = _DrawCount;
    }

    Player player { get; set; }
    int DrawCount { get; set; }

    public IEnumerator Draw()
    {
        for (int i = 0; i < DrawCount; i++)
        {
            yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(player));

            if (player.LibraryCards.Count > 0)
            {
                CardSource DrawCard = player.LibraryCards[0];

                player.LibraryCards.RemoveAt(0);

                yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(player));

                yield return ContinuousController.instance.StartCoroutine(CardObjectController.AddHandCard(DrawCard, true));
            }
        }
    }
}
#endregion

#region 山札の上から1枚支援エリアで置く
public class IAddSupportFromLibrary
{
    public IAddSupportFromLibrary(Player _player)
    {
        player = _player;
    }

    Player player { get; set; }

    public IEnumerator AddSupport()
    {
        if (player.LibraryCards.Count > 0)
        {
            CardSource card = player.LibraryCards[0];

            player.LibraryCards.RemoveAt(0);

            yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(player));

            yield return ContinuousController.instance.StartCoroutine(new IAddSupport(card).AddSupport());
        }
    }
}
#endregion

#region 支援エリアにカードを追加する
public class IAddSupport
{
    public IAddSupport(CardSource _card)
    {
        card = _card;
    }

    CardSource card;

    public IEnumerator AddSupport()
    {
        yield return ContinuousController.instance.StartCoroutine(CardObjectController.SetSupportCard(card));

        yield return ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowSupportEffect(card));

        yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(card.Owner));
    }
}
#endregion

#region 山札の上からオーブに裏で置く
public class IAddOrbFromLibrary
{
    public IAddOrbFromLibrary(Player _player, int _AddOrbCount)
    {
        player = _player;
        AddOrbCount = _AddOrbCount;
    }

    Player player { get; set; }
    int AddOrbCount { get; set; }

    public IEnumerator AddOrb()
    {
        for (int i = 0; i < AddOrbCount; i++)
        {
            yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(player));

            if (player.LibraryCards.Count > 0)
            {
                CardSource OrbCard = player.LibraryCards[0];

                player.LibraryCards.RemoveAt(0);

                yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(player));

                yield return ContinuousController.instance.StartCoroutine(CardObjectController.AddOrbCard(OrbCard, false));
            }
        }
    }
}
#endregion

#region デッキのカードを公開する
public class IShowLibraryCard
{
    public IShowLibraryCard(List<CardSource> cardSources,Hashtable hashtable, bool willHide)
    {
        this.cardSources = new List<CardSource>();

        foreach(CardSource cardSource in cardSources)
        {
            this.cardSources.Add(cardSource);
        }

        this.hashtable = hashtable;
        this.willHide = willHide;
    }

    List<CardSource> cardSources { get; set; } = new List<CardSource>();
    Hashtable hashtable { get; set; } = new Hashtable();
    bool willHide { get; set; }

    public IEnumerator ShowLibraryCard()
    {
        ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(cardSources, "Deck Top Card", willHide));

        yield return new WaitForSeconds(0.5f);

        #region 「カードを公開した時の効果」
        List<SkillInfo> skillInfos = new List<SkillInfo>();
        foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players_ForTurnPlayer)
        {
            foreach(Unit unit in player.FieldUnit)
            {
                foreach(ICardEffect cardEffect in unit.EffectList(EffectTiming.OnOpponentShowLibraryBySkill))
                {
                    if(cardEffect is ActivateICardEffect)
                    {
                        if(cardEffect.CanUse(hashtable))
                        {
                            skillInfos.Add(new SkillInfo(cardEffect, hashtable));
                        }
                    }
                }
            }
        }
        yield return ContinuousController.instance.StartCoroutine(GManager.instance.availableMultipleSkills.ActivateMultipleSkills(skillInfos));
        #endregion
    }
}
#endregion

#region 退避エリアからカードを手札に加える
public class IAddHandCardFromTrash
{
    public IAddHandCardFromTrash(CardSource cardSource, Hashtable hashtable)
    {
        this.cardSource = cardSource;
        this.hashtable = hashtable;
        
    }

    CardSource cardSource { get; set; }
    Hashtable hashtable { get; set; }

    public IEnumerator AddHandCardFromTrash()
    {
        #region Hashtableを設定
        Hashtable _hashtable = new Hashtable();
        _hashtable.Add("Card", cardSource);

        if(hashtable != null)
        {
            if(hashtable.ContainsKey("cardEffect"))
            {
                if(hashtable["cardEffect"] is ICardEffect)
                {
                    ICardEffect cardEffect = (ICardEffect)hashtable["cardEffect"];

                    if(cardEffect != null)
                    {
                        _hashtable.Add("cardEffect", cardEffect);
                    }
                }
            }
        }
        #endregion

        cardSource.SetFace();
        yield return ContinuousController.instance.StartCoroutine(CardObjectController.AddHandCard(cardSource, false));

        if(cardSource.Owner.TrashCards.Contains(cardSource))
        {
            cardSource.Owner.TrashCards.Remove(cardSource);
        }

        #region 「カードを回収した時の効果」
        List<SkillInfo> skillInfos = new List<SkillInfo>();
        foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players_ForTurnPlayer)
        {
            foreach (Unit unit in player.FieldUnit)
            {
                foreach (ICardEffect cardEffect in unit.EffectList(EffectTiming.OnAddHandCardFromTrash))
                {
                    if (cardEffect is ActivateICardEffect)
                    {
                        if (cardEffect.CanUse(_hashtable))
                        {
                            skillInfos.Add(new SkillInfo(cardEffect, _hashtable));
                            //yield return ContinuousController.instance.StartCoroutine(((ActivateICardEffect)cardEffect).Activate_Optional_Effect_Cost_Execute(_hashtable));
                        }
                    }
                }
            }
        }
        yield return ContinuousController.instance.StartCoroutine(GManager.instance.availableMultipleSkills.ActivateMultipleSkills(skillInfos));
        #endregion
    }
}
#endregion

#region 行動フェイズ中にタップしてユニットを移動させる
public class IMoveUnitDuringAction
{
    public IMoveUnitDuringAction(Unit _targetUnit)
    {
        targetUnit = _targetUnit;
    }

    Unit targetUnit { get; set; }

    public IEnumerator MoveUnit()
    {
        if(targetUnit.CanMoveDurinAction)
        {
            if(targetUnit.Character != null)
            {
                yield return ContinuousController.instance.StartCoroutine(targetUnit.Tap());
                yield return ContinuousController.instance.StartCoroutine(new IMoveUnit(new List<Unit>() { targetUnit }, false,null).MoveUnits());
            }
        }
    }

}
#endregion

#region ユニットを移動させる

public class IMoveUnit
{
    public IMoveUnit(List<Unit> _targetUnits,bool _isSkill,Hashtable _hashtable)
    {
        targetUnits = new List<Unit>();

        foreach (Unit unit in _targetUnits)
        {
            targetUnits.Add(unit);
        }

        isSkill = _isSkill;

        hashtable = _hashtable;
    }

    List<Unit> targetUnits { get; set; }
    bool isSkill;
    Hashtable hashtable { get; set; }

    public IEnumerator MoveUnits()
    {
        List<Unit> FromFrontToBackUnits = new List<Unit>();
        List<Unit> FromBackToFrontUnits = new List<Unit>();

        foreach (Unit unit in targetUnits)
        {
            if (unit.Character != null)
            {
                if(!unit.CanMove)
                {
                    continue;
                }

                if (isSkill && !unit.CanMoveBySkill)
                {
                    continue;
                }

                if (unit.Character.Owner.GetFrontUnits().Contains(unit))
                {
                    unit.Character.Owner.BackUnits.Add(unit);
                    unit.Character.Owner.FrontUnits.Remove(unit);

                    FromFrontToBackUnits.Add(unit);

                }

                else if (unit.Character.Owner.GetBackUnits().Contains(unit))
                {
                    unit.Character.Owner.FrontUnits.Add(unit);
                    unit.Character.Owner.BackUnits.Remove(unit);

                    FromBackToFrontUnits.Add(unit);
                }
            }
        }

        yield return ContinuousController.instance.StartCoroutine(CardObjectController.MoveFromFrontToBack(FromFrontToBackUnits));
        yield return ContinuousController.instance.StartCoroutine(CardObjectController.MoveFromBackToFront(FromBackToFrontUnits));

        //進軍チェック
        yield return ContinuousController.instance.StartCoroutine(March.CheckMarch());

        #region 「ユニットが移動した時」の効果
        List<SkillInfo> skillInfos = new List<SkillInfo>();

        List<Unit> MovedUnit = new List<Unit>();

        foreach(Unit unit in FromFrontToBackUnits)
        {
            MovedUnit.Add(unit);
        }

        foreach (Unit unit in FromBackToFrontUnits)
        {
            MovedUnit.Add(unit);
        }

        foreach (Unit unit in MovedUnit)
        {
            foreach(Player player in GManager.instance.turnStateMachine.gameContext.Players_ForTurnPlayer)
            {
                foreach (Unit otherUnit in player.FieldUnit)
                {
                    foreach (ICardEffect cardEffect in otherUnit.EffectList(EffectTiming.OnMovedAnyone))
                    {
                        if (cardEffect is ActivateICardEffect)
                        {
                            #region Hashtableを設定
                            Hashtable _hashtable = new Hashtable();
                            _hashtable.Add("Unit", unit);

                            if(hashtable != null)
                            {
                                if(hashtable.ContainsKey("cardEffect"))
                                {
                                    if(hashtable["cardEffect"] is ICardEffect)
                                    {
                                        if((ICardEffect)hashtable["cardEffect"] != null)
                                        {
                                            ICardEffect cardEffect1 = (ICardEffect)hashtable["cardEffect"];
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
        }

        yield return ContinuousController.instance.StartCoroutine(GManager.instance.availableMultipleSkills.ActivateMultipleSkills(skillInfos));
        #endregion
    }
}

#endregion

#region 対象のカードを加えて対象のユニットを成長させる
public class IGrow
{
    public IGrow(Unit unit, List<CardSource> cardSources)
    {
        this.unit = unit;
        this.cardSources = cardSources;
    }

    Unit unit { get; set; }
    List<CardSource> cardSources { get; set; }

    public IEnumerator Grow()
    {
        if (cardSources.Count == 0)
        {
            yield break;
        }

        foreach (CardSource cardSource in cardSources)
        {
            if (cardSource.Owner.BondCards.Contains(cardSource))
            {
                cardSource.Owner.BondCards.Remove(cardSource);
            }

            else if (cardSource.Owner.LibraryCards.Contains(cardSource))
            {
                cardSource.Owner.LibraryCards.Remove(cardSource);
            }

            else if (cardSource.Owner.TrashCards.Contains(cardSource))
            {
                cardSource.Owner.TrashCards.Remove(cardSource);
            }

            else if (cardSource.Owner.HandCards.Contains(cardSource))
            {
                cardSource.Owner.HandCards.Remove(cardSource);
            }

            else if (cardSource.Owner.SupportCards.Contains(cardSource))
            {
                cardSource.Owner.SupportCards.Remove(cardSource);
            }

            unit.Characters.Add(cardSource);
        }

        List<SkillInfo> skillInfos = new List<SkillInfo>();

        #region Hashtableを設定
        Hashtable _hashtable = new Hashtable();
        _hashtable.Add("Unit", unit);
        #endregion

        //"他のユニットが成長した時"効果を使用
        foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players)
        {
            foreach (Unit _unit in player.FieldUnit)
            {
                foreach (ICardEffect cardEffect in _unit.Character.cEntity_EffectController.GetCardEffects(EffectTiming.OnGrowAnyone))
                {
                    if (cardEffect is ActivateICardEffect)
                    {
                        if (cardEffect.CanUse(_hashtable))
                        {
                            if(cardEffect.isNotCheck_Effect)
                            {
                                yield return ContinuousController.instance.StartCoroutine(((ActivateICardEffect)cardEffect).Activate(_hashtable));
                            }

                            else
                            {
                                skillInfos.Add(new SkillInfo(cardEffect, _hashtable));
                            }
                        }
                    }
                }
            }
        }
        yield return ContinuousController.instance.StartCoroutine(GManager.instance.availableMultipleSkills.ActivateMultipleSkills(skillInfos));
    }
}
#endregion

#region ユニット同士をバトルさせる
public class IBattle
{
    public IBattle(Unit _Attacker, Unit _Defender)
    {
        Attacker = _Attacker;
        Defender = _Defender;
    }

    Unit Attacker { get; set; }
    Unit Defender { get; set; }

    public IEnumerator Battle()
    {
        Debug.Log($"バトル:Attacker:{Attacker.Character.gameObject.name},Defender:{Defender.Character.gameObject.name}");

        if (Attacker.Power >= Defender.Power)
        {
            #region 戦闘で破壊されないなら処理終了
            if(Defender.CanNotDestroyedByBattle(Attacker))
            {
                yield break;
            }
            #endregion

            GManager.instance.turnStateMachine.isDestroydeByBattle = true;

            //バトル前のユニットの情報を記録
            Unit _Attacker = new Unit(Attacker.Characters);
            Unit _Defender = new Unit(Defender.Characters);

            Hashtable _hashtable = new Hashtable
        {
            {"Attacker", _Attacker },
            {"Defender",_Defender },
            {"Unit",_Defender },
        };

            BreakOrbMode breakOrbMode = BreakOrbMode.Hand;

            #region 破壊したオーブを手札以外の領域に送る効果
            #endregion

            //ユニットを撃破
            yield return ContinuousController.instance.StartCoroutine(new IDestroyUnit(Defender, Attacker.Strike, breakOrbMode,_hashtable).Destroy());

            List<SkillInfo> skillInfos = new List<SkillInfo>();

            #region 「戦闘終了時」能力

            #region 攻撃側の「戦闘終了時」能力
            if (GManager.instance.turnStateMachine.AttackingUnit != null)
            {
                if (GManager.instance.turnStateMachine.AttackingUnit.Character != null)
                {
                    foreach (ICardEffect cardEffect in GManager.instance.turnStateMachine.AttackingUnit.EffectList(EffectTiming.OnEndBattle))
                    {
                        if (cardEffect is ActivateICardEffect)
                        {
                            if (cardEffect.CanUse(_hashtable))
                            {
                                skillInfos.Add(new SkillInfo(cardEffect, _hashtable));
                            }
                        }
                    }
                }
            }
            #endregion

            #region 防御側の「戦闘終了時」能力
            if (GManager.instance.turnStateMachine.DefendingUnit != null)
            {
                if(GManager.instance.turnStateMachine.DefendingUnit.Character != null)
                {
                    foreach (ICardEffect cardEffect in GManager.instance.turnStateMachine.DefendingUnit.EffectList(EffectTiming.OnEndBattle))
                    {
                        if (cardEffect is ActivateICardEffect)
                        {
                            if (cardEffect.CanUse(_hashtable))
                            {
                                skillInfos.Add(new SkillInfo(cardEffect, _hashtable));
                            }
                        }
                    }
                }
            }
            #endregion

            #endregion

            yield return ContinuousController.instance.StartCoroutine(GManager.instance.availableMultipleSkills.ActivateMultipleSkills(skillInfos));
        }
    }
}
#endregion

#region ユニットを撃破
public enum DestroyMode
{
    Trash,
    Bond,
    Infinity,
}
public class IDestroyUnit
{
    public IDestroyUnit(Unit _DestroyedUnit, int _breakOrbCount,BreakOrbMode _mode,Hashtable _hashtable)
    {
        DestroyedUnit = _DestroyedUnit;
        breakOrbCount = _breakOrbCount;
        mode = _mode;
        hashtable = _hashtable;
    }

    Unit DestroyedUnit { get; set; }
    int breakOrbCount { get; set; }
    BreakOrbMode mode { get; set; }
    Hashtable hashtable { get; set; }
    public bool Destroyed { get; set; } = false;
    public IEnumerator Destroy()
    {
        Unit _DestroyedUnit = new Unit(DestroyedUnit.Characters);

        ICardEffect _cardEffect = null;

        #region 撃破に使用されたスキルを取得
        if (hashtable != null)
        {
            if(hashtable.ContainsKey("cardEffect"))
            {
                if(hashtable["cardEffect"] is ICardEffect)
                {
                    if((ICardEffect)hashtable["cardEffect"] != null)
                    {
                        _cardEffect = (ICardEffect)hashtable["cardEffect"];
                    }
                }
            }
        }
        #endregion

        Unit _Attacker = null;

        #region 戦闘による撃破の場合、攻撃ユニットを取得
        if (_cardEffect == null)
        {
            if (hashtable != null)
            {
                if (hashtable.ContainsKey("Attacker"))
                {
                    if (hashtable["Attacker"] is Unit)
                    {
                        _Attacker = (Unit)hashtable["Attacker"];
                    }
                }
            }
        }
        #endregion

        #region スキルによる撃破で、スキル撃破されないなら処理終了
        if (_cardEffect != null)
        {
            if (DestroyedUnit.CanNotDestroyedBySkill(_cardEffect))
            {
                yield break;
            }
        }
        #endregion

        #region スキルによる撃破でなく、戦闘による撃破でもないなら(コストによる撃破)、コスト撃破されないなら処理終了
        if(_cardEffect == null && _Attacker == null)
        {
            if(DestroyedUnit.CanNotDestroyedByCost)
            {
                yield break;
            }
        }
        #endregion

        //撃破されたフラグをオンにする
        Destroyed = true;

        #region 負けたユニットが主人公でなければカードを全て破棄
        if (DestroyedUnit != DestroyedUnit.Character.Owner.Lord)
        {
            DestroyMode destroyMode = DestroyMode.Trash;

            #region 撃破したユニットを墓地以外の領域に送る効果
            foreach(Player player in GManager.instance.turnStateMachine.gameContext.Players_ForTurnPlayer)
            {
                foreach(Unit unit in player.FieldUnit)
                {
                    foreach(ICardEffect cardEffect in unit.EffectList(EffectTiming.None))
                    {
                        if(cardEffect is IChangePlaceDestroyedUnitEffect)
                        {
                            if(cardEffect.CanUse(null))
                            {
                                destroyMode = ((IChangePlaceDestroyedUnitEffect)cardEffect).GetDestroyMode(DestroyedUnit);
                            }
                        }
                    }
                }
            }
            #endregion

            foreach (CardSource cardSource in _DestroyedUnit.Characters)
            {
                switch (destroyMode)
                {
                    case DestroyMode.Trash:
                        CardObjectController.RemoveField(cardSource);
                        CardObjectController.AddTrashCard(cardSource);
                        break;

                    case DestroyMode.Bond:
                        yield return ContinuousController.instance.StartCoroutine(new ISetBondCard(cardSource, true).SetBond());
                        yield return ContinuousController.instance.StartCoroutine(cardSource.Owner.bondObject.SetBond_Skill(cardSource.Owner));
                        break;
                }
            }

            DestroyedUnit.Characters = new List<CardSource>();

        }
        #endregion

        #region 負けたユニットが主人公である場合
        else
        {
            #region オーブがあればオーブを破壊
            if (DestroyedUnit.Character.Owner.OrbCards.Count > 0)
            {
                yield return ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<BreakOrb>().BreakOrbCoroutine(DestroyedUnit.Character.Owner, breakOrbCount, mode));
            }
            #endregion

            #region オーブが無ければゲーム終了
            else
            {
                GManager.instance.turnStateMachine.EndGame(DestroyedUnit.Character.Owner.Enemy);
            }
            #endregion
        }
        #endregion

        if(!GManager.instance.turnStateMachine.endGame)
        {
            #region 「ユニットが撃破された時」効果
            List<SkillInfo> skillInfos = new List<SkillInfo>();

            #region 「このユニットが撃破された時」効果
            foreach (ICardEffect cardEffect in _DestroyedUnit.EffectList(EffectTiming.OnDestroyedAnyone))
            {
                if (cardEffect is ActivateICardEffect)
                {
                    if (cardEffect.CanUse(hashtable))
                    {
                        skillInfos.Add(new SkillInfo(cardEffect, hashtable));
                    }
                }
            }
            #endregion

            #region 「他のユニットが撃破された時」効果
            foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players)
            {
                foreach (Unit unit in player.FieldUnit)
                {
                    foreach (ICardEffect cardEffect in unit.EffectList(EffectTiming.OnDestroyedAnyone))
                    {
                        if (cardEffect is ActivateICardEffect)
                        {
                            if (cardEffect.CanUse(hashtable))
                            {
                                skillInfos.Add(new SkillInfo(cardEffect, hashtable));
                            }
                        }
                    }
                }
            }
            #endregion

            #region 戦闘による撃破の場合
            if (_cardEffect == null)
            {
                if(_Attacker != null)
                {
                    #region「味方が戦闘で撃破した時」効果
                    foreach (Unit unit in _Attacker.Character.Owner.FieldUnit)
                    {
                        foreach (ICardEffect cardEffect in unit.EffectList(EffectTiming.OnDestroyDuringBattleAlly))
                        {
                            if (cardEffect is ActivateICardEffect)
                            {
                                if (cardEffect.CanUse(hashtable))
                                {
                                    skillInfos.Add(new SkillInfo(cardEffect, hashtable));
                                }
                            }
                        }
                    }
                    #endregion

                    #region「このユニットが戦闘で撃破された時」効果
                    foreach (ICardEffect cardEffect in _DestroyedUnit.EffectList(EffectTiming.OnDestroyedDuringBattleAlly))
                    {
                        if (cardEffect is ActivateICardEffect)
                        {
                            if (cardEffect.CanUse(hashtable))
                            {
                                skillInfos.Add(new SkillInfo(cardEffect, hashtable));
                            }
                        }
                    }
                    #endregion

                    #region「他の味方が戦闘で撃破された時」効果
                    foreach (Unit unit in _DestroyedUnit.Character.Owner.FieldUnit)
                    {
                        if (unit != _DestroyedUnit)
                        {
                            foreach (ICardEffect cardEffect in unit.EffectList(EffectTiming.OnDestroyedDuringBattleAlly))
                            {
                                if (cardEffect is ActivateICardEffect)
                                {
                                    if (cardEffect.CanUse(hashtable))
                                    {
                                        skillInfos.Add(new SkillInfo(cardEffect, hashtable));
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
            }
            #endregion

            yield return ContinuousController.instance.StartCoroutine(GManager.instance.availableMultipleSkills.ActivateMultipleSkills(skillInfos));
            #endregion

            //進軍チェック
            yield return ContinuousController.instance.StartCoroutine(March.CheckMarch());
        }
    }
}
#endregion

public class Refresh
{
    #region リフレッシュチェック
    public static IEnumerator RefreshCheck(Player player)
    {
        if(player.LibraryCards.Count == 0)
        {
            yield return ContinuousController.instance.StartCoroutine(RefreshLibrary(player));
        }
    }
    #endregion

    #region ライブラリーをリフレッシュする
    public static IEnumerator RefreshLibrary(Player player)
    {
        List<CardSource> TrashCards = new List<CardSource>();

        foreach (CardSource cardSource in player.TrashCards)
        {
            TrashCards.Add(cardSource);
        }

        foreach (CardSource cardSource in TrashCards)
        {
            player.TrashCards.Remove(cardSource);
            player.LibraryCards.Add(cardSource);
        }

        yield return ContinuousController.instance.StartCoroutine(CardObjectController.Shuffle(player));
    }
    #endregion
}

public class March
{
    #region 進軍チェック
    public static IEnumerator CheckMarch()
    {
        if (GManager.instance.turnStateMachine.gameContext.NonTurnPlayer.GetFrontUnits().Count == 0)
        {
            List<Unit> MoveUnits = new List<Unit>();

            foreach (Unit Unit in GManager.instance.turnStateMachine.gameContext.NonTurnPlayer.GetBackUnits())
            {
                MoveUnits.Add(Unit);
            }

            yield return ContinuousController.instance.StartCoroutine(new IMoveUnit(MoveUnits,false,null).MoveUnits());
        }
    }
    #endregion
}

