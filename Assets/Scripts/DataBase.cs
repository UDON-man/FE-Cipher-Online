using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class DataBase : MonoBehaviour
{
    public static DataBase instance = null;

    private void Awake()
    {
        instance = this;
    }

    public static Dictionary<CardColor, string> CardColorNameDictionary = new Dictionary<CardColor, string>()
    {
        { CardColor.Red,"赤"},
        { CardColor.Blue,"青"},
        { CardColor.Green,"緑"},
        { CardColor.Yellow,"黄"},
        { CardColor.Purple,"紫"},
        { CardColor.White,"白"},
        { CardColor.Black,"黒"},
        { CardColor.Brown,"茶"},
        { CardColor.Colorless,"無色"},
    };

    public static Dictionary<Weapon, string> WeaponNameDictionary = new Dictionary<Weapon, string>()
    {
        { Weapon.Sword,"剣" },
        { Weapon.Lance,"槍" },
        { Weapon.Axe,"斧" },
        { Weapon.MagicBook,"魔導書" },
        { Weapon.Rod,"杖" },
        { Weapon.DragonStone,"竜石" },
        { Weapon.Dragon,"竜" },
        { Weapon.DarkWeapon,"暗器" },
        { Weapon.Horse,"馬" },
        { Weapon.Bow,"弓" },
        { Weapon.Armor,"鎧" },
        { Weapon.Wing,"羽" },
        { Weapon.Beast,"獣" },
        { Weapon.Sharp,"シャープ" },
        { Weapon.Demon,"魔物" },
    };

    public static Dictionary<CardColor, Color> CardColor_ColorLightDictionary = new Dictionary<CardColor, Color>()
    {

        { CardColor.Red,new Color32(253,63,49,255)},
        { CardColor.Blue, new Color32(49,118,253,255)},
        { CardColor.Green, new Color32(55,255,49,255)},
        { CardColor.Yellow, new Color32(255,231,64,255)},
        { CardColor.Purple,new Color32(255,32,192,255)},
        { CardColor.White,new Color32(255,184,202,255)},
        { CardColor.Black,new Color32(79,69,106,255)},
        { CardColor.Brown,new Color32(226,103,44,255)},
        { CardColor.Colorless,new Color32(218,255,253,255)},
    };

    public static Dictionary<CardColor, Color> CardColor_ColorDarkDictionary = new Dictionary<CardColor, Color>()
    {

        { CardColor.Red,new Color32(195,44,33,255)},
        { CardColor.Blue, new Color32(1,71,207,255)},
        { CardColor.Green, new Color32(6,203,0,255)},
        { CardColor.Yellow, new Color32(227,198,0,255)},
        { CardColor.Purple,new Color32(191,0,137,255)},
        { CardColor.White,new Color32(255,153,179,255)},
        { CardColor.Black,new Color32(45,26,97,255)},
        { CardColor.Brown,new Color32(164,54,0,255)},
        { CardColor.Colorless,new Color32(125,243,247,255)},
    };

    public static Color32 BondColor = new Color32(231, 255, 42, 255);

    public ColorSpriteDic CardColorIconDictionary;

    public static Color SelectColor_Orange 
    { 
        get
        {
            return new Color32(255, 98, 31, 255);
        }
    }

    public static Color SelectColor_Blue
    {
        get
        {
            return new Color32(30, 246, 255, 255);
        }
    }
}

[System.Serializable]
public class ColorSpriteDic : TableBase<CardColor, Sprite, SamplePair>
{

}

[System.Serializable]
public class TableBase<TKey, TValue, Type> where Type : KeyAndValue<TKey, TValue>
{
    [SerializeField]
    private List<Type> list;
    private Dictionary<TKey, TValue> table;


    public Dictionary<TKey, TValue> GetTable()
    {
        if (table == null)
        {
            table = ConvertListToDictionary(list);
        }
        return table;
    }

    /// <summary>
    /// Editor Only
    /// </summary>
    public List<Type> GetList()
    {
        return list;
    }

    static Dictionary<TKey, TValue> ConvertListToDictionary(List<Type> list)
    {
        Dictionary<TKey, TValue> dic = new Dictionary<TKey, TValue>();
        foreach (KeyAndValue<TKey, TValue> pair in list)
        {
            dic.Add(pair.Key, pair.Value);
        }
        return dic;
    }
}

/// <summary>
/// シリアル化できる、KeyValuePair
/// </summary>
[System.Serializable]
public class KeyAndValue<TKey, TValue>
{
    public TKey Key;
    public TValue Value;

    public KeyAndValue(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }
    public KeyAndValue(KeyValuePair<TKey, TValue> pair)
    {
        Key = pair.Key;
        Value = pair.Value;
    }
}

[System.Serializable]
public class SamplePair : KeyAndValue<CardColor, Sprite>
{
    public SamplePair(CardColor key, Sprite value) : base(key, value)
    {

    }
}

public class DictionaryUtility
{
    public static CardColor GetColor(string ColorName, Dictionary<CardColor, string> ColorNameDictionary)
    {
        CardColor color = ColorNameDictionary.First(x => x.Value == ColorName).Key;

        return color;
    }

    public static Weapon GetWeapon(string ColorName, Dictionary<Weapon, string> WeaponNameDictionary)
    {
        Weapon weapon = WeaponNameDictionary.First(x => x.Value == ColorName).Key;

        return weapon;
    }
}