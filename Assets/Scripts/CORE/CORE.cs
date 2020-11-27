using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CORE : MonoBehaviour
{
    public static CORE Instance;
    
    public CGDatabase Data;

    public RoomData Room;
    
    public bool DEBUG = false;
    
    public Dictionary<string, UnityEvent> DynamicEvents = new Dictionary<string, UnityEvent>();


    private void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;
        Time.fixedDeltaTime = 0.01666667f;
        Application.runInBackground = true;
        DontDestroyOnLoad(this.gameObject);
    }


    public static void ClearContainer(Transform container)
    {
        while (container.childCount > 0)
        {
            container.GetChild(0).gameObject.SetActive(false);
            container.GetChild(0).SetParent(Instance.transform);
        }
    }

    public void SubscribeToEvent(string eventKey, UnityAction action)
    {
        if (!DynamicEvents.ContainsKey(eventKey))
        {
            DynamicEvents.Add(eventKey, new UnityEvent());
        }

        DynamicEvents[eventKey].AddListener(action);
    }

    public void UnsubscribeFromEvent(string eventKey, UnityAction action)
    {
        if (!DynamicEvents.ContainsKey(eventKey))
        {
            Debug.LogError("EVENT " + eventKey + " does not exist!");
            return;
        }

        DynamicEvents[eventKey].RemoveListener(action);
    }

    public void InvokeEvent(string eventKey)
    {
        if (DEBUG)
        {
            Debug.Log("CORE - Event Invoked " + eventKey);
        }

        if (!DynamicEvents.ContainsKey(eventKey))
        {
            DynamicEvents.Add(eventKey, new UnityEvent());
        }

        DynamicEvents[eventKey].Invoke();
    }

    public void DelayedInvokation(float time, System.Action action)
    {
        StartCoroutine(DelayedInvokationRoutine(time, action));
    }

    IEnumerator DelayedInvokationRoutine(float time, System.Action action)
    {
        yield return new WaitForSeconds(time);

        action.Invoke();
    }

    public void LogMessage(string message)
    {
        if(!DEBUG)
        {
            return;
        }

        Debug.Log(message);
    }

    public void LogMessageError(string message)
    {
        if (!DEBUG)
        {
            return;
        }

        Debug.LogError(message);
    }

    public void LoadScene(string sceneKey, Action onComplete = null)
    {
        if(LoadSceneRoutineInstance != null)
        {
            StopCoroutine(LoadSceneRoutineInstance);
        }
        
        LoadSceneRoutineInstance = StartCoroutine(LoadSceneRoutine(sceneKey,onComplete));
    }

    public void SpawnActor(ActorData actorData)
    {


        GameObject actorObject = ResourcesLoader.Instance.GetRecycledObject(actorData.actorType);
        actorObject.transform.position = new Vector3(actorData.positionX,actorData.positionY,0f);
        actorData.ActorObject = actorObject;
        actorObject.GetComponent<Actor>().SetActorInfo(actorData);

        Room.ActorJoined(actorData);
    }

    public void DespawnActor(string actorId)
    {
        Room.ActorLeft(actorId);
    }

    Coroutine LoadSceneRoutineInstance;
    IEnumerator LoadSceneRoutine(string sceneKey, Action onComplete = null)
    {
        string currentSceneKey = SceneManager.GetActiveScene().name;

        SceneManager.LoadScene(sceneKey);

        if(RoomUpdateRoutineInstance != null)
        {
            StopCoroutine(RoomUpdateRoutineInstance);
            RoomUpdateRoutineInstance = null;
        }

        yield return 0;

        while (SceneManager.GetActiveScene().name != sceneKey)
        {
            yield return 0;
        }

        yield return 0;

        RoomUpdateRoutineInstance = StartCoroutine(RoomUpdateRoutine());

        onComplete?.Invoke();

        LoadSceneRoutineInstance = null;
    }

    Coroutine RoomUpdateRoutineInstance;
    IEnumerator RoomUpdateRoutine()
    {
        while(true)
        {
            Room.SendActorsPositions();
            yield return 0;
        }
    }

    
}

[Serializable]
public class RoomData
{
    public List<ActorData> Actors = new List<ActorData>();

    public void ActorJoined(ActorData actor)
    {
        Actors.Add(actor);
    }

    public void ActorLeft(string actorID)
    {
        ActorData actor = Actors.Find(x => x.actorId == actorID);

        if(actor == null)
        {
            CORE.Instance.LogMessageError("No actorId " + actorID + " in room.");
            return;
        }

        actor.ActorObject.SetActive(false);
        Actors.Remove(actor);
    }

    public void SendActorsPositions()
    {
        List<ActorData> actorsToUpdate = new List<ActorData>();
        JSONNode node = new JSONClass();
        for(int i=0;i<Actors.Count;i++)
        {
            ActorData actor = Actors[i];
            if (actor.IsPlayer && actor.ActorObject != null)
            {
                actorsToUpdate.Add(actor);
                actor.positionX = actor.ActorObject.transform.position.x;
                actor.positionY = actor.ActorObject.transform.position.y;
            }
        }

        for(int i=0;i<actorsToUpdate.Count;i++)
        {
            ActorData actor = actorsToUpdate[i];
            node["actorPositions"][i]["actorId"] = actor.actorId;
            node["actorPositions"][i]["x"] = actor.positionX.ToString();
            node["actorPositions"][i]["y"] = actor.positionY.ToString();
        }

        SocketHandler.Instance.SendEvent("actors_moved",node);
    }

    public void ReceiveActorPositions(JSONNode data)
    {
        for(int i=0;i<data["actorPositions"].Count;i++)
        {
            ActorData actor = Actors.Find(x => x.actorId == data["actorPositions"][i]["actorId"].Value);

            if(actor == null)
            {
                CORE.Instance.LogMessageError("No actor with id " + data["actorPositions"][i]["actorId"].Value);
                continue;
            }

            actor.positionX = float.Parse(data["actorPositions"][i]["x"]);
            actor.positionY = float.Parse(data["actorPositions"][i]["y"]);
        }
    }
}
