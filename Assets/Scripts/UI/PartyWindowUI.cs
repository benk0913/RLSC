using System;
using EdgeworldBase;
using SimpleJSON;
using Steamworks;
using UnityEngine;
using UnityEngine.Events;

public class PartyWindowUI : MonoBehaviour, WindowInterface
{
    public static PartyWindowUI Instance;

    [SerializeField]
    Transform PartyMembersContainer;
    
    [SerializeField]
    SelectionGroupUI SelectionGroup;

    [SerializeField]
    public GameObject AddButton;


    [SerializeField]
    SelectionGroupUI SGroup;
    
    [SerializeField]
    GameObject QueuePanel;

    public bool IsOpen;

    public string OpenSound;
    public string HideSound;

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    void Start()
    {
        if(Instance == null)
        {
            return;
        }

        CORE.Instance.SubscribeToEvent("PartyUpdated", RefreshUI);
        CORE.Instance.SubscribeToEvent("MatchQueueUpdate", MatchQueueRefresh);
        
        RefreshUI();
    }


    [Obsolete("Do not call Show directly. Call `CORE.Instance.ShowWindow()` instead.")]
    public void Show(ActorData actorData, object data = null)
    {
        IsOpen = true;

        this.gameObject.SetActive(true);
        
        
        AudioControl.Instance.Play(OpenSound);

        RefreshUI();
        
        if (CORE.Instance.CurrentParty != null && CORE.Instance.CurrentParty.members.Length >= CORE.Instance.Data.content.MaxPartyMembers)
        {
            AddButton.SetActive(false);
        }
        else
        {
            AddButton.SetActive(true);
        }


        CORE.Instance.DelayedInvokation(0.1f, () => SGroup.RefreshGroup(true));
    }

    [Obsolete("Do not call Hide directly. Call `CORE.Instance.CloseCurrentWindow()` instead.")]
    public void Hide()
    {
        IsOpen = false;
        this.gameObject.SetActive(false);

        AudioControl.Instance.Play(HideSound);
    }
    
    public void MatchQueueRefresh()
    {
        QueuePanel.gameObject.SetActive(ExpeditionQueTimerUI.Instance.IsSearching);
    }

    public void RefreshUI()
    {
        
        MatchQueueRefresh();
        
        if (!IsOpen)
        {
            return;
        }

        CORE.ClearContainer(PartyMembersContainer);


        if (CORE.Instance.CurrentParty == null)
        {
            return;
        }

        foreach (string member in CORE.Instance.CurrentParty.members)
        {
            PartyMemberDisplayUI element = ResourcesLoader.Instance.GetRecycledObject("PartyMemberDisplay").GetComponent<PartyMemberDisplayUI>();

            bool isOffline = CORE.Instance.CurrentParty.membersOffline  != null &&  CORE.Instance.CurrentParty.membersOffline.ContainsKey(member);
            bool isInRoom = CORE.Instance.Room.Actors.Find(X => X.name == member) == null;
            element.SetInfo(member, isOffline);
            element.transform.SetParent(PartyMembersContainer, false);
            element.transform.localScale = Vector3.one;
            element.transform.position = Vector3.zero;
        }


        CORE.Instance.DelayedInvokation(0.1f,()=>SGroup.RefreshGroup(true));

    }

    public void RemoveMember(PartyMemberDisplayUI partyMemberDisplayUI)
    {
        partyMemberDisplayUI.gameObject.SetActive(false);
        partyMemberDisplayUI.transform.SetParent(transform);
    }

    public void InviteMember()
    {

        if (CORE.Instance.CurrentParty == null)
        {
            CreateParty();
        }

        CORE.Instance.ConditionalInvokation((X) => { return CORE.Instance.CurrentParty != null; }, () =>
        {
            InputLabelWindow.Instance.Show("Invite Player", "Player Name...", (string name) => { SocketHandler.Instance.SendPartyInvite(name); });
        });

    }
    
    public void InviteMemberFromSteam()
    {

        if (CORE.Instance.CurrentParty == null)
        {
            CreateParty();
        }

        CORE.Instance.ConditionalInvokation((X) => { return CORE.Instance.CurrentParty != null && CORE.Instance.CurrentParty.steamLobbyId != default; }, () =>
        {
            SteamFriends.ActivateGameOverlayInviteDialog(new CSteamID(CORE.Instance.CurrentParty.steamLobbyId));
            //SteamFriends.ActivateGameOverlayInviteDialogConnectString(CORE.PlayerActor.steamID.ToString());
        });
        //SteamFriends.ActivateGameOverlayInviteDialog(new CSteamID(CORE.PlayerActor.steamID));
    }

    public void CreateParty()
    {
        JSONNode node = new JSONClass();
        SocketHandler.Instance.SendEvent("party_create", node);
    }

    public void LeaveQueue()
    {

        ExpeditionQueTimerUI.Instance.StopSearching();
        QueuePanel.gameObject.SetActive(false);
    }
}
