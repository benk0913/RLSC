using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MobConditionEntity : MonoBehaviour
{
    public string MobKey;

    public UnityEvent OnMobExists;

    public UnityEvent OnMobDoesntExist;

    public bool MobExists = false;

    void Start()
    {
        CORE.Instance.DelayedInvokation(1f,()=>{Refresh();});
    }

    public void Refresh()
    {
        ActorData foundMob = CORE.Instance.Room.Actors.Find(X=>X.ActorEntity != null && X.classJob == MobKey && !X.ActorEntity.IsDead);

        if(foundMob == null)
        {
            if(MobExists)
            {
                OnMobDoesntExist?.Invoke();
                MobExists = false;
            }
        }
        else
        {
            if(!MobExists)
            {
                OnMobExists?.Invoke();
                MobExists = true;
            }
        }

        CORE.Instance.DelayedInvokation(3f,()=>{Refresh();});
    }
}
