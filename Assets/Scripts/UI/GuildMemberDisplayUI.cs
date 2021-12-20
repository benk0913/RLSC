using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuildMemberDisplayUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI NameLabel;

    [SerializeField]
    TextMeshProUGUI LVLLabel;

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
    Image LeaderMarker;


    Actor CurrentActor;

    string CurrentMemberName = "";

    public bool IsPlayer;
    public bool IsOffline;
    public bool IsInRoom;
    public bool IsLeader;
    public bool IsOfficer;


    void OnEnable()
    {
        CORE.Instance.SubscribeToEvent("GuildUpdated", OnGuildUpdated);
        OnGuildUpdated();
    }

    private void OnDisable()
    {
        CORE.Instance.UnsubscribeFromEvent("GuildUpdated", OnGuildUpdated);
    }

    public void OnGuildUpdated()
    {
        if(CORE.Instance.CurrentGuild == null)
        {
            return;
        }

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
        IsLeader = CORE.Instance.CurrentGuild.leaderName == CurrentMemberName;

        LeaveButton.SetActive(IsPlayer);
        KickButton.SetActive(CORE.Instance.CurrentGuild.IsPlayerLeader && !IsPlayer);
        PromoteButton.SetActive(!IsPlayer && !IsLeader && CORE.Instance.CurrentGuild.IsPlayerLeader);
        LeaderMarker.gameObject.SetActive(IsLeader || IsOfficer);
        if(IsOfficer)
        {
            LeaderMarker.color = Color.blue;
        }
        else
        {
            LeaderMarker.color = Color.yellow;
        }

        
        if(IsPlayer)
        {
            NameLabel.color = Colors.AsColor(Colors.COLOR_HIGHLIGHT);
        }
        else
        {
            NameLabel.color = Colors.AsColor(Colors.COLOR_TEXT);
        }

        if (IsInRoom)
        {
            ClassIcon.gameObject.SetActive(true);
            ClassIcon.sprite = CurrentActor.State.Data.ClassJobReference.Icon;
            Background.color = CurrentActor.State.Data.ClassJobReference.ClassColor;

            LVLLabel.gameObject.SetActive(true);
            LVLLabel.text = CurrentActor.State.Data.level.ToString();
            LVLLabel.color = CurrentActor.State.Data.ClassJobReference.ClassColor; 
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

            LVLLabel.gameObject.SetActive(false);
        }
    }
   

    public void KickPlayer()
    {
        SocketHandler.Instance.SendGuildKick(CurrentMemberName);
    }

    public void LeaveGuild()
    {
        SocketHandler.Instance.SendGuildLeave();
        //TODO Remove self()?
    }

    public void PromotePlayer()
    {
        SocketHandler.Instance.SendGuildPromote(CurrentMemberName);
    }



}