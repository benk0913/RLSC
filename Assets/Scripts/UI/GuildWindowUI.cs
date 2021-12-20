using System;
using EdgeworldBase;
using SimpleJSON;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GuildWindowUI : MonoBehaviour, WindowInterface
{
    public static GuildWindowUI Instance;

    [SerializeField]
    Transform GuildMembersContainer;

    [SerializeField]
    public GameObject InviteButton;

    [SerializeField]
    public GameObject DonateButton;

    [SerializeField]
    public GameObject UpgradeButton;


    [SerializeField]
    SelectionGroupUI SGroup;

    [SerializeField]
    TextMeshProUGUI GuildNameLabel;

    [SerializeField]
    TextMeshProUGUI GuildCountLabel;

    [SerializeField]
    TextMeshProUGUI ScrapStatusLabel;

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

        CORE.Instance.SubscribeToEvent("GuildUpdated", RefreshUI);
        
        RefreshUI();
    }


    [Obsolete("Do not call Show directly. Call `CORE.Instance.ShowWindow()` instead.")]
    public void Show(ActorData actorData, object data = null)
    {
        IsOpen = true;

        this.gameObject.SetActive(true);
        
        
        AudioControl.Instance.Play(OpenSound);

        RefreshUI();
        
        if (CORE.Instance.CurrentGuild == null || CORE.Instance.CurrentGuild.members.Length >= CORE.Instance.Data.content.MaxPartyMembers)
        {
            InviteButton.SetActive(false);
        }
        else
        {
            InviteButton.SetActive(true);
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
    

    public void RefreshUI()
    {
        
        if (!IsOpen)
        {
            return;
        }

        CORE.ClearContainer(GuildMembersContainer);


        if (CORE.Instance.CurrentGuild == null)
        {
            return;
        }

        foreach (string member in CORE.Instance.CurrentGuild.members)
        {
            GuildMemberDisplayUI element = ResourcesLoader.Instance.GetRecycledObject("GuildMemberDisplay").GetComponent<GuildMemberDisplayUI>();

            bool isOffline = CORE.Instance.CurrentGuild.membersOffline  != null &&  CORE.Instance.CurrentGuild.membersOffline.ContainsKey(member);
            bool isInRoom = CORE.Instance.Room.Actors.Find(X => X.name == member) == null;
            element.SetInfo(member, isOffline);
            element.transform.SetParent(GuildMembersContainer, false);
            element.transform.localScale = Vector3.one;
            element.transform.position = Vector3.zero;
        }


        CORE.Instance.DelayedInvokation(0.1f,()=>SGroup.RefreshGroup(true));

    }

    public void RemoveMember(GuildMemberDisplayUI guildMemberDisplay)
    {
        guildMemberDisplay.gameObject.SetActive(false);
        guildMemberDisplay.transform.SetParent(transform);
    }

    public void InviteMember()
    {


        CORE.Instance.ConditionalInvokation((X) => { return CORE.Instance.CurrentGuild != null; }, () =>
        {
            InputLabelWindow.Instance.Show("Invite Player", "Player Name...", (string name) => { SocketHandler.Instance.SendGuildInvite(name); });
        });

    }

    public void DonateScrap()
    {
        InputLabelWindow.Instance.Show("Donate Scrap","Set Amount",(string finalValue)=>
        {
            int finalValueInt = 0;
            if(int.TryParse(finalValue, out finalValueInt))
            {
                SocketHandler.Instance.SendDonateScrapToGuild(finalValueInt);
            }
            else
            {
                WarningWindowUI.Instance.Show(CORE.QuickTranslate("Wrong Amount")+"!",()=>{},false,null);
            }
        });
    }

    public void ShowUpgradeWindow()
    {
        
    }
    

}
