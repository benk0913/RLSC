using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsPanelUI : MonoBehaviour
{
    public Transform Container;

    private void Start()
    {
        CORE.Instance.SubscribeToEvent("StatsChanged", () =>
        {
            if (this.gameObject.activeInHierarchy)
                RefreshStats();
        });
    }

    private void OnEnable()
    {
        RefreshStats();
    }

    public void RefreshStats()
    {
        if (CORE.Instance == null)
        {
            return;
        }

        CORE.ClearContainer(Container);
        StatItemUI statItem;

        statItem = GenerateStatItem();
        statItem.SetStat("Power", CORE.Instance.Room.PlayerActor.attributes.Power);

        statItem = GenerateStatItem();
        statItem.SetStat("HP", CORE.Instance.Room.PlayerActor.attributes.HP);

        statItem = GenerateStatItem();
        statItem.SetStat("Defense", CORE.Instance.Room.PlayerActor.attributes.Defense);

        statItem = GenerateStatItem();
        statItem.SetStat("Block", CORE.Instance.Room.PlayerActor.attributes.Block);

        statItem = GenerateStatItem();
        statItem.SetStat("CDReduction", CORE.Instance.Room.PlayerActor.attributes.CDReduction);

        statItem = GenerateStatItem();
        statItem.SetStat("CTReduction", CORE.Instance.Room.PlayerActor.attributes.CTReduction);

        statItem = GenerateStatItem();
        statItem.SetStat("Lifesteal", CORE.Instance.Room.PlayerActor.attributes.Lifesteal);

        statItem = GenerateStatItem();
        statItem.SetStat("LongRangeMultiplier", CORE.Instance.Room.PlayerActor.attributes.LongRangeMultiplier);

        statItem = GenerateStatItem();
        statItem.SetStat("ShortRangeMultiplier", CORE.Instance.Room.PlayerActor.attributes.ShortRangeMultiplier);

        statItem = GenerateStatItem();
        statItem.SetStat("WildMagicChance", CORE.Instance.Room.PlayerActor.attributes.WildMagicChance);

        statItem = GenerateStatItem();
        statItem.SetStat("SpellDuration", CORE.Instance.Room.PlayerActor.attributes.SpellDuration);

        statItem = GenerateStatItem();
        statItem.SetStat("AntiDebuff", CORE.Instance.Room.PlayerActor.attributes.AntiDebuff);

        statItem = GenerateStatItem();
        statItem.SetStat("Threat", CORE.Instance.Room.PlayerActor.attributes.Threat);

        statItem = GenerateStatItem();
        statItem.SetStat("MovementSpeed", CORE.Instance.Room.PlayerActor.attributes.MovementSpeed);

        statItem = GenerateStatItem();
        statItem.SetStat("JumpHeight", CORE.Instance.Room.PlayerActor.attributes.JumpHeight);

        statItem = GenerateStatItem();
        statItem.SetStat("DoubleCast", CORE.Instance.Room.PlayerActor.attributes.DoubleCast);

        statItem = GenerateStatItem();
        statItem.SetStat("Explode", CORE.Instance.Room.PlayerActor.attributes.Explode);

        statItem = GenerateStatItem();
        statItem.SetStat("HpRegen", CORE.Instance.Room.PlayerActor.attributes.HpRegen);


    }

    public StatItemUI GenerateStatItem()
    {
        StatItemUI item = ResourcesLoader.Instance.GetRecycledObject("StatItemUI").GetComponent<StatItemUI>();

        item.transform.SetParent(Container, false);
        item.transform.localScale = Vector3.one;
        item.transform.position = Vector3.zero;

        return item;
    }
}
