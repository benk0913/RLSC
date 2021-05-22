using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEV_ItemsView : MonoBehaviour
{
    public Transform Container;

    private void Start()
    {
#if UNITY_EDITOR
        CORE.Instance.ConditionalInvokation((object o) =>
        {
            return Input.GetKeyDown(KeyCode.PageUp);
        }, () =>
        {
            if (this.gameObject.activeInHierarchy)
            {
                this.gameObject.SetActive(false);
            }
            else
            {
                Show();
            }
        }, 1/60f,true);
#endif
        this.gameObject.SetActive(false);
    }


    public void Show()
    {
        this.gameObject.SetActive(true);

        StopAllCoroutines();

        CORE.ClearContainer(Container);

        StartCoroutine(ShowRoutine());
    }

    IEnumerator ShowRoutine()
    {
        foreach (ItemData itemData in CORE.Instance.Data.content.Items)
        {
            InventorySlotUI slot = ResourcesLoader.Instance.GetRecycledObject("InventorySlotUI").GetComponent<InventorySlotUI>();
            Item item = new Item();
            item.Data = itemData;
            slot.SetItem(item);
            slot.transform.SetParent(Container, false);
            slot.transform.localScale = Vector3.one;
            slot.transform.position = Vector3.one;

            yield return 0;
        }
    }
}
