using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LootRollItemUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI ItemTitleText;

    [SerializeField]
    InventorySlotUI Slot;

    [SerializeField]
    Image TimerFillImage;

    [SerializeField]
    CanvasGroup CG;

    [SerializeField]
    TextMeshProUGUI KeyOption1Label;

    [SerializeField]
    TextMeshProUGUI KeyOption2Label;

    [SerializeField]
    TextMeshProUGUI KeyOption3Label;

    [SerializeField]
    public GameObject LockedPanel;

    [SerializeField]
    TooltipTargetUI TooltipTarget;

    [SerializeField]
    Transform TooltipPosition;

    Coroutine TimerRoutineInstance;

    public Item CurrentItem;
    
    public bool IsTopActiveRoll
    {
        get
        {
            for (int i = 0; i < LootRollPanelUI.Instance.transform.childCount; i++)
            {
                if (LootRollPanelUI.Instance.transform.GetChild(i).gameObject.activeInHierarchy)
                {
                    if (LootRollPanelUI.Instance.transform.GetChild(i).gameObject == this.gameObject)
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }
    }

    public void SetInfo(Item item, float TimeLeft)
    {
        this.CurrentItem = item;

        Release();

        ItemTitleText.text = CurrentItem.itemName;
        Slot.SetItem(CurrentItem, null);
        CORE.Instance.DelayedInvokation(0.1f, () => {
            TooltipTarget.ShowOnPosition(TooltipPosition.position);
        });


        if(TimerRoutineInstance != null)
        {
            StopCoroutine(TimerRoutineInstance);
        }

        TimerRoutineInstance = StartCoroutine(TimerRoutine(TimeLeft));

        if (CORE.Instance.IsUsingJoystick)
        {
            KeyOption1Label.text = "X";
            KeyOption2Label.text = "Y";
            KeyOption3Label.text = "B";
        }
        else
        {
            KeyOption1Label.text = InputMap.Map["Vote Option 1"].ToString();
            KeyOption2Label.text = InputMap.Map["Vote Option 2"].ToString();
            KeyOption3Label.text = InputMap.Map["Vote Option 3"].ToString();
        }
    }

    IEnumerator TimerRoutine(float timeLeft)
    {
        timeLeft-=2;//Two second before the timer ends just in case

        float startTime = timeLeft;
        while(timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;

            TimerFillImage.fillAmount = timeLeft / startTime;

            yield return 0;
        }

        Decline();

        TimerRoutineInstance = null;
    }

    private void Update()
    {
        if(!CG.interactable)
        {
            return;
        }

        if(!IsTopActiveRoll)
        {
            return;
        }

        if (Input.GetKeyDown(InputMap.Map["Vote Option 1"]) || (CORE.Instance.IsUsingJoystick && Input.GetButtonDown("Joystick 3")))
        {
            if (CORE.Instance.IsUsingJoystick)
            {
                WarningWindowUI.Instance.Show("Vote to pick up the item?", () =>
                {
                        Need();
                });
            }
            else
                Need();
        }
        else if (Input.GetKeyDown(InputMap.Map["Vote Option 2"]) || (CORE.Instance.IsUsingJoystick && Input.GetButtonDown("Joystick 0")))
        {
            if (CORE.Instance.IsUsingJoystick)
            {
                WarningWindowUI.Instance.Show("Vote 'Greed' on the item?", () => {
                    Greed();
                });
            }
            else
                Greed();
        }

        else if (Input.GetKeyDown(InputMap.Map["Vote Option 3"]) || (CORE.Instance.IsUsingJoystick && Input.GetButtonDown("Joystick 1")))
        {
            if (CORE.Instance.IsUsingJoystick)
            {
                WarningWindowUI.Instance.Show("Skip the item?", () =>
                {
                        Decline();
                });
            }
            else
                Decline();

        }
    }

    public void Decline()
    {
        JSONClass node = new JSONClass();
        node["itemId"] = Slot.CurrentItem.itemId;
        node["choice"] = "skip";
        SocketHandler.Instance.SendEvent("rolled_item_choice",node);
        CG.interactable = false;
        LockedPanel.gameObject.SetActive(true);
    }

    public void Need()
    {
        JSONClass node = new JSONClass();
        node["itemId"] = Slot.CurrentItem.itemId;
        node["choice"] = "need";
        SocketHandler.Instance.SendEvent("rolled_item_choice", node);
        CG.interactable = false;
        LockedPanel.gameObject.SetActive(true);
    }

    public void Greed()
    {
        JSONClass node = new JSONClass();
        node["itemId"] = Slot.CurrentItem.itemId;
        node["choice"] = "greed";
        SocketHandler.Instance.SendEvent("rolled_item_choice", node);
        CG.interactable = false;
        LockedPanel.gameObject.SetActive(true);
    }

    public void Release()
    {
        CG.interactable = true;
        LockedPanel.gameObject.SetActive(false);
    }
}
