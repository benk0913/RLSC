using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    Item currentItem;

    [SerializeField]
    Image IconImage;

    [SerializeField]
    TooltipTargetUI TooltipTarget;

    public void SetItem(Item item)
    {
        currentItem = item;

        RefreshUI();
    }

    void RefreshUI()
    {
        if(currentItem == null || currentItem.Data == null)
        {
            IconImage.enabled = false;
            TooltipTarget.Text = "Empty Inventory Space";
            return;
        }
        
        IconImage.enabled = true;
        IconImage.sprite = currentItem.Data.Icon;

        TooltipTarget.Text = currentItem.Data.name;
        TooltipTarget.Text += System.Environment.NewLine + currentItem.Data.Type.name;
        TooltipTarget.Text += System.Environment.NewLine + "<color=#"+ColorUtility.ToHtmlStringRGBA(currentItem.Data.Rarity.RarityColor)+">"+ currentItem.Data.Rarity.name+"</color>";
        TooltipTarget.Text += System.Environment.NewLine + currentItem.Data.Description;
    }
}
