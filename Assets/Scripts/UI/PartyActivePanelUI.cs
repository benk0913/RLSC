using EdgeworldBase;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartyActivePanelUI : MonoBehaviour
{
    public static PartyActivePanelUI Instance;

    [SerializeField]
    CanvasGroup CG;

    [SerializeField]
    Transform MembersContainer;


    public List<PartyMemberActivePanelUI> Members = new List<PartyMemberActivePanelUI>();

    public string checksumString;

    private void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        CORE.Instance.SubscribeToEvent("PartyUpdated", OnPartyUpdated);
        OnPartyUpdated();
    }

    void OnPartyUpdated()
    {
        if(CORE.Instance == null)
        {
            return;
        }

        if(CORE.Instance.CurrentParty == null || CORE.Instance.CurrentParty.members == null)
        {
            checksumString = "";
            HideActivePanel();
            return;
        }
        else
        {
            ShowActivePanel();
        }


        //Checksum prevents going over useless operations.
        string tempString = "";
        foreach(string member in CORE.Instance.CurrentParty.members)
        {
            tempString += member + (CORE.Instance.CurrentParty.membersOffline.ContainsKey(member) ? "1" : "0");
        }

        if(tempString == checksumString)
        {
            return;
        }
        

        //Locate and remove players who left-
        List<PartyMemberActivePanelUI> toRemove = new List<PartyMemberActivePanelUI>();
        foreach (PartyMemberActivePanelUI member in Members)
        {
            if (!CORE.Instance.CurrentParty.members.Any(X => X == member.CurrentActorName))
            {
                toRemove.Add(member);
            }
        }

        while (toRemove.Count > 0)
        {
            PartyMemberActivePanelUI member = toRemove[0];
            member.gameObject.SetActive(false);
            member.transform.SetParent(transform);
            Members.Remove(member);
            toRemove.RemoveAt(0);
        }

        //Add players who joined
        foreach (string member in CORE.Instance.CurrentParty.members)
        {
            PartyMemberActivePanelUI memberPanel = Members.Find(x => x.CurrentActorName == member);

            if (memberPanel == null)
            {
                memberPanel = ResourcesLoader.Instance.GetRecycledObject("PartyMemberActivePanelUI").GetComponent<PartyMemberActivePanelUI>();
                memberPanel.SetCurrentActor(member);
                memberPanel.transform.SetParent(MembersContainer, false);
                memberPanel.transform.localScale = Vector3.one;
                memberPanel.transform.position = Vector3.zero;
                Members.Add(memberPanel);
            }

            memberPanel.transform.SetAsLastSibling();
        }

        

        checksumString = tempString;

    }



    public void ShowActivePanel()
    {
        if (this.CG.alpha < 1f)
        {
            this.CG.alpha = 1f;
        }
    }

    public void HideActivePanel()
    {
        if(this.CG.alpha > 0f)
        {
            this.CG.alpha = 0f;
        }
    }
}
