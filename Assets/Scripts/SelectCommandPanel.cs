using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SelectCommandPanel : MonoBehaviour
{
    [Header("コマンド選択ボタンプレハブ")]
    public SelectCommand selectCommandPrefab;



    #region コマンド選択パネルを開く
    public Coroutine SetUpCommandButton(List<Command_SelectCommand> commands)
    {
        if (this.gameObject.activeSelf)
        {
            return null;
        }

        Vector2 Position = new Vector2(0, -480);

        this.transform.parent.gameObject.SetActive(true);
        this.gameObject.SetActive(true);

        GetComponent<Animator>().SetInteger("Close", 0);

        return StartCoroutine(SetUpCommandButtonCoroutine(commands, Position));
    }

    IEnumerator SetUpCommandButtonCoroutine(List<Command_SelectCommand> commands, Vector2 Position)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        yield return new WaitWhile(() => transform.childCount > 0);

        if (commands != null)
        {
            foreach (Command_SelectCommand command in commands)
            {
                SelectCommand _selectCommandButton = Instantiate(selectCommandPrefab, transform);

                _selectCommandButton.OpenSelectCommandButton(command.CommandName, command.Command, command.SpriteIndex);

                _selectCommandButton.OnClickEvent.AddListener(() => { for (int i = 0; i < this.transform.childCount; i++) { this.transform.GetChild(i).gameObject.SetActive(false); } });

                _selectCommandButton.OnClickEvent.AddListener(CloseSelectCommandPanel);

                //transform.localPosition = Position;
            }

            if (commands.Count == 0)
            {
                Off();
            }
        }

        else
        {
            Off();
        }
    }
    #endregion

    #region コマンド選択パネルを閉じる
    public void CloseSelectCommandPanel()
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

public class Command_SelectCommand
{
    public string CommandName { get; set; }
    public UnityAction Command { get; set; }

    public int SpriteIndex { get; set; }

    public Command_SelectCommand(string _CommandName, UnityAction _Command, int _SpriteIndex)
    {
        CommandName = _CommandName;
        Command = _Command;
        SpriteIndex = _SpriteIndex;
    }
}