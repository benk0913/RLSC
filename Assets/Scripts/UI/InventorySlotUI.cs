using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using TMPro;

public class InventorySlotUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Item CurrentItem;

    [SerializeField]
    Image IconImage;

    [SerializeField]
    TooltipTargetUI TooltipTarget;


    [SerializeField]
    GameObject SelectedFrame;

    [SerializeField]
    TextMeshProUGUI AmountLabel;
    Action OnSelect;

    public bool IsEquipmentSlot;

    public bool IsTradeSlot;

    public ItemType SlotType = null;

    public bool IsInspecting = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        if(IsInspecting)
        {
            return;
        }

        if(eventData.pointerId != 0)
        {
            return;
        }

        InventoryUI.Instance.DragItem(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (IsInspecting)
        {
            return;
        }

        if (eventData.pointerId != 0)
        {
            return;
        }

        InventoryUI.Instance.UndragItem(this);
    }

    public void SetItem(Item item, Action onSelect = null, ItemType slotType = null, bool isInspecting = false)
    {
        
        CurrentItem = item;

        IsInspecting = isInspecting;

        this.SlotType = slotType;

        this.OnSelect = onSelect;

        RefreshUI();
    }

    void RefreshUI()
    {
        if(CurrentItem == null || string.IsNullOrEmpty(CurrentItem.itemId))
        {
            IconImage.enabled = false;

            if (IsEquipmentSlot)
            {
                TooltipTarget.Text = "Empty Equipment Slot ("+SlotType.name+")";
            }
            else if(IsTradeSlot)
            {
                TooltipTarget.Text = "Empty Trade Slot";
            }
            else
            {
                TooltipTarget.Text = "Empty Inventory Space";
            }
            return;
        }

        if(CurrentItem.Data == null)
        {
            IconImage.enabled = false;
            TooltipTarget.Text = "-ITEM REMOVED- (Sorry...)";
            return;
        }
        
        IconImage.enabled = true;
        IconImage.sprite = CurrentItem.Data.Icon;

        TooltipTarget.Text = ItemsLogic.GetItemTooltip(CurrentItem.Data);

        if(CurrentItem.amount > 1)
        {
            AmountLabel.text = "x"+CurrentItem.amount;
        }
        else
        {
            AmountLabel.text = "";
        }
        
        Deselect();
    }

    public void Select()
    {
        OnSelect?.Invoke();
    }

    public void SetSelected()
    {
        SelectedFrame.SetActive(true);
    }

    public void Deselect()
    {
        SelectedFrame.SetActive(false);
    }
}
