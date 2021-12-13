using SimpleJSON;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
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
        SteamFriends.ActivateGameOverlayToUser("steamid",new CSteamID(CurrentActor.steamID));
    }
    
    public void AddFriend()
    {
        foreach(UserData.FriendData friend in SocketHandler.Instance.CurrentUser.friends)
        {
            if(friend.name == CurrentActor.name)
            {
                CORE.Instance.ShowFriendsWindow();
                return;
            }
        }

        SocketHandler.Instance.SendEvent("add_friend",CurrentActor.name);
        //SteamFriends.ActivateGameOverlayToUser("friendadd", new CSteamID(CurrentActor.steamID));
    }

    public void SendPrivateMessage()
    {
        SteamFriends.ActivateGameOverlayToUser("chat", new CSteamID(CurrentActor.steamID));
    }
}
