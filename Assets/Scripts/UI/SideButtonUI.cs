using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideButtonUI : MonoBehaviour, WindowInterface
{
    public static SideButtonUI Instance;

    [SerializeField]
    Animator Animer;

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

    public void Show(ActorData actorData)
    {
        if(CameraChaseEntity.Instance.IsFocusing)
        {
            return;
        }

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
}
