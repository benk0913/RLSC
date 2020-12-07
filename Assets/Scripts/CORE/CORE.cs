using Newtonsoft.Json;
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

    public CanvasGroup GameUICG;

    public CGDatabase Data;

    public RoomData Room;
    
    public bool DEBUG = false;

    public bool DEBUG_SPAMMY_EVENTS = false;

    public Dictionary<string, UnityEvent> DynamicEvents = new Dictionary<string, UnityEvent>();

    public bool IsBitch;

    private void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;
        Time.fixedDeltaTime = 0.01666667f;
        Application.runInBackground = true;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        SubscribeToEvent("ActorDied", Room.RefreshThreat);
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
        actorObject.transform.position = new Vector3(actorData.x,actorData.y,0f);
        actorData.ActorEntity = actorObject.GetComponent<Actor>();
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

        GameUICG.alpha = 1f;

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

    public ActorData PlayerActor;

    [JsonIgnore]
    public Actor MostThreateningActor;

    public Actor GetMostThreateningActor()
    {
        Actor mostThreatAct = null;
        float mostThreat = Mathf.NegativeInfinity;
        for(int i=0;i<Actors.Count;i++)
        {
            if (Actors[i].ActorEntity.State.Data.hp <= 0)
            {
                continue;
            }

            float currentThreat = Actors[i].ActorEntity.State.Data.threat;//TODO THREAT_IMP_SERVER

            if (currentThreat > mostThreat)
            {
                mostThreatAct = Actors[i].ActorEntity;
                mostThreat = currentThreat;
            }
        }

        return mostThreatAct;
        
    }

    public void ActorJoined(ActorData actor)
    {
        Actors.Add(actor);

        if(actor.IsPlayer)
        {
            PlayerActor = actor;
        }

        RefreshThreat();
    }

    public void ActorLeft(string actorID)
    {
        ActorData actor = Actors.Find(x => x.actorId == actorID);

        if(actor == null)
        {
            CORE.Instance.LogMessageError("No actorId " + actorID + " in room.");
            return;
        }

        CORE.Destroy(actor.ActorEntity.gameObject);

        if(PlayerActor == actor)
        {
            PlayerActor = null;
        }

        Actors.Remove(actor);

        RefreshThreat();
    }

    public void RefreshThreat()
    {
        MostThreateningActor = GetMostThreateningActor();
    }

    public void SendActorsPositions()
    {
        List<ActorData> actorsToUpdate = new List<ActorData>();
        JSONNode node = new JSONClass();
        for(int i=0;i<Actors.Count;i++)
        {
            ActorData actor = Actors[i];
            if (actor.IsPlayer && actor.ActorEntity != null)
            {
                actorsToUpdate.Add(actor);
                actor.x = actor.ActorEntity.transform.position.x;
                actor.y = actor.ActorEntity.transform.position.y;
                actor.faceRight = actor.ActorEntity.Body.localScale.x < 0f;
            }
        }

        for(int i=0;i<actorsToUpdate.Count;i++)
        {
            ActorData actor = actorsToUpdate[i];
            node["actorPositions"][i]["actorId"] = actor.actorId;
            node["actorPositions"][i]["x"] = actor.x.ToString();
            node["actorPositions"][i]["y"] = actor.y.ToString();
            node["actorPositions"][i]["faceRight"].AsBool = actor.faceRight;
        }

        if (actorsToUpdate.Count > 0)
        {
            SocketHandler.Instance.SendEvent("actors_moved", node);
        }
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

            actor.x = float.Parse(data["actorPositions"][i]["x"]);
            actor.y = float.Parse(data["actorPositions"][i]["y"]);
            actor.faceRight = bool.Parse(data["actorPositions"][i]["faceRight"]);
        }
    }
}
