using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GEStartQuest ", menuName = "Data/GEStartQuest ", order = 2)]
public class GEStartQuest : GameEvent
{

    public QuestData TheQuest;

    public override void Execute(System.Object obj = null)
    {
        base.Execute(obj);

        if(!TheQuest.CanStart)
        {
            TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Can not start queset! (Conditions Not met)",Color.red));
            return;
        }

        if(CORE.PlayerActor.quests.started.ContainsKey(TheQuest.name))
        {
            TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Can not start queset! (Already started)",Color.red));
            return;
        }

        SocketHandler.Instance.SendQuestStart(TheQuest.name);

    }
}
