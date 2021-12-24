using SimpleJSON;
using UnityEngine;

public class InspectionPanelUI : MonoBehaviour
{


    ActorData CurrentActor;

    public bool IsFocusingOnActor = false;

    public void SetActor(ActorData currentActor)
    {
        if((currentActor == CORE.PlayerActor && CameraChaseEntity.Instance.ReferenceObject == CORE.PlayerActor.ActorEntity.transform))
        {
            return;
        }

        if(currentActor == null)
        {
            IsFocusingOnActor = false;
            CurrentActor = null;

            if(!CORE.PlayerActor.ActorEntity.IsDead)
                CameraChaseEntity.Instance.ReferenceObject = null;

            return;
        }

        CurrentActor = currentActor;
        IsFocusingOnActor = true;

        if(!CORE.PlayerActor.ActorEntity.IsDead)
            CameraChaseEntity.Instance.ReferenceObject = CurrentActor.ActorEntity.transform;
    }

    void Update()
    {
        if(CameraChaseEntity.Instance != null && (CurrentActor == null || CurrentActor.ActorEntity == null ) && !CORE.PlayerActor.ActorEntity.IsDead)
        {
            CameraChaseEntity.Instance.ReferenceObject = null;
            IsFocusingOnActor = false;
        }
    }

    public void SendPartyInvite()
    {
        SocketHandler.Instance.SendPartyInvite(CurrentActor.name);
    }

    public void SendStartTrade()
    {
        SocketHandler.Instance.SendEvent("start_trade",CurrentActor.actorId);
    }

    public void SendReportPlayer()
    {
        InputLabelWindow.Instance.Show("Report Player", "What's Wrong?", (string msg) => 
        {
            JSONNode node = new JSONClass();
            node["message"] = msg;
            node["repotedName"] = CurrentActor.name;
            SocketHandler.Instance.SendEvent("report_player", node);
        });
    }

    public void OpenAccountProfile()
    {
        #if !UNITY_ANDROID && !UNITY_IOS
        SteamFriends.ActivateGameOverlayToUser("steamid",new CSteamID(CurrentActor.steamID));
        #endif
    }
    
    public void AddFriend()
    {
        foreach (var KeyValuePair in FriendsDataHandler.Instance.Friends)
        {
            FriendData Friend = KeyValuePair.Value;
            if(Friend.currentName == CurrentActor.name)
            {
                CORE.Instance.ShowFriendsWindow();
                return;
            }
        }
        JSONNode node = new JSONClass();
        node["actorName"] = CurrentActor.name;
        SocketHandler.Instance.SendEvent("friend_add", node);
    }

    public void SendPrivateMessage()
    {
        #if !UNITY_ANDROID && !UNITY_IOS
        SteamFriends.ActivateGameOverlayToUser("chat", new CSteamID(CurrentActor.steamID));
        #endif
    }
}
