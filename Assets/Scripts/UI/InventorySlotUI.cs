using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class InventorySlotUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Item CurrentItem;

    [SerializeField]
    Image IconImage;

    [SerializeField]
    TooltipTargetUI TooltipTarget;

    [SerializeField]
    GameObject SelectedFrame;

    Action OnSelect;

    public bool IsEquipmentSlot;

    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.pointerId != 0)
        {
            return;
        }

        InventoryUI.Instance.DragItem(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != 0)
        {
            return;
        }

        InventoryUI.Instance.UndragItem(this);
    }

    public void SetItem(Item item, Action onSelect = null)
    {
        CurrentItem = item;

        this.OnSelect = onSelect;

        RefreshUI();
    }

    void RefreshUI()
    {
        if(CurrentItem == null)
        {
            IconImage.enabled = false;
            TooltipTarget.Text = "Empty Inventory Space";
            return;
        }
        
        IconImage.enabled = true;
        IconImage.sprite = CurrentItem.Data.Icon;

        TooltipTarget.Text = CurrentItem.Data.name;
        TooltipTarget.Text += System.Environment.NewLine + CurrentItem.Data.Type.name;
        TooltipTarget.Text += System.Environment.NewLine + "<color=#"+ColorUtility.ToHtmlStringRGBA(CurrentItem.Data.Rarity.RarityColor)+">"+ CurrentItem.Data.Rarity.name+"</color>";
        TooltipTarget.Text += System.Environment.NewLine + CurrentItem.Data.Description;
    }

    public void Select()
    {
        SelectedFrame.SetActive(true);
        OnSelect?.Invoke();
    }

    public void Deselect()
    {
        SelectedFrame.SetActive(false);
    }
}
