using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeWindow : MonoBehaviour
{
    public void Open()
    {
#if !UNITY_EDITOR && UNITY_STANDALONE
        this.gameObject.SetActive(true);
#endif

    }

    public void Close()
    {
        this.gameObject.SetActive(false);

    }
    public void SetUp_0()
    {
        Screen.SetResolution(1024, 576,false);
        Close();
    }

    public void SetUp_1()
    {
        Screen.SetResolution(1280,720, false);
        Close();

        Debug.Log("1280*720");
    }

    public void SetUp_2()
    {
        Screen.SetResolution(1600,900, false);
        Close();

        Debug.Log("1600*900");
    }

    public void SetUp_3()
    {
        Screen.SetResolution(1920,1080, false);
        Close();

        Debug.Log("19200*1080");
    }
}
