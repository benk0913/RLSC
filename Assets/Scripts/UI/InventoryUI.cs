using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    private Dictionary<string, ItemData> _itemDataMap = new Dictionary<string, ItemData>();

    public ItemData GetItemData(string itemName)
    {
        if (string.IsNullOrEmpty(itemName))
        {
            return null;
        }
        if(!_itemDataMap.ContainsKey(itemName))
        {
            _itemDataMap.Add(itemName, CORE.Instance.Data.content.Items.Find(X => X.name == itemName));
        }

        return _itemDataMap[itemName];
    }

    InventoryDraggedItemUI currentlyDraggedItem;

    ActorData currentActor;

    [SerializeField]
    Transform ItemsContainer;

    public List<EquippableSlot> EquipSlots = new List<EquippableSlot>();


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

    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void Show(ActorData ofActor)
    {
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
            if (!currentActor.equips.ContainsKey(EquipSlots[i].Type.name))
            {
                return;
            }

            EquipSlots[i].Slot.SetItem(currentActor.equips[EquipSlots[i].Type.name]);
        }
    }


    ////// TEST!
    public bool Test;

    private void Update()
    {
        if(Test)
        {
            Show(SocketHandler.Instance.CurrentUser.actor);
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
