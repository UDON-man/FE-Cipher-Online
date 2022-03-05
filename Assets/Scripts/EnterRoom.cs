using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.Events;
public class EnterRoom : MonoBehaviourPunCallbacks
{
    [Header("ルームIDInputField")]
    public InputField RoomIDInputField;

    [Header("アニメータ")]
    public Animator anim;

    [Header("ルーム画面")]
    public RoomManager roomManager;

    [Header("入室ボタン")]
    public Button EnterRoomButton;

    [Header("YesNoオブジェクト")]
    public YesNoObject yesNoObject;

    Image EnterRoomButtonImage;

    private void Start()
    {
        EnterRoomButtonImage = EnterRoomButton.GetComponent<Image>();
    }


    public void SetUpEnterRoom()
    {
        if (this.gameObject.activeSelf)
        {
            return;
        }

        RoomIDInputField.text = "";

        this.gameObject.SetActive(true);

        anim.SetInteger("Open", 1);
        anim.SetInteger("Close", 0);
    }

    public void CloseEnterRoom()
    {
        anim.SetInteger("Open", 0);
        anim.SetInteger("Close", 1);
    }

    public void Off()
    {
        this.gameObject.SetActive(false);
    }

    bool canClick = true;

    public void OnClickEnterRoomButton()
    {
        if (CanClickEnterRoomButton() && canClick)
        {
            PhotonNetwork.JoinRoom(RoomIDInputField.text);
        }
    }

    IEnumerator JoinRoomCoroutine()
    {
        canClick = false;

        if(!PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        yield return new WaitUntil(() => PhotonNetwork.IsConnectedAndReady);

        if(!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        yield return new WaitUntil(() => PhotonNetwork.InLobby && PhotonNetwork.IsConnectedAndReady);

        PhotonNetwork.JoinRoom(RoomIDInputField.text);

        canClick = true;
    }

    public override void OnJoinedRoom()
    {
        ContinuousController.instance.isAI = false;
        ContinuousController.instance.isRandomMatch = false;
        roomManager.SetUpRoom();// BattleRule.Unlimited);
        CloseEnterRoom();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        yesNoObject.SetUpYesNoObject(new List<UnityAction>() { null }, new List<string>() { "OK" }, "Error!\nThe room was not found.", true);
    }

    bool CanClickEnterRoomButton()
    {
        if (!string.IsNullOrEmpty(RoomIDInputField.text))
        {
            if (RoomIDInputField.text.Length == 5)
            {
                return true;
            }
        }

        return false;
    }

    private void Update()
    {
        if(CanClickEnterRoomButton())
        {
            EnterRoomButton.enabled = true;

            if(EnterRoomButtonImage != null)
            {
                EnterRoomButtonImage.color = Color.white;
            }
        }

        else
        {
            EnterRoomButton.enabled = false;

            if (EnterRoomButtonImage != null)
            {
                EnterRoomButtonImage.color = new Color32(144, 144, 144, 255);
            }
        }
    }
}
