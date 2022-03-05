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

public class SelectBattleMode : MonoBehaviour
{
    [Header("アニメーター")]
    public Animator anim;

    [Header("マッチング方式決定カードボタン")]
    public List<KiraCard_SelectBattleMode> kiraCards = new List<KiraCard_SelectBattleMode>();

    public List<SelectKiraCardParent> kiraCardParents = new List<SelectKiraCardParent>();

    [Header("バトルルール選択")]
    public SelectKiraCardParent selectBattleRule;

    [Header("ルームマッチ選択")]
    public SelectKiraCardParent selectRoomMatch;

    [Header("読み込み中オブジェクト")]
    public LoadingObject loadingObject;

    public CreateRoom createRoom;
    public EnterRoom enterRoom;

    public void OffSelectBattleMode()
    {
        Off();
    }

    public void Off()
    {
        this.gameObject.SetActive(false);
    }

    bool connecting = false;

    public void SetUpSelectBattleMode()
    {
        if(connecting || this.gameObject.activeSelf)
        {
            return;
        }

        ContinuousController.instance.StartCoroutine(SetUpSelectBattleModeCoroutine());
    }

    public IEnumerator SetUpSelectBattleModeCoroutine()
    {
        foreach(KiraCard_SelectBattleMode kiraCard_SelectBattle in kiraCards)
        {
            kiraCard_SelectBattle.gameObject.SetActive(true);
        }

        foreach(SelectKiraCardParent selectKiraCardParent in kiraCardParents)
        {
            foreach(SelectKiraCard selectKiraCard in selectKiraCardParent.kiraCards)
            {
                selectKiraCard.gameObject.SetActive(true);
            }
        }

        Opening.instance.battle.selectBattleDeck.Off();
        createRoom.Off();
        enterRoom.Off();

        yield return ContinuousController.instance.StartCoroutine(loadingObject.StartLoading("Connecting"));

        connecting = true;

        yield return ContinuousController.instance.StartCoroutine(PhotonUtility.ConnectCoroutine());

        ContinuousController.instance.BattleDeckData = null;
        yield return ContinuousController.instance.StartCoroutine(PhotonUtility.DeleteBattleDeckData());

        connecting = false;

        yield return ContinuousController.instance.StartCoroutine(loadingObject.EndLoading());

        selectBattleRule.Off();
        selectRoomMatch.Off();

        this.gameObject.SetActive(true);

        anim.SetInteger("Open", 1);
        anim.SetInteger("Close", 0);

        foreach (KiraCard_SelectBattleMode kiraCard_SelectBattleMode in kiraCards)
        {
            kiraCard_SelectBattleMode.SetUp();
        }
    }
}
