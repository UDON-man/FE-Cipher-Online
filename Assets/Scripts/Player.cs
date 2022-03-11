using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Player : MonoBehaviour
{
    private void Start()
    {
        SupportHandCard.gameObject.SetActive(false);
        TrashHandCard.gameObject.SetActive(false);


        if (PlayMat_Back_Select != null)
        {
            PlayMat_Back_Select?.gameObject.SetActive(false);
        }
        
        if(PlayMat_Front_Select)
        {
            PlayMat_Front_Select?.gameObject.SetActive(false);
        }
        
    }

    #region デッキレシピ
    [Header("デッキレシピ")]
    public List<CEntity_Base> DeckRecipie = new List<CEntity_Base>();
    #endregion

    #region キーカード
    [Header("主人公ユニット")]
    public Unit Lord;

    [Header("主人公カード")]
    public CEntity_Base LordCard;
    #endregion

    #region カードの情報
    #region ライブラリのカード
    public List<CardSource> LibraryCards = new List<CardSource>();
    #endregion

    #region 手札のカード
    public List<CardSource> HandCards = new List<CardSource>();
    #endregion

    #region 退避エリアのカード
    public List<CardSource> TrashCards = new List<CardSource>();
    #endregion

    #region 無限ゾーンのカード
    public List<CardSource> InfinityCards = new List<CardSource>();
    #endregion

    #region オーブゾーンのカード
    public List<CardSource> OrbCards = new List<CardSource>();
    #endregion

    #region 絆ゾーンのカード
    public List<CardSource> BondCards = new List<CardSource>();
    #endregion

    #region 支援エリアのカード
    public List<CardSource> SupportCards = new List<CardSource>();
    #endregion
    #endregion

    #region 自分側かどうか
    [Header("自分側かどうか")]
    public bool isYou;
    #endregion

    #region カードを置く領域
    [Header("カードデータオブジェクトの置き場所")]
    public Transform CardSorcesParent;
    #endregion

    #region カードの置き場所

    [Header("手札の場所")]
    public Transform HandTransform;

    [Header("オーブゾーンの場所")]
    public Transform OrbTransform;

    [Header("前衛の場所")]
    public Transform FrontZoneTransform;

    [Header("後衛の場所")]
    public Transform BackZoneTransform;
    #endregion

    #region カードの置き場所の幅
    [Header("手札領域の幅")]
    public float HandWidth;
    #endregion

    #region 前衛のユニット
    public List<Unit> FrontUnits = new List<Unit>();

    public List<Unit> GetFrontUnits()
    {
        List<Unit> temp = new List<Unit>();

        foreach (Unit Unit in FrontUnits)
        {
            if (Unit.Characters.Count > 0)
            {
                temp.Add(Unit);
            }
        }

        return temp;
    }
    #endregion

    #region 後衛のユニット
    public List<Unit> BackUnits = new List<Unit>();

    public List<Unit> GetBackUnits()
    {
        List<Unit> temp = new List<Unit>();

        foreach (Unit Unit in BackUnits)
        {
            if (Unit.Characters.Count > 0)
            {
                temp.Add(Unit);
            }
        }

        return temp;
    }
    #endregion

    #region 場のユニット
    public List<Unit> FieldUnit
    {
        get
        {
            List<Unit> fieldUnit = new List<Unit>();

            foreach (Unit Unit in GetFrontUnits())
            {
                fieldUnit.Add(Unit);
            }

            foreach (Unit Unit in GetBackUnits())
            {
                fieldUnit.Add(Unit);
            }

            return fieldUnit;
        }
    }
    #endregion

    #region 絆
    public int BondConsumed { get; set; }
    public int Bond { get; set; }

    public void SetBond()
    {
        Bond = NowBond();
    }

    public int NowBond()
    {
        int _Bond = 0;

        _Bond = BondCards.Count;

        return _Bond;
    }
    #endregion

    #region プレイヤー名
    public string PlayerName { get; set; }

    [Header("プレイヤー名表示Text")]
    public Text PlayerNameText;

    public void ShowPlayerName()
    {
        if (GManager.instance != null)
        {
            if(GManager.instance.turnStateMachine != null)
            {
                if(GManager.instance.turnStateMachine.gameContext != null)
                {
                    if(GManager.instance.turnStateMachine.gameContext.TurnPhase == GameContext.phase.Bond)
                    {
                        return;
                    }
                }
            }
        }
        
        PlayerNameText.transform.parent.gameObject.SetActive(true);
    }

    public void OffPlayerName()
    {
        PlayerNameText.transform.parent.gameObject.SetActive(false);
    }
    #endregion

    #region 勝利数
    public int WinCount { get; set; }

    [Header("勝利数表示Text")]
    public Text WinCountText;

    public void ShowWinCount()
    {
        WinCountText.transform.parent.gameObject.SetActive(true);
    }

    public void HideWinCount()
    {
        WinCountText.transform.parent.gameObject.SetActive(false);
    }
    #endregion

    #region プレイヤーID
    public int PlayerID { get; set; }
    #endregion

    #region ダメージ
    public int OrbCount
    {
        get
        {
            return OrbCards.Count;
        }
    }
    #endregion

    #region ダメージを受ける
    public IEnumerator GetDamage(int damage)
    {
        yield return null;
    }
    #endregion

    #region 対戦相手
    public Player Enemy
    {
        get
        {
            if (GManager.instance.turnStateMachine.gameContext.Players.Contains(this))
            {
                foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players)
                {
                    if (player != this)
                    {
                        return player;
                    }
                }
            }

            return null;
        }
    }
    #endregion

    #region プレイマット
    [Header("プレイマット")]
    public GameObject PlayMat;

    [Header("前衛プレイマット")]
    public GameObject PlayMat_Front;

    [Header("後衛プレイマット")]
    public GameObject PlayMat_Back;

    [Header("前衛プレイマット選択")]
    public SpriteRenderer PlayMat_Front_Select;

    [Header("後衛プレイマット選択")]
    public SpriteRenderer PlayMat_Back_Select;
    #endregion

    #region 手札のドロップエリア
    [Header("手札のドロップエリア")]
    public DropArea HandDropArea;
    #endregion

    #region 場のカードオブジェクト
    public List<FieldUnitCard> FieldUnitObjects
    {
        get
        {
            List<FieldUnitCard> _FieldUnitObjects = new List<FieldUnitCard>();

            for (int i = 0; i < FrontZoneTransform.childCount; i++)
            {
                FieldUnitCard FieldUnitCard = FrontZoneTransform.GetChild(i).GetComponent<FieldUnitCard>();

                //if (!FieldUnitCard.destroyed)
                {
                    _FieldUnitObjects.Add(FieldUnitCard);
                }
            }

            for (int i = 0; i < BackZoneTransform.childCount; i++)
            {
                FieldUnitCard FieldUnitCard = BackZoneTransform.GetChild(i).GetComponent<FieldUnitCard>();

                //if (!FieldUnitCard.destroyed)
                {
                    _FieldUnitObjects.Add(FieldUnitCard);
                }
            }

            return _FieldUnitObjects;
        }
    }

    public List<FieldUnitCard> FrontUnitObjects
    {
        get
        {
            List<FieldUnitCard> _FrontUnitObjects = new List<FieldUnitCard>();

            for (int i = 0; i < FrontZoneTransform.childCount; i++)
            {
                FieldUnitCard FieldUnitCard = FrontZoneTransform.GetChild(i).GetComponent<FieldUnitCard>();

                //if (!FieldUnitCard.destroyed)
                {
                    _FrontUnitObjects.Add(FieldUnitCard);
                }
            }

            return _FrontUnitObjects;
        }
    }
    #endregion

    #region 手札のカードオブジェクト
    public List<HandCard> HandCardObjects
    {
        get
        {
            List<HandCard> _HandCards = new List<HandCard>();

            foreach (CardSource cardSource in HandCards)
            {
                _HandCards.Add(cardSource.ShowingHandCard);
            }

            return _HandCards;
        }
    }
    #endregion

    #region 絆表示
    [Header("絆表示")]
    public BondObject bondObject;
    #endregion

    #region 墓地アイコン
    [Header("墓地アイコン")]
    public GameObject TrashIcon;
    #endregion

    #region プレイヤーにかかる効果

    #region プレイヤーに掛かっている全ての効果
    public List<ICardEffect> PlayerEffects(EffectTiming timing)
    {
        List<ICardEffect> PlayerEffects = new List<ICardEffect>();

        foreach (Func<EffectTiming, ICardEffect> GetCardEffect in UntilEndBattleEffects)
        {
            if (GetCardEffect(timing) != null)
            {
                PlayerEffects.Add(GetCardEffect(timing));
            }
        }

        foreach (Func<EffectTiming, ICardEffect> GetCardEffect in UntilEachTurnEndEffects)
        {
            if (GetCardEffect(timing) != null)
            {
                PlayerEffects.Add(GetCardEffect(timing));
            }
        }

        foreach (Func<EffectTiming, ICardEffect> GetCardEffect in UntilOpponentTurnEndEffects)
        {
            if (GetCardEffect(timing) != null)
            {
                PlayerEffects.Add(GetCardEffect(timing));
            }
        }

        foreach(ICardEffect cardEffect in PlayerEffects)
        {
            cardEffect._card = Lord.Character;
        }

        return PlayerEffects;
    }
    #endregion

    #region 攻撃終了時に消える、プレイヤーに掛かる効果
    public List<Func<EffectTiming, ICardEffect>> UntilEndBattleEffects = new List<Func<EffectTiming, ICardEffect>>();
    #endregion

    #region ターン終了時に消える、プレイヤーに掛かる効果
    public List<Func<EffectTiming, ICardEffect>> UntilEachTurnEndEffects = new List<Func<EffectTiming, ICardEffect>>();
    #endregion

    #region 相手のターン終了時に消える、プレイヤーに掛かる効果
    public List<Func<EffectTiming, ICardEffect>> UntilOpponentTurnEndEffects = new List<Func<EffectTiming, ICardEffect>>();
    #endregion

    #endregion

    #region ライブラリー枚数表示テキスト
    [Header("ライブラリー枚数表示テキスト")]
    public Text LibraryCountText;

    public void OnMBCountText()
    {
        LibraryCountText.transform.parent.gameObject.SetActive(true);

        LibraryCountText.text = $"Deck\n{LibraryCards.Count}";
    }

    public void OffMBCountText()
    {
        LibraryCountText.transform.parent.gameObject.SetActive(false);
    }
    #endregion

    #region エフェクト関連
    public HandCard SupportHandCard;
    public HandCard TrashHandCard;
    #endregion

    #region エリスの「オーム」を使っているか
    public bool DoneUseOrm { get; set; }
    #endregion

    #region フローラの「間に合いました」を使っているか
    public bool DoneUseInTime { get; set; }
    #endregion
}
