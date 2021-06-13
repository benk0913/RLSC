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

    Actor CurrentActor;

    string CurrentMemberName;

    public bool IsPlayer;

    PartyMemberState State;

    public void SetInfo(string memberName)
    {
        CurrentMemberName = memberName;
        CurrentActor = null;
        RefreshUI();
    }

    public void RefreshUI()
    {
        NameLabel.text = CurrentMemberName;


        ActorData actor = CORE.Instance.Room.Actors.Find(x => x.name == CurrentMemberName);

        if(actor == null)
        {
            SetFaraway();
            return;
        }

        IsPlayer = actor == CORE.Instance.Room.PlayerActor;

        LeaveButton.SetActive(IsPlayer);
        KickButton.SetActive(!IsPlayer);

        CurrentActor = actor.ActorEntity;

        if(CurrentActor == null)
        {
            SetFaraway();
            return;
        }


        ClassIcon.sprite = CurrentActor.State.Data.ClassJobReference.Icon;
        Background.color = CurrentActor.State.Data.ClassJobReference.ClassColor;
    }

    public void SetFaraway()
    {
        State = PartyMemberState.FarAway;
    }

}

public enum PartyMemberState
{
    Nearby,
    FarAway,
    Offline
}