using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectionPanelUI : MonoBehaviour
{


    ActorData CurrentActor;

    public void SetActor(ActorData currentActor)
    {
        CurrentActor = currentActor;
    }

    public void SendPartyInvite()
    {
        SocketHandler.Instance.SendPartyInvite(CurrentActor.name);
    }
}
