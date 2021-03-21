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

    public List<EquippableSlot> EquipSlots = new List<EquippableSlot>();

    public GridLayoutGroup InventoryGridLayout;


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
            slot.SetItem(currentActor.items[i]);
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

    internal void DropItem(InventorySlotUI inventorySlotUI)
    {
        throw new NotImplementedException();
    }

    ////// TEST!
    public bool Test;

    private void Update()
    {
        if(Test)
        {
            Toggle(SocketHandler.Instance.CurrentUser.actor);
            Test = false;
        }
    }
    //////
}

[System.Serializable]
public class EquippableSlot
{
    public InventorySlotUI Slot;
    public ItemType Type;
}
