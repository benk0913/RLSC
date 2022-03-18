using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchFoundPanelUI : PartyInvitePanelUI
{
    [SerializeField]
    GameObject WaitingLabel;


    public override void SetInfo()
    {
        base.SetInfo();
        TitleText.text = "Match Found!";
        WaitingLabel.SetActive(false);
    }

    public override void Accept()
    {
        SocketHandler.Instance.SendExpeditionQueueMatchResponse(true);
        CG.interactable = false;
        WaitingLabel.SetActive(true);
    }

    public override void Decline()
    {
        SocketHandler.Instance.SendExpeditionQueueMatchResponse(false);
        CG.interactable = false;
        ExpeditionQueTimerUI.Instance.StopSearching();
    }
    
    protected override int GetTimeoutSeconds()
    {
        return CORE.Instance.Data.content.ExpeditionQueueMatchDurationSeconds;
    }

}
