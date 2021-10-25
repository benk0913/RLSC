using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VendorSelectionUI : MonoBehaviour, WindowInterface
{
    public static VendorSelectionUI Instance;

    [SerializeField]
    TextMeshProUGUI NameLabel;

    [SerializeField]
    TextMeshProUGUI PriceLabel;

    [SerializeField]
    TextMeshProUGUI KeyText;

    [SerializeField]
    TextMeshProUGUI ItemDescriptionText;

    [SerializeField]
    TextMeshProUGUI PlayerMoneyLabel;

    public bool IsActive;


    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Show(ActorData actorData, object data)
    {
        this.gameObject.SetActive(true);

        if(data != null)
            RefreshUI((ItemData)data);

        IsActive = true;
    }

    public void RefreshUI(ItemData item)
    {   
        KeyText.text = "Press '" + InputMap.Map["Interact"].ToString() + "' To Purchase!";

        string itemName = item.DisplayName;
        CORE.Instance.Data.Localizator.mSource.TryGetTranslation(itemName, out itemName);
        NameLabel.text = itemName;
        PriceLabel.text = item.VendorPrice.ToString("N0");
        PlayerMoneyLabel.text = CORE.Instance.Room.PlayerActor.money.ToString("N0");
        ItemDescriptionText.text = ItemsLogic.GetItemTooltip(item);
    }

    public void Left()
    {
        VendorEntity.CurrentInstance.SetLeftItem();
    }

    public void Right()
    {
        VendorEntity.CurrentInstance.SetRightItem();
    }

    public void Hide()
    {
        IsActive = false;
        this.gameObject.SetActive(false);
    }


}
