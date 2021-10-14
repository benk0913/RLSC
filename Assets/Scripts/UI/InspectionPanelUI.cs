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
            CameraChaseEntity.Instance.ReferenceObject = null;
            return;
        }

        CurrentActor = currentActor;
        IsFocusingOnActor = true;
        CameraChaseEntity.Instance.ReferenceObject = CurrentActor.ActorEntity.transform;
    }

    void Update()
    {
        if(CameraChaseEntity.Instance != null && (CurrentActor == null || CurrentActor.ActorEntity == null ))
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
            node["reporterId"] = CORE.PlayerActor.actorId;
            node["repotedId"] = CurrentActor.actorId;
            SocketHandler.Instance.SendEvent("report_player", node);
        });
    }

    public void OpenAccountProfile()
    {
        SteamFriends.ActivateGameOverlayToUser("steamid",new CSteamID(CurrentActor.steamID));
    }
    
    public void AddFriend()
    {
        SteamFriends.ActivateGameOverlayToUser("friendadd", new CSteamID(CurrentActor.steamID));
    }

    public void SendPrivateMessage()
    {
        SteamFriends.ActivateGameOverlayToUser("chat", new CSteamID(CurrentActor.steamID));
    }
}
