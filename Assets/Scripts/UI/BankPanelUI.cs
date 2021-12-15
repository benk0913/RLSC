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

    [SerializeField]
    Transform InventoryPlusButton;

    void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
        CORE.Instance.SubscribeToEvent("BankUpdated",RefreshUI);
    }

    public void Show()
    {
        CORE.Instance.ShowInventoryUiWindow();
        
        this.gameObject.SetActive(true);

        CORE.Instance.DelayedInvokation(0.1f,()=>
        {
            RefreshUI();
            CORE.Instance.DelayedInvokation(0.1f,()=>
            {
                InventoryUI.Instance.SelectionGroup.RefreshGroup();
            });
        });
    }

    public void RefreshUI()
    {
        if(!this.gameObject.activeInHierarchy)
        {
            return;
        }

        MoneyLabel.text = System.String.Format("{0:n0}", SocketHandler.Instance.CurrentUser.info.bankMoney);

        CORE.ClearContainer(BankInventoryContainer);

        for(int i=0;i<SocketHandler.Instance.CurrentUser.info.bankItems.Count;i++)
        {
            InventorySlotUI slot = ResourcesLoader.Instance.GetRecycledObject("InventorySlotUI").GetComponent<InventorySlotUI>();

            slot.SetItem(SocketHandler.Instance.CurrentUser.info.bankItems[i], () => InventoryUI.Instance.Select(slot));
            slot.IsBankSlot = true;
            slot.transform.SetParent(BankInventoryContainer, false);
            slot.transform.localScale = Vector3.one;
            slot.transform.position = Vector3.zero;
        }

        if(CORE.Instance.Data.content.BankData.MaxSlots > BankInventoryContainer.childCount)
        {
            InventoryPlusButton.SetParent(BankInventoryContainer,false);
            InventoryPlusButton.gameObject.SetActive(true);
            InventoryPlusButton.transform.localScale = Vector3.one;
            InventoryPlusButton.transform.position = Vector3.zero;
        }
        else
        {
            InventoryPlusButton.gameObject.SetActive(false);
        }
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

     public void IncreaseSlotSize()
    {
        int currentLevel = ((BankInventoryContainer.childCount-1)-CORE.Instance.Data.content.BankData.StartingSlots)/CORE.Instance.Data.content.BankData.IncreaseSlotsAmount;
        
        WarningWindowUI.Instance.Show(CORE.QuickTranslate("Increase Bank Capacity? This will cost you ")+CORE.Instance.Data.content.BankData.BuySlotsPrices[currentLevel]+CORE.QuickTranslate(" gold!"),()=>
        {
            SocketHandler.Instance.SendEvent("increase_bank_slots");
        });
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

    internal void UsedBankItem(InventorySlotUI selectedSlot)
    {
        JSONClass node = new JSONClass();
        node["bankSlotIndex"].AsInt = selectedSlot.transform.GetSiblingIndex();
        SocketHandler.Instance.SendEvent("bank_used_bank_item", node);
    }

    internal void UsedInventoryItem(InventorySlotUI selectedSlot)
    {
        JSONClass node = new JSONClass();
        node["inventorySlotIndex"].AsInt = selectedSlot.transform.GetSiblingIndex();
        SocketHandler.Instance.SendEvent("bank_used_inventory_item", node);
    }

    internal void UsedEquipItem(InventorySlotUI selectedSlot)
    {
        JSONClass node = new JSONClass();
        node["equipType"] = selectedSlot.SlotType.name;
        SocketHandler.Instance.SendEvent("bank_used_equip_item", node);
    }

    internal void SwapBankSlot(InventorySlotUI slotA, InventorySlotUI slotB)
    {
        JSONClass node = new JSONClass();
        node["slotIndex1"].AsInt = slotA.transform.GetSiblingIndex();
        node["slotIndex2"].AsInt = slotB.transform.GetSiblingIndex();
        SocketHandler.Instance.SendEvent("bank_swap_slot",node);
    }

    internal void RetreiveItemToSlot(InventorySlotUI bankSlot, InventorySlotUI inventorySlot)
    {
        JSONClass node = new JSONClass();
        node["bankSlotIndex"].AsInt = bankSlot.transform.GetSiblingIndex();
        node["inventorySlotIndex"].AsInt = inventorySlot.transform.GetSiblingIndex();
        SocketHandler.Instance.SendEvent("bank_to_inventory",node);
    }

    internal void BankFromEquipSlot(InventorySlotUI bankSlot, InventorySlotUI equipSlot)
    {
        JSONClass node = new JSONClass();
        node["bankSlotIndex"].AsInt = bankSlot.transform.GetSiblingIndex();
        node["equipType"] = equipSlot.SlotType.name;
        SocketHandler.Instance.SendEvent("bank_to_equip",node);
    }

    internal void SetItemInBank(InventorySlotUI inventorySlot, InventorySlotUI bankSlot)
    {
        JSONClass node = new JSONClass();
        node["bankSlotIndex"].AsInt = bankSlot.transform.GetSiblingIndex();
        node["inventorySlotIndex"].AsInt = inventorySlot.transform.GetSiblingIndex();
        SocketHandler.Instance.SendEvent("bank_from_inventory",node);
    }
}
