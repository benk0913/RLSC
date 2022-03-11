using System.Collections;
using System.Collections.Generic;
using DigitalRuby.LightningBolt;
using UnityEngine;

public class BindLinkEntity : MonoBehaviour
{
    [SerializeField]
    BuffCollider RelevantBuff;

    [SerializeField]
    List<Actor> LinkedActors = new List<Actor>();

    [SerializeField]
    LineRenderer Renderer;

    [SerializeField]
    LightningBoltScript Lightning;

    public bool BaseOnActorSource;

    private void LateUpdate()
    {
        if(LinkedActors.Count < 2)
        {
            return;
        }
        
        List<Vector3> linkPositions = new List<Vector3>();
        for(int i=0;i<LinkedActors.Count;i++)
        {
            linkPositions.Add(LinkedActors[i].Body.transform.position);
        }

        if(Renderer != null)
        {
            Renderer.positionCount = linkPositions.Count;
            Renderer.SetPositions(linkPositions.ToArray());
        }
        
        if(Lightning != null)
        {
            Lightning.StartObject = LinkedActors[0].gameObject;
            Lightning.EndObject = LinkedActors[1].gameObject;
        }
    }

    private void OnEnable()
    {
        if(Lightning != null)
        {
            Lightning.gameObject.SetActive(false);
        }

        CORE.Instance.DelayedInvokation(0.1f, () =>
        {
            LinkedActors.Clear();

            if(BaseOnActorSource)
            {
                LinkedActors.Add(RelevantBuff.ActorSource);

                if(RelevantBuff.CasterActor != null)
                {
                    LinkedActors.Add(RelevantBuff.CasterActor);
                }
            }
            else
            {
                if (RelevantBuff.ActorSource.State.Data.states.ContainsKey("Bind Buff"))
                {
                    for (int i = 0; i < RelevantBuff.ActorSource.State.Data.states["Bind Buff"].linkedActorIds.Length; i++)
                    {
                        ActorData actorDat = CORE.Instance.Room.Actors.Find(x => x.actorId == RelevantBuff.ActorSource.State.Data.states["Bind Buff"].linkedActorIds[i]);

                        if (actorDat == null || actorDat.ActorEntity == null)
                        {
                            CORE.Instance.LogMessageError("No actor with actorId " + CORE.Instance.Room.Actors.Find(x => x.actorId == RelevantBuff.ActorSource.State.Data.states["Bind Buff"].linkedActorIds[i]));
                            continue;
                        }
                        LinkedActors.Add(actorDat.ActorEntity);
                    }
                }
            }
            
            if(Lightning != null)
            {
                Lightning.gameObject.SetActive(true);
            } 
        });
    }
}
