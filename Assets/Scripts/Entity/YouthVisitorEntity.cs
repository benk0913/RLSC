using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YouthVisitorEntity : MonoBehaviour
{
    void Start()
    {
        CORE.Instance.SubscribeToEvent("ActorSpawned",()=>NewActor(CORE.Instance.Room.Actors[CORE.Instance.Room.Actors.Count - 1].ActorEntity));

        GameObject[] puppets = GameObject.FindGameObjectsWithTag("ActorPuppet");
        foreach(GameObject puppet in puppets)
        {
            NewActor(puppet.GetComponent<Actor>());
        }
    }

    public void NewActor(Actor newActor)
    {
        if(newActor.State.Data == null || ( !newActor.State.Data.isCharacter && !newActor.State.Data.IsPlayer && !newActor.IsDisplayActor))
        {
            return;
        }

        newActor.Animer.transform.localScale = new Vector3(0.66f,0.66f,1f);
        newActor.Animer.transform.Find("Torso/Head").localScale = new Vector3(1.1f,1.1f,1f);
        newActor.Animer.transform.Find("Torso/Breast").gameObject.SetActive(false);
    }
}
