using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    ActorData currentActor;

    [SerializeField]
    Transform ItemsContainer;

    public List<EquippableSlot> EquipSlots = new List<EquippableSlot>();

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

    void RefreshUI()
    {
        CORE.ClearContainer(ItemsContainer);

        for(int i=0;i<currentActor.items.inventory.Count;i++)//TODO Need to make sure server sends a full inventory with "nulls" as free slots.
        {
            InventorySlotUI slot = ResourcesLoader.Instance.GetRecycledObject("InventorySlotUI").GetComponent<InventorySlotUI>();
            slot.SetItem(currentActor.items.inventory[i]);
            slot.transform.SetParent(ItemsContainer, false);
            slot.transform.localScale = Vector3.one;
            slot.transform.position = Vector3.zero;
        }
        
        for (int i = 0; i < currentActor.items.equips.Count; i++)
        {
            EquipSlots.Find(x => x.Type == currentActor.items.equips[i].type).Slot.SetItem(currentActor.items.equips[i].item);
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
