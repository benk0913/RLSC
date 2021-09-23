using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CashShopProductUI : MonoBehaviour
{
    [SerializeField]
    Image IconImage;

    [SerializeField]
    TooltipTargetUI TooltipTarget;

    [SerializeField]
    TextMeshProUGUI NameLabel;

    [SerializeField]
    TextMeshProUGUI PriceLabel;


    public ItemData CurrentItem;

    [SerializeField]
    GameObject SelectedObject;
    
    public void SetInfo(ItemData Item)
    {
        this.CurrentItem = Item;

        RefreshUI();
    }

    void RefreshUI()
    {
        IconImage.sprite = CurrentItem.Icon;
        TooltipTarget.Text = ItemsLogic.GetItemTooltip(CurrentItem);
        NameLabel.text = CurrentItem.DisplayName;
        PriceLabel.text = String.Format("{0:n0}", CurrentItem.CashItemPrice);
    }

    public void AttemptSelect()
    {
        CashShopWindowUI.Instance.SelectProduct(this);
    }

    internal void SetSelected()
    {
        SelectedObject.SetActive(true);
    }

    internal void SetDeselected()
    {
        SelectedObject.SetActive(false);
    }
}
