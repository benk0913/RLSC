using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampolineEntity : MonoBehaviour
{
    Actor NearbyActor;

    [SerializeField]
    float InteractableCooldown = 0f;



    [SerializeField]
    float InteractableCooldownValue = 2f;

    [SerializeField]
    Vector2 ForceDirection;

    [SerializeField]
    float ForceAmount;

    [SerializeField]
    Animator Anim;

    public void Interact(Actor actor)
    {
        if (InteractableCooldown > 0f)
        {
            return;
        }

        if(actor == null)
        {
            return;
        }

        InteractableCooldown = InteractableCooldownValue;

        Anim.SetTrigger("Activate");
        actor.Rigid.velocity = Vector2.zero;
        actor.Rigid.AddForce(transform.TransformDirection(ForceDirection) * ForceAmount, ForceMode2D.Impulse);


    }

    private void Update()
    {
        if (InteractableCooldown > 0f)
        {
            InteractableCooldown -= Time.deltaTime;
        }

        if (NearbyActor != null && (Input.GetKeyDown(InputMap.Map["Jump"]) || Input.GetKeyDown(InputMap.Map["Move Up"]) || Input.GetKey(InputMap.Map["Secondary Move Up"]) || (CORE.Instance.IsUsingJoystick && Input.GetAxis("Vertical") > 0f)) && NearbyActor.CanLookAround)
        {
            Interact(NearbyActor);
        }
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
        Interact(nearActor);
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
