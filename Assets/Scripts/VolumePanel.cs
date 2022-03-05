using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumePanel : MonoBehaviour
{
    [Header("SEスライダー")]
    public Slider SESlier;

    [Header("BGMスライダー")]
    public Slider BGMSlier;

    public GameObject VolumePanelObject;

    public void OpenVolumePanel()
    {
        VolumePanelObject.SetActive(true);
    }

    public void CloseVolumePanel()
    {
        VolumePanelObject.SetActive(false);
    }

    public void Init()
    {
        SESlier.onValueChanged.RemoveAllListeners();
        BGMSlier.onValueChanged.RemoveAllListeners();

        SESlier.value = ContinuousController.instance.SEVolume;
        BGMSlier.value = ContinuousController.instance.BGMVolume;

        SESlier.onValueChanged.AddListener(SetSEVlolume);
        BGMSlier.onValueChanged.AddListener(SetBGMVlolume);

        CloseVolumePanel();
    }

    public void SetSEVlolume(float value)
    {
        ContinuousController.instance.SetSEVolume(value);
    }

    public void SetBGMVlolume(float value)
    {
        ContinuousController.instance.SetBGMVolume(value);
    }
}
