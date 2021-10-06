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
}
