using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AbilityCollider : HitCollider
{

    public bool StickToActor;
    public bool StickToSkilledShot;
    public bool HitEventOnWalls;

    [SerializeField]
    Transform SkilledShotPoint;

    [SerializeField]
    LayerMask SkilledshotLayermask;

    [SerializeField]
    Vector3 InitForce;

    [SerializeField]
    string onHitObject;

    [SerializeField]
    Transform OnHitSource;

    public string AudioOnHit;

    private void Awake()
    {
        if (!string.IsNullOrEmpty(onHitObject))
        {
            OnHitEvent.AddListener(() =>
            {
                GameObject obj = ResourcesLoader.Instance.GetRecycledObject(onHitObject);
                if (OnHitSource != null)
                {
                    obj.transform.position = OnHitSource.position;
                }
                else
                {
                    obj.transform.position = transform.position;
                }

                AbilityCollider abilityCol = obj.GetComponent<AbilityCollider>();
                if (abilityCol != null)
                {
                    abilityCol.SetInfo(this.AbilitySource, this.ActorSource);
                }
            });
        }

        if (!string.IsNullOrEmpty(AudioOnHit))
        {
            OnHitEvent.AddListener(() =>
            {

                if (OnHitSource != null)
                {
                    AudioControl.Instance.PlayInPosition(AudioOnHit, OnHitSource.position);
                }
                else
                {
                    AudioControl.Instance.PlayInPosition(AudioOnHit, transform.position);
                }
            });
        }
    }

    private void Update()
    {
        if(StickToActor)
        {
            transform.position = ActorSource.transform.position;
        }
    }

    public override void SetInfo(Ability abilitySource, Actor actorSource)
    {
        base.SetInfo(abilitySource, actorSource);

        if(StickToSkilledShot)
        {
            RaycastHit2D rhit = Physics2D.Raycast(SkilledShotPoint.position, Vector2.down, Mathf.Infinity, SkilledshotLayermask);
            if(rhit)
            {
                transform.position = rhit.point;
            }
            else
            {
                TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("No nearby ground to hit!", Color.red, 2f));
                this.gameObject.SetActive(false);
                return;
            }
        }

        if(InitForce != default)
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(-1f * transform.localScale.x * InitForce.x,InitForce.y));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Actor")
        {   

            Actor actorVictim = other.GetComponent<Actor>();

            AttemptHitAbility(actorVictim);
        }
        else if(other.tag == "Untagged")
        {
            if(HitEventOnWalls)
            {
                OnHitEvent?.Invoke();
            }
        }
    }


    public void ForceOnHit()
    {
        OnHitEvent?.Invoke();
    }

}
