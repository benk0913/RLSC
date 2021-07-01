using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchFoundPanelUI : PartyInvitePanelUI
{
    public override void SetInfo()
    {
        base.SetInfo();
        TitleText.text = "Match Found!";
    }

    public override void Accept()
    {
        SocketHandler.Instance.SendExpeditionQueueMatchResponse(true);
        CG.interactable = false;
    }

    public override void Decline()
    {
        SocketHandler.Instance.SendExpeditionQueueMatchResponse(false);
        CG.interactable = false;
    }
}
