using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SideButtonUI : MonoBehaviour, WindowInterface
{
    public static SideButtonUI Instance;

    [SerializeField]
    Animator Animer;


    [SerializeField]
    Button ReportBugButton;
    
    [SerializeField]
    Button AbordExpeditionButton;

    public bool isVisible;

    void Awake()
    {
        Instance = this;
    }

    [Obsolete("Do not call Hide directly. Call `CORE.Instance.CloseCurrentWindow()` instead.")]
    public void Hide()
    {
        Animer.SetTrigger("Hide");
        isVisible = false;
    }

    [Obsolete("Do not call Show directly. Call `CORE.Instance.ShowWindow()` instead.")]
    public void Show(ActorData actorData, object data = null)
    {
        Animer.SetTrigger("Show");
        isVisible = true;

        AbordExpeditionButton.gameObject.SetActive(CORE.Instance.Data.content.Expeditions.Find(X=>{ return X.ContainsScene(CORE.Instance.ActiveSceneInfo.sceneName);}) != null);
    }

    public void Toggle()
    {
        if(isVisible)
        {
            CORE.Instance.CloseCurrentWindow();
        }
        else
        {
            CORE.Instance.ShowSideButtonUiWindow();
        }
    }

    public void LockReportABug()
    {
        ReportBugButton.interactable = false;
    }

    public void SendReportABug()
    {
        CORE.Instance.ReportBug(()=>
        {
            LockReportABug(); 
            TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Thank you for the report!",Color.green,2f));
        });
    }

    public void AbortExped()
    {
        WarningWindowUI.Instance.Show(CORE.QuickTranslate("Are you sure")+"?",()=>SocketHandler.Instance.SendEvent("suicide"));
    }
}
