using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UserData;

public class FriendDisplayDisplayUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI NameLabel;

    FriendData CurrentFriendData;


    void OnEnable()
    {
        CORE.Instance.SubscribeToEvent("FriendsUpdated", OnFriendsUpdated);
        OnFriendsUpdated();
    }

    private void OnDisable()
    {
        CORE.Instance.UnsubscribeFromEvent("FriendsUpdated", OnFriendsUpdated);
    }

    public void OnFriendsUpdated()
    {


        RefreshUI();
    }

    public void SetInfo(FriendData data)
    {
        CurrentFriendData = data;
        RefreshUI();

    }

    public void RefreshUI()
    {
        if(CurrentFriendData.isOnline)
        {
            NameLabel.text = CurrentFriendData.name;
            NameLabel.color = Color.white;
        }
        else
        {
            NameLabel.text = CurrentFriendData.name + " (offline)";
            NameLabel.color = Color.grey;
        }
    }
   

    public void RemoveFriend()
    {
        WarningWindowUI.Instance.Show("Remove "+CurrentFriendData.name+" from the friends list!?",()=>
        {
            SocketHandler.Instance.SendEvent("remove_friend",CurrentFriendData.name);
        });
    }

 
}