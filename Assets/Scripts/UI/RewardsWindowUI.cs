using System;
using System.Collections;
using System.Collections.Generic;
using EdgeworldBase;
using TMPro;
using UnityEngine;

public class RewardsWindowUI : MonoBehaviour
{
    public static RewardsWindowUI Instance;

    [SerializeField]
    TextMeshProUGUI XPGainedLabel;

    [SerializeField]
    TextMeshProUGUI MoneyGainedLabel;


    public Transform Container;


    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Hide(bool accepted = false)
    {
        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(InputMap.Map["Exit"]) || Input.GetButtonDown("Joystick 8")||  Input.GetButtonDown("Joystick 11"))
        {
            Hide();
        }
        else if (Input.GetKeyDown(InputMap.Map["Confirm"]) || Input.GetButtonDown("Joystick 0"))
        {
            Accept();
        }
    }

    public void Show(List<ItemData> itemRewards, int xp, int money)
    {
        this.gameObject.SetActive(true);

        XPGainedLabel.gameObject.SetActive(xp > 0);
        XPGainedLabel.text = CORE.QuickTranslate("+"+System.String.Format("{0:n0}", xp)+" EXP");

        MoneyGainedLabel.gameObject.SetActive(money > 0);
        MoneyGainedLabel.text = CORE.QuickTranslate("+"+System.String.Format("{0:n0}", money));

        AudioControl.Instance.Play("sound_reward2");

        CORE.ClearContainer(Container);

        foreach(ItemData itemData in itemRewards)
        {
            InventorySlotUI slot =  ResourcesLoader.Instance.GetRecycledObject("InventorySlotUIUninteractable").GetComponent<InventorySlotUI>();

            slot.transform.SetParent(Container,false);
            slot.transform.localScale =Vector3.one;
            slot.transform.position = Vector3.zero;
            slot.SetItem(itemData);
        }
    }

    public void Accept()
    {
        Hide(true);
    }

    public class WarningWindowData
    {
        public string Message;
        public Action AcceptCallback;
        public bool CantHide = false;

        public WarningWindowData(string msg, Action callback, bool canHide = false,Action skipCallback = null)
        {
            this.Message = msg;
            this.AcceptCallback = callback;
            this.CantHide = canHide;
        }
    }
}
