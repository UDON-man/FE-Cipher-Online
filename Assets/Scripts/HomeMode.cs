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
public class HomeMode : MonoBehaviour
{
    [Header("プレイヤー情報")]
    public PlayerInfo playerInfo;

    public GameObject HomeCharacters;

    [Header("読み込み中オブジェクト")]
    public LoadingObject loadingObject;

    public GameObject UpdateButtonParent;

    bool first = false;
    public void OffHome()
    {
        if(!first)
        {
            first = true;
        }
        
        playerInfo.OffPlayerInfo();

        Opening.instance.OffModeButtons();

        HomeCharacters.SetActive(false);

        Opening.instance.checkUpdate.UpdateButton.SetActive(false);

        UpdateButtonParent.SetActive(false);
    }

    public void SetUpHome()
    {
        playerInfo.SetPlayerInfo();

        Opening.instance.OnModeButtons();

        HomeCharacters.SetActive(true);

        Opening.instance.checkUpdate.UpdateButton.SetActive(ContinuousController.instance.NeedUpdate);

        UpdateButtonParent.SetActive(true);
    }

    public void SetUpHomeMode_Disconnect()
    {
        StartCoroutine(SetUpHomeMode_DisconnectCoroutine());
    }

    public IEnumerator SetUpHomeMode_DisconnectCoroutine()
    {
        if(PhotonNetwork.IsConnected)
        {
            yield return ContinuousController.instance.StartCoroutine(loadingObject.StartLoading("Disconnecting"));

            yield return ContinuousController.instance.StartCoroutine(PhotonUtility.DisconnectCoroutine());

            yield return ContinuousController.instance.StartCoroutine(loadingObject.EndLoading());
        }

        SetUpHome();
    }
}