using EdgeworldBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsPanelUI : MonoBehaviour
{
    public Transform Container;

    ActorData CurrentActor;

    public List<string> AlwaysDisplayStats = new List<string>();

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

        foreach (KeyValuePair<string, ItemsLogic.DisplayAttribute> keyValuePair in ItemsLogic.DisplayAttributes)
        {
            float propertyValue = (float)keyValuePair.Value.FieldInfo.GetValue(CurrentActor.attributes);
            
            GenerateStatItem(keyValuePair.Key, keyValuePair.Value.Name, propertyValue);
        }
    }

    public void GenerateStatItem(string statName, string statValue)
    {
        StatItemUI item = ResourcesLoader.Instance.GetRecycledObject("StatItemUI").GetComponent<StatItemUI>();

        item.transform.SetParent(Container, false);
        item.transform.localScale = Vector3.one;
        item.transform.position = Vector3.zero;

        item.SetStat(statName, statValue);
    }

    public void GenerateStatItem(string statName, string displayName, float statValue)
    {
        if(statValue == 0f)
        {
            if(!AlwaysDisplayStats.Contains(statName))
            {
                return;
            }
        }

        StatItemUI item = ResourcesLoader.Instance.GetRecycledObject("StatItemUI").GetComponent<StatItemUI>();

        item.transform.SetParent(Container, false);
        item.transform.localScale = Vector3.one;
        item.transform.position = Vector3.zero;

        item.SetStat(statName, displayName, statValue);
    }
}
