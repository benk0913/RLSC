using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BuffCollider : HitCollider
{
    public const float SIZE_STANDARD = 2f;

    public BuffState BuffSource;

    public bool StickToActor;

    public bool HitCollider;

    [SerializeField]
    Animator Anim;

    private void Awake()
    {
        if(Anim ==null)
        {
            Anim = GetComponent<Animator>();
        }
    }

    private void OnEnable()
    {
        CORE.Instance.DelayedInvokation(0.1f, () =>
        {
            float SizeValue = (ActorSource.Collider.bounds.extents.magnitude / SIZE_STANDARD);

            transform.position += transform.TransformDirection(0f, SizeValue, 0f);

            transform.localScale = Vector3.one * SizeValue;
        });
    }


    protected void Update()
    {
        if(StickToActor)
        {
            if(ActorSource == null)
            {
                this.gameObject.SetActive(false);
                return;
            }
            transform.position = ActorSource.transform.position;
        }

        Anim.SetBool("Ending", (BuffSource.CurrentLength < 2f));
    }

    public void SetInfo(BuffState buffSource, Actor actorSource, string abilityInstanceId)
    {
        BuffSource = buffSource;
        ActorSource = actorSource;

        SetInfo(buffSource.CurrentBuff.HitAbility, actorSource, abilityInstanceId);
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Actor")
        {
            Actor actorVictim = other.GetComponent<Actor>();
            if (HitCollider)
            {
                AttemptHitAbility(actorVictim);
            }


        }

    }

    
}
