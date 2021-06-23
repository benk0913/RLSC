using EdgeworldBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootRollPanelUI : MonoBehaviour
{
    public static LootRollPanelUI Instance;

    List<LootRollItemUI> Items = new List<LootRollItemUI>();

    List<PartyInvitePanelUI> Invites = new List<PartyInvitePanelUI>();

    private void Awake()
    {
        Instance = this;
    }

    public void AddLootRollItem(Item item, float duration)
    {
        LootRollItemUI lootRollItem = ResourcesLoader.Instance.GetRecycledObject("LootRollItem").GetComponent<LootRollItemUI>();

        lootRollItem.SetInfo(item, duration);
        lootRollItem.transform.SetParent(transform, false);

        Items.Add(lootRollItem);
    }

    public void RemoveLootRollItem(Item item)
    {
        LootRollItemUI itm = Items.Find(x => x.CurrentItem.itemId == item.itemId);
        
        if(itm == null)
        {
            CORE.Instance.LogMessageError("NO ROLL ITEM INSTANCE FOR ID " + item.itemId);
            return;
        }

        itm.gameObject.SetActive(false);
        Items.Remove(itm);
    }

    public void ReleaseLootRollItem(Item item)
    {
        LootRollItemUI itm = Items.Find(x => x.CurrentItem.itemId == item.itemId);

        if (itm == null)
        {
            CORE.Instance.LogMessageError("NO ROLL ITEM INSTANCE FOR ID " + item.itemId);
            return;
        }

        itm.Release();
    }

    public void AddPartyInvitation(string fromPlayer)
    {
        PartyInvitePanelUI partyInviteUI = ResourcesLoader.Instance.GetRecycledObject("PartyInvitePanelUI").GetComponent<PartyInvitePanelUI>();

        partyInviteUI.SetInfo(fromPlayer);
        partyInviteUI.transform.SetParent(transform, false);
        Invites.Add(partyInviteUI);
    }

    public void RemovePartyInvitation(string fromPlayer = "")
    {
        if(Invites.Count == 0)
        {
            return;
        }

        PartyInvitePanelUI partyInviteUI;
        if (string.IsNullOrEmpty(fromPlayer))
        {
            partyInviteUI = Invites[0];
        }
        else
        {
            partyInviteUI = Invites.Find(x => x.CurrentFromPlayer == fromPlayer);
        }
        

        if (partyInviteUI == null)
        {
            CORE.Instance.LogMessageError("NO PARTY INVITE FROM " + fromPlayer);
            return;
        }

        partyInviteUI.gameObject.SetActive(false);
        Invites.Remove(partyInviteUI);
    }

}
