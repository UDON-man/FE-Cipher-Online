using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingObject : MonoBehaviour
{
    public Animator anim;

    public Text LoadingText;


    public IEnumerator StartLoading(string DefaultString)
    {
        this.transform.parent.gameObject.SetActive(true);
        this.gameObject.SetActive(true);
        anim.SetInteger("Close", 0);
        LoadingText.gameObject.SetActive(true);

        yield return new WaitWhile(() => !this.gameObject.activeSelf || !this.transform.parent.gameObject.activeSelf);

        StartCoroutine(SetLoadingText(DefaultString));
    }

    IEnumerator SetLoadingText(string DefaultString)
    {
        float waitTime = 0.18f;

        int count = 0;

        while(true)
        {
            count++;

            if(count >= 4)
            {
                count = 0;
            }

            LoadingText.text = DefaultString;

            for(int i=0;i<count;i++)
            {
                LoadingText.text += ".";
            }

            yield return new WaitForSeconds(waitTime);
        }
    }


    public IEnumerator EndLoading()
    {
        anim.SetInteger("Close", 1);

        LoadingText.gameObject.SetActive(false);

        yield return new WaitWhile(() => this.gameObject.activeSelf);
    }

    public void Off()
    {
        this.gameObject.SetActive(false);
    }
}
