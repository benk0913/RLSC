using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDraggedItemUI : MonoBehaviour
{
    Image ItemIcon;

    Item currentItem;

    public int InventoryIndex;

    public void SetInfo(Item item)
    {
        currentItem = item;

        ItemIcon.sprite = currentItem.Data.Icon;
    }

    private void Update()
    {
        transform.position = Input.mousePosition;
    }
}
