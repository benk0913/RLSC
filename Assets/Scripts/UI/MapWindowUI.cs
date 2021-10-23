using EdgeworldBase;
using UnityEngine;

public class MapWindowUI : MonoBehaviour, WindowInterface
{
    public static MapWindowUI Instance;


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

        CORE.Instance.SubscribeToEvent("MapUpdated", RefreshUI);
        
        RefreshUI();
    }


    public void Show(ActorData actorData, object data = null)
    {
        IsOpen = true;

        this.gameObject.SetActive(true);
        
        
        AudioControl.Instance.Play(OpenSound);

        RefreshUI();

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


    }

}
