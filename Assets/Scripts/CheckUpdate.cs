using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class CheckUpdate : MonoBehaviour
{
    public YesNoObject yesNoObject;
    public GameObject UpdateButton;

    private void Awake()
    {
        yesNoObject.Close();
        UpdateButton.SetActive(false);
    }

    public IEnumerator CheckUpdateCoroutine()
    {
        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            yield break;
        }

        bool end = false;
        GetComponent<GSSReader>().OnLoadEnd.RemoveAllListeners();
        GetComponent<GSSReader>().OnLoadEnd.AddListener(() => { EndLoad();end = true; }) ;
        GetComponent<GSSReader>().Reload();

        yield return new WaitWhile(() => !end);
        end = false;
    }

    public void EndLoad()
    {
        GetComponent<OpenURL>().URL = GetComponent<GSSReader>().Datas[0][1];

#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
        GetComponent<OpenURL>().URL = GetComponent<GSSReader>().Datas[0][2];
#endif

        ContinuousController.instance.NeedUpdate = GetComponent<GSSReader>().Datas[0][0] != ContinuousController.instance.GameVer.ToString();

        if (ContinuousController.instance.NeedUpdate)
        {
            yesNoObject.SetUpYesNoObject(new List<UnityAction>(){() => GetComponent<OpenURL>().Open(),null},new List<string>() { "DL the\nlatest version", "Do not DL"},Message(),true);
        }

        UpdateButton.SetActive(ContinuousController.instance.NeedUpdate);

        string Message() { return "There is the latest version of this game.\n Do you want to download the latest version?\n(Updates will add new cards and fix bugs.\nAlso, if the Ver is different, matching will not be possible)"; };
    }
}
