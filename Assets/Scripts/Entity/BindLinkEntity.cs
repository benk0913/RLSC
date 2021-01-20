using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindLinkEntity : MonoBehaviour
{
    [SerializeField]
    BuffCollider RelevantBuff;

    [SerializeField]
    List<Actor> LinkedActors = new List<Actor>();

    [SerializeField]
    LineRenderer Renderer;

    private void LateUpdate()
    {
        if(LinkedActors.Count == 0)
        {
            return;
        }

        List<Vector3> linkPositions = new List<Vector3>();
        for(int i=0;i<LinkedActors.Count;i++)
        {
            linkPositions.Add(LinkedActors[i].Body.transform.position);
        }

        Renderer.positionCount = linkPositions.Count;
        Renderer.SetPositions(linkPositions.ToArray());
    }

    private void OnEnable()
    {
        CORE.Instance.DelayedInvokation(0.5f, () =>
        {
            LinkedActors.Clear();
            if (RelevantBuff.ActorSource.State.Data.States.ContainsKey("Bind"))
            {
                for (int i = 0; i < RelevantBuff.ActorSource.State.Data.States["Bind"].linkedActorIds.Length; i++)
                {
                    ActorData actorDat = CORE.Instance.Room.Actors.Find(x => x.actorId == RelevantBuff.ActorSource.State.Data.States["Bind"].linkedActorIds[i]);

                    if (actorDat == null || actorDat.ActorEntity == null)
                    {
                        CORE.Instance.LogMessageError("No actor with actorId " + CORE.Instance.Room.Actors.Find(x => x.actorId == RelevantBuff.ActorSource.State.Data.States["Bind"].linkedActorIds[i]));
                        continue;
                    }
                    LinkedActors.Add(actorDat.ActorEntity);
                }
            }

        });
    }
}
