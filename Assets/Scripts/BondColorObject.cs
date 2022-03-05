using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class BondColorObject : MonoBehaviour
{
    public Player player;

    public List<ColorIcon> colorIcons;

    int count = 0;
    int UpdateFrame = 4;

    private void Update()
    {
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

        SetColorIcons();
    }

    public void SetColorIcons()
    {
        foreach (ColorIcon colorIcon in colorIcons)
        {
            if (player.BondCards.Count((cardSource) => !cardSource.IsReverse && cardSource.cardColors.Contains(colorIcon.cardColor)) > 0)
            {
                if (colorIcon.Icon.sprite == DataBase.instance.CardColorIconDictionary.GetTable()[CardColor.None])
                {
                   // StartCoroutine(GManager.instance.GetComponent<Effects>().CreateColorEffect(colorIcon.cardColor, this));
                }

                colorIcon.Icon.sprite = DataBase.instance.CardColorIconDictionary.GetTable()[colorIcon.cardColor];
            }

            else
            {
                colorIcon.Icon.sprite = DataBase.instance.CardColorIconDictionary.GetTable()[CardColor.None];
            }
        }
    }
}

[System.Serializable]
public class ColorIcon
{
    public CardColor cardColor;
    public Image Icon;
}
