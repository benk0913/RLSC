using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GEUnlockClass ", menuName = "Data/GEUnlockClass ", order = 2)]
public class GEUnlockClass : GameEvent
{

    public string Class;

    public override void Execute(System.Object obj = null)
    {
        base.Execute(obj);

         if(CORE.PlayerActor.unlockedClassJobs.Find(x=>x == Class) == null //Don't already have the class 
        && CORE.PlayerActor.ClassJobReference.NextClassJobs.Find(x=>x.name == Class) != null // Currnet class can unlock the class
        && CORE.Instance.Data.content.Classes.Find(X=>X.name==Class).UnlockLevel <= CORE.PlayerActor.level) //Has sufficient level
        {
            SocketHandler.Instance.SendUnlockClassJob(Class);   
        }
        else
        {  
            TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("You are unable to unlock the class - Your current class can not preceed that class!",Color.red));
            return;
        }
    }
}
