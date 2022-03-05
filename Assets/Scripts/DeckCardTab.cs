using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;
using Coffee.UIExtensions;
using Coffee.UIEffects;
using UnityEngine.EventSystems;

public class DeckCardTab : MonoBehaviour
{
    [Header("背景")]
    public Image BackGround;

    [Header("コスト背景")]
    public Image CostBackGround;

    [Header("コスト")]
    public Text PlayCostText;

    [Header("カード種別テキスト")]
    public Text CardKindText;

    [Header("カード名テキスト")]
    public Text CardNameText;

    [Header("カード画像")]
    public Image CardImage;

    [Header("枚数テキスト背景")]
    public Image CardCountBackGround;

    [Header("枚数テキスト")]
    public Text CardCount;

    [Header("スクロール")]
    public ScrollRect scroll;

    [Header("アウトライン")]
    public GameObject Outline;

    [Header("アニメーター")]
    public Animator anim;

    public CEntity_Base cEntity_Base { get; set; }

    public UnityAction OnClickBackGroundAction;

    public UnityAction OnEnterBackGroundAction;

    public UnityAction OnExitBackGroundAction;

    public UnityAction OnClickCountImageAction;

    public UnityAction OnEnterCountImageAction;

    public UnityAction OnExitCountImageAction;

    //public int TabNumber { get { return transform.GetSiblingIndex(); } }

    public bool isVisible { get; set; }
    void OnWillRenderObject()

    {

#if UNITY_EDITOR

        if (Camera.current.name != "SceneCamera" && Camera.current.name != "Preview Camera")

#endif

        {
            Debug.Log("見えるようになった");
            isVisible = true;

        }

    }

    void OnBecameInvisible()
    {

        Debug.Log("見えないようになった");
        isVisible = false;

    }

    public void SetUpDeckCardPrefab(CEntity_Base _cEntity_Base, DeckData deckData)
    {
        cEntity_Base = _cEntity_Base;

        //背景色
        //BackGround.color = DataBase.CardColor_ColorDarkDictionary[cEntity_Base.cardColor];

        //コスト背景色
        //CostBackGround.color = DataBase.CardColor_ColorLightDictionary[cEntity_Base.cardColor];

        //コスト
        PlayCostText.text = cEntity_Base.PlayCost.ToString();

        //カード名
        CardNameText.text = cEntity_Base.CardName;

        //カード画像
        CardImage.sprite = cEntity_Base.CardImage;

        //同名カードをカウント
        SetCountText(cEntity_Base, deckData);

        Outline.SetActive(false);
    }

    
    public void SetCountText(CEntity_Base _cEntity_Base, DeckData deckData)
    {
        CardCount.text = deckData.DeckCards().Count((card) => card == _cEntity_Base).ToString();
    }

    public void OnClickBackGround()
    {
        OnClickBackGroundAction?.Invoke();
    }

    public void OnEnterBackGround()
    {
        OnEnterBackGroundAction?.Invoke();

        OnEnter();
    }

    public void OnEnter()
    {
        Outline.SetActive(true);

        BackGround.GetComponent<UIShiny>().enabled = true;

        anim.SetInteger("Open", 1);
        anim.SetInteger("Close", 0);
    }

    public void OnExitBackGround()
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, results);

        foreach(RaycastResult result in results)
        {
            if(result.gameObject == CardCountBackGround.gameObject)
            {
                return;
            }
        }

        OnExit();
    }

    public void OnExit()
    {
        OnExitBackGroundAction?.Invoke();

        Outline.SetActive(false);

        BackGround.GetComponent<UIShiny>().enabled = false;

        anim.SetInteger("Open", 0);
        anim.SetInteger("Close",1);
    }

    public void OnClickCountImage()
    {
        OnClickCountImageAction.Invoke();
    }

    public void OnEnterCountImage()
    {
        //CardCount.text = "+";

        OnEnterCountImageAction?.Invoke();

        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, results);

        if (results.Count((result) => result.gameObject == BackGround.gameObject) > 0)
        {
            OnEnter();
        }
    }

    public void OnExitCountImage()
    {
        OnExitCountImageAction?.Invoke();

        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, results);

        if(results.Count((result)=>result.gameObject == BackGround.gameObject) == 0)
        {
            OnExit();
        }
    }
}
