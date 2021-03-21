using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEntity : MonoBehaviour
{
    public Item CurrentItem;

    bool IsPickable;

    [SerializeField]
    SpriteRenderer ItemIcon;

    [SerializeField]
    SpriteRenderer RarityGradient;

    Actor NearbyActor;

    float InteractableCooldown = 0f;

    private void Update()
    {
        if (InteractableCooldown > 0f)
        {
            InteractableCooldown -= Time.deltaTime;
        }

        if (NearbyActor != null && Input.GetKeyDown(InputMap.Map["Interact"]) && NearbyActor.CanLookAround)
        {
            Interact();
        }
    }

    public void Interact()
    {
        if (InteractableCooldown > 0f)
        {
            return;
        }

        InteractableCooldown = 2f;


        SocketHandler.Instance.SendPickedItem(CurrentItem.itemId);
    }

    public void SetInfo(Item item)
    {
        CurrentItem = item;
        IsPickable = true;

        RefreshUI();
    }

    public void BePickedBy(Actor actor)
    {
        if(!IsPickable)
        {
            return;
        }

        IsPickable = false;

        StartCoroutine(PickRoutine(actor));
    }

    IEnumerator PickRoutine(Actor actor)
    {
        Vector2 initPos = transform.position;

        float t = 0f;
        while(t<1f)
        {
            t += t+Time.deltaTime;

            transform.position = Vector2.Lerp(initPos, actor.transform.position, t);

            yield return 0;
        }

        PickPop();
    }

    void PickPop()
    {
        ResourcesLoader.Instance.GetRecycledObject("ItemPickEffect").transform.position = transform.position;
        CORE.Instance.DespawnItem(CurrentItem.itemId);
    }

    void RefreshUI()
    {
        ItemIcon.sprite = CurrentItem.Data.Icon;
        RarityGradient.color = CurrentItem.Data.Rarity.RarityColor;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (NearbyActor != null)
        {
            return;
        }

        if (collision.tag != "Actor")
        {
            return;
        }

        Actor nearActor = collision.GetComponent<Actor>();

        if (nearActor == null)
        {
            return;
        }

        if (!nearActor.State.Data.IsPlayer)
        {
            return;
        }

        NearbyActor = nearActor;
        
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
    }
}
