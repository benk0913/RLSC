using EdgeworldBase;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InteractableEntity : MonoBehaviour
{
    public Interactable Data;

    public UnityEvent OnInteract;
    public UnityEvent OnHover;
    public UnityEvent OnUnhover;

    Actor NearbyActor;

    public static bool ByInteractable;

    public float InteractableCooldown = 0f;

    public float BusyCooldown = 3f;
    
    public bool Once = false;
    bool onced;

    public bool IsBusy;

    public bool isClientOnly = false;

    //TODO Frowned upon / Fix?
    public bool Item = false;

    public bool MapInteractable = false;

    private Vector3 centerPoint;

    void Awake()
    {
        
        
        if(MapInteractable)
        {
            this.gameObject.SetActive(false);
        }

        AttemptToGetCenter();

    }

    private void AttemptToGetCenter()
    {
        Collider2D collider = GetComponent<Collider2D>();

        if (collider == null)
        {
            centerPoint = transform.position;
            return;
        }

        centerPoint = collider.bounds.center;
    }

    public void SetInfo(Interactable data)
    {
        this.gameObject.SetActive(true);
        Data = data;

        ByInteractable = false;

        if(string.IsNullOrEmpty(data.interactableId))
        {
            CORE.Instance.LogMessageError("Current interactable spawned with NO ID " + Data.interactableId);
        }
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

        if(Once && onced)
        {
            return;
        }

        
        onced = true;

        CORE.Instance.DelayedInvokation(0.1f,()=> // Delayed in order to interact with all nearby interactables.
        {
            InteractableCooldown = 0.1f;

            IsBusy = true;
            BusyCooldown = 3f;

            if (isClientOnly)
            {
                if (NearbyActor != null)
                {
                    Interacted(NearbyActor.State.Data.actorId);
                }
                else
                {
                    IsBusy = false;
                    BusyCooldown = 0f;
                }
                return;
            }

            JSONNode node = new JSONClass();
            node["interactableId"] = Data.interactableId;
            SocketHandler.Instance.SendEvent("used_interactable", node);
        });
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

            AudioControl.Instance.Play("CashShopSubCategoryClick");
        }
    }

    public void SetBusy(bool state)
    {
        IsBusy = state;
    }

    public async void Teleport(Transform targetTransform)
    {
        if(NearbyActor == null)
        {
            CORE.PlayerActor.ActorEntity.transform.position = new Vector3(targetTransform.position.x,targetTransform.position.y,CORE.PlayerActor.ActorEntity.transform.position.z);    
            return;
        }

        NearbyActor.transform.position = new Vector3(targetTransform.position.x,targetTransform.position.y,NearbyActor.transform.position.z);
    }

    public void ScreenEffect(string effectKey)
    {
        CORE.Instance.ShowScreenEffect(effectKey);
    }
    private void Update()
    {
        if(IsBusy)
        {
            if(BusyCooldown > 0f)
                BusyCooldown -= Time.deltaTime;
            else
            {
                IsBusy = false;
                BusyCooldown = 3f;
            }
        }

        if(InteractableCooldown > 0f)
        {
            InteractableCooldown -= Time.deltaTime;
        }

        bool KeyIsDown = Item? (MultiplatformUIManager.IsUniversalPickUp || Input.GetKey(InputMap.Map["Pick Up Item"]) || Input.GetButtonDown("Joystick 0")) :  (MultiplatformUIManager.IsUniversalInteract|| Input.GetKeyDown(InputMap.Map["Interact"]) || Input.GetButtonDown("Joystick 0"));
        
        if(NearbyActor != null && KeyIsDown && NearbyActor.CanInteract)
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
        ByInteractable = true;

        if(Item)
        {
            MultiplatformUIManager.SetCurrentItem(this);
        }
        else
        {
            MultiplatformUIManager.SetCurrentInteractable(this);
        }

        if(!CORE.IsMachinemaMode)
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
        ByInteractable = false;
        if(Item)
        {
            if(MultiplatformUIManager.CurrentItem == this)
            {
                MultiplatformUIManager.SetCurrentItem(null);
            }
        }
        else
        {
            if(MultiplatformUIManager.CurrentInteractable == this)
            {
                MultiplatformUIManager.SetCurrentInteractable(null);
            }
        }

        OnUnhover?.Invoke();
    }

    void OnMouseDown()
    {
        if (!CORE.Instance.IsInputEnabled) return;

        Vector2 screenPoint = CameraChaseEntity.Instance.CurrentCam.WorldToScreenPoint(centerPoint);

        if (Vector2.Distance(screenPoint, Input.mousePosition) > Screen.width / 4f) return;
        
        if(NearbyActor != CORE.PlayerActor.ActorEntity) return;
    
        if(DecisionContainerUI.Instance.CurrentDecisions.Count > 0 && DecisionContainerUI.Instance.gameObject.activeInHierarchy) return;

        AudioControl.Instance.Play("CashShopItemHover");

        MultiplatformUIManager.IsUniversalInteract = true;
    }
}
