using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using TMPro;
using UnityEngine;

public class BankPanelUI : MonoBehaviour
{
    public static BankPanelUI Instance;

    public Transform Container;

    public TextMeshProUGUI MoneyLabel;


    void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void Show()
    {
        InventoryUI.Instance.Show(CORE.PlayerActor);
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void TransferMoney()
    {
        InputLabelWindow.Instance.Show("Transfer Money",default,(string setValue)=>
        {

            int intValue = 0;

            if(!int.TryParse(setValue, out intValue))
            {
                WarningWindowUI.Instance.Show("INVALID AMOUNT",null);
                return;
            }

            JSONClass node = new JSONClass();
            node["money"].AsInt = intValue;
            SocketHandler.Instance.SendEvent("bank_transfer_money",node);
        });
    }

    
}
