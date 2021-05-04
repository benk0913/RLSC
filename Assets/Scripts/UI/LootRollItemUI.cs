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

    Coroutine TimerRoutineInstance;

    public Item CurrentItem;

    public void SetInfo(Item item, float TimeLeft)
    {
        this.CurrentItem = item;

        Release();

        ItemTitleText.text = CurrentItem.itemName;
        Slot.SetItem(CurrentItem, null);


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
        CG.interactable = false;
    }

    public void Need()
    {
        JSONClass node = new JSONClass();
        node["itemId"] = Slot.CurrentItem.itemId;
        node["choice"] = "need";
        SocketHandler.Instance.SendEvent("rolled_item_choice", node);
        CG.interactable = false;
    }

    public void Greed()
    {
        JSONClass node = new JSONClass();
        node["itemId"] = Slot.CurrentItem.itemId;
        node["choice"] = "greed";
        SocketHandler.Instance.SendEvent("rolled_item_choice", node);
        CG.interactable = false;
    }

    public void Release()
    {
        CG.interactable = true;
    }
}
