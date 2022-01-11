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

    [SerializeField]
    GameObject LoadingWindow;

    [SerializeField]
    SelectionGroupUI SG;

    public bool CantHide = false;

    float previousMusicVolume = -1f;

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    void OnDisable()
    {
        AudioControl.Instance.SetVolume("Music",0.6f,false);
    }
    public void Hide(bool accepted = false)
    {
        MoviePlayer.Stop();


        if(!accepted)
        {
            SkipAction?.Invoke();
        }
        else
        {
            AcceptAction?.Invoke();
        }
        AudioControl.Instance.SetVolume("Music",0.6f,false);
        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!CantHide && Input.GetKeyDown(InputMap.Map["Exit"]) || Input.GetButtonDown("Joystick 8")||  Input.GetButtonDown("Joystick 11"))
        {
            Hide();
        }
        else if (Input.GetKeyDown(InputMap.Map["Confirm"]) || Input.GetButtonDown("Joystick 0"))
        {
            Accept();
        }
    }

    public void Show(VideoClip clip, Action acceptCallback, bool cantHide = false, Action skipCallback = null)
    {
        this.gameObject.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(ShowRoutine(clip, acceptCallback, cantHide, skipCallback));
    }

    IEnumerator ShowRoutine(VideoClip clip, Action acceptCallback, bool cantHide, Action skipCallback)
    {
        MoviePlayer.clip = clip;
        MoviePlayer.Prepare();
        float timeout = 5f;

        LoadingWindow.SetActive(true);

        previousMusicVolume = AudioControl.Instance.GetVolumeByTag("Music");

        while (!MoviePlayer.isPrepared)
        {
            timeout -= Time.deltaTime;
            if(timeout <= 0)
            {
                break;
            }
            yield return 0;
        }

        LoadingWindow.SetActive(false);

        CantHide = cantHide;

        HideButton.gameObject.SetActive(!CantHide);

        this.gameObject.SetActive(true);

        MoviePlayer.Play();
        AcceptAction = acceptCallback;
        SkipAction = skipCallback;
        
        AudioControl.Instance.SetVolume("Music", 0f, false);

        CORE.Instance.DelayedInvokation(0.1f, () => { AudioControl.Instance.SetVolume("Music",0.6f,false);SG.RefreshGroup(false); });

        if(!MoviePlayer.isPrepared)
        {
            Hide();
        }
    }

    public void Accept()
    {
        Hide(true);
    }
}
