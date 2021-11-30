using System;
using System.Collections.Generic;

[Serializable]
public class ActorQuests
{
    public Dictionary<string, ActorQuestProgress> started = new Dictionary<string, ActorQuestProgress>();
    public Dictionary<string, int> canComplete = new Dictionary<string, int>();
    public Dictionary<string, int> completed = new Dictionary<string, int>();
}

[Serializable]
public class ActorQuestProgress
{
    public Dictionary<string, int> expedition = new Dictionary<string, int>();
    public Dictionary<string, int> expeditionGoal = new Dictionary<string, int>();
    public Dictionary<string, int> kill = new Dictionary<string, int>();
    public Dictionary<string, int> killGoal = new Dictionary<string, int>();
    public Dictionary<string, int> collect = new Dictionary<string, int>();
    public Dictionary<string, int> collectGoal = new Dictionary<string, int>();

}