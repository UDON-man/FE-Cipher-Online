using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Text;
public class SearchCardList : MonoBehaviour
{
    [Header("検索インプットフィールド")]
    public InputField SearchInputField;

    [Header("デッキ編集")]
    public EditDeck editDeck;

    public UnityAction OnEndEditAction;

    public void OnEndEdit(string _text)
    {
        OnEndEditAction?.Invoke();
    }

    public void Init()
    {
        OnEndEditAction = null;
        SearchInputField.text = null;

        OnEndEditAction = editDeck.ShowPoolCard_MatchCondition;
    }

    public Func<CEntity_Base, bool> OnlyContainsName()
    {
        string text = SearchInputField.text;

        bool _OnlyContainsName(CEntity_Base cEntity_Base)
        {
            if (!string.IsNullOrEmpty(text))
            {
                if (Convert(cEntity_Base.CardName).Contains(Convert(text)))
                {
                    return true;
                }

                if (Convert(cEntity_Base.UnitName_English).Contains(Convert(text)))
                {
                    return true;
                }

                if (Convert(cEntity_Base.CardImage.name).Contains(Convert(text)))
                {
                    return true;
                }

                if (Convert(cEntity_Base.CardImage_English.name).Contains(Convert(text)))
                {
                    return true;
                }

                foreach (string skillName in cEntity_Base.SkillNames)
                {
                    if (Convert(skillName).Contains(Convert(text)))
                    {
                        return true;
                    }
                }

                foreach (string supportSkillName in cEntity_Base.SupportSkillNames)
                {
                    if (Convert(supportSkillName).Contains(Convert(text)))
                    {
                        return true;
                    }
                }
            }

            else
            {
                return true;
            }


            return false;
        }

        return _OnlyContainsName;
    }

    static internal string Convert(string s)
    {
        StringBuilder sb = new StringBuilder();
        char[] target = s.ToCharArray();
        char c;
        for (int i = 0; i < target.Length; i++)
        {
            c = target[i];
            if (c >= 'ぁ' && c <= 'ゔ')
            { //-> ひらがなの範囲
                c = (char)(c + 0x0060);  //-> 変換
            }
            sb.Append(c);
        }

        string text = sb.ToString();

        while (text.Contains(" ") || text.Contains("　") || text.Contains("\n"))
        {
            text = text.Replace(" ", "");
            text = text.Replace("　", "");
            text = text.Replace("\n", "");
        }

        return text.ToLower();
    }
}
