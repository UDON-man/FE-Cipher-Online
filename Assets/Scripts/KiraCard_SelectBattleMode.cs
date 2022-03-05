using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class KiraCard_SelectBattleMode : MonoBehaviour
{
    [Header("アニメーター")]
    public Animator anim;

    [Header("Glowアニメーター")]
    public Animator GlowAnim;

    [Header("移動開始位置")]
    public Vector3 StartPos;

    [Header("移動開始スケール")]
    public Vector3 StartScale;

    [Header("移動終了位置")]
    public Vector3 EndPos;

    [Header("移動終了スケール")]
    public Vector3 EndScale;

    [Header("マッチングモード")]
    public MatchingMode matchingMode;

    [Header("もどるボタン")]
    public Button ReturnButton;

    [Header("もうひとつの方のカード")]
    public List<KiraCard_SelectBattleMode> Anothers;

    [Header("バトルルール・ルーム入室作成選択")]
    public SelectKiraCardParent selectBattleOption;

    public bool isSelected { get; set; } = false;
    bool CanClick = true;

    public UnityAction OnClickAction;

    public Button ReturnHomeButton;

    public Button RetrunSelectMatcingModeButton;

    public bool isAI;

    #region クリック時
    public void OnClick()
    {
        if(CanClick)
        {
            //OnClickAction?.Invoke();

            if (!isSelected)
            {
                OnClick_Move_Select();

                if(selectBattleOption != null)
                {
                    selectBattleOption.SetUpSelectBattleMode();
                }
                
            }

            else
            {
                OnClick_Move_RemoveSelect();

                if (selectBattleOption != null)
                {
                    selectBattleOption.Off();
                }
            }
        }
    }
    #endregion

    #region 非表示
    public void Off()
    {
        SetUp();
        this.gameObject.SetActive(false);
    }
    #endregion

    #region 初期化
    public void SetUp()
    {
        this.gameObject.SetActive(true);
        this.transform.localPosition = StartPos;
        this.transform.localScale = StartScale;
        GlowAnim.SetInteger("IsLit", 0);

        isSelected = false;
        CanClick = true;

        ReturnButton.enabled = true;

        ReturnButton.onClick = ReturnHomeButton.onClick;

        anim.SetInteger("IsRotate", 1);
    }
    #endregion

    #region 選択
    public void OnClick_Move_Select()
    {
        if(!isSelected)
        {
            StartCoroutine(MoveCoroutine_Select());
        }
    }

    public IEnumerator MoveCoroutine_Select()
    {
        isSelected = true;

        anim.SetInteger("IsRotate", 0);

        foreach(KiraCard_SelectBattleMode Another in Anothers)
        {
            Another.Off();
        }
        
        GlowAnim.SetInteger("IsLit", 1);

        ReturnButton.enabled = false;
        ReturnButton.onClick.RemoveAllListeners();

        
        CanClick = false;

        bool end = false;

        var sequence = DOTween.Sequence();

        sequence
            .Append(this.transform.DOLocalMove(EndPos, 0.2f))
            .Join(this.transform.DOScale(EndScale, 0.2f))
            .AppendCallback(() => end = true);

        sequence.Play();

        yield return new WaitWhile(() => !end);
        end = true;

        CanClick = true;

        ReturnButton.enabled = true;
        ReturnButton.onClick = RetrunSelectMatcingModeButton.onClick;

        ContinuousController.instance.isAI = isAI;
    }
    #endregion

    #region 選択解除
    public void OnClick_Move_RemoveSelect()
    {
        if (isSelected)
        {
            StartCoroutine(MoveCoroutine_RemoveSelect());
        }
    }

    public IEnumerator MoveCoroutine_RemoveSelect()
    {
        isSelected = false;

        anim.SetInteger("IsRotate", 1);

        foreach (KiraCard_SelectBattleMode Another in Anothers)
        {
            Another.SetUp();
        }
        
        GlowAnim.SetInteger("IsLit", 0);

        ReturnButton.enabled = false;
        ReturnButton.onClick.RemoveAllListeners();

        CanClick = false;

        bool end = false;

        var sequence = DOTween.Sequence();

        sequence
            .Append(this.transform.DOLocalMove(StartPos, 0.2f))
            .Join(this.transform.DOScale(StartScale, 0.2f))
            .AppendCallback(() => end = true);

        sequence.Play();

        yield return new WaitWhile(() => !end);
        end = true;

        SetUp();
    }
    #endregion

    public SelectBattleDeck selectBattleDeck;
    
    public void OnClick_RandomMatch()
    {
        ContinuousController.instance.isAI = false;
        OnClick_StartBattle();
    }

    public void OnClick_BotMatch()
    {
        ContinuousController.instance.isAI = true;
        OnClick_StartBattle();
    }

    public void OnClick_StartBattle()
    {
        if (!ContinuousController.instance.isAI)
        {
            selectBattleDeck.SetUpSelectBattleDeck(selectBattleDeck.OnClickSelectButton_RandomMatch);
        }

        else
        {
            selectBattleDeck.SetUpSelectBattleDeck(() =>
            {
                selectBattleDeck.OnClickSelectButton_BotMatch();
                ContinuousController.instance.StartCoroutine(StartBattleCoroutine());
            });
        }

        IEnumerator StartBattleCoroutine()
        {
            yield return ContinuousController.instance.StartCoroutine(Opening.instance.LoadingObject.StartLoading("Now Loading"));
            Opening.instance.MainCamera.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            SceneManager.LoadSceneAsync("BattleScene", LoadSceneMode.Additive);
        }
    }
}

public enum MatchingMode
{
    RandomMatch,
    RoomMatch,
}

/*
public enum BattleRule
{
    Unlimited,
    JCS_Final,
}
*/