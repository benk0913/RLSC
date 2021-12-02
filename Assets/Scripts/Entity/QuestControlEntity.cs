using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestControlEntity : MonoBehaviour
{
    // This is a quicker alternative to ConditionalInvokation.cs
    public List<QuestData> RelevantQuests = new List<QuestData>();


    public GameObject HasQuestObject;

    public GameObject HasQuestCompleteObject;

    void Start()
    {
        CORE.Instance.SubscribeToEvent("RefreshQuests", Refresh);
        
        Refresh();
    }

    private void Refresh()
    {
        foreach(QuestData quest in RelevantQuests)
        {
            if(CORE.PlayerActor.quests.canComplete.ContainsKey(quest.name) && CORE.PlayerActor.quests.canComplete[quest.name] == 1)
            {
                HasQuestCompleteObject.SetActive(true);
                HasQuestObject.SetActive(false);
                return;
            }
        }


        foreach(QuestData quest in RelevantQuests)
        {
            if(!CORE.PlayerActor.quests.started.ContainsKey(quest.name) && quest.CanStart)
            {
                HasQuestObject.SetActive(true);
                HasQuestCompleteObject.SetActive(false);
                return;
            }
        }

        HasQuestCompleteObject.SetActive(false);
        HasQuestObject.SetActive(false);
    }
}
