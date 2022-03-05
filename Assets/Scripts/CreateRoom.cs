using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class CreateRoom : OffAnimation
{
    [Header("バトルルールドロップダウン")]
    public Dropdown BattleRuleDropdown;

    [Header("アニメータ")]
    public Animator anim;

    [Header("ルーム画面")]
    public RoomManager roomManager;

    public void SetUpCreateRoom()
    {
        if (this.gameObject.activeSelf)
        {
            return;
        }

        BattleRuleDropdown.value = 0;

        this.gameObject.SetActive(true);

        anim.SetInteger("Open", 1);
        anim.SetInteger("Close", 0);
    }

    public void CloseCreateRoom()
    {
        anim.SetInteger("Open", 0);
        anim.SetInteger("Close", 1);
    }

    public void OnClickCreateRoomButton()
    {
        ContinuousController.instance.isAI = false;
        ContinuousController.instance.isRandomMatch = false;
        roomManager.SetUpRoom();// (BattleRule)Enum.ToObject(typeof(BattleRule), BattleRuleDropdown.value));
        CloseCreateRoom();
    }
}
