using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
public static class PlayerPrefsUtil
{
    public static bool GetBool(string key, bool defaultValue)
    {
        if (!PlayerPrefs.HasKey(key))
        {
            return defaultValue;
        }
        return GetBool(key);
    }

    public static bool GetBool(string key)
    {
        return PlayerPrefs.GetInt(key) != 0;
    }

    public static void SetBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
    }

    public static T GetObject<T>(string key, T defaultValue)
    {
        if (!PlayerPrefs.HasKey(key))
        {
            return defaultValue;
        }
        return GetObject<T>(key);
    }

    public static T GetObject<T>(string key)
    {
        string json = PlayerPrefs.GetString(key, "{}");
        return JsonUtility.FromJson<T>(json);
    }

    public static void SetObject<T>(string key, T value)
    {
        string json = JsonUtility.ToJson(value);

        PlayerPrefs.SetString(key, json);
    }

    /// <summary>
    /// リストを保存
    /// </summary>
    public static void SaveList<T>(string key, List<T> value)
    {
        string serizlizedList = Serialize<List<T>>(value);
        PlayerPrefs.SetString(key, serizlizedList);
    }

    /// <summary>
    /// リストを読み込み
    /// </summary>
    public static List<T> LoadList<T>(string key)
    {
        //keyがある時だけ読み込む
        if (PlayerPrefs.HasKey(key))
        {
            string serizlizedList = PlayerPrefs.GetString(key);
            return Deserialize<List<T>>(serizlizedList);
        }

        return new List<T>();
    }

    //=================================================================================
    //シリアライズ、デシリアライズ
    //=================================================================================

    private static string Serialize<T>(T obj)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        MemoryStream memoryStream = new MemoryStream();
        binaryFormatter.Serialize(memoryStream, obj);
        return Convert.ToBase64String(memoryStream.GetBuffer());
    }

    private static T Deserialize<T>(string str)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(str));
        return (T)binaryFormatter.Deserialize(memoryStream);
    }

}