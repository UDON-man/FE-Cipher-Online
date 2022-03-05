using AutoLayout3D;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using Photon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class FieldUnitCard : MonoBehaviour
{
    [Header("選択時アウトライン")]
    public SpriteRenderer Outline_Select;

    [Header("主人公エフェクト")]
    public GameObject LordEffect;

    [Header("カード画像")]
    public SpriteRenderer CardImage;

    [Header("当たり判定用オブジェクト")]
    public GameObject Collider;

    [Header("タップオブジェクト")]
    public GameObject TapObject;

    [Header("パワーテキスト")]
    public Text PowerText;

    [Header("アニメーター")]
    public Animator anim;

    [Header("攻撃時矢印(自分)")]
    public Image AttackArrow_You;

    [Header("攻撃時矢印(相手)")]
    public Image AttackArrow_Opponent;

    [Header("防御時矢印(自分)")]
    public Image BlockArrow_You;

    [Header("防御時矢印(相手)")]
    public Image BlockArrow_Opponent;

    [Header("射程表示")]
    public ShowRangeClass showRangeClass;

    [Header("攻撃・起動効果表示")]
    public FieldUnitCommandPanel fieldUnitCommandPanel;

    [Header("オーブ破壊枚数テキスト")]
    public Text BreakOrbCountText;

    public GameObject Parent;

    public GameObject UsingSkillEffect;

    public Vector3 StartScale;//{ get; set; }

    public UnityAction<FieldUnitCard> OnClickFieldUnitCardAction;

    //このカードが表すMajinデータ
    public Unit thisUnit { get; set; }

    private void Awake()
    {
        if (this.transform.parent.GetComponent<XAxisLayoutGroup3D>() != null)
        {
            OldParentSpacing = this.transform.parent.GetComponent<XAxisLayoutGroup3D>().spacing;
        }

        OffAttackerDefenderEffect();

        RemoveSelectEffect();

        CloseFieldUnitCommandPanel();

        Outline_Select.gameObject.SetActive(false);

        OffUsingSkillEffect();

        if(BreakOrbCountText != null)
        {
            BreakOrbCountText.gameObject.SetActive(false);
        }
    }

    public void OffUsingSkillEffect()
    {
        if(UsingSkillEffect != null)
        {
            UsingSkillEffect.SetActive(false);
        }
    }

    public void OnUsingSkillEffect()
    {
        if (UsingSkillEffect != null)
        {
            UsingSkillEffect.SetActive(true);
        }
    }

    #region 攻撃・防御エフェクト
    #region 攻撃エフェクトを表示
    public void SetAttackerEffect()
    {
        if (thisUnit != null)
        {
            if (thisUnit.Character != null)
            {
                if (thisUnit.Character.Owner.isYou)
                {
                    AttackArrow_You.gameObject.SetActive(true);
                }

                else
                {
                    AttackArrow_Opponent.gameObject.SetActive(true);
                }
            }
        }
    }
    #endregion

    #region 防御エフェクトを表示
    public void SetDefenderEffect()
    {
        if (thisUnit.Character.Owner.isYou)
        {
            BlockArrow_You.gameObject.SetActive(true);
        }

        else
        {
            BlockArrow_Opponent.gameObject.SetActive(true);
        }
    }
    #endregion

    #region 攻撃・防御エフェクトを削除
    public void OffAttackerDefenderEffect()
    {
        AttackArrow_You.gameObject.SetActive(false);
        AttackArrow_Opponent.gameObject.SetActive(false);

        BlockArrow_You.gameObject.SetActive(false);
        BlockArrow_Opponent.gameObject.SetActive(false);
    }
    #endregion
    #endregion

    #region クリック
    #region クリック時の処理
    public void OnClick()
    {
        if(thisUnit != null)
        {
            if(thisUnit.Character != null)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    OnClickFieldUnitCardAction?.Invoke(this);
                }

                else if (Input.GetMouseButtonUp(1))
                {
                    if (!thisUnit.Character.IsReverse || thisUnit.Character.Owner.isYou)
                    {
                        GManager.instance.unitDetail.OpenUnitDetail(thisUnit);
                    }
                }
            }
        }
    }
    #endregion

    #region クリック時の処理を追加
    public void AddClickTarget(UnityAction<FieldUnitCard> _OnClickAction)
    {
        OnClickFieldUnitCardAction = _OnClickAction;
    }
    #endregion

    #region クリック時の削除
    public void OffClickTarget()
    {
        OnClickFieldUnitCardAction = null;
    }
    #endregion
    #endregion

    #region Unitデータを設定
    public void SetFieldUnitCard(Unit unit)
    {
        //Unitデータを登録
        thisUnit = unit;

        ShowUnitData();
    }
    #endregion

    #region Unitの情報を反映
    public void ShowUnitData()
    {
        if(thisUnit == null)
        {
            return;
        }

        if (thisUnit.Character != null)
        {
            //主人公エフェクト表示
            if(thisUnit == thisUnit.Character.Owner.Lord && !thisUnit.Character.IsReverse)
            {
                LordEffect.SetActive(true);
            }

            else
            {
                LordEffect.SetActive(false);
            }

            if(!thisUnit.Character.IsReverse || thisUnit.Character.Owner.isYou)
            {
                //カード画像
                CardImage.sprite = thisUnit.Character.cEntity_Base.CardImage;
                CardImage.gameObject.SetActive(true);

                //パワー
                PowerText.text = thisUnit.Power.ToString();
                PowerText.transform.parent.gameObject.SetActive(true);
            }

            else
            {
                //カード画像
                CardImage.gameObject.SetActive(false);

                //パワー
                PowerText.transform.parent.gameObject.SetActive(false);
            }

            //射程
            showRangeClass.ShowRange(thisUnit);
        }

        //タップ
        if (thisUnit.IsTapped)
        {
            TapObject.SetActive(true);

            if (thisUnit.OldIsTapped != thisUnit.IsTapped)
            {
                thisUnit.OldIsTapped = thisUnit.IsTapped;
                anim.SetInteger("Tap", 1);
            }
        }

        else
        {
            TapObject.SetActive(false);

            if (thisUnit.OldIsTapped != thisUnit.IsTapped)
            {
                thisUnit.OldIsTapped = thisUnit.IsTapped;
                anim.SetInteger("Tap", -1);
            }
        }

        if (BreakOrbCountText != null)
        {
            BreakOrbCountText.gameObject.SetActive(false);

            if(GManager.instance.turnStateMachine.AttackingUnit != null && GManager.instance.turnStateMachine.DefendingUnit != null)
            {
                if(GManager.instance.turnStateMachine.DefendingUnit.Character != null)
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit == thisUnit && GManager.instance.turnStateMachine.DefendingUnit == GManager.instance.turnStateMachine.DefendingUnit.Character.Owner.Lord)
                    {
                        if(GManager.instance.turnStateMachine.endTapAttackingUnit)
                        {
                            BreakOrbCountText.gameObject.SetActive(true);
                            BreakOrbCountText.text = $"×{thisUnit.Strike}";
                        }
                    }
                }
            }
        }
    }
    #endregion

    #region 自動でUnitの情報を反映
    int count = 0;
    int UpdateFrame = 4;
    public bool destroyed { get; set; } = false;
    private void LateUpdate()
    {
        #region 例外チェック
        if (destroyed)
        {
            return;
        }

        if (thisUnit == null)
        {
            return;
        }

        if (thisUnit.Characters.Count == 0)
        {
            StartCoroutine(DestroyCoroutine());
            return;
        }
        #endregion

        #region 数フレームに一回だけ更新
        count++;

        if (count < UpdateFrame)
        {
            return;
        }

        else
        {
            count = 0;
        }
        #endregion

        ShowUnitData();
    }
    #endregion

    #region このオブジェクトを削除
    IEnumerator DestroyCoroutine()
    {
        yield return null;
        destroyed = true;
        //エフェクトとか
        Destroy(this.gameObject);
    }
    #endregion

    #region 選択状態
    public bool isExpand { get; set; }

    float OldParentSpacing;

    #region 選択状態表示
    public void OnSelectEffect(float expand)
    {
        if (isExpand)
        {
            return;
        }

        Outline_Select.gameObject.SetActive(true);

        StartCoroutine(ExpandCoroutine(expand));

        isExpand = true;

        CheckParentSpacing(expand);

        SetOrangeOutline();
    }
    #endregion

    #region 選択アウトラインオレンジ
    public void SetOrangeOutline()
    {
        Outline_Select.color = DataBase.SelectColor_Orange;
    }
    #endregion

    #region　選択アウトラインブルー
    public void SetBlueOutline()
    {
        Outline_Select.color = DataBase.SelectColor_Blue;
    }
    #endregion

    #region 拡大
    IEnumerator ExpandCoroutine(float expand)
    {
        float ExpandTime = 0.06f;

        float targetScale = StartScale.x * expand;

        float expandSpeed = (targetScale - transform.localScale.x) / ExpandTime;

        while (transform.localScale.x < targetScale)
        {
            transform.localScale += new Vector3(expandSpeed * Time.deltaTime, expandSpeed * Time.deltaTime, 0);

            yield return new WaitForSeconds(Time.deltaTime);

            if (!isExpand)
            {
                transform.localScale = StartScale;
                yield break;
            }
        }

        transform.localScale = new Vector3(targetScale, targetScale, 1);

        if (!isExpand)
        {
            transform.localScale = StartScale;
            yield break;
        }
    }
    #endregion

    #region 拡大時に親のSpacingを調整
    void CheckParentSpacing(float expand)
    {
        if (thisUnit.Character != null)
        {
            if (this.transform.parent == thisUnit.Character.Owner.BackZoneTransform)
            {
                return;
            }
        }

        this.transform.parent.GetComponent<XAxisLayoutGroup3D>().spacing = OldParentSpacing;

        for (int i = 0; i < this.transform.parent.childCount; i++)
        {
            if (this.transform.parent.GetChild(i).GetComponent<FieldUnitCard>().isExpand)
            {
                this.transform.parent.GetComponent<XAxisLayoutGroup3D>().spacing = OldParentSpacing * expand;
                break;
            }
        }
    }
    #endregion

    #region 選択状態リセット
    public void RemoveSelectEffect()
    {
        if (!isExpand)
        {
            return;
        }

        Outline_Select.gameObject.SetActive(false);

        transform.localScale = StartScale;// new Vector3(1.2f, 1.2f, 1);

        isExpand = false;
    }
    #endregion
    #endregion

    #region 場のカードのワールド座標をキャンバス座標に変換
    public Vector3 GetLocalCanvasPosition()
    {
        if (thisUnit != null)
        {
            if (thisUnit.Character != null)
            {
                if (thisUnit.Character.Owner.isYou)
                {
                    if (thisUnit.Character.Owner.GetFrontUnits().Contains(thisUnit))
                    {
                        return new Vector3(160 + 9.44f * (this.transform.position.x - 18), -110, 0);
                    }

                    else
                    {
                        return new Vector3(160 + 9.44f * (this.transform.position.x - 18), -245, 0);
                    }

                }

                else
                {
                    if (thisUnit.Character.Owner.GetFrontUnits().Contains(thisUnit))
                    {
                        return new Vector3(160 + 9.44f * (this.transform.position.x - 18), 200, 0);
                    }

                    else
                    {
                        return new Vector3(160 + 9.44f * (this.transform.position.x - 18), 335, 0);
                    }
                }
            }
        }

        return Vector3.zero;
    }
    #endregion

    #region コマンドパネルを閉じる
    //public UnityAction OnClickCloseFieldUnitCommandPanelButtonAction;
    public void CloseFieldUnitCommandPanel()
    {
        fieldUnitCommandPanel.CloseFieldUnitCommandPanel();
    }
    #endregion

    #region ドラッグ
    public UnityAction<FieldUnitCard> OnBeginDragAction { get; set; }
    public UnityAction<FieldUnitCard,List<DropArea>> OnDragAction { get; set; }
    public UnityAction<FieldUnitCard, List<DropArea>> OnEndDragAction { get; set; }

    public void RemoveDragTarget()
    {
        this.OnBeginDragAction = null;
        this.OnDragAction = null;
        this.OnEndDragAction = null;
    }

    public void AddDragTarget(UnityAction<FieldUnitCard> OnBeginDragAction, UnityAction<FieldUnitCard, List<DropArea>> OnDragAction, UnityAction<FieldUnitCard, List<DropArea>> OnEndDragAction)
    {
        this.OnBeginDragAction = OnBeginDragAction;
        this.OnDragAction = OnDragAction;
        this.OnEndDragAction = OnEndDragAction;
    }

    public void OnBeginDrag()
    {
        OnBeginDragAction?.Invoke(this);
    }

    public void OnDrag()
    {
        OnDragAction?.Invoke(this,Draggable.GetRaycastArea(null));
    }

    public void OnEndDrag()
    {
        OnEndDragAction?.Invoke(this, Draggable.GetRaycastArea(null));
    }
    #endregion
}

#region 射程
[System.Serializable]
public class ShowRangeClass
{
    public List<Text> RangeText = new List<Text>();

    public void ShowRange(Unit unit)
    {
        List<int> Range = new List<int>();

        foreach(int range in unit.Range)
        {
            if(range != 0)
            {
                Range.Add(range);
            }
        }

        int RangeCount = Range.Count;

        if(unit.Character.IsReverse)
        {
            RangeCount = 0;
        }

        for (int i =0;i<RangeText.Count;i++)
        {
            if (i < RangeCount)
            {
                RangeText[i].transform.parent.gameObject.SetActive(true);

                RangeText[i].text = Range[i].ToString();
            }

            else
            {
                RangeText[i].transform.parent.gameObject.SetActive(false);
            }
        }
    }
}
#endregion

#region 攻撃・起動効果
#region 攻撃・起動効果パネル
[System.Serializable]
public class FieldUnitCommandPanel
{
    [Header("コマンドパネルオブジェクト")]
    public GameObject CommandPanel;

    [Header("コマンドパネル背景")]
    public RectTransform CommandPanelBackGround;

    [Header("コマンドボタン")]
    public List<FieldUnitCommandButton> Buttons = new List<FieldUnitCommandButton>();

    public void SetUpFieldUnitCommandPanel(List<FieldUnitCommand> FieldUnitCommands,FieldUnitCard fieldUnitCard)
    {
        for (int i=0;i< Buttons.Count;i++)
        {
            if(i < FieldUnitCommands.Count)
            {
                Buttons[i].SetUpFieldUnitCommandButton(FieldUnitCommands[i].ButtonMessage, FieldUnitCommands[i].OnClickAction, FieldUnitCommands[i].Active);
            }

            else
            {
                Buttons[i].CloseFieldUnitCommandButton();
            }
        }

        CommandPanelBackGround.sizeDelta = new Vector2(CommandPanelBackGround.sizeDelta.x, 4 * (FieldUnitCommands.Count + 1));

        CommandPanel.transform.localRotation = Quaternion.Euler(0, 0, fieldUnitCard.transform.localRotation.eulerAngles.y);

        CommandPanel.SetActive(true);
    }

    public void CloseFieldUnitCommandPanel()
    {
        CommandPanel.SetActive(false);
    }
}

public class FieldUnitCommand
{
    public FieldUnitCommand(string ButtonMessage, UnityAction OnClickAction,bool Active)
    {
        this.ButtonMessage = ButtonMessage;
        this.OnClickAction = OnClickAction;
        this.Active = Active;
    }

    public string ButtonMessage { get; set; }
    public UnityAction OnClickAction { get; set; }
    public bool Active { get; set; }
}
#endregion

[System.Serializable]
public class FieldUnitCommandButton
{
    [Header("ボタン")]
    public Button button;

    [Header("ボタンテキスト")]
    public Text ButtonText;

    public void SetUpFieldUnitCommandButton(string ButtonMessage,UnityAction OnClickAtion,bool Active)
    {
        ButtonText.text = ButtonMessage;
        button.gameObject.SetActive(true);
        button.interactable = Active;

        button.onClick.RemoveAllListeners();

        if (Active)
        {
            button.onClick.AddListener(() => { OnClickAtion?.Invoke(); });
        }
    }

    public void CloseFieldUnitCommandButton()
    {
        button.gameObject.SetActive(false);
    }
}
#endregion
