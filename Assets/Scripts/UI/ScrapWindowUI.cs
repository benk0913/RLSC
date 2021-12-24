using System;
using System.Collections;
using System.Collections.Generic;
using EdgeworldBase;
using SimpleJSON;
using TMPro;
using UnityEngine;

public class ScrapWindowUI : MonoBehaviour
{
    public static ScrapWindowUI Instance;

    public InventorySlotUI ScrapSlot;

    [SerializeField]
    SelectionGroupUI SG;

    [SerializeField]
    TextMeshProUGUI PriceLabel;

    Item CurrentItem;

    int SlotIndex;

    public GameObject ClearButton;

    void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void Show()
    {
        CORE.Instance.ShowInventoryUiWindow();
        
        this.gameObject.SetActive(true);

        SetItem(null);
    }

    public void SetItem(Item item = null, int slotIndex = 0)
    {
        SlotIndex = slotIndex;
        CurrentItem = item;
        RefreshUI();
    }

    public void RefreshUI()
    {
        if(!this.gameObject.activeInHierarchy)
        {
            return;
        }

        PriceLabel.text =  System.String.Format("{0:n0}", CORE.Instance.Data.content.ScrapCost)+"c";
        ScrapSlot.SetItem(CurrentItem, ()=>{InventoryUI.Instance.Select(ScrapSlot);});

        ClearButton.SetActive(CurrentItem != null);

        CORE.Instance.DelayedInvokation(0.1f,()=>SG.RefreshGroup());
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void Scrap()
    {
        if(CurrentItem == null)
        {
            return;
        }

        if(CORE.PlayerActor.money < CORE.Instance.Data.content.ScrapCost)
        {
            WarningWindowUI.Instance.Show(CORE.QuickTranslate("Not enough money")+"! ("+CORE.PlayerActor.money+"/"+CORE.Instance.Data.content.ScrapCost+")",null);
            return;
        }

         WarningWindowUI.Instance.Show("Are you Sure?",()=>
         {
            JSONClass node = new JSONClass();
            node["slotIndex"].AsInt = SlotIndex;
            SocketHandler.Instance.SendEvent("scrap_item",node);
            CurrentItem = null;
            RefreshUI();
         });
    }

    public void Clear()
    {
        if(CurrentItem == null)
        {
            return;
        }

        CurrentItem = null;
        RefreshUI();
    }
}
