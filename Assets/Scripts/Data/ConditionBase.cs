using UnityEngine;

public interface ConditionBase
{    bool IsValid(System.Object obj);
}

public class ConditionLogic
{
    public static bool IsValid(System.Object obj, ScriptableObject ObjectValue, string Value, string Type, bool Inverse)
    {
        string ActualValue = ObjectValue == null ? Value : ObjectValue.name;

        switch(Type)
        {
            case "HasBuff":
                {
                    Actor target = ((Actor)obj);

                    if(target.State.Buffs.Find(x=>x.CurrentBuff.name == ActualValue) != null)
                    {
                        return !Inverse; //  False
                    }
                    else
                    {
                        return Inverse; //  True
                    }
                }
            case "HasItem":
                {
                    Actor target = ((Actor)obj);

                    if(target == null)
                    {
                        target = CORE.PlayerActor.ActorEntity;
                    }

                    if(target.State.Data.items.Find(x=>x != null && x.itemName == ActualValue) != null)
                    {
                        return !Inverse; //  False
                    }
                    else
                    {
                        return Inverse; //  True
                    }
                }
            case "Chance":
                {
                    if(UnityEngine.Random.Range(0f,1f) < float.Parse(ActualValue))
                    {
                        return !Inverse; //  False
                    }
                    else
                    {
                        return Inverse; //  True
                    }
                }
            case "InExpeditionQueue":
                {
                    if(ExpeditionQueTimerUI.Instance.IsSearching)
                    {
                        return !Inverse;
                    }
                    else
                    {
                        return Inverse;
                    }
                }
            case "HasMoney":
                {
                    if (CORE.PlayerActor.money >= int.Parse(ActualValue))
                    {
                        return !Inverse;
                    }
                    else
                    {
                        return Inverse;
                    }
                }
            case "FinishedQuest":
                {
                    if (CORE.PlayerActor.quests.completed.ContainsKey(ActualValue))
                    {
                        return !Inverse;
                    }
                    else
                    {
                        return Inverse;
                    }
                }
            case "CanFinishQuest":
                {
                    if (CORE.PlayerActor.quests.canComplete.ContainsKey(ActualValue))
                    {
                        return !Inverse;
                    }
                    else
                    {
                        return Inverse;
                    }
                }
            case "QuestStarted":
                {
                    if (CORE.PlayerActor.quests.started.ContainsKey(ActualValue))
                    {
                        return !Inverse;
                    }
                    else
                    {
                        return Inverse;
                    }
                }
            case "CanStartQuest":
                {
                    if (CORE.Instance.Data.content.Quests.Find(X=>X.name == ActualValue).CanStart)
                    {
                        return !Inverse;
                    }
                    else
                    {
                        return Inverse;
                    }
                }
            case "HasLevel":
                {
                    if(CORE.PlayerActor.level >= int.Parse(ActualValue))
                    {
                        return !Inverse;
                    }
                    else
                    {
                        return Inverse;
                    }
                }
            case "HasParty":
                {
                    if(CORE.Instance.CurrentParty != null && CORE.Instance.CurrentParty.members.Length > 1)
                    {
                        return !Inverse;
                    }
                    else
                    {
                        return Inverse;
                    }
                }
            case "CanUnlockClass":
                {
                    if(CORE.PlayerActor.unlockedClassJobs.Find(x=>x == ActualValue) == null //Don't already have the class 
                    && CORE.PlayerActor.ClassJobReference.NextClassJobs.Find(x=>x.name == ActualValue) != null // Currnet class can unlock the class
                    && CORE.Instance.Data.content.Classes.Find(X=>X.name==ActualValue).UnlockLevel <= CORE.PlayerActor.level) //Has sufficient level
                    {
                        return !Inverse;
                    }
                    else
                    {
                        return Inverse;
                    }
                }
            case "HasSessionRule":
            {
                if (CORE.Instance.HasSessionRule(ActualValue))
                {
                    return !Inverse;
                }
                else
                {
                    return Inverse;
                }
            }
        } 

        return !Inverse; //  True 
    }
}