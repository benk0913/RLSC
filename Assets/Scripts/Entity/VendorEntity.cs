using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class VendorEntity : MonoBehaviour
{
    public Vendor VendorReference;

    public List<VendorWorldItem> ItemsEntities = new List<VendorWorldItem>();

    public UnityEvent OnRefresh;

    void Start()
    {
        CORE.Instance.SubscribeToEvent("VendorsUpdate"+VendorReference.ID, OnVendorsUpdate);
        OnVendorsUpdate();
    }

    public void OnVendorsUpdate()
    {
        if(!CORE.Instance.Room.Vendors.ContainsKey(VendorReference.ID))
        {
            this.gameObject.SetActive(false);
            return;
        }

        VendorReference.Items = CORE.Instance.Room.Vendors[VendorReference.ID];

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
    }
}
