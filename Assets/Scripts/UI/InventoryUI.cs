using EdgeworldBase;
using SimpleJSON;
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
    Transform CashItemsContainer;

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
    TextMeshProUGUI CashPointsLabel;

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

    [SerializeField]
    public List<GameObject> InventoryTabsSelectedHalos;

    [SerializeField]
    public List<GameObject> InventoryPanelsGameObjects;

    [SerializeField]
    public List<ScrollRect> InventoryContainersScrolls;

    [SerializeField]
    public List<GameObject> EquipmentTabsSelectedHalos;
    
    [SerializeField]
    public List<GameObject> EquipmentPanelsGameObjects; 

    public List<EquippableSlotTab> EquipSlotsByTab = new List<EquippableSlotTab>();
    

    InventorySlotUI SelectedSlot;
    float SelectedTime;
    
    public bool IsOpen;

    bool isInspecting;
    public bool IsCashTab
    {
        get
        {
            return InventoryPanelsGameObjects[1].activeInHierarchy;
        }
    }
    public int CurrentEquipmentTab
    {
        get
        {
            
            for (int i = 0; i < EquipmentPanelsGameObjects.Count; i++)
            {
                if (EquipmentPanelsGameObjects[i].activeInHierarchy)
                {
                    return i;
                }
            }
            return 0;
        }
    }


    private void Awake()
    {
        Instance = this;
        Hide();
    }
    

    private void Update()
    {   
        if(Input.GetKeyDown(InputMap.Map["Drop Inventory Item"]) || (CORE.Instance.IsUsingJoystick && Input.GetButtonDown("Joystick 1")))
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
        CORE.Instance.SubscribeToEvent("InventoryUpdated", RefreshUI);

        IsOpen = true;
        this.gameObject.SetActive(true);
        currentActor = ofActor;

        isInspecting = ofActor != CORE.PlayerActor;

        if(isInspecting)
        {
            AudioControl.Instance.Play("partyWindowOpen");
        }

        StatsPanel.SetActor(ofActor);
        StatsPanel.RefreshStats();
        InspectPanel.SetActor(ofActor);

        InventoryPanel.SetActive(!isInspecting);
        InspectPanel.gameObject.SetActive(isInspecting);

        SetInventoryTab(0);
        SetEquipmentTab(0);

        RefreshUI(false);

        string translatedDrop = "- Drop";
        CORE.Instance.Data.Localizator.mSource.TryGetTranslationCodywise(translatedDrop, out translatedDrop);

        string translatedUse = "- Use";
        CORE.Instance.Data.Localizator.mSource.TryGetTranslationCodywise(translatedUse, out translatedUse);


        if (CORE.Instance.IsUsingJoystick)
        {
           
            IsSelectedDropText.text = "<color=" + Colors.COLOR_BAD + ">" + "B "+translatedDrop+"</color>";
            IsSelectedUseText.text = "<color=" + Colors.COLOR_HIGHLIGHT + ">" + "A "+ translatedUse +"</ color>";
        }
        else
        {
            IsSelectedDropText.text = "<color=" + Colors.COLOR_BAD + ">" + InputMap.Map["Drop Inventory Item"].ToString() + translatedDrop +"</color>";
            IsSelectedUseText.text = "<color=" + Colors.COLOR_HIGHLIGHT + ">" + InputMap.Map["Use Inventory Item"].ToString() +translatedUse+"</color>";
        }

        AudioControl.Instance.Play(ShowSound);

    }

    public void Hide()
    {
        IsOpen = false;
        this.gameObject.SetActive(false);

        if(CORE.Instance != null)
            CORE.Instance.UnsubscribeFromEvent("InventoryUpdated", RefreshUI);

        AudioControl.Instance.Play(HideSound);
    }

    void OnDisable()
    {
        if(InspectPanel.IsFocusingOnActor)
        {
            InspectPanel.SetActor(null);
        }
    }

    public void RefreshUI()
    {
        RefreshUI(true);
    }

    public void RefreshUI(bool restoreSelectionPlacement = true)
    {
        if (!IsOpen)
        {
            return;
        }

        if (!isInspecting)
        {
            
            if(IsCashTab)
            {
                CashPointsLabel.text =  System.String.Format("{0:n0}", SocketHandler.Instance.CurrentUser.info.cashPoints);

                CORE.ClearContainer(CashItemsContainer);

                for (int i = 0; i < SocketHandler.Instance.CurrentUser.info.cashItems.Count; i++)
                {
                    InventorySlotUI slot = ResourcesLoader.Instance.GetRecycledObject("InventorySlotUI").GetComponent<InventorySlotUI>();

                    slot.SetItem(SocketHandler.Instance.CurrentUser.info.cashItems[i], () => Select(slot));
                    slot.transform.SetParent(CashItemsContainer, false);
                    slot.transform.localScale = Vector3.one;
                    slot.transform.position = Vector3.zero;

                }
            }
            else
            {
                MoneyLabel.text = System.String.Format("{0:n0}", currentActor.money);

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
        }

        List<EquippableSlot> EquipSlots = EquipSlotsByTab[CurrentEquipmentTab].Slots;
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

        CORE.Instance.DelayedInvokation(0.5f, () => SelectionGroup.RefreshGroup(restoreSelectionPlacement));
        Deselect();
    }

    public void SetInventoryTab(int index)
    {
        SetTab(index, InventoryPanelsGameObjects, InventoryTabsSelectedHalos, InventoryContainersScrolls);
    }
    public void SetInventoryTabWithRefresh(int index)
    {
        SetInventoryTab(index);
        RefreshUI();
    }

    public void SetEquipmentTab(int index)
    {
        SetTab(index, EquipmentPanelsGameObjects, EquipmentTabsSelectedHalos);
    }
    public void SetEquipmentTabWithRefresh(int index)
    {
        SetEquipmentTab(index);
        RefreshUI();
    }

    public void SetTab(int index, List<GameObject> GameObjects, List<GameObject> SelectedHalos, List<ScrollRect> ContainersScrolls = null)
    {
        for (int i = 0; i < GameObjects.Count; i++)
        {
            bool active = i == index;
            GameObjects[i].SetActive(active);
            SelectedHalos[i].SetActive(active);
        }

        if (ContainersScrolls != null)
        {
            CORE.Instance.DelayedInvokation(0f,()=>{
                ContainersScrolls[index].verticalNormalizedPosition = 1f;
            });
        }
    }

    public int GetTabOfEquip(Item item)
    {
        if (item == null || item.Data == null)
        {
            return 0;
        }

        ItemType ExampleSlotType = item.Data.Type;

        // EquipSlotsByTab has slot names (e.g. 'emote 1') and item type is the actual type (e.g. 'emote'),
        // so in order to find the tab index, we get any slot name with the item type. 
        foreach (var typeOverride in CORE.Instance.Data.content.EquipSlotOverrides)
        {
            if (typeOverride.overrideType.name == item.Data.Type.name)
            {
                ExampleSlotType = typeOverride.itemType;
            }
        }

        for (int i = 0; i < EquipSlotsByTab.Count; i++)
        {
            List<EquippableSlot> EquipSlots = EquipSlotsByTab[i].Slots;
            for (int j = 0; j < EquipSlots.Count; j++)
            {
                if (EquipSlots[j].Type.name == ExampleSlotType.name)
                {
                    return i;
                }
            }
        }
        return 0;
    }

    public void EquippedItem(Item item)
    {
        if (item != null && item.Data != null)
        {
            int tabIndex = GetTabOfEquip(item);
            SetEquipmentTab(tabIndex);
        }
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


    public void Select(InventorySlotUI slot) //TODO Bad imp, refactor in future
    {
        AudioControl.Instance.Play(SelectSound);

        if (SelectedSlot != null)
        {
            if(SelectedSlot.IsTradeSlot)
            {
                if(slot.IsTradeSlot) //From trade slot to trade slot 
                {
                    if(slot == SelectedSlot)// Doubleclick
                    {
                        TradeWindowUI.Instance.SetItem(null,SelectedSlot);    
                    }
                    return;
                }
                if(slot.IsEquipmentSlot) //From trade slot to other slot 
                {
                    TradeWindowUI.Instance.SetItem(null,SelectedSlot);
                }
                else //From trade slot to other slot 
                {
                    TradeWindowUI.Instance.SetItem(null,SelectedSlot);
                }
            }
            else if(slot.IsTradeSlot)
            {
                if(SelectedSlot.IsTradeSlot) //From trade slot to trade slot 
                {
                    if(slot == SelectedSlot) // Doubleclick
                    {
                        TradeWindowUI.Instance.SetItem(null,slot);    
                    }
                    return;
                }
                if(SelectedSlot.IsEquipmentSlot) //From equip slot to trade slot 
                {
                    return;
                }
                else //From inventory slot to trade slot 
                {
                    TradeWindowUI.Instance.SetItem(SelectedSlot.CurrentItem,slot);
                }
            }
            else
            {

                if (SelectedSlot == slot && Time.time - SelectedTime > 1f)
                {
                    // If the user selects the same slot but after a delay, instead of equip/unequip, just deselect it.
                }
                else if (!SelectedSlot.IsEquipmentSlot && SelectedSlot == slot) //Doubleclick equip
                {
                    SocketHandler.Instance.SendUsedItem(SelectedSlot.transform.GetSiblingIndex(), IsCashTab);
                }
                else if (SelectedSlot.IsEquipmentSlot && SelectedSlot == slot) //Doubleclick unequip
                {
                    SocketHandler.Instance.SendUnequippedItem(SelectedSlot.SlotType.name);
                }
                else if (SelectedSlot.IsEquipmentSlot & slot.IsEquipmentSlot) //Reposition equipment
                {
                    SocketHandler.Instance.SendSwappedEquipAndEquipSlots(slot.SlotType.name, SelectedSlot.SlotType.name);
                }
                else if (SelectedSlot.IsEquipmentSlot )//swapped between equo and inventory 
                {
                    SocketHandler.Instance.SendSwappedItemAndEquipSlots(slot.transform.GetSiblingIndex(), SelectedSlot.SlotType.name, IsCashTab);
                }
                else if (slot.IsEquipmentSlot) //swapped between inventory and equip 
                {
                    SocketHandler.Instance.SendSwappedItemAndEquipSlots(SelectedSlot.transform.GetSiblingIndex(), slot.SlotType.name, IsCashTab);
                }
                else //Swap in inventory
                {
                    SocketHandler.Instance.SendSwappedItemSlots(SelectedSlot.transform.GetSiblingIndex(), slot.transform.GetSiblingIndex(), IsCashTab);
                }
            }

            Deselect();
        }
        else if (slot.CurrentItem != null)
        {
            SelectedSlot = slot;
            SelectedTime = Time.time;

            if (!SelectedSlot.IsEquipmentSlot && !SelectedSlot.IsTradeSlot)
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

        if(SelectedSlot.IsTradeSlot)
        {
            TradeWindowUI.Instance.SetItem(null, SelectedSlot);    
        }
        else if(TradeWindowUI.Instance.gameObject.activeInHierarchy)
        {
            TradeWindowUI.Instance.AddItem(SelectedSlot.CurrentItem);
        }
        else
        {
            if (!SelectedSlot.IsEquipmentSlot)
            {
                SocketHandler.Instance.SendUsedItem(SelectedSlot.transform.GetSiblingIndex(), IsCashTab);
            }
            else
            {
                SocketHandler.Instance.SendUnequippedItem(SelectedSlot.SlotType.name);
            }
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

    Coroutine dropRoutine;
    public void AttemptDrop()
    {
        if (dropRoutine != null)
        {
            return;
        }

        if (SelectedSlot == null || SelectedSlot.CurrentItem == null)
        {
            return;
        }


        if (SelectedSlot.IsTradeSlot)
        {
            TradeWindowUI.Instance.SetItem(null,SelectedSlot);
            return;
        }
        
        if (IsCashTab)
        {
            TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Cash items cannot be dropped.", Colors.AsColor(Colors.COLOR_BAD)));
            return;
        }

        if (SelectedSlot.IsEquipmentSlot)
        {
            SocketHandler.Instance.SendDroppedEquip(SelectedSlot.SlotType.name);
        }
        else
        {
            if(SelectedSlot.CurrentItem.amount > 1)
            {
                InventorySlotUI slot = SelectedSlot;
                InputLabelWindow.Instance.Show(CORE.QuickTranslate("How many items should you drop")+"?","Set Amount",(string finalValue)=>
                {
                    int finalValueInt = 0;
                    if(int.TryParse(finalValue, out finalValueInt))
                    {
                        SocketHandler.Instance.SendDroppedItem(slot.transform.GetSiblingIndex(),finalValueInt);
                    }
                    else
                    {
                        WarningWindowUI.Instance.Show(CORE.QuickTranslate("Wrong Amount")+"!",()=>{},false,null);
                    }
                },
                slot.CurrentItem.amount);
            }
            else
            {
                SocketHandler.Instance.SendDroppedItem(SelectedSlot.transform.GetSiblingIndex());
            }
        }

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
                    TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Invalid amount of money...", Colors.AsColor(Colors.COLOR_BAD)));
                    return;
                }

                if (result > CORE.Instance.Room.PlayerActor.money)
                {
                    TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Not enough money at your disposal!", Colors.AsColor(Colors.COLOR_BAD)));
                    return;
                }

                JSONClass node = new JSONClass();

                node["money"].AsInt = result;
                
                SocketHandler.Instance.SendEvent("dropped_money",node);
                AudioControl.Instance.PlayInPosition("sound_coins", transform.position);
            }
            else
            {
                TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Invalid amount of money!", Colors.AsColor(Colors.COLOR_BAD)));
                return;
            }
        });
    }

    public void RefreshInventorySlotIndex(int index)
    {
        if(!this.gameObject.activeInHierarchy)
        {
            return;
        }
        InventorySlotUI slot = ItemsContainer.GetChild(index).GetComponent<InventorySlotUI>();
        slot.SetItem(currentActor.items[index], () => Select(slot));
    }

    public void RefreshCashShopSlotIndex(int index)
    {
        if(!this.gameObject.activeInHierarchy)
        {
            return;
        }
        InventorySlotUI slot = CashItemsContainer.GetChild(index).GetComponent<InventorySlotUI>();
        slot.SetItem(SocketHandler.Instance.CurrentUser.info.cashItems[index], () => Select(slot));
    }

    // public void RefreshEquipmentSlot(string equipType)
    // {
    //     if(!this.gameObject.activeInHierarchy)
    //     {
    //         return;
    //     }

    //     List<EquippableSlot> EquipSlots = EquipSlotsByTab[CurrentEquipmentTab].Slots;
    //     for (int i = 0; i < EquipSlots.Count; i++)
    //     {
    //         Item item = null;
    //         currentActor.equips.TryGetValue(EquipSlots[i].Type.name, out item); 
    //         InventorySlotUI slot = EquipSlots[i].Slot;
            
    //         slot.SetItem(item, () => {
    //             if (!isInspecting)
    //             {
    //                 Select(slot);
    //             }
    //         }, EquipSlots[i].Type);
    //     }
    // }
}

[System.Serializable]
public class EquippableSlot
{
    public InventorySlotUI Slot;
    public ItemType Type;
}

[System.Serializable]
public class EquippableSlotTab
{
    public List<EquippableSlot> Slots;
}
