using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    private Dictionary<string, ItemData> _itemDataMap = new Dictionary<string, ItemData>();

    InventoryDraggedItemUI currentlyDraggedItem;

    ActorData currentActor;

    [SerializeField]
    Transform ItemsContainer;

    [SerializeField]
    SelectionGroupUI SelectionGroup;

    public List<EquippableSlot> EquipSlots = new List<EquippableSlot>();

    public GridLayoutGroup InventoryGridLayout;

    InventorySlotUI SelectedSlot;
    
    public bool IsOpen;    


    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Toggle(ActorData ofActor)
    {
        if(IsOpen)
        {
            Hide();
            return;
        }

        IsOpen = true;
        this.gameObject.SetActive(true);
        currentActor = ofActor;
        RefreshUI();
    }

    public void Hide()
    {
        IsOpen = false;
        this.gameObject.SetActive(false);
    }

    public void RefreshUI()
    {
        if (!IsOpen)
        {
            return;
        }

        CORE.ClearContainer(ItemsContainer);

        for(int i = 0; i < currentActor.items.Count; i++)
        {
            InventorySlotUI slot = ResourcesLoader.Instance.GetRecycledObject("InventorySlotUI").GetComponent<InventorySlotUI>();
            slot.SetItem(currentActor.items[i], ()=> Select(slot));
            slot.transform.SetParent(ItemsContainer, false);
            slot.transform.localScale = Vector3.one;
            slot.transform.position = Vector3.zero;

        }

        
        for (int i = 0; i < EquipSlots.Count; i++)
        {
            Item item = null;
            currentActor.equips.TryGetValue(EquipSlots[i].Type.name, out item); 
            InventorySlotUI slot = EquipSlots[i].Slot;
            slot.SetItem(item, () => Select(slot));
        }

        CORE.Instance.DelayedInvokation(0f, () => SelectionGroup.RefreshGroup());
    }

    internal void DragItem(InventorySlotUI inventorySlotUI)
    {
        if (currentlyDraggedItem != null)
        {
            currentlyDraggedItem.gameObject.SetActive(false);
            currentlyDraggedItem = null;
        }

        currentlyDraggedItem = ResourcesLoader.Instance.GetRecycledObject("InventoryDraggedItemUI").GetComponent<InventoryDraggedItemUI>();
        currentlyDraggedItem.transform.SetParent(transform);
        currentlyDraggedItem.SetInfo(inventorySlotUI.CurrentItem);

    }

    internal void UndragItem(InventorySlotUI inventorySlotUI)
    { 
        if(currentlyDraggedItem != null)
        {
            Select(inventorySlotUI);
            currentlyDraggedItem.gameObject.SetActive(false);
        }
    }

    public void Select(InventorySlotUI slot)
    {
        if(SelectedSlot != null)
        {
            if (SelectedSlot.CurrentItem == null && slot.CurrentItem == null)
            {
                // Do nothing, ignore swaps between 2 empty slots.
            }
            else if (!SelectedSlot.IsEquipmentSlot && SelectedSlot == slot)
            {
                SocketHandler.Instance.SendEquippedItem(SelectedSlot.transform.GetSiblingIndex());
            }
            else if (SelectedSlot.IsEquipmentSlot && SelectedSlot == slot)
            {
                SocketHandler.Instance.SendUnequippedItem(SelectedSlot.name);
            }
            else if (SelectedSlot.IsEquipmentSlot & slot.IsEquipmentSlot)
            {
                TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("First unequip the item and then re equip it.", Color.red));
            }
            else if (SelectedSlot.IsEquipmentSlot)
            {
                SocketHandler.Instance.SendSwappedItemAndEquipSlots(slot.transform.GetSiblingIndex(), SelectedSlot.name);
            }
            else if (slot.IsEquipmentSlot)
            {
                SocketHandler.Instance.SendSwappedItemAndEquipSlots(SelectedSlot.transform.GetSiblingIndex(), slot.name);
            }
            else
            {
                SocketHandler.Instance.SendSwappedItemSlots(SelectedSlot.transform.GetSiblingIndex(), slot.transform.GetSiblingIndex());
            }

            Deselect();
            slot.Deselect();
        }
        else
        {
            SelectedSlot = slot;
        }
    }

    public void Deselect()
    {
        SelectedSlot.Deselect();
        SelectedSlot = null;
    }

    public void AttemptDrop()
    {
        if(SelectedSlot == null)
        {
            return;
        }

        if(SelectedSlot.IsEquipmentSlot)
        {
            TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("First unequip the item and only then, you may drop it.", Color.red));
            return;
        }

        SocketHandler.Instance.SendDroppedItem(SelectedSlot.transform.GetSiblingIndex());

        Deselect();
    }

    ////// TEST!
    //public bool Test;

    //private void Update()
    //{
    //    if(Test)
    //    {
    //        Toggle(SocketHandler.Instance.CurrentUser.actor);
    //        Test = false;
    //    }
    //}
    //////
}

[System.Serializable]
public class EquippableSlot
{
    public InventorySlotUI Slot;
    public ItemType Type;
}
