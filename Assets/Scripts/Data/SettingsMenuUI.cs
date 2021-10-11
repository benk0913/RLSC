using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenuUI : MonoBehaviour, WindowInterface
{
    public static SettingsMenuUI Instance;

    [SerializeField]
    Canvas Canv;

    public GameObject KeyboardBindings;
    public GameObject ControllerBindings;


    void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void Show(ActorData actorData, object data)
    {
        this.gameObject.SetActive(true);

        //TODO Yes, it's stupid, I know, remember to next time build your own settings menu.
        CORE.Instance.DelayedInvokation(0.1f,()=>{Canv.enabled = true;});

        KeyboardBindings.SetActive(!CORE.Instance.IsUsingJoystick);
        ControllerBindings.SetActive(CORE.Instance.IsUsingJoystick);
    }

    public void StartMachinemaMode()
    {
        CORE.IsMachinemaMode = true;
        CORE.Instance.InvokeEvent("MachinemaModeRefresh");
    }
}
