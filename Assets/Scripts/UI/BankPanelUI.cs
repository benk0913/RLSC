using System;
using System.Collections;
using System.Collections.Generic;
using EdgeworldBase;
using SimpleJSON;
using TMPro;
using UnityEngine;

public class BankPanelUI : MonoBehaviour
{
    public static BankPanelUI Instance;

    public Transform BankInventoryContainer;

    public TextMeshProUGUI MoneyLabel;


    void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
        CORE.Instance.SubscribeToEvent("BankUpdated",RefreshUI);
    }

    public void Show()
    {
        InventoryUI.Instance.Show(CORE.PlayerActor);
        this.gameObject.SetActive(true);

        RefreshUI();
    }

    public void RefreshUI()
    {
        if(!this.gameObject.activeInHierarchy)
        {
            return;
        }

        MoneyLabel.text = System.String.Format("{0:n0}", SocketHandler.Instance.CurrentUser.info.bankMoney);

        for(int i=0;i<SocketHandler.Instance.CurrentUser.info.bankItems.Count;i++)
        {
            InventorySlotUI slot = ResourcesLoader.Instance.GetRecycledObject("InventorySlotUI").GetComponent<InventorySlotUI>();

            slot.SetItem(SocketHandler.Instance.CurrentUser.info.bankItems[i], () => InventoryUI.Instance.Select(slot));
            slot.IsBankSlot = true;
            slot.transform.SetParent(BankInventoryContainer, false);
            slot.transform.localScale = Vector3.one;
            slot.transform.position = Vector3.zero;
        }
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void DepositMoney()
    {
        InputLabelWindow.Instance.Show("Deposit Money",default,(string setValue)=>
        {

            int intValue = 0;

            if(!int.TryParse(setValue, out intValue))
            {
                WarningWindowUI.Instance.Show("INVALID AMOUNT",null);
                return;
            }

            JSONClass node = new JSONClass();
            node["money"].AsInt = intValue;
            SocketHandler.Instance.SendEvent("bank_deposit_money",node);
        },CORE.PlayerActor.money);
    }

    public void WithdrawMoney()
    {
        InputLabelWindow.Instance.Show("Withdraw Money",default,(string setValue)=>
        {

            int intValue = 0;

            if(!int.TryParse(setValue, out intValue))
            {
                WarningWindowUI.Instance.Show("INVALID AMOUNT",null);
                return;
            }

            JSONClass node = new JSONClass();
            node["money"].AsInt = intValue;
            SocketHandler.Instance.SendEvent("bank_withdraw_money",node);
        },SocketHandler.Instance.CurrentUser.info.bankMoney);
    }

    internal void RetreiveItem(InventorySlotUI selectedSlot)
    {
        for(int i=0;i<CORE.PlayerActor.items.Count;i++)
        {
            if(CORE.PlayerActor.items[i] == null)
            {
                RetreiveItemToSlot(selectedSlot,InventoryUI.Instance.ItemsContainer.GetChild(i).GetComponent<InventorySlotUI>());
                return;
            }
        }

        WarningWindowUI.Instance.Show("Inventory is FULL",null);
    }

    internal void SwapBankSlot(InventorySlotUI slotA, InventorySlotUI slotB)
    {
        JSONClass node = new JSONClass();
        node["slotIndexA"].AsInt = slotA.transform.GetSiblingIndex();
        node["slotIndexB"].AsInt = slotB.transform.GetSiblingIndex();
        SocketHandler.Instance.SendEvent("bank_swap_slot",node);
    }

    internal void RetreiveItemToSlot(InventorySlotUI bankSlot, InventorySlotUI inventorySlot)
    {
        JSONClass node = new JSONClass();
        node["bankSlotIndex"].AsInt = bankSlot.transform.GetSiblingIndex();
        node["inventorySlotIndex"].AsInt = inventorySlot.transform.GetSiblingIndex();
        SocketHandler.Instance.SendEvent("bank_retreive_to_slot",node);
    }

    internal void EquipFromBankSlot(InventorySlotUI bankSlot, InventorySlotUI equipSlot)
    {
        JSONClass node = new JSONClass();
        node["bankSlotIndex"].AsInt = bankSlot.transform.GetSiblingIndex();
        node["equipSlotType"] = equipSlot.SlotType.name;
        SocketHandler.Instance.SendEvent("bank_equip_from_bank",node);
    }

    internal void BankFromEquipSlot(InventorySlotUI equipSlot, InventorySlotUI bankSlot)
    {
        JSONClass node = new JSONClass();
        node["bankSlotIndex"].AsInt = bankSlot.transform.GetSiblingIndex();
        node["equipSlotType"] = equipSlot.SlotType.name;
        SocketHandler.Instance.SendEvent("bank_equip_to_bank",node);
    }

    internal void SetItemInBank(InventorySlotUI inventorySlot, InventorySlotUI bankSlot)
    {
        JSONClass node = new JSONClass();
        node["bankSlotIndex"].AsInt = bankSlot.transform.GetSiblingIndex();
        node["inventorySlotIndex"].AsInt = inventorySlot.transform.GetSiblingIndex();
        SocketHandler.Instance.SendEvent("bank_set_item",node);
    }
}
