using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetailCard : MonoBehaviour
{
    public RectTransform Rect;

    public Transform Top;

    public Transform Bottom;

    public Transform Right;

    public Image DetailCardImage;

    public Image EnglishImage;
    public void SetUpDetailCard(CEntity_Base cEntity_Base, Vector3 position)
    {
        this.gameObject.SetActive(true);
        DetailCardImage.sprite = cEntity_Base.CardImage;
        this.transform.position = position;
        SetPosition(this.transform.localPosition);

        if (EnglishImage != null)
        {
            EnglishImage.gameObject.SetActive(false);

            if(cEntity_Base != null)
            {
                if (cEntity_Base.CardImage_English != null)
                {
                    if (ContinuousController.instance.language == Language.ENG)
                    {
                        EnglishImage.gameObject.SetActive(true);
                        EnglishImage.sprite = cEntity_Base.CardImage_English;
                    }
                }
            }
        }
    }

    public void OffDetailCard()
    {
        this.gameObject.SetActive(false);
    }

    float TopPos
    {
        get
        {
            return Rect.localPosition.y + transform.localScale.y * Rect.sizeDelta.y / 2f;
        }
    }

    float BottomPos
    {
        get
        {
            return Rect.localPosition.y - transform.localScale.y * Rect.sizeDelta.y / 2f;
        }
    }

    float RightPos
    {
        get
        {
            return Rect.localPosition.x + transform.localScale.x * Rect.sizeDelta.x / 2f;
        }
    }

    bool TopOver()
    {
        return TopPos > Top.localPosition.y;
    }

    bool BottomOver()
    {
        return BottomPos < Bottom.localPosition.y;
    }

    bool RightOver()
    {
        if (Right != null)
        {
            return RightPos > Right.localPosition.x;
        }

        return false;
    }

    public void SetPosition(Vector3 pos)
    {
        Rect.localPosition = pos;

        //上にはみ出していた場合
        while (TopOver())
        {
            Rect.localPosition -= new Vector3(0, 5, 0);
        }

        //下にはみ出していた場合
        while (BottomOver())
        {
            Rect.localPosition += new Vector3(0, 5, 0);
        }

        //右にはみ出していた場合
        while (RightOver())
        {
            Rect.localPosition -= new Vector3(5, 0, 0);
        }
    }
}
