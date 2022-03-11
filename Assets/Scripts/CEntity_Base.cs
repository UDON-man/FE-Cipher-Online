using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create/CEntity_Base")]
public class CEntity_Base : ScriptableObject
{
    public Sprite CardImage;
    public Sprite CardImage_English;
    public string CardName;
    public string UnitName;
    public string UnitName_English;
    public int PlayCost;
    public bool HasCC;
    public int CCCost;
    public List<CardColor> cardColors = new List<CardColor>();
    public int Power;
    public int SupportPower;
    public List<int> Ranges = new List<int>();
    public Sex sex;
    public List<Weapon> Weapons = new List<Weapon>();
    public string ClassName;
    public List<string> SkillNames = new List<string>();
    public List<string> SupportSkillNames = new List<string>();
    public int CardID;
    public int MaxCountInDeck = 4;

    public string CardID_String
    {
        get
        {
            string CardID_String = ConvertBinaryNumber.IntToNString(CardID, DeckData.m);

            while(CardID_String.Length < DeckData.CardKindCellLength)
            {
                CardID_String = $"0{CardID_String}";
            }

            return CardID_String;
        }
    }
}

public enum CardColor
{
    Red,
    Blue,
    Green,
    Yellow,
    Purple,
    White,
    Black,
    Brown,
    Colorless,
    None,
}

public enum Sex
{
    male,
    female,
}

public enum Weapon
{
    Sword,
    Lance,
    Axe,
    MagicBook,
    Rod,
    DragonStone,
    Dragon,
    DarkWeapon,
    Horse,
    Bow,
    Armor,
    Wing,
    Beast,
    Sharp,
    Demon,
}