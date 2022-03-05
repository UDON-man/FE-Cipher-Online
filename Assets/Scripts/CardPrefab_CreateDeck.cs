using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;
using Shapes2D;
using Coffee.UIEffects;
using UnityEngine.EventSystems;
public class CardPrefab_CreateDeck : MonoBehaviour
{
    [Header("カード画像")]
    public Image CardImage;

    [Header("プレイコスト背景")]
    public List<Image> PlayCostBackGround = new List<Image>();

    [Header("プレイコスト")]
    public Text PlayCostText;

    [Header("CCコスト背景")]
    public Image CCCostBackGround;

    [Header("CCコスト")]
    public Text CCCostText;

    [Header("スクロール")]
    public List<ScrollRect> scroll = new List<ScrollRect>();

    [Header("カバー")]
    public List<GameObject> Cover = new List<GameObject>();

    [Header("アウトライン")]
    public GameObject Outline;

    [Header("アニメーター")]
    public Animator anim;

    public Transform Parent;

    public CEntity_Base cEntity_Base { get; set; }

    public UnityAction OnClickAction;

    public UnityAction OnEnterAction;

    public UnityAction OnExitAction;

    public UnityAction<CardPrefab_CreateDeck> OnBeginDragAction;

    public bool isActive = true;

    private void Start()
    {
        Outline.SetActive(false);
        if (CardImage.GetComponent<UIShiny>() != null)
        {
            CardImage.GetComponent<UIShiny>().enabled = false;
        }
    }
    public void SetUpCardPrefab_CreateDeck(CEntity_Base _cEntity_Base)
    {
        cEntity_Base = _cEntity_Base;

        SetCover(false);

        //カード画像
        CardImage.sprite = cEntity_Base.CardImage;

        CardImage.color = new Color(1, 1, 1, 1);

        //プレイコスト背景色
        for(int i=0;i<PlayCostBackGround.Count;i++)
        {
            if(i < cEntity_Base.cardColors.Count)
            {
                PlayCostBackGround[i].color = DataBase.CardColor_ColorLightDictionary[cEntity_Base.cardColors[i]];
            }

            else
            {
                PlayCostBackGround[i].color = DataBase.CardColor_ColorLightDictionary[cEntity_Base.cardColors[0]];
            }
        }
        
        //プレイコスト
        PlayCostText.text = cEntity_Base.PlayCost.ToString();

        PlayCostBackGround[0].transform.parent.gameObject.SetActive(true);

        if (cEntity_Base.CCCost > 0)
        {
            //CCコスト背景
            CCCostBackGround.gameObject.SetActive(cEntity_Base.HasCC);

            //CCコスト
            CCCostText.text = cEntity_Base.CCCost.ToString();

            CCCostBackGround.transform.parent.gameObject.SetActive(true);
        }

        else
        {
            CCCostBackGround.transform.parent.gameObject.SetActive(false);
        }
        
        CardImage.GetComponent<EventTrigger>().enabled = true;

        /*
        Outline.SetActive(false);
        if (CardImage.GetComponent<UIShiny>()!= null)
        {
            CardImage.GetComponent<UIShiny>().enabled = false;
        }
        */
    }

    public void HideDeckCardTab()
    {
        PlayCostText.transform.parent.gameObject.SetActive(false);
        CCCostText.transform.parent.gameObject.SetActive(false);
        CardImage.color = new Color(1, 1, 1, 0);
        CardImage.GetComponent<EventTrigger>().enabled = false;
        Outline.SetActive(false);

        if (CardImage.GetComponent<UIShiny>() != null)
        {
            CardImage.GetComponent<UIShiny>().enabled = false;
        }
    }

    public void SetCover(bool active)
    {
        foreach (GameObject g in Cover)
        {
            g.SetActive(active);
        }
    }

    public void CheckCover(DeckData deckData)
    {
        SetCover(OverMaxCount(deckData));
    }

    public bool OverMaxCount(DeckData deckData)
    {
        return deckData.DeckCards().Count((_cEntity_Base) => _cEntity_Base.CardName == cEntity_Base.CardName) >= cEntity_Base.MaxCountInDeck;
    }

    public void OnClick()
    {
        OnClickAction?.Invoke();
    }

    public void OnBeginDrag()
    {
        OnBeginDragAction?.Invoke(this);
    }

    public void OnEnter()
    {
        Outline.SetActive(true);
        OnEnterAction?.Invoke();
        anim.SetInteger("Open", 1);
        anim.SetInteger("Close", 0);
        CardImage.GetComponent<UIShiny>().enabled = true;
    }

    public void OnExit()
    {
        Outline.SetActive(false);
        OnExitAction?.Invoke();
        anim.SetInteger("Open", 0);
        anim.SetInteger("Close", 1);
        CardImage.GetComponent<UIShiny>().enabled = false;
    }
}
