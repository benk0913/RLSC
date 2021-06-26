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
        FocusOnItem(0);
        IsFocusing = true;
    }

    public void FocusOnItem(int itemIndex)
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

        ItemData CurrentItem = ItemsEntities[ItemIndex].CurrentItem;

        CameraChaseEntity.Instance.FocusOn(new FocusInstance(ItemsEntities[ItemIndex].transform,5f));

        TooltipTargetUI tooltipTarget = ItemsEntities[itemIndex].GetComponent<TooltipTargetUI>();

        tooltipTarget.Text = CurrentItem.name;
        tooltipTarget.Text += System.Environment.NewLine + CurrentItem.Type.name;
        tooltipTarget.Text += System.Environment.NewLine + CurrentItem.VendorPrice+"c";
        //TooltipTarget.Text += System.Environment.NewLine + "<color=#"+ColorUtility.ToHtmlStringRGBA(CurrentItem.Data.Rarity.RarityColor)+">"+ CurrentItem.Data.Rarity.name+"</color>";
        string description = CurrentItem.Description.Trim();
        if (!string.IsNullOrEmpty(description))
        {
            tooltipTarget.Text += System.Environment.NewLine + description;
        }

        tooltipTarget.Text += ItemsLogic.GetTooltipTextFromItem(CurrentItem);

        tooltipTarget.ShowOnPosition(Camera.main.WorldToScreenPoint(ItemsEntities[ItemIndex].transform.position ) + new Vector3(-20f, 0f,0f));

        CORE.Instance.ShowVendorSelectionWindow(CurrentItem);
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
            if (Input.GetKeyDown(InputMap.Map["Move Right"]))
            {
                SetRightItem();
            }
            else if (Input.GetKeyDown(InputMap.Map["Move Left"]))
            {
                SetLeftItem();
            }
            else if (Input.GetKeyDown(InputMap.Map["Exit"]))
            {
                StopFocusing();
            }
            else if(Input.GetKeyDown(InputMap.Map["Interact"]) || Input.GetKeyDown(InputMap.Map["Confirm"]))
            {
                PurchaseItem(ItemIndex);
            }
        }
    }

    public void SetRightItem()
    {
        FocusOnItem(ItemIndex + 1);
    }

    public void SetLeftItem()
    {
        FocusOnItem(ItemIndex - 1);
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

        JSONNode node = new JSONClass();
        node["vendorId"] = VendorReference.ID;
        node["itemName"] = item.name;

        SocketHandler.Instance.SendEvent("purchased_item", node);

        AudioControl.Instance.PlayInPosition("sound_purchase", transform.position);

        StopFocusing();
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
