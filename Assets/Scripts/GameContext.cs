using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//ゲーム全体の状況を管理するクラス
[System.Serializable]
public class GameContext
{
    #region コンストラクタ
    public GameContext(Player _You, Player _Opponent)
    {
        You = _You;
        Opponent = _Opponent;

        if (PhotonNetwork.IsConnected)
        {
            SetPlayerID();
        }

        TurnPhase = phase.UnTap;
    }
    #endregion

    #region シーン中のカードリスト
    public List<CardSource> ActiveCardList
    {
        get; set;
    } = new List<CardSource>();
    #endregion

    #region プレイヤー
    public Player You;
    public Player Opponent;

    public List<Player> Players
    {
        get
        {
            List<Player> players = new List<Player>();

            players.Add(PlayerFromID(0));
            players.Add(PlayerFromID(1));

            return players;
        }
    }

    public List<Player> Players_ForTurnPlayer
    {
        get
        {
            List<Player> players = new List<Player>();

            players.Add(TurnPlayer);
            players.Add(NonTurnPlayer);

            return players;
        }
    }

    public Player TurnPlayer;

    public Player NonTurnPlayer
    {
        get
        {
            Player _player = null;

            foreach (Player player in Players)
            {
                if (player != TurnPlayer)
                {
                    _player = player;
                    break;
                }
            }

            return _player;
        }
    }

    #endregion

    #region ターンのフェイズ
    public enum phase
    {
        //アンタップ
        UnTap,

        //ドロー
        Draw,

        //進軍
        March,

        //絆
        Bond,

        //絆カウント
        CountBond,

        //出撃
        Deploy,

        //行動
        Action,

        //終了
        End,
    }

    public phase TurnPhase;
    #endregion

    #region プレイヤーIDの割り当て
    public void SetPlayerID()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            You.PlayerID = 0;
            Opponent.PlayerID = 1;
        }

        else
        {
            You.PlayerID = 1;
            Opponent.PlayerID = 0;
        }
    }
    #endregion

    #region プレイヤーIDに対応するプレイヤーを返す
    public Player PlayerFromID(int playerID)
    {
        if(You.PlayerID == playerID)
        {
            return You;
        }

        else if(Opponent.PlayerID == playerID)
        {
            return Opponent;
        }

        return null;
    }
    #endregion

    #region ターンプレイヤー切り替え
    public void SwitchTurnPlayer()
    {
        foreach (Player player in Players)
        {
            if (player != TurnPlayer)
            {
                TurnPlayer = player;
                break;
            }
        }
    }
    #endregion
}

