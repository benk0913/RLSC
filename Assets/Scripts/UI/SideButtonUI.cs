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

    public bool isVisible;

    void Awake()
    {
        Instance = this;
    }

    public void Hide()
    {
        Animer.SetTrigger("Hide");
        isVisible = false;
    }

    public void Show(ActorData actorData, object data = null)
    {


        Animer.SetTrigger("Show");
        isVisible = true;


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
}
