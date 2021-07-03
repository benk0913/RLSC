using EdgeworldBase;
using SimpleJSON;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class VendorEntity : MonoBehaviour
{
    public static VendorEntity CurrentInstance;

    public VendorData VendorReference;

    public List<VendorWorldItem> ItemsEntities = new List<VendorWorldItem>();

    public UnityEvent OnRefresh;

    public int ItemIndex = 0;

    public bool IsFocusing = false;


    private void OnDisable()
    {
        StopFocusing();
    }

    public void StartFocusing()
    {
        CurrentInstance = this;
        IsFocusing = true;
        FocusOnItem(0, true);
    }

    public void FocusOnItem(int itemIndex, bool initialFocus)
    {

        if(itemIndex < 0)
        {
            itemIndex = ItemsEntities.Count - 1;
        }
        else if(itemIndex >= ItemsEntities.Count)
        {
            itemIndex = 0;
        }

        ItemIndex = itemIndex;

        ItemData CurrentItemData = ItemsEntities[ItemIndex].CurrentItem;

        CameraChaseEntity.Instance.FocusOn(new FocusInstance(ItemsEntities[ItemIndex].transform,5f));

        TooltipTargetUI tooltipTarget = ItemsEntities[itemIndex].GetComponent<TooltipTargetUI>();

        tooltipTarget.Text = ItemsLogic.GetItemTooltip(CurrentItemData);

        // The camera is being moved, so wait for it to snap to position before showing the tooltip.
        CORE.Instance.DelayedInvokation(initialFocus ? 0.5f : 0.25f, () => {
            if (IsFocusing && ItemIndex == itemIndex)
            {
                tooltipTarget.ShowOnPosition(Camera.main.WorldToScreenPoint(ItemsEntities[ItemIndex].TooltipPosition.position), 0f, 0.5f);
            }
        });

        if(!VendorSelectionUI.Instance.IsActive)
        {
            CORE.Instance.ShowVendorSelectionWindow(CurrentItemData);
        }
        else
        {
            VendorSelectionUI.Instance.RefreshUI(CurrentItemData);
        }
    }

    public void StopFocusing()
    {
        CameraChaseEntity.Instance.Unfocus();
        PointAndClickTooltipUI.Instance.Hide();
        IsFocusing = false;
        
        if(CurrentInstance == this)
            CurrentInstance = null;

        CORE.Instance.CloseCurrentWindow();
    }

    void Start()
    {
        CORE.Instance.SubscribeToEvent("VendorsUpdate"+VendorReference.ID, OnVendorsUpdate);
        OnVendorsUpdate();
    }



    public bool Test;


    void Update()
    {
        if (Test)
        {
            ItemsEntities[0].SetInfo(CORE.Instance.Data.content.Items[0]);
            ItemsEntities[1].SetInfo(CORE.Instance.Data.content.Items[1]);
            ItemsEntities[2].SetInfo(CORE.Instance.Data.content.Items[2]);
            StartFocusing();
            Test = false;
        }

        if (IsFocusing)
        {
            if(!WarningWindowUI.Instance.isActiveAndEnabled)
            {
                if (Input.GetKeyDown(InputMap.Map["Move Right"]))
                {
                    SetRightItem();
                }
                else if (Input.GetKeyDown(InputMap.Map["Move Left"]))
                {
                    SetLeftItem();
                }
                else if(Input.GetKeyDown(InputMap.Map["Interact"]) || Input.GetKeyDown(InputMap.Map["Confirm"]))
                {  
                    PurchaseItem(ItemIndex);
                }
            }

            if (Input.GetKeyDown(InputMap.Map["Exit"]))
            {
                StopFocusing();
            }
        }
    }

    public void SetRightItem()
    {
        FocusOnItem(ItemIndex + 1, false);
    }

    public void SetLeftItem()
    {
        FocusOnItem(ItemIndex - 1, false);
    }

    public void PurchaseItem(int itemIndex)
    {
        ItemData item = ItemsEntities[itemIndex].CurrentItem;

        if(item.VendorPrice > CORE.Instance.Room.PlayerActor.money)
        {
            TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("You do not have enugh money! (" + CORE.Instance.Room.PlayerActor.money + "/" + item.VendorPrice + ")",Color.red));
            AudioControl.Instance.PlayInPosition("sound_coins", transform.position);
            return;
        }

        WarningWindowUI.Instance.Show("Purchase "+item.name+" for "+item.VendorPrice+" coins?",()=>
        {
            JSONNode node = new JSONClass();
            node["vendorId"] = VendorReference.ID;
            node["itemName"] = item.name;

            SocketHandler.Instance.SendEvent("purchased_item", node);

            AudioControl.Instance.PlayInPosition("sound_purchase", transform.position);

            StopFocusing();
        });

    }

    public void OnVendorsUpdate()
    {
        if(!CORE.Instance.Room.Vendors.ContainsKey(VendorReference.ID))
        {
            this.gameObject.SetActive(false);
            return;
        }

        List<ItemData> Items = new List<ItemData>();
        foreach (var item in CORE.Instance.Room.Vendors[VendorReference.ID])
        {
            if (item.Data != null)
            {
                Items.Add(item.Data);
            }
        }

        VendorReference.Items = Items;

        this.gameObject.SetActive(true);

        for(int i=0;i<ItemsEntities.Count;i++)
        {
            if(i>= VendorReference.Items.Count)
            {
                break;
            }

            ItemsEntities[i].SetInfo(VendorReference.Items[i]);
        }

        OnRefresh?.Invoke();

        AudioControl.Instance.PlayInPosition("RabbitEscape", transform.position);
    }
}
