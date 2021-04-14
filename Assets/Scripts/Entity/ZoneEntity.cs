using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ZoneEntity : MonoBehaviour
{
    public UnityEvent OnEnter;
    public UnityEvent OnExit;

    public UnityEvent<Collider2D> OnEnterCol;
    public UnityEvent<Collider2D> OnExitCol;

    Actor NearbyActor;

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
        OnEnter?.Invoke();
        OnEnterCol?.Invoke(collision);
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

        OnExit?.Invoke();

        OnExitCol?.Invoke(collision);
    }
}
