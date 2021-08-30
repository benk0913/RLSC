using System;
using System.Collections;
using System.Collections.Generic;
using EdgeworldBase;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class VideoWindowUI : MonoBehaviour
{
    public static VideoWindowUI Instance;

    Action AcceptAction;

    Action SkipAction;

    [SerializeField]
    GameObject HideButton;

    [SerializeField]
    VideoPlayer MoviePlayer;

    public bool CantHide = false;

    float previousMusicVolume = -1f;

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Hide(bool accepted = false)
    {
        MoviePlayer.Stop();

        if(previousMusicVolume != -1f)
            AudioControl.Instance.SetVolume("Music",previousMusicVolume);

        if(!accepted)
        {
            SkipAction?.Invoke();
        }
        else
        {
            AcceptAction?.Invoke();
        }
        
        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!CantHide && Input.GetKeyDown(InputMap.Map["Exit"]))
        {
            Hide();
        }
        else if (Input.GetKeyDown(InputMap.Map["Confirm"]))
        {
            Accept();
        }
    }

    public void Show(VideoClip clip, Action acceptCallback, bool cantHide = false, Action skipCallback = null)
    {
        CantHide = cantHide;

        HideButton.gameObject.SetActive(!CantHide);

        this.gameObject.SetActive(true);

        MoviePlayer.clip = clip;
        MoviePlayer.Play();
        AcceptAction = acceptCallback;
        SkipAction = skipCallback;
        previousMusicVolume = AudioControl.Instance.GetVolumeByTag("Music");
        AudioControl.Instance.SetVolume("Music",0f);
    }

    public void Accept()
    {
        Hide(true);
    }
}
