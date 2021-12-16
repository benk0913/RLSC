using System;
using EdgeworldBase;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Events;
using static UserData;

public class FriendsWindowUI : MonoBehaviour, WindowInterface
{
    public static FriendsWindowUI Instance;

    [SerializeField]
    Transform FriendsContainer;
    



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

        CORE.Instance.SubscribeToEvent("FriendsUpdated", RefreshUI);
        
        RefreshUI();
    }


    [Obsolete("Do not call Show directly. Call `CORE.Instance.ShowWindow()` instead.")]
    public void Show(ActorData actorData, object data = null)
    {
        IsOpen = true;

        this.gameObject.SetActive(true);
        
        
        AudioControl.Instance.Play(OpenSound);

        RefreshUI();

        CORE.Instance.DelayedInvokation(0.1f, () => SGroup.RefreshGroup(true));
    }

    [Obsolete("Do not call Hide directly. Call `CORE.Instance.CloseCurrentWindow()` instead.")]
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

        CORE.ClearContainer(FriendsContainer);


        if (CORE.Instance.CurrentParty == null)
        {
            return;
        }

        foreach (var KeyValuePair in FriendsDataHandler.Instance.Friends)
        {
            FriendData Friend = KeyValuePair.Value;
            FriendDisplayDisplayUI element = ResourcesLoader.Instance.GetRecycledObject("FriendDisplay").GetComponent<FriendDisplayDisplayUI>();

            element.SetInfo(Friend);
            element.transform.SetParent(FriendsContainer, false);
            element.transform.localScale = Vector3.one;
            element.transform.position = Vector3.zero;
        }


        CORE.Instance.DelayedInvokation(0.1f,()=>SGroup.RefreshGroup(true));

    }

    public void AddMember()
    {
        InputLabelWindow.Instance.Show("Add Friend", "Player Name...", (string name) =>
        {    
            JSONNode node = new JSONClass();
            node["actorName"] = name;
            SocketHandler.Instance.SendEvent("friend_add", node);
        });
    }
}
