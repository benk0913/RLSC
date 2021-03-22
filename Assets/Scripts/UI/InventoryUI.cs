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


    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void Toggle(ActorData ofActor)
    {
        if(this.gameObject.activeInHierarchy)
        {
            Hide();
            return;
        }

        this.gameObject.SetActive(true);
        currentActor = ofActor;
        RefreshUI();
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void RefreshUI()
    {
        if (!this.gameObject.activeInHierarchy)
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
            EquipSlots[i].Slot.SetItem(item);
        }

        CORE.Instance.DelayedInvokation(1f, () => SelectionGroup.RefreshGroup());
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
            //REPLACE

            if(SelectedSlot.IsEquipmentSlot & slot.IsEquipmentSlot)
            {
                TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("First unequip the item and then re equip it.", Color.red));
                return;
            }
            else if (SelectedSlot.IsEquipmentSlot)
            {
                SocketHandler.Instance.SendSwappedItemAndEquipSlots(slot.transform.GetSiblingIndex(), SelectedSlot.CurrentItem.Data.Type.name);
            }
            else if (slot.IsEquipmentSlot)
            {
                SocketHandler.Instance.SendSwappedItemAndEquipSlots(slot.transform.GetSiblingIndex(), SelectedSlot.CurrentItem.Data.Type.name);
            }
            else
            {
                SocketHandler.Instance.SendSwappedItemSlots(SelectedSlot.transform.GetSiblingIndex(), slot.transform.GetSiblingIndex());
            }

            Deselect();
            slot.Deselect();
            return;
        }

        SelectedSlot = slot;
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
