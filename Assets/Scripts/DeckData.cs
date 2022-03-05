using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class DeckData
{
    public static int m = 256;
    public static int n = 256;
    public static int log_n_m = (int)Mathf.Log(m,n);
    public static int maxCardKind = 2200;

    public static int CardKindCellLength = (int)Math.Ceiling(Mathf.Log(maxCardKind, m));

    #region  コンストラクタ("デッキ名","キーカードID(256進数)","重複なしカードIDリスト(256進数)","カード枚数リスト(256進数)")
    public DeckData(string DeckCode)
    {
        List<int> _DeckCardIDs = new List<int>();

        //コンマで区切り
        string[] parseByComma = DeckCode.Split(',');

        List<int> DistinctDeckCardIDs = new List<int>();
        List<int> DistinctDeckCardCounts = new List<int>();

        for (int i = 0; i < parseByComma.Length; i++)
        {
            //デッキ名
            if (i == 0)
            {
                DeckName = parseByComma[i];
            }

            //キーカードID
            else if (i == 1)
            {
                if (!string.IsNullOrEmpty(parseByComma[i]))
                {
                    MainCharacterID = ConvertBinaryNumber.NStringToInt(parseByComma[i], m);
                }
            }

            //デッキカード(重複なし)
            else if (i == 2)
            {
                /*
                //1文字ずつ区切り
                string[] SplitText = SplitClass.Split(parseByComma[i], 1);

                for (int j = 0; j < SplitText.Length; j++)
                {
                    DistinctDeckCardIDs.Add(ConvertBinaryNumber.NStringToInt(SplitText[j], m));
                    //Debug.Log($"カード取得:{ContinuousController.instance.CardList[DistinctDeckCardIDs[j]].CardName},256進数:{SplitText[j]}");
                }
                */

                //2文字ずつ区切り
                string[] SplitText = SplitClass.Split(parseByComma[i], CardKindCellLength);

                for (int j = 0; j < SplitText.Length; j++)
                {
                    DistinctDeckCardIDs.Add(ConvertBinaryNumber.NStringToInt(SplitText[j], m));
                    //Debug.Log($"カード取得:{ContinuousController.instance.CardList[DistinctDeckCardIDs[j]].CardName},256進数:{SplitText[j]}");
                }
            }

            //枚数
            else if (i == 3)
            {
                //m進数の文字列
                string x_m = parseByComma[i];

                if (!string.IsNullOrEmpty(x_m))
                {
                    //m進数の文字列をn進数に変換
                    string x_n = ConvertBinaryNumber.NKStringToNString(x_m, n, log_n_m);

                    //n進数の文字列を1文字ずつ区切る
                    string[] Split_x_n = SplitClass.Split(x_n, 1);

                    //1足してリストに枚数を格納
                    for (int j = 0; j < Split_x_n.Length; j++)
                    {
                        //n進数→10進数に変換
                        DistinctDeckCardCounts.Add(ConvertBinaryNumber.NStringToInt(Split_x_n[j], n) + 1);
                    }
                }

            }
        }

        for (int i = 0; i < DistinctDeckCardIDs.Count; i++)
        {
            if(i < DistinctDeckCardCounts.Count)
            {
                for (int j = 0; j < DistinctDeckCardCounts[i]; j++)
                {
                    _DeckCardIDs.Add(DistinctDeckCardIDs[i]);
                }
            }
        }

        DeckCardIDs = _DeckCardIDs;
    }
    #endregion

    #region デッキ名
    [SerializeField] string deckName;
    public string DeckName
    {
        get
        {
            if (string.IsNullOrEmpty(deckName))
            {
                return "NewDeck";
            }

            return deckName;
        }

        set
        {
            deckName = value;
        }
    }
    #endregion

    #region デッキに含まれるカード
    public List<CEntity_Base> DeckCards()
    {
        List<CEntity_Base> deckCards = new List<CEntity_Base>();

        foreach (int DeckCardID in DeckCardIDs)
        {
            foreach (CEntity_Base Card in ContinuousController.instance.CardList)
            {
                if (Card.CardID == DeckCardID)
                {
                    deckCards.Add(Card);
                    break;
                }
            }
        }

        return deckCards;
    }
    #endregion

    #region 主人公
    [SerializeField] public int MainCharacterID { get; set; } = -1;

    public void ResetMainCharacter()
    {
        MainCharacterID = -1;
    }

    public CEntity_Base MainCharacter
    {
        get
        {
            foreach (CEntity_Base cEntity_Base in ContinuousController.instance.CardList)
            {
                if (cEntity_Base.CardID == MainCharacterID)
                {
                    return cEntity_Base;
                }
            }

            return null;
        }
    }
    #endregion

    #region デッキに含まれるカードのカードIDリスト
    public List<int> DeckCardIDs { get; set; } = new List<int>();

    #region デッキにカードを追加
    public void AddDeckCard(CEntity_Base cEntity_Base)
    {
        List<CEntity_Base> _DeckCards = DeckCards();

        _DeckCards.Add(cEntity_Base);

        _DeckCards = SortedList(_DeckCards);

        DeckCardIDs = GetDeckCardCodes(_DeckCards);
    }
    #endregion

    #region デッキからカードを抜く
    public void RemoveDeckCard(CEntity_Base cEntity_Base)
    {
        List<CEntity_Base> _DeckCards = DeckCards();

        _DeckCards.Remove(cEntity_Base);

        _DeckCards = SortedList(_DeckCards);

        DeckCardIDs = GetDeckCardCodes(_DeckCards);
    }
    #endregion

    #region カードのリストからカードIDのリストを取得
    public static List<int> GetDeckCardCodes(List<CEntity_Base> DeckCards)
    {
        List<int> _DeckCardCodes = new List<int>();

        foreach (CEntity_Base DeckCard in DeckCards)
        {
            _DeckCardCodes.Add(DeckCard.CardID);
        }

        return _DeckCardCodes;
    }
    #endregion

    #endregion

    #region カードリストをソート
    public static List<CEntity_Base> SortedList(List<CEntity_Base> DeckCards)
    {
        DeckCards = DeckCards
                 .OrderBy(value => Array.IndexOf(new[] { CardColor.Red, CardColor.Blue, CardColor.Green, CardColor.Yellow, CardColor.Purple, CardColor.White, CardColor.Black, CardColor.Brown, CardColor.Colorless }, value.cardColors[0]))
                 .ThenBy(value => value.CardID)
                 .ToList();

        return DeckCards;
    }
    #endregion

    #region 256進数のデッキコードを取得
    #region デッキ名とカードリストから256進数のデッキコードを取得
    public static string GetDeckCode(string _DeckName, CEntity_Base _KeyCard, List<CEntity_Base> _DeckCards)
    {
        string _DeckDataString = null;

        //デッキ名
        _DeckDataString += _DeckName + ",";

        //キーカードID
        if (_KeyCard != null)
        {
            _DeckDataString += ConvertBinaryNumber.IntToNString(_KeyCard.CardID, m) + ",";
        }

        else
        {
            _DeckDataString += ConvertBinaryNumber.IntToNString(-1, m) + ",";
        }

        //カードリストを重複なしのリストにする
        List<CEntity_Base> DistinctDeckCards = _DeckCards.Distinct().ToList();

        //重複なしカードリストを登録
        foreach (CEntity_Base cardData in DistinctDeckCards)
        {
            _DeckDataString += cardData.CardID_String;
        }

        _DeckDataString += ",";

        //重複なしカードリストの各々のデッキ内枚数を格納(1減らす)
        List<int> _DistinctDeckCardCounts = new List<int>();

        foreach (CEntity_Base cardData in DistinctDeckCards)
        {
            _DistinctDeckCardCounts.Add(_DeckCards.Count((_CardData) => _CardData == cardData) - 1);
        }

        //デッキ内枚数をn進数の文字列に変換
        string x_n = null;

        for (int i = 0; i < _DistinctDeckCardCounts.Count; i++)
        {
            x_n += ConvertBinaryNumber.IntToNString(_DistinctDeckCardCounts[i], n);
        }

        //桁数が log(n)m の倍数になるように末尾を0で埋める
        if (x_n != null)
        {
            while (x_n.Count() % log_n_m != 0)
            {
                x_n += "0";
            }
        }

        //n進数の文字列をm進数の文字列に変換
        if (x_n != null)
        {
            string x_m = ConvertBinaryNumber.NStringToNKString(x_n, n, log_n_m);
            _DeckDataString += x_m;
        }

        return _DeckDataString;
    }
    #endregion

    #region 自分のDeckDataの256進数のデッキコードを取得
    public string GetThisDeckCode()
    {
        return DeckData.GetDeckCode(DeckName, MainCharacter, DeckCards());
    }
    #endregion
    #endregion

    #region その文字列がデッキコードして正しいか
    public static bool IsValidDeckCode(string DeckCode)
    {
        if (!DeckCode.Contains(","))
        {
            Debug.Log(",が含まれていません");
            return false;
        }

        string[] parseByComma = DeckCode.Split(',');

        if (parseByComma.Length != 4 && parseByComma.Length != 5)
        {
            Debug.Log(",の個数がおかしいです");
            return false;
        }

        if (!string.IsNullOrEmpty(parseByComma[1]))
        {
            if (ConvertBinaryNumber.NStringToInt(parseByComma[1], m) == 114514)
            {
                Debug.Log("[1]:114514");
                return false;
            }
        }

        if (!string.IsNullOrEmpty(parseByComma[2]))
        {
            string[] SplitText = SplitClass.Split(parseByComma[2], 1);

            for (int j = 0; j < SplitText.Length; j++)
            {
                if (ConvertBinaryNumber.NStringToInt(SplitText[j], m) == 114514)
                {
                    Debug.Log("[2]:114514");
                    return false;
                }
            }
        }

        else
        {
            return false;
        }

        if (!string.IsNullOrEmpty(parseByComma[3]))
        {
            //256進数の文字列
            string x_m = parseByComma[3];

            //256進数の文字列をn進数に変換
            string x_n = ConvertBinaryNumber.NKStringToNString(x_m, n, log_n_m);

            if (x_n == "114514")
            {
                Debug.Log("[3]:114514");
                return false;
            }
        }

        else
        {
            return false;
        }

        return true;
    }
    #endregion

    #region このデッキデータがバトルに使えるか
    public bool IsValidDeckData()
    {
        //デッキ枚数は50枚以上
        if (DeckCards().Count < 50)
        {
            return false;
        }

        //デッキ枚数は流石に1000枚以下
        if (DeckCards().Count > 1000)
        {
            return false;
        }

        //カードリストを重複なしのリストにする
        List<CEntity_Base> DistinctDeckCards = DeckCards().Distinct().ToList();

        //同名カードは4枚まで
        foreach (CEntity_Base cEntity_Base in DistinctDeckCards)
        {
            int SameCount = DeckCards().Count((cardData) => cardData.CardName == cEntity_Base.CardName);

            if (SameCount < 1 || cEntity_Base.MaxCountInDeck < SameCount)
            {
                return false;
            }
        }

        if (MainCharacter == null)
        {
            return false;
        }

        if (MainCharacter.PlayCost != 1)
        {
            return false;
        }

        if (!DeckCards().Contains(MainCharacter))
        {
            return false;
        }

        return true;
    }
    #endregion

    #region 空DeckData
    public static DeckData EmptyDeckData()
    {
        return new DeckData("");
    }
    #endregion
}

#region 文字列を分割
public static class SplitClass
{
    public static string[] Split(this string str, int count)
    {
        var list = new List<string>();
        int length = (int)Math.Ceiling((double)str.Length / count);

        for (int i = 0; i < length; i++)
        {
            int start = count * i;
            if (str.Length <= start)
            {
                break;
            }
            if (str.Length < start + count)
            {
                list.Add(str.Substring(start));
            }
            else
            {
                list.Add(str.Substring(start, count));
            }
        }

        return list.ToArray();
    }
}
#endregion