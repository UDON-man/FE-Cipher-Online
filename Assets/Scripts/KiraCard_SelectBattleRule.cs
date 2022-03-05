using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KiraCard_SelectBattleRule : SelectKiraCard
{
    //[Header("バトルルール")]
    //public BattleRule battleRule;

    [Header("バトルデッキ選択")]
    public SelectBattleDeck selectBattleDeck;

    #region クリック時の処理
    public override void OnClick()
    {
        base.OnClick();

        if(!ContinuousController.instance.isAI)
        {
            selectBattleDeck.SetUpSelectBattleDeck(selectBattleDeck.OnClickSelectButton_RandomMatch);
        }

        else
        {
            selectBattleDeck.SetUpSelectBattleDeck(() => 
            { 
                selectBattleDeck.OnClickSelectButton_BotMatch();
                ContinuousController.instance.StartCoroutine(StartBattleCoroutine()); 
            });
        }

        IEnumerator StartBattleCoroutine()
        {
            yield return ContinuousController.instance.StartCoroutine(Opening.instance.LoadingObject.StartLoading("Now Loading"));
            Opening.instance.MainCamera.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            SceneManager.LoadSceneAsync("BattleScene", LoadSceneMode.Additive);
        }
       
    }
    #endregion
}
