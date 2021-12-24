using SimpleJSON;
using TMPro;
using UnityEngine;

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
            string displayName = CurrentFriendData.currentName;
            if (CurrentFriendData.currentName != CurrentFriendData.mainActorName)
            {
                // If the friend is playing on a different char, show the original name too
                displayName += " (" + CurrentFriendData.mainActorName + ")";
            }
            NameLabel.text = CurrentFriendData.mainActorName;
            NameLabel.color = Colors.AsColor(Colors.COLOR_TEXT);
        }
        else
        {
            string translatedPart = " (offline)";
            CORE.Instance.Data.Localizator.mSource.TryGetTranslationCodywise(translatedPart, out translatedPart);
            
            NameLabel.text = CurrentFriendData.mainActorName + translatedPart;
            NameLabel.color = Color.grey;
        }

        InviteButton.SetActive(true);
        if(CORE.Instance.CurrentParty != null)
        {
            foreach(string partyMemberName in CORE.Instance.CurrentParty.members)
            {
                if(partyMemberName == CurrentFriendData.mainActorName)
                {
                    InviteButton.SetActive(false);
                    break;
                }
            }
        }
    }
   

    public void RemoveFriend()
    {
        WarningWindowUI.Instance.Show("Remove "+CurrentFriendData.mainActorName+" from the friends list!?",()=>
        {   
            JSONNode node = new JSONClass();
            node["friendUserId"] = CurrentFriendData.userId;
            SocketHandler.Instance.SendEvent("friend_delete", node);
        });
    }

    public void InviteToParty()
    {
        SocketHandler.Instance.SendPartyInvite(CurrentFriendData.mainActorName);
    }
     public void OpenAccountProfile()
    {
        #if !UNITY_ANDROID && !UNITY_IOS
        SteamFriends.ActivateGameOverlayToUser("steamid",new CSteamID(CurrentFriendData.steamId));
        #endif
    }

 
}