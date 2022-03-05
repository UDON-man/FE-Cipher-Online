using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMode : MonoBehaviour
{
    [Header("バトルボタン")]
    public OpeningButton BattleButton;

    [Header("バトルモード選択")]
    public SelectBattleMode selectBattleMode;

    [Header("バトルデッキ選択")]
    public SelectBattleDeck selectBattleDeck;

    [Header("ランダムマッチ")]
    public LobbyManager_RandomMatch lobbyManager_RandomMatch;

    [Header("ルーム作成")]
    public CreateRoom createRoom;

    [Header("ルーム入室")]
    public EnterRoom enterRoom;

    [Header("ルーム画面")]
    public RoomManager roomManager;

    bool first = false;

    public void OffBattle()
    {
        roomManager.Off();

        selectBattleDeck.Off();

        selectBattleMode.OffSelectBattleMode();

        lobbyManager_RandomMatch.OffLobby();

        createRoom.Off();

        enterRoom.Off();

        if (!first)
        {
            BattleButton.OnExit();
            first = true;
        }
    }

    public void SetUpBattleMode()
    {
        selectBattleMode.SetUpSelectBattleMode();
    }
}
