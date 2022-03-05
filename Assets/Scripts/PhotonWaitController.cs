using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;

public class PhotonWaitController : MonoBehaviour
{
    public int waitCount = 0;

    public void SetWaiting(string key, bool isGo, bool isAdd)
    {
        Hashtable PlayerProp = PhotonNetwork.LocalPlayer.CustomProperties;

        object value;

        if (isAdd)
        {
            if (PlayerProp.TryGetValue(key, out value))
            {
                if ((bool)PlayerProp[key] && !isGo)
                {
                    return;
                }

                PlayerProp[key] = isGo;
            }

            else
            {
                PlayerProp.Add(key, isGo);
            }
        }

        else
        {
            if (PlayerProp.TryGetValue(key, out value))
            {
                PlayerProp.Remove(key);
            }
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(PlayerProp);
    }


    public static bool isWaiting(string key, Photon.Realtime.Player player)
    {
        Hashtable PlayerProp = PhotonNetwork.LocalPlayer.CustomProperties;

        object value;

        if (PlayerProp.TryGetValue(key, out value))
        {
            if ((bool)value)
            {
                return false;
            }

            else
            {
#if !UNITY_WEBGL
                //Debug.Log($"自分のkey:{key}が{(bool)value}なので待機します");
#endif
                return true;
            }
        }

#if !UNITY_WEBGL
        //Debug.Log($"自分がkey:{key}を持っていないので待機します");
#endif
        return true;
    }

    public static bool AllIsWaiting(string key)//全員がkeyをfalseで持っていればtrue
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            Hashtable PlayerProp = player.CustomProperties;

            object value;

            if (PlayerProp.TryGetValue(key, out value))
            {
                if ((bool)value)
                {
                    return false;
                }
            }

            else
            {
                return false;
            }
        }

        return true;
    }

    bool RoomHasTrueKey(string key)
    {
        Hashtable roomHash = PhotonNetwork.CurrentRoom.CustomProperties;
        object value;

        if(roomHash != null)
        {
            if (roomHash.TryGetValue(key, out value))
            {
                if ((bool)value)
                {
                    return true;
                }
            }
        }
        
        return false;
    }

    void SetRoomTrueKey(string key)
    {
        Hashtable roomHash = PhotonNetwork.CurrentRoom.CustomProperties;
        object value;

        if(roomHash == null)
        {
            roomHash = new Hashtable();
        }

        if (roomHash.TryGetValue(key, out value))
        {
            roomHash[key] = true;//trueなら通ってよし
        }

        else
        {
            roomHash.Add(key, true);
        }

        PhotonNetwork.CurrentRoom.SetCustomProperties(roomHash);
    }

    public IEnumerator Wait(string key)
    {
        while (isWaiting(key, PhotonNetwork.LocalPlayer))
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (AllIsWaiting(key))
                {
                    SetRoomTrueKey(key);
                }
            }

            if (RoomHasTrueKey(key))
            {
                SetWaiting(key, true, false);
                break;
            }

            yield return null;
        }

#if !UNITY_WEBGL

        //Debug.Log(key + "を通過");
#endif
    }

    public Coroutine StartWait(string key)
    {
        waitCount++;
        key += "_" + waitCount.ToString();
        key += "_" + PhotonNetwork.CurrentRoom.Name;
        //key += "_" + "PhotonWaitController";
        key += "_" + UnityEngine.Random.Range(0, 999).ToString();

        keys.Add(key);

        if (!GManager.instance.IsAI)
        {
            if (!RoomHasTrueKey(key))
            {
                SetWaiting(key, false, true);

                //Debug.Log($"待機開始:{key}");
                return StartCoroutine(Wait(key));
            }
        }
        
        return null;
    }

    public List<string> keys = new List<string>();

    public void ResetKeys()
    {
        Hashtable PlayerProp = PhotonNetwork.LocalPlayer.CustomProperties;

        foreach(string key in keys)
        {
            PlayerProp.Remove(key);
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(PlayerProp);
    }
}
