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

    [SerializeField]
    GameObject InviteButton;


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
            NameLabel.color = Colors.AsColor(Colors.COLOR_TEXT);
        }
        else
        {
            NameLabel.text = CurrentFriendData.name + " (offline)";
            NameLabel.color = Color.grey;
        }

        InviteButton.SetActive(true);
        if(CORE.Instance.CurrentParty != null)
        {
            foreach(string partyMemberName in CORE.Instance.CurrentParty.members)
            {
                if(partyMemberName == CurrentFriendData.name)
                {
                    InviteButton.SetActive(false);
                    break;
                }
            }
        }
    }
   

    public void RemoveFriend()
    {
        WarningWindowUI.Instance.Show("Remove "+CurrentFriendData.name+" from the friends list!?",()=>
        {
            SocketHandler.Instance.SendEvent("remove_friend",CurrentFriendData.name);
        });
    }

    public void InviteToParty()
    {
        SocketHandler.Instance.SendPartyInvite(CurrentFriendData.name);
    }

 
}