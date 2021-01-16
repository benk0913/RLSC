using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableEntity : MonoBehaviour
{
    public Interactable Data;

    public UnityEvent OnInteract;
    public UnityEvent OnHover;
    public UnityEvent OnUnhover;

    Actor NearbyActor;

    public float InteractableCooldown = 0f;

    public void SetInfo(Interactable data)
    {
        Data = data;
    }

    public void Interact()
    {
        if(InteractableCooldown > 0f)
        {
            return;
        }

        JSONNode node = new JSONClass();
        node["interactableId"] = Data.interactableId;
        SocketHandler.Instance.SendEvent("used_interactable",node);

        InteractableCooldown = 1f;
    }

    public void Interacted(string byActorID)
    {
        OnInteract?.Invoke();

        if(!string.IsNullOrEmpty(Data.Data.AbilityOnInteract))
        {
            Ability ability = CORE.Instance.Data.content.Abilities.Find(X => X.name == Data.Data.AbilityOnInteract);

            if(ability == null)
            {
                CORE.Instance.LogMessageError("No ability with key " + Data.Data.AbilityOnInteract);
                return;
            }

            ActorData casterData;

            if (!string.IsNullOrEmpty(Data.actorId))
            {
                casterData = CORE.Instance.Room.Actors.Find(x => x.actorId == Data.actorId);
            }
            else
            {
                casterData = CORE.Instance.Room.Actors.Find(x => x.actorId == byActorID);
            }

            if (casterData == null)
            {
                CORE.Instance.LogMessageError("No caster with id " + Data.actorId);
                return;
            }

            if (casterData.ActorEntity == null)
            {
                CORE.Instance.LogMessageError("No entity for actor with id " + Data.actorId);
                return;
            }

            if (!string.IsNullOrEmpty(ability.ExecuteAbilitySound))
                AudioControl.Instance.PlayInPosition(ability.ExecuteAbilitySound, transform.position);

            if (!string.IsNullOrEmpty(ability.AbilityColliderObject))
            {
                GameObject colliderObj = ResourcesLoader.Instance.GetRecycledObject(ability.AbilityColliderObject);
                colliderObj.transform.position = transform.position;
                colliderObj.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                colliderObj.transform.localScale = Vector3.one;

                colliderObj.GetComponent<AbilityCollider>().SetInfo(ability, casterData.ActorEntity);
            }
            
        }
    }

    private void Update()
    {
        if(InteractableCooldown > 0f)
        {
            InteractableCooldown -= Time.deltaTime;
        }

        if(NearbyActor != null && Input.GetKeyDown(InputMap.Map["Interact"]))
        {
            Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(NearbyActor != null)
        {
            return;
        }

        if(collision.tag != "Actor")
        {
            return;
        }

        Actor nearActor = collision.GetComponent<Actor>();

        if(nearActor == null)
        {
            return;
        }

        if (!nearActor.State.Data.IsPlayer)
        {
            return;
        }

        NearbyActor = nearActor;

        OnHover?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag != "Actor")
        {
            return;
        }

        Actor nearActor = collision.GetComponent<Actor>();

        if (nearActor == null)
        {
            return;
        }

        if (NearbyActor != nearActor)
        {
            return;
        }

        NearbyActor = null;

        OnUnhover?.Invoke();
    }
}
