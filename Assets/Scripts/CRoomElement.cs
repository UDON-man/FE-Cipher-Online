using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;

public class CRoomElement : MonoBehaviour
{
    //Room情報UI表示用
    public Text PlayerNumber;   //人数
    public Text RoomCreator;    //部屋作成者名

    //入室ボタンroomname格納用
    private string roomname;

    public string RoomName { get => roomname; }

    //GetRoomListからRoom情報をRoomElementにセットしていくための関数
    public void SetRoomInfo(string _RoomName, int _PlayerNumber, string _RoomCreator)
    {
        roomname = _RoomName;
        //入室ボタン用roomname取得
        PlayerNumber.text = "人　数：" + _PlayerNumber + "/2";
        RoomCreator.text = "作成者：" + _RoomCreator;
    }

    [HideInInspector] public UnityAction OnClick;

    //入室ボタン処理
    public void OnJoinRoomButton()
    {
        if (OnClick != null)
        {
            OnClick.Invoke();
        }
    }
}
