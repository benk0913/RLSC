using System;
using EdgeworldBase;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Events;
using static UserData;

public class CashShopWindowUI : MonoBehaviour, WindowInterface
{
    public static CashShopWindowUI Instance;


    [SerializeField]
    SelectionGroupUI SGroup;

    [SerializeField]
    Canvas CameraCanvas;

    [SerializeField]
    GameObject DisplayActorPanel;

    [SerializeField]
    DisplayCharacterUI DisplayActor;

    


    public bool IsOpen;

    public string OpenSound;
    public string HideSound;

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    void Start()
    {
        if(Instance == null)
        {
            return;
        }

        CORE.Instance.SubscribeToEvent("CashShopUpdated", RefreshUI);
        
        RefreshUI();
    }


    public void Show(ActorData actorData, object data = null)
    {
        IsOpen = true;

        CameraCanvas.worldCamera = Camera.main;

        CORE.IsMachinemaMode = true;
        CORE.Instance.InvokeEvent("MachinemaModeRefresh");

        AudioControl.Instance.SetMusic("music_CashShopBaP");
        AudioControl.Instance.SetSoundscape("");

        this.gameObject.SetActive(true);
        
        
        AudioControl.Instance.Play(OpenSound);

        RefreshUI();

        CORE.Instance.DelayedInvokation(0.1f, () => SGroup.RefreshGroup(true));
    }

    public void ShowDisplayActor()
    {
        DisplayActorPanel.SetActive(true);
        DisplayActor.AttachedCharacter.SetActorInfo(CORE.PlayerActor);
    }

    public void HideDisplayActor()
    {
        DisplayActorPanel.SetActive(false);
    }

    public void Hide()
    {
        if(CORE.Instance != null && CORE.PlayerActor.ActorEntity != null)
        {
            CORE.IsMachinemaMode = false;
            CORE.Instance.InvokeEvent("MachinemaModeRefresh");
            CORE.Instance.RefreshSceneInfo();
            
            AudioControl.Instance.Play(HideSound);
        }

        IsOpen = false;
        this.gameObject.SetActive(false);

    }
    
    public void RefreshUI()
    {
       
        
        if (!IsOpen)
        {
            return;
        }



        CORE.Instance.DelayedInvokation(0.1f,()=>SGroup.RefreshGroup(true));

    }


}
