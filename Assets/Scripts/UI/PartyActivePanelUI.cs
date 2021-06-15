using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyActivePanelUI : MonoBehaviour
{
    public static PartyActivePanelUI Instance;

    [SerializeField]
    CanvasGroup CG;

    [SerializeField]
    Transform MembersContainer;

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
        if(CORE.Instance.CurrentParty == null)
        {
            HideActivePanel();
            return;
        }
        else
        {
            ShowActivePanel();
        }

        CORE.ClearContainer(MembersContainer);

        foreach(string member in CORE.Instance.CurrentParty.members)
        {
            PartyMemberActivePanelUI element = ResourcesLoader.Instance.GetRecycledObject("PartyMemberActivePanelUI").GetComponent<PartyMemberActivePanelUI>();
            element.SetCurrentActor(member);
            element.transform.SetParent(MembersContainer, false);
            element.transform.localScale = Vector3.one;
            element.transform.position = Vector3.zero;
        }
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
