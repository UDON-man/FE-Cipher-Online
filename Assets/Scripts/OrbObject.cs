using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrbObject : MonoBehaviour
{
    [Header("オーブ数テキスト")]
    public Text OrbCountText;

    [Header("オーブカード")]
    public List<Image> OrbCards = new List<Image>();

    [Header("プレイヤー")]
    public Player player;

    #region オーブの情報を反映
    public void ShowOrb()
    {
        OrbCountText.text = $"{player.OrbCards.Count}";

        for(int i=0;i<OrbCards.Count;i++)
        {
            if (i < player.OrbCards.Count)
            {
                OrbCards[i].gameObject.SetActive(true);
            }

            else
            {
                OrbCards[i].gameObject.SetActive(false);
            }
        }
    }
    #endregion

    #region 自動でオーブの情報を反映
    int count = 0;
    int UpdateFrame = 4;
    public bool destroyed { get; set; } = false;
    private void LateUpdate()
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

        ShowOrb();
    }
    #endregion
}

