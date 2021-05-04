using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootRollPanelUI : MonoBehaviour
{
    public static LootRollPanelUI Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void AddLootRollItem(Item item, float duration)
    {
        LootRollItemUI lootRollItem = ResourcesLoader.Instance.GetRecycledObject("LootRollItem").GetComponent<LootRollItemUI>();

        lootRollItem.SetInfo(item, duration);
        lootRollItem.transform.SetParent(transform, false);
    }
}
