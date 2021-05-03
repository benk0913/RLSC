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
        JSONClass node = new JSONClass();
        node["itemId"] = Slot.CurrentItem.itemId;
        node["choice"] = "skip";
        SocketHandler.Instance.SendEvent("rolled_item_choice",node);
    }

    public void Need()
    {
        JSONClass node = new JSONClass();
        node["itemId"] = Slot.CurrentItem.itemId;
        node["choice"] = "need";
        SocketHandler.Instance.SendEvent("rolled_item_choice", node);
    }

    public void Greed()
    {
        JSONClass node = new JSONClass();
        node["itemId"] = Slot.CurrentItem.itemId;
        node["choice"] = "greed";
        SocketHandler.Instance.SendEvent("rolled_item_choice", node);
    }
}
