using EdgeworldBase;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour, WindowInterface
{
    public static InventoryUI Instance;

    private Dictionary<string, ItemData> _itemDataMap = new Dictionary<string, ItemData>();

    InventoryDraggedItemUI currentlyDraggedItem;

    ActorData currentActor;

    [SerializeField]
    Transform ItemsContainer;

    [SerializeField]
    SelectionGroupUI SelectionGroup;

    [SerializeField]
    GameObject SelectedPanel;

    [SerializeField]
    TextMeshProUGUI IsSelectedDropText;

    [SerializeField]
    TextMeshProUGUI IsSelectedUseText;

    [SerializeField]
    TextMeshProUGUI MoneyLabel;

    [SerializeField]
    public StatsPanelUI StatsPanel;

    [SerializeField]
    public string ShowSound;

    [SerializeField]
    public string HideSound;

    [SerializeField]
    public string DragItemSound;

    [SerializeField]
    public string UndragItemSound;

    [SerializeField]
    public string SelectSound;

    [SerializeField]
    public string UseSelectedSound;

    [SerializeField]
    public string DeselectSound;

    [SerializeField]
    public string DropSound;

    [SerializeField]
    GameObject InventoryPanel;

    [SerializeField]
    InspectionPanelUI InspectPanel;



    public List<EquippableSlot> EquipSlots = new List<EquippableSlot>();
    

    InventorySlotUI SelectedSlot;
    float SelectedTime;
    
    public bool IsOpen;

    bool isInspecting;




    private void Awake()
    {
        Instance = this;
        Hide();
    }

    private void Update()
    {
        if(Input.GetKeyDown(InputMap.Map["Drop Inventory Item"]))
        {
            AttemptDrop();
        }

        if(currentlyDraggedItem != null)
        {
            currentlyDraggedItem.transform.position = Input.mousePosition;
        }
    }

    public void Show(ActorData ofActor, object data = null)
    {
        IsOpen = true;
        this.gameObject.SetActive(true);
        currentActor = ofActor;


        isInspecting = ofActor != CORE.Instance.Room.PlayerActor;

        StatsPanel.SetActor(ofActor);
        StatsPanel.RefreshStats();
        InspectPanel.SetActor(ofActor);

        InventoryPanel.SetActive(!isInspecting);
        InspectPanel.gameObject.SetActive(isInspecting);

        RefreshUI(false);



        IsSelectedDropText.text = "<color=red>"+InputMap.Map["Drop Inventory Item"].ToString()+" - Drop</color>";
        IsSelectedUseText.text = "<color=yellow>" + InputMap.Map["Use Inventory Item"].ToString() + " - Use</color>";

        AudioControl.Instance.Play(ShowSound);

    }

    public void Hide()
    {
        IsOpen = false;
        this.gameObject.SetActive(false);

        AudioControl.Instance.Play(HideSound);
    }

    public void RefreshUI(bool restoreSelectionPlacement = true)
    {
        if (!IsOpen)
        {
            return;
        }

        if (!isInspecting)
        {
            MoneyLabel.text = currentActor.money.ToString();

            CORE.ClearContainer(ItemsContainer);

            for (int i = 0; i < currentActor.items.Count; i++)
            {
                InventorySlotUI slot = ResourcesLoader.Instance.GetRecycledObject("InventorySlotUI").GetComponent<InventorySlotUI>();
                slot.SetItem(currentActor.items[i], () => Select(slot));
                slot.transform.SetParent(ItemsContainer, false);
                slot.transform.localScale = Vector3.one;
                slot.transform.position = Vector3.zero;

            }
        }

        
        for (int i = 0; i < EquipSlots.Count; i++)
        {
            Item item = null;
            currentActor.equips.TryGetValue(EquipSlots[i].Type.name, out item); 
            InventorySlotUI slot = EquipSlots[i].Slot;
            
                slot.SetItem(item, () => {
                    if (!isInspecting)
                    {
                        Select(slot);
                    }
                }, EquipSlots[i].Type);
        }

        CORE.Instance.DelayedInvokation(0f, () => SelectionGroup.RefreshGroup(restoreSelectionPlacement));
        Deselect();
    }

    internal void DragItem(InventorySlotUI inventorySlotUI)
    {
        if (currentlyDraggedItem != null)
        {
            currentlyDraggedItem.gameObject.SetActive(false);
            currentlyDraggedItem = null;
        }
        if (inventorySlotUI.CurrentItem == null || inventorySlotUI.CurrentItem.Data == null)
        {
            return;
        }

        currentlyDraggedItem = ResourcesLoader.Instance.GetRecycledObject("InventoryDraggedItemUI").GetComponent<InventoryDraggedItemUI>();
        currentlyDraggedItem.transform.SetParent(transform);
        currentlyDraggedItem.SetInfo(inventorySlotUI.CurrentItem);

        AudioControl.Instance.Play(DragItemSound);
        //Select(inventorySlotUI);

    }

    internal void UndragItem(InventorySlotUI inventorySlotUI)
    { 
        if(currentlyDraggedItem != null)
        {
            //Select(inventorySlotUI);
            currentlyDraggedItem.gameObject.SetActive(false);
        }

        AudioControl.Instance.Play(UndragItemSound);

        //Deselect();
    }

    public void Select(InventorySlotUI slot)
    {
        AudioControl.Instance.Play(SelectSound);

        if (SelectedSlot != null)
        {
            if (SelectedSlot == slot && Time.time - SelectedTime > 1f)
            {
                // If the user selects the same slot but after a delay, instead of equip/unequip, just deselect it.
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
        }
        else if (slot.CurrentItem != null)
        {
            SelectedSlot = slot;
            SelectedTime = Time.time;

            if (!SelectedSlot.IsEquipmentSlot)
            {
                SelectedPanel.SetActive(true);
            }
            SelectedSlot.SetSelected();
            DragItem(SelectedSlot);
        }
    }

    public void UseSelected()
    {
        if(SelectedSlot == null || SelectedSlot.CurrentItem == null)
        {
            return;
        }

        if (!SelectedSlot.IsEquipmentSlot)
        {
            SocketHandler.Instance.SendEquippedItem(SelectedSlot.transform.GetSiblingIndex());
        }
        else
        {
            SocketHandler.Instance.SendUnequippedItem(SelectedSlot.name);
        }

        AudioControl.Instance.Play(UseSelectedSound);
    }

    public void Deselect()
    {
        SelectedPanel.SetActive(false);

        if (SelectedSlot != null)
        {
            UndragItem(SelectedSlot);

            SelectedSlot.Deselect();
            SelectedSlot = null;
        }

        AudioControl.Instance.Play(DeselectSound);
    }

    public void AttemptDrop()
    {
        if (SelectedSlot == null || SelectedSlot.CurrentItem == null)
        {
            return;
        }

        if (SelectedSlot.IsEquipmentSlot)
        {
            TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("First unequip the item and only then, you may drop it.", Color.red));
            return;
        }

        SocketHandler.Instance.SendDroppedItem(SelectedSlot.transform.GetSiblingIndex());

        Deselect();

        AudioControl.Instance.Play(DropSound);
        
    }

    public void AttemptDropMoney()
    {
        InputLabelWindow.Instance.Show("Throw Away Money", "Amount Of Money", (string setAmount) => 
        {
            int result = 0;
            if(int.TryParse(setAmount, out result))
            {
                if (result <= 0)
                {
                    TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Invalid amount of money...", Color.red));
                    return;
                }

                if (result > CORE.Instance.Room.PlayerActor.money)
                {
                    TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Not enough money at your disposal!", Color.red));
                    return;
                }

                JSONClass node = new JSONClass();

                node["money"].AsInt = result;
                
                SocketHandler.Instance.SendEvent("dropped_money",node);
                AudioControl.Instance.PlayInPosition("sound_coins", transform.position);
            }
            else
            {
                TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Invalid amount of money!", Color.red));
                return;
            }
        });
    }
}

[System.Serializable]
public class EquippableSlot
{
    public InventorySlotUI Slot;
    public ItemType Type;
}
