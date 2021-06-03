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

    public bool IsBusy;

    public bool isClientOnly = false;

    
    public void SetInfo(Interactable data)
    {
        this.gameObject.SetActive(true);
        Data = data;
    }

    public void Interact()
    {
        if(InteractableCooldown > 0f)
        {
            return;
        }

        if(IsBusy)
        {
            return;
        }

        InteractableCooldown = 1f;

        IsBusy = true;

        if (isClientOnly)
        {
            Interacted(NearbyActor.State.Data.actorId);
            return;
        }

        JSONNode node = new JSONClass();
        node["interactableId"] = Data.interactableId;
        SocketHandler.Instance.SendEvent("used_interactable", node);
    }

    public void Interacted(string byActorID)
    {
        IsBusy = false;

        OnInteract?.Invoke();

        if(Data.Data != null && Data.Data.OnInteractParams.Count > 0)
        {
            
            
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

            if (!string.IsNullOrEmpty(Data.Data.InteractionSound))
                AudioControl.Instance.PlayInPosition(Data.Data.InteractionSound, transform.position);

            ActorData byActor = CORE.Instance.Room.Actors.Find(x => x.actorId == byActorID);

            if(byActor == null)
            {
                byActor = casterData;
            }

            CORE.Instance.ActivateParams(Data.Data.OnInteractParams, casterData.ActorEntity, byActor.ActorEntity);
        }
    }

    public void SetBusy(bool state)
    {
        IsBusy = state;
    }

    private void Update()
    {
        if(InteractableCooldown > 0f)
        {
            InteractableCooldown -= Time.deltaTime;
        }

        if(NearbyActor != null && Input.GetKeyDown(InputMap.Map["Interact"]) && NearbyActor.CanLookAround)
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
