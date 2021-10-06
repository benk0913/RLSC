using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTSCRIPTactor : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(TestscriptROutine());
    }

    JSONClass node = new JSONClass();
    IEnumerator TestscriptROutine()
    {
        for (int i=0;i<25;i++)
        {
            node["message"] = "/spawn air";

            SocketHandler.Instance.SendEvent("console_message", node);

            yield return new WaitForSeconds(0.1f);
        }
        for (int i = 0; i < 25; i++)
        {
            node["message"] = "/spawn earth";

            SocketHandler.Instance.SendEvent("console_message", node);

            yield return new WaitForSeconds(0.1f);
        }
        for (int i = 0; i < 25; i++)
        {
            node["message"] = "/spawn fire";

            SocketHandler.Instance.SendEvent("console_message", node);

            yield return new WaitForSeconds(0.1f);
        }
        for (int i = 0; i < 25; i++)
        {
            node["message"] = "/spawn water";

            SocketHandler.Instance.SendEvent("console_message", node);

            yield return new WaitForSeconds(0.1f);
        }

        CORE.Instance.CurrentParty = null;

        while (true)
        {
            foreach (ActorData actor in CORE.Instance.Room.Actors)
            {
                int randomAction = Random.Range(0, 6);

                if (randomAction == 0)
                {
                    actor.ActorEntity.AttemptMoveLeft();
                }
                else if (randomAction == 1)
                {
                    actor.ActorEntity.AttemptMoveRight();
                }
                else if (randomAction == 2)
                {
                    actor.ActorEntity.AttemptJump();
                }
                else if (randomAction == 3)
                {
                    int abilityIndex = Random.Range(0, 3);

                    actor.ActorEntity.AttemptPrepareAbility(abilityIndex);

                }
                else if (randomAction == 4)
                {
                    node["message"] = "OMG! So cool! Sup homies! i am a player! Penis poenis penis peeeeniiisisss!!!!";
                    node["actorId"] = actor.actorId;
                    SocketHandler.Instance.OnActorChatMessage("actor_message",node);
                }
                else
                {
                    int abilityIndex = Random.Range(0, 3);

                    Ability ability = CORE.Instance.Data.content.Abilities.Find(x => x.name == CORE.Instance.Data.content.Classes.Find(X => X.name == actor.classJob).Abilities[abilityIndex]);


                    actor.ActorEntity.AttemptExecuteAbility(ability, actor.ActorEntity);
                }
            }


            yield return new WaitForSeconds(1f);
        }
    }
}
