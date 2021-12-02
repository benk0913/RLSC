using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GECompleteQuest ", menuName = "Data/GECompleteQuest ", order = 2)]
public class GECompleteQuest : GameEvent
{

    public QuestData TheQuest;

    public override void Execute(System.Object obj = null)
    {
        base.Execute(obj);

        if(CORE.PlayerActor.quests.canComplete.ContainsKey(TheQuest.name) && CORE.PlayerActor.quests.canComplete[TheQuest.name] == 1)
        {
            TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Can not start quest! (Already started)",Color.red));
            return;
        }   

        SocketHandler.Instance.SendQuestComplete(TheQuest.name);

    }
}
