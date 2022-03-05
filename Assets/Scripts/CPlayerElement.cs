using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CPlayerElement : MonoBehaviour
{
    //Room情報UI表示用
    public Text PlayerName;   //プレイヤー名
    public Text IsReady; //待機状態

    //入室ボタンroomname格納用
    private string playername;

    //GetRoomListからRoom情報をRoomElementにセットしていくための関数
    public void SetPlayerInfo(string _PlayerName, bool _IsReady)
    {
        //入室ボタン用roomname取得
        playername = _PlayerName;
        PlayerName.text = _PlayerName;//"プレイヤー名：" + _PlayerName;

        if (_IsReady)
        {
            IsReady.text = "OK!";//"待機状態:準備完了";

            IsReady.color = new Color32(53, 255, 4, 255);
        }

        else
        {
            IsReady.text = "In preparation...";//"待機状態:準備中";

            IsReady.color = Color.red;
        }
    }
}
