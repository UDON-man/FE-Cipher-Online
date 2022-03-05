using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandText : MonoBehaviour
{
    public Text commandText;

    public void Init()
    {
        Off();
    }

    #region コマンドメッセージを開く
    public void OpenCommandText(string Text)
    {
        commandText.text = Text;
        this.gameObject.SetActive(true);

        GetComponent<Animator>().SetInteger("Close", 0);
    }
    #endregion

    #region コマンドメッセージを閉じる
    public void CloseCommandText()
    {
        if (!this.gameObject.activeSelf)
        {
            return;
        }

        GetComponent<Animator>().SetInteger("Close", 1);
    }

    public void Off()
    {
        this.gameObject.SetActive(false);
    }
    #endregion
}