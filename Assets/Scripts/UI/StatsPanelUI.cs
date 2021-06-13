using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsPanelUI : MonoBehaviour
{
    public Transform Container;

    ActorData CurrentActor;

    private void Start()
    {
        CORE.Instance.SubscribeToEvent("StatsChanged", () =>
        {
            if (this.gameObject.activeInHierarchy)
                RefreshStats();
        });
    }

    public void SetActor()
    {
        SetActor(CORE.Instance.Room.PlayerActor);
    }

    public void SetActor(ActorData actor)
    {
        CurrentActor = actor;
    }

    public void RefreshStats()
    {
        if (CurrentActor == null)
        {
            if (CORE.Instance == null)
            {
                return;
            }

            CurrentActor = CORE.Instance.Room.PlayerActor;
        }

        CORE.ClearContainer(Container);

        GenerateStatItem("Name", CurrentActor.name);
        GenerateStatItem("Class", CurrentActor.ClassJobReference.name);
        GenerateStatItem("Level", CurrentActor.level.ToString());
        GenerateStatItem("EXP", CurrentActor.exp.ToString());
        GenerateStatItem("Power", CurrentActor.attributes.Power);
        GenerateStatItem("HP", CurrentActor.attributes.HP);
        GenerateStatItem("Defense", CurrentActor.attributes.Defense);
        GenerateStatItem("Block", CurrentActor.attributes.Block);
        GenerateStatItem("CDReduction", CurrentActor.attributes.CDReduction);
        GenerateStatItem("CTReduction", CurrentActor.attributes.CTReduction);
        GenerateStatItem("Lifesteal", CurrentActor.attributes.Lifesteal);
        GenerateStatItem("LongRangeMultiplier", CurrentActor.attributes.LongRangeMultiplier);
        GenerateStatItem("ShortRangeMultiplier", CurrentActor.attributes.ShortRangeMultiplier);
        GenerateStatItem("WildMagicChance", CurrentActor.attributes.WildMagicChance);
        GenerateStatItem("SpellDuration", CurrentActor.attributes.SpellDuration);
        GenerateStatItem("AntiDebuff", CurrentActor.attributes.AntiDebuff);
        GenerateStatItem("Threat", CurrentActor.attributes.Threat);
        GenerateStatItem("MovementSpeed", CurrentActor.attributes.MovementSpeed);
        GenerateStatItem("JumpHeight", CurrentActor.attributes.JumpHeight);
        GenerateStatItem("DoubleCast", CurrentActor.attributes.DoubleCast);
        GenerateStatItem("Explode", CurrentActor.attributes.Explode);
        GenerateStatItem("HpRegen", CurrentActor.attributes.HpRegen);


    }

    public void GenerateStatItem(string statName, string statValue)
    {
        StatItemUI item = ResourcesLoader.Instance.GetRecycledObject("StatItemUI").GetComponent<StatItemUI>();

        item.transform.SetParent(Container, false);
        item.transform.localScale = Vector3.one;
        item.transform.position = Vector3.zero;

        item.SetStat(statName, statValue);
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
