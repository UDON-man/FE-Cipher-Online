using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using UnityEngine.Events;

public class UnitDetail : MonoBehaviour
{
    [Header("ユニット情報パネル")]
    public GameObject unitInfoPanel;

    [Header("キャラクター情報プレハブ")]
    public CharacterInfo characterInfoPrefab;

    [Header("キャラクター情報親")]
    public Transform characterInfoParent;

    [Header("カード詳細メッセージ")]
    public GameObject ShowCardDetailMessage;

    public ScrollRect scrollRect;

    Unit _unit;

    //bool oldExistOutline = false;

    public void OpenUnitDetail(Unit unit)
    {
        ShowCardDetailMessage.SetActive(false);

        _unit = unit;

        //oldExistOutline = _unit.ShowingFieldUnitCard.Outline_Select.gameObject.activeSelf;

        //_unit.ShowingFieldUnitCard.Outline_Select.gameObject.SetActive(true);

        this.gameObject.SetActive(true);

        for(int i=0;i< characterInfoParent.childCount;i++)
        {
            Destroy(characterInfoParent.GetChild(i).gameObject);
        }

        foreach(CardSource character in unit.Characters)
        {
            CharacterInfo characterInfo = Instantiate(characterInfoPrefab, characterInfoParent);
            characterInfo.SetUpCharacterInfo(character,OnEnter,OnExit);
            characterInfo.scrollRect.content = scrollRect.content;
            characterInfo.scrollRect.viewport = scrollRect.viewport;


            void OnEnter()
            {
                ShowCardDetailMessage.SetActive(true);

                int index = characterInfo.transform.GetSiblingIndex();

                ShowCardDetailMessage.transform.localPosition = new Vector3(ShowCardDetailMessage.transform.localPosition.x, 205.5f - 134.78f * index ,0);
            }

            void OnExit()
            {
                ShowCardDetailMessage.SetActive(false);
            }
        }

        Vector3 targetPositon = Vector3.zero;
        Vector3 startPosition = Vector3.zero;

        if (_unit.ShowingFieldUnitCard.transform.position.x > 27)
        {
            targetPositon = new Vector3(-160, 0, 0);
            startPosition = new Vector3(-130, 0, 0);
        }

        else if (_unit.ShowingFieldUnitCard.transform.position.x > -27)
        {
            targetPositon = new Vector3(510, 0, 0);
            startPosition = new Vector3(480, 0, 0);
        }

        else
        {
            targetPositon = new Vector3(160, 0, 0);
            startPosition = new Vector3(130, 0, 0);
        }

        unitInfoPanel.transform.localPosition = startPosition;
        unitInfoPanel.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);

        float animationTime = 0.12f;

        var sequence = DOTween.Sequence();

        sequence
            .Append(unitInfoPanel.transform.DOLocalMove(targetPositon, animationTime))
            .Join(unitInfoPanel.transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), animationTime));

        sequence.Play();
    }

    public void CloseUnitDetail()
    {
        this.gameObject.SetActive(false);

        if (_unit != null)
        {
            if (_unit.ShowingFieldUnitCard != null)
            {
                //_unit.ShowingFieldUnitCard.Outline_Select.gameObject.SetActive(oldExistOutline);
            }
        }
    }
}