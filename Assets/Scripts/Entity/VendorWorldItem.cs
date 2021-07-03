using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class VendorWorldItem : MonoBehaviour
{
    public ItemData CurrentItem;

    [SerializeField]
    Image ItemIcon;

    [SerializeField]
    Image RarityGradient;

    [SerializeField]
    TextMeshProUGUI NameLabel;

    [SerializeField]
    public Transform TooltipPosition;


    public void SetInfo(ItemData item)
    {
        CurrentItem = item;

        RefreshUI();
    }
    
    
    void RefreshUI()
    {
        NameLabel.text = CurrentItem.name;
        ItemIcon.sprite = CurrentItem.Icon;
        RarityGradient.color = CurrentItem.Rarity.RarityColor;
    }
}
