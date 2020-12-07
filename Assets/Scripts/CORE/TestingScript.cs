using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingScript : MonoBehaviour
{
    public bool Test;

    void Update()
    {
        if(Test)
        {
            Test = false;

            JSONNode node = new JSONClass();

            node["actor"]["scene"] = "TestScene1";
            node["actor"]["name"] = "TestScene1";
            node["actor"]["classJob"] = "ActorRabbit";
            node["actor"]["scene"] = "TestScene1";
            node["actor"]["actorType"] = "ActorRabbit";
            node["actor"]["actorId"] = Random.Range(0, 99999).ToString();
            node["actor"]["x"].AsFloat = 0f;
            node["actor"]["y"].AsFloat = 0f;
            node["actor"]["hp"].AsInt = 100;

            SocketHandler.Instance.OnActorSpawn("actor_spawn",node);

        }
    }
}
