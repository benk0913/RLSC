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

    Coroutine TimerRoutineInstance;

    public void SetInfo(Item item, float TimeLeft)
    {
        ItemTitleText.text = item.itemName;
        Slot.SetItem(item, null);


        if(TimerRoutineInstance != null)
        {
            StopCoroutine(TimerRoutineInstance);
        }

        TimerRoutineInstance = StartCoroutine(TimerRoutine(TimeLeft));
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

    public void Decline()
    {
        this.gameObject.SetActive(false);

        JSONClass node = new JSONClass();
        node["itemId"] = Slot.CurrentItem.itemId;
        SocketHandler.Instance.SendEvent("item_roll_decline",node);
    }

    public void Need()
    {
        if(SocketHandler.Instance.CurrentUser.actor.items.Count >= CORE.Instance.Data.content.MaxInventorySlots)
        {
            TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Inventory Is Full!", Color.red, 2f));
            return;
        }

        this.gameObject.SetActive(false);

        JSONClass node = new JSONClass();
        node["itemId"] = Slot.CurrentItem.itemId;
        SocketHandler.Instance.SendEvent("item_roll_need", node);
    }

    public void Greed()
    {
        if (SocketHandler.Instance.CurrentUser.actor.items.Count >= CORE.Instance.Data.content.MaxInventorySlots)
        {
            TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Inventory Is Full!", Color.red, 2f));
            return;
        }

        this.gameObject.SetActive(false);

        JSONClass node = new JSONClass();
        node["itemId"] = Slot.CurrentItem.itemId;
        SocketHandler.Instance.SendEvent("item_roll_greed", node);
    }
}
