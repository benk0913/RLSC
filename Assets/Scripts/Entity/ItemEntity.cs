using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEntity : MonoBehaviour
{
    Item CurrentItem;

    public void SetInfo(Item item)
    {
        CurrentItem = item;

        RefreshUI();
    }

    void RefreshUI()
    {

    }
}
