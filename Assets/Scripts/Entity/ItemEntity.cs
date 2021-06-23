using EdgeworldBase;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemEntity : MonoBehaviour
{
    public Item CurrentItem;

    bool IsPickable;

    [SerializeField]
    Image ItemIcon;

    [SerializeField]
    Image RarityGradient;

    [SerializeField]
    Rigidbody2D Rigid;

    [SerializeField]
    Animator Animer;

    [SerializeField]
    TextMeshProUGUI NameLabel;

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

        InteractableCooldown = 0.5f;

        CORE.Instance.AttemptPickUpItem(CurrentItem);
    }

    public void SetInfo(Item item)
    {
        CurrentItem = item;
        IsPickable = true;

        RefreshUI();

        StartCoroutine(InterpolateToSpawn());
    }

    public void BePickedBy(Actor actor)
    {
        if (!IsPickable)
        {
            return;
        }

        IsPickable = false;

        Animer.SetTrigger("Pick");

        StartCoroutine(PickRoutine(actor));
    }

    IEnumerator InterpolateToSpawn()
    {
        Vector2 targetPoint = new Vector2(CurrentItem.spawnX, CurrentItem.spawnY);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;

            Rigid.position += (Rigid.position - targetPoint) * Time.deltaTime;

            yield return 0;
        }
    }

    IEnumerator PickRoutine(Actor actor)
    {
        Vector2 initPos = transform.position;

        float t = 0f;
        while (t < 1f)
        {
            t += t + Time.deltaTime;

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
        NameLabel.text = CurrentItem.itemName;
        ItemIcon.sprite = CurrentItem.Data.Icon;
        RarityGradient.color = CurrentItem.Data.Rarity.RarityColor;
    }

    public void OnEnter(Collider2D collision)
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
        
        Animer.SetBool("Hover", true);
        NearbyActor = nearActor;

    }

    public void OnExit(Collider2D collision)
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
        
        Animer.SetBool("Hover", false);
        NearbyActor = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnEnter(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        OnExit(collision);
    }
}
