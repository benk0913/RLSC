using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Item CurrentItem;

    [SerializeField]
    Image IconImage;

    [SerializeField]
    TooltipTargetUI TooltipTarget;
    

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

        InventoryUI.Instance.DropItem(this);
    }

    public void SetItem(Item item)
    {
        CurrentItem = item;

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
}
