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

        this.gameObject.SetActive(true);
        
        
        AudioControl.Instance.Play(OpenSound);

        RefreshUI();

        CORE.Instance.DelayedInvokation(0.1f, () => SGroup.RefreshGroup(true));
    }

    public void Hide()
    {
        IsOpen = false;
        this.gameObject.SetActive(false);

        AudioControl.Instance.Play(HideSound);
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
