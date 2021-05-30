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

        GenerateStatItem("Power", CORE.Instance.Room.PlayerActor.attributes.Power);
        GenerateStatItem("HP", CORE.Instance.Room.PlayerActor.attributes.HP);
        GenerateStatItem("Defense", CORE.Instance.Room.PlayerActor.attributes.Defense);
        GenerateStatItem("Block", CORE.Instance.Room.PlayerActor.attributes.Block);
        GenerateStatItem("CDReduction", CORE.Instance.Room.PlayerActor.attributes.CDReduction);
        GenerateStatItem("CTReduction", CORE.Instance.Room.PlayerActor.attributes.CTReduction);
        GenerateStatItem("Lifesteal", CORE.Instance.Room.PlayerActor.attributes.Lifesteal);
        GenerateStatItem("LongRangeMultiplier", CORE.Instance.Room.PlayerActor.attributes.LongRangeMultiplier);
        GenerateStatItem("ShortRangeMultiplier", CORE.Instance.Room.PlayerActor.attributes.ShortRangeMultiplier);
        GenerateStatItem("WildMagicChance", CORE.Instance.Room.PlayerActor.attributes.WildMagicChance);
        GenerateStatItem("SpellDuration", CORE.Instance.Room.PlayerActor.attributes.SpellDuration);
        GenerateStatItem("AntiDebuff", CORE.Instance.Room.PlayerActor.attributes.AntiDebuff);
        GenerateStatItem("Threat", CORE.Instance.Room.PlayerActor.attributes.Threat);
        GenerateStatItem("MovementSpeed", CORE.Instance.Room.PlayerActor.attributes.MovementSpeed);
        GenerateStatItem("JumpHeight", CORE.Instance.Room.PlayerActor.attributes.JumpHeight);
        GenerateStatItem("DoubleCast", CORE.Instance.Room.PlayerActor.attributes.DoubleCast);
        GenerateStatItem("Explode", CORE.Instance.Room.PlayerActor.attributes.Explode);
        GenerateStatItem("HpRegen", CORE.Instance.Room.PlayerActor.attributes.HpRegen);


    }

    public void GenerateStatItem(string statName, float statValue)
    {
        if(statValue == 0f)
        {
            return;
        }

        StatItemUI item = ResourcesLoader.Instance.GetRecycledObject("StatItemUI").GetComponent<StatItemUI>();

        item.transform.SetParent(Container, false);
        item.transform.localScale = Vector3.one;
        item.transform.position = Vector3.zero;

        item.SetStat(statName, statValue);
    }
}
