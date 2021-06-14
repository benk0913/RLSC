using System;
using SimpleJSON;
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

    public bool IsOpen;

    public string OpenSound;
    public string HideSound;

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    void OnEnable()
    {
        if(Instance == null)
        {
            return;
        }

        CORE.Instance.SubscribeToEvent("PartyUpdated", RefreshUI);
        RefreshUI();
    }

    private void OnDisable()
    {
        if (Instance == null)
        {
            return;
        }

        CORE.Instance.UnsubscribeFromEvent("PartyUpdated", RefreshUI);
    }

    public void Show()
    {
        this.Show(null);
    }

    public void Show(ActorData actorData)
    {
        IsOpen = true;

        this.gameObject.SetActive(true);
        
        RefreshUI();
        
        AudioControl.Instance.Play(OpenSound);

        if(CORE.Instance.CurrentParty != null && CORE.Instance.CurrentParty.members.Length + CORE.Instance.CurrentParty.membersOffline.Length >= CORE.Instance.Data.content.MaxPartyMembers)
        {
            AddButton.SetActive(false);
        }
        else
        {
            AddButton.SetActive(true);
        }
    }

    public void Hide()
    {
        IsOpen = false;
        this.gameObject.SetActive(false);

        AudioControl.Instance.Play(HideSound);
    }
    

    public void RefreshUI()
    {
        
        if (!IsOpen)
        {
            return;
        }

        CORE.ClearContainer(PartyMembersContainer);

        if(CORE.Instance.CurrentParty == null)
        {
            return;
        }

        foreach (string member in CORE.Instance.CurrentParty.members)
        {
            PartyMemberDisplayUI element = ResourcesLoader.Instance.GetRecycledObject("PartyMemberDisplay").GetComponent<PartyMemberDisplayUI>();
            element.SetInfo(member, false);
            element.transform.SetParent(PartyMembersContainer, false);
            element.transform.localScale = Vector3.one;
            element.transform.position = Vector3.zero;
        }

        foreach (string member in CORE.Instance.CurrentParty.membersOffline)
        {
            PartyMemberDisplayUI element = ResourcesLoader.Instance.GetRecycledObject("PartyMemberDisplay").GetComponent<PartyMemberDisplayUI>();
            element.SetInfo(member, true);
            element.transform.SetParent(PartyMembersContainer, false);
            element.transform.localScale = Vector3.one;
            element.transform.position = Vector3.zero;
        }
    }

    public void RemoveMember(PartyMemberDisplayUI partyMemberDisplayUI)
    {
        partyMemberDisplayUI.gameObject.SetActive(false);
        partyMemberDisplayUI.transform.SetParent(transform);
    }

    public void InviteMember()
    {
        InputLabelWindow.Instance.Show("Invite Player", "Player Name...", (string name) => { SocketHandler.Instance.SendPartyInvite(name); });
    }
}
