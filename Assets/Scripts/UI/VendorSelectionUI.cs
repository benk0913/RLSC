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

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Show(ActorData actorData, object data)
    {
        Show((ItemData)data, actorData);
    }

    public void Show(ItemData item, ActorData actor = null)
    {
        this.gameObject.SetActive(true);

        KeyText.text = "Press '" + InputMap.Map["Interact"].ToString() + "' To Purchase!";
        NameLabel.text = item.name;
        PriceLabel.text = item.VendorPrice.ToString();
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
        this.gameObject.SetActive(false);
    }


}
