using System;
using System.Collections.Generic;

[Serializable]
public class ActorQuests
{
    public Dictionary<string, Dictionary<string, Dictionary<string, int>>> started = new Dictionary<string, Dictionary<string, Dictionary<string, int>>>();
    public Dictionary<string, int> canComplete = new Dictionary<string, int>();
    public Dictionary<string, int> completed = new Dictionary<string, int>();

    private Dictionary<string, ActorQuestProgress> QuestProgressCache = new Dictionary<string, ActorQuestProgress>();

    public void RemoveQuestCache(string questName)
    {
        if (QuestProgressCache.ContainsKey(questName))
        {
            QuestProgressCache.Remove(questName);
        }
    }

    public ActorQuestProgress GetQuestProgress(string questName)
    {
        if (!QuestProgressCache.ContainsKey(questName))
        {
            if(!started.ContainsKey(questName))
            {
                return null;
            }
            
            QuestProgressCache.Add(questName,new ActorQuestProgress(questName, started[questName]));
        }
        return QuestProgressCache[questName];
    }
}

public class ActorQuestProgress
{
    public string QuestName;

    public QuestData QuestData
    {
        get
        {
            if (_QuestData == null)
            {
                _QuestData = CORE.Instance.Data.content.Quests.Find(X => X.name == QuestName);
            }
            return _QuestData;
        }
    }

    private Dictionary<string, Dictionary<string, int>> QuestMap;

    private QuestData _QuestData;
    public List<QuestGoal> Goals
    {
        get
        {
            return QuestData.Goals;
        }
    }

    public List<int> Values
    {
        get
        {
            if (_Values.Count == 0)
            {
                ReadFromMap();
            }
            return _Values;
        }
    }
    private List<int> _Values = new List<int>();

    public ActorQuestProgress(string QuestName, Dictionary<string, Dictionary<string, int>> QuestMap)
    {
        this.QuestName = QuestName;
        this.QuestMap = QuestMap;
    }

    private void ReadFromMap()
    {
        if (QuestData == null)
        {
            return;
        }

        foreach (QuestGoal QuestGoal in QuestData.Goals)
        {
            if (QuestMap.ContainsKey(QuestGoal.Action.name))
            {
                Dictionary<string, int> ActionMap = QuestMap[QuestGoal.Action.name];
                string ValueKey = QuestGoal.ObjectValue == null ? QuestGoal.Value : QuestGoal.ObjectValue.name;
                if (ActionMap.ContainsKey(ValueKey))
                {
                    _Values.Add(ActionMap[ValueKey]);
                    continue;
                }
            }
            _Values.Add(0);
        }
    }
}