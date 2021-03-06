using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberDisplayUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI NameLabel;

    [SerializeField]
    Image ClassIcon;

    [SerializeField]
    Image Background;

    [SerializeField]
    GameObject KickButton;

    [SerializeField]
    GameObject LeaveButton;

    [SerializeField]
    GameObject PromoteButton;

    [SerializeField]
    GameObject InspectButton;

    [SerializeField]
    GameObject LeaderMarker;

    Actor CurrentActor;

    string CurrentMemberName = "";

    public bool IsPlayer;
    public bool IsOffline;
    public bool IsInRoom;
    public bool IsLeader;


    void OnEnable()
    {
        CORE.Instance.SubscribeToEvent("PartyUpdated", OnPartyUpdated);
        OnPartyUpdated();
    }

    private void OnDisable()
    {
        CORE.Instance.UnsubscribeFromEvent("PartyUpdated", OnPartyUpdated);
    }

    public void OnPartyUpdated()
    {
        if(CORE.Instance.CurrentParty == null)
        {
            return;
        }

        //IsOffline = CORE.Instance.CurrentParty.membersOffline == null || CORE.Instance.CurrentParty.membersOffline.ContainsKey(CurrentMemberName);
        IsOffline = false;

        RefreshUI();
    }

    public void SetInfo(string memberName, bool isOffline = false)
    {
        this.IsOffline = isOffline;
        CurrentMemberName = memberName;
        CurrentActor = null;
        RefreshUI();
    }

    public void RefreshUI()
    {
        NameLabel.text = CurrentMemberName;

        if (IsOffline)
        {
            IsInRoom = false;
        }
        else
        {
            ActorData actor = CORE.Instance.Room.Actors.Find(x => x.name == CurrentMemberName);

            if (actor == null)
            {
                IsInRoom = false;
            }
            else
            {
                IsInRoom = true;
            }

            if (IsInRoom)
            {
                CurrentActor = actor.ActorEntity;

                if (CurrentActor == null)
                {
                    IsInRoom = false;
                }
            }
        }

        IsPlayer = CurrentMemberName == CORE.Instance.Room.PlayerActor.name;
        IsLeader = CORE.Instance.CurrentParty.leaderName == CurrentMemberName;


        LeaveButton.SetActive(IsPlayer);
        KickButton.SetActive(CORE.Instance.CurrentParty.IsPlayerLeader && !IsPlayer);
        InspectButton.SetActive(!IsOffline && IsInRoom);
        PromoteButton.SetActive(!IsPlayer && !IsLeader && CORE.Instance.CurrentParty.IsPlayerLeader);
        LeaderMarker.SetActive(IsLeader);

        if (IsInRoom)
        {
            ClassIcon.gameObject.SetActive(true);
            ClassIcon.sprite = CurrentActor.State.Data.ClassJobReference.Icon;
            Background.color = CurrentActor.State.Data.ClassJobReference.ClassColor;
        }
        else
        {
            ClassIcon.gameObject.SetActive(false);

            if (IsOffline)
            {
                NameLabel.text = CurrentMemberName + " (OFFLINE)";
                Background.color = Color.black;
            }
            else
            {
                Background.color = Color.grey;
            }
        }
    }
   

    public void KickPlayer()
    {
        SocketHandler.Instance.SendPartyKick(CurrentMemberName);
    }

    public void LeaveParty()
    {
        SocketHandler.Instance.SendPartyLeave();
        //TODO Remove self()?
    }

    public void PromotePlayer()
    {
        SocketHandler.Instance.SendPartyLeader(CurrentMemberName);
    }

    public void InspectPlayer()
    {
        if (IsOffline || !IsInRoom)
        {
            TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Player is not nearby...", Color.red));
            return;
        }

        CORE.Instance.ShowInventoryUiWindow(CORE.Instance.Room.Actors.Find(X => X.name == CurrentMemberName));
    }

}