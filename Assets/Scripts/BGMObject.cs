using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BGMObject : MonoBehaviour
{
    public AudioSource _audio { get; set; }

    private void Start()
    {

    }

    public void StopPlayBGM()
    {
        _audio = GetComponent<AudioSource>();

        _audio.Stop();

        _audio.clip = null;
    }

    public void StartPlayBGM(AudioClip clip)
    {
        _audio = GetComponent<AudioSource>();

        if (clip != null)
        {
            _audio.clip = clip;
        }

        if (ContinuousController.instance != null)
        {
            ContinuousController.instance.ChangeBGMVolume(_audio);
        }

        _audio.Play();
    }

    private void Update()
    {
        if (ContinuousController.instance != null)
        {
            if (_audio != null)
            {
                ContinuousController.instance.ChangeBGMVolume(_audio);
            }
        }
    }
}
