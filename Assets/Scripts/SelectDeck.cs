using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class SelectDeck : MonoBehaviour
{
    [Header("デッキ選択オブジェクト")]
    public GameObject SelectDeckObject;

    [Header("デッキ情報タブプレハブ")]
    public DeckInfoPrefab deckInfoPrefab;

    [Header("デッキ情報タブを置くScrollRect")]
    public ScrollRect deckInfoPrefabParentScroll;

    [Header("デッキ情報パネル")]
    public DeckInfoPanel deckInfoPanel;

    [Header("アニメーター")]
    public Animator anim;

    [Header("デッキ作成スクリプト")]
    public EditDeck editDeck;

    [Header("デッキカード一覧")]
    public DeckListPanel deckListPanel;

    public bool isOpen { get; set; } = false;

    public void OffSelectDeck()
    {
        //Off();
        anim.SetInteger("Open", 0);
        anim.SetInteger("Close", 1);

        isOpen = false;
    }

    public void Off()
    {
        SelectDeckObject.SetActive(false);
    }

    public void SetUpSelectDeck()
    {
        if (isOpen)
        {
            return;
        }

        deckListPanel.Off();

        isOpen = true;

        ContinuousController.instance.StartCoroutine(SetDeckList());

        SelectDeckObject.SetActive(true);

        anim.SetInteger("Open", 1);
        anim.SetInteger("Close", 0);

        if (ContinuousController.instance.DeckDatas.Count > 0)
        {
            deckInfoPanel.SetUpDeckInfoPanel(ContinuousController.instance.DeckDatas[0]);

            for(int i=0;i< deckInfoPrefabParentScroll.content.childCount;i++)
            {
                if(deckInfoPrefabParentScroll.content.GetChild(i).GetComponent<DeckInfoPrefab>() != null)
                {
                    if(deckInfoPrefabParentScroll.content.GetChild(i).GetComponent<DeckInfoPrefab>().thisDeckData == deckInfoPanel.ShowingDeckData)
                    {
                        deckInfoPrefabParentScroll.content.GetChild(i).GetComponent<DeckInfoPrefab>().Outline.SetActive(true);
                    }
                }
            }
        }

        else
        {
            ResetDeckInfoPanel();
        }
    }



    public void ResetDeckInfoPanel()
    {
        deckInfoPanel.SetUpDeckInfoPanel(null);
    }

    public IEnumerator SetDeckList()
    {
        for (int i = 0; i < deckInfoPrefabParentScroll.content.childCount; i++)
        {
            if (i > 0)
            {
                Destroy(deckInfoPrefabParentScroll.content.GetChild(i).gameObject);
            }
        }

        for (int i = 0; i < ContinuousController.instance.DeckDatas.Count; i++)
        {
            DeckInfoPrefab _deckInfoPrefab = Instantiate(deckInfoPrefab, deckInfoPrefabParentScroll.content);

            _deckInfoPrefab.scrollRect.content = deckInfoPrefabParentScroll.content;

            _deckInfoPrefab.scrollRect.viewport = deckInfoPrefabParentScroll.viewport;

            _deckInfoPrefab.scrollRect.verticalScrollbar = deckInfoPrefabParentScroll.verticalScrollbar;

            _deckInfoPrefab.SetUpDeckInfoPrefab(ContinuousController.instance.DeckDatas[i]);

            _deckInfoPrefab.transform.localScale = Opening.instance.DeckInfoPrefabStartScale * 1.02f;

            //yield return null;

            //_deckInfoPrefab.transform.localScale = Opening.instance.DeckInfoPrefabStartScale;

            _deckInfoPrefab.OnClickAction = (deckdata) => 
            { 
                deckInfoPanel.SetUpDeckInfoPanel(deckdata);

                Opening.instance.CreateOnClickEffect();
            };
        }

        yield return null;

        for(int i=0;i< deckInfoPrefabParentScroll.content.childCount;i++)
        {
            deckInfoPrefabParentScroll.content.GetChild(i).transform.localScale = Opening.instance.DeckInfoPrefabStartScale;
        }
    }
}
