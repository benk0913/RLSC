using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    ItemData currentItem;

    [SerializeField]
    Image IconImage;

    [SerializeField]
    TooltipTargetUI TooltipTarget;

    public void SetItem(ItemData item)
    {
        currentItem = item;

        RefreshUI();
    }

    void RefreshUI()
    {
        if(currentItem == null)
        {
            IconImage.enabled = false;
            TooltipTarget.Text = "Empty Inventory Space";
            return;
        }
        
        IconImage.enabled = true;
        IconImage.sprite = currentItem.Icon;

        TooltipTarget.Text = currentItem.name;
        TooltipTarget.Text += System.Environment.NewLine + currentItem.Type.name;
        TooltipTarget.Text += System.Environment.NewLine + "<color=#"+ColorUtility.ToHtmlStringRGBA(currentItem.Rarity.RarityColor)+">"+ currentItem.Rarity.name+"</color>";
        TooltipTarget.Text += System.Environment.NewLine + currentItem.Description;
    }
}
