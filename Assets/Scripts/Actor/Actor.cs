using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    
    public ActorState State;

    [SerializeField]
    public Rigidbody2D Rigid;

    [SerializeField]
    public Transform Body;

    [SerializeField]
    public ActorControl PlayerControl;

    [SerializeField]
    protected Animator Animer;

    [SerializeField]
    protected LayerMask GroundMask;

    [SerializeField]
    protected RaycastHit2D Rhit;

    [SerializeField]
    protected SpriteRenderer Shadow;


    public bool IsGrounded;

    public float GroundCheckDistance = 10f;
    public float GroundedDistance= 1f;

    public float VelocityMinimumThreshold = 0.1f;

    protected Vector3 deltaPosition;
    protected Vector3 lastPosition;


    //TODO Replace with  actor atribtes
    public float MovementSpeed = 1f;

    public float JumpHeight = 1f;
    //

    public float InterpolationSpeed = 1f;

    public bool IsClientControl
    {
        get
        {
            return PlayerControl.enabled;
        }
    }

    public void SetActorInfo(ActorData data)
    {
        this.State.Data = data;

        this.State.Abilities.Clear();

        //TODO Replace this with ability from seleted set.
        for (int i = 0; i < 3; i++)
        {
            string abilityName = CORE.Instance.Data.content.Classes.Find(x => x.name == State.Data.classJob).Abilities[i];
            this.State.Abilities.Add(new AbilityState(CORE.Instance.Data.content.Abilities.Find(x => x.name == abilityName)));
        }

        if (data.IsPlayer)
        {
            PlayerControl.enabled = true;
        }
        else
        {
            PlayerControl.enabled = false;
        }
    }

    protected void FixedUpdate()
    {
        if(!IsClientControl)
        {
            UpdateFromActorData();
        }

        RefreshVelocity();
    }

    protected void LateUpdate()
    {
        if (IsClientControl)
        {
            RefreshActorState();
        }

        RefreshGroundedState();
    }

    void RefreshActorState()
    {


        foreach(AbilityState abilityState in State.Abilities)
        {
            if(abilityState.CurrentCD > 0f)
            {
                abilityState.CurrentCD -= Time.deltaTime;
            }

            if (abilityState.CurrentCastingTime > 0f)
            {
                abilityState.CurrentCastingTime -= Time.deltaTime;

                if(abilityState.CurrentCastingTime <= 0f && State.IsPreparingAbility)
                {
                    AttemptExecuteAbility(abilityState.Ability);
                }
            }
        }
    }

    void RefreshVelocity()
    {
        deltaPosition = transform.position - lastPosition;
        lastPosition = transform.position;
        Animer.SetFloat("VelocityX", deltaPosition.x);
        Animer.SetFloat("VelocityY", deltaPosition.y);

        if (deltaPosition.x > 0.1f)
        {
            Body.transform.localScale = Vector3.one;
        }
        else if (deltaPosition.x < -0.1f)
        {
            Body.transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void RefreshGroundedState()
    {
        Rhit = Physics2D.Raycast(transform.position, Vector2.down, GroundCheckDistance, GroundMask);

        if (Rhit)
        {
            float distance = Vector2.Distance(transform.position, Rhit.point);

            if (distance < GroundedDistance)
            {
                IsGrounded = true;
            }
            else
            {
                IsGrounded = false;
            }
            Shadow.transform.position = Rhit.point;
            Shadow.color = new Color(0f, 0f, 0f, Mathf.Lerp(0f, 0.75f, 1f - (distance / GroundCheckDistance)));
        }
        else
        {
            IsGrounded = false;
            Shadow.color = Color.clear;
        }

        Animer.SetBool("InAir", !IsGrounded);
    }

    void UpdateFromActorData()
    {
        Vector3 targetPosition = new Vector2(State.Data.x, State.Data.y);

        //if (targetPosition != lastPosition)
        //{
        //    Rigid.isKinematic = true;
        //}
        //else 
        //{
        //    if(Rigid.isKinematic)
        //        Rigid.isKinematic = false;
        //}


        Rigid.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * InterpolationSpeed); 
    }

    public void PrepareAbility(Ability ability)
    {
        Animer.Play(ability.PreparingAnimation);
    }

    public void ExecuteAbility(Ability ability, Vector3 position = default, bool faceRight = default)
    {
        Animer.Play(ability.ExecuteAnimation);

        transform.position = position;
        transform.localScale = new Vector3(faceRight ? 1 : -1, 1, 1);
    }


    #region ClientControl

    public void AttemptMoveLeft()
    {
        Rigid.position += Vector2.left * MovementSpeed * Time.deltaTime;
    }

    public void AttemptMoveRight()
    {
        Rigid.position += Vector2.right * MovementSpeed * Time.deltaTime;
    }

    public void AttemptJump()
    {
        if(!IsGrounded)
        {
            return;
        }

        Rigid.AddForce(Vector2.up * JumpHeight, ForceMode2D.Impulse);
    }

    public void AttemptPrepareAbility(int abilityIndex)
    {
        if(!IsAbleToUseAbility(State.Abilities[abilityIndex].Ability))
        {
            return;
        }

        State.Abilities[abilityIndex].CurrentCastingTime = State.Abilities[abilityIndex].Ability.CastingTime;

        State.CurrentControlState = ActorState.ControlState.Immobile;

        PrepareAbility(State.Abilities[abilityIndex].Ability);

        JSONNode node = new JSONClass();
        node["abilityName"] = State.Abilities[abilityIndex].Ability.name;
        node["actorId"] = State.Data.actorId;
        SocketHandler.Instance.SendEvent("prepared_ability", node);

        State.IsPreparingAbility = true;
    }

    public void AttemptExecuteAbility(Ability ability)
    {
        JSONNode node = new JSONClass();
        node["abilityName"] = ability.name;
        node["actorId"] = State.Data.actorId;
        node["x"] = transform.position.x.ToString();
        node["y"] = transform.position.y.ToString();
        node["faceRight"] = (transform.localScale.x > 0).ToString();
        SocketHandler.Instance.SendEvent("executed_ability", node);
    }

    public bool IsAbleToUseAbility(Ability ability)
    {
        AbilityState abilityState = State.Abilities.Find(x => x.Ability.name == ability.name);

        if(State.CurrentControlState == ActorState.ControlState.Silenced || State.CurrentControlState == ActorState.ControlState.Stunned)
        {
            return false;
        }

        return abilityState.CurrentCD <= 0f && abilityState.CurrentCastingTime <= 0f && !State.IsPreparingAbility;
    }

    #endregion
}

[Serializable]
public class ActorState
{
    public ActorData Data;

    public List<AbilityState> Abilities = new List<AbilityState>();

    public bool IsPreparingAbility;

    public ControlState CurrentControlState = ControlState.Normal;



    public enum ControlState
    {
        Normal,
        Immobile,
        Silenced,
        Stunned
    }
}

[Serializable]
public class AbilityState
{
    public Ability Ability;
    public float CurrentCD;
    public float CurrentCastingTime;

    public bool IsCanDoAbility
    {
        get
        {
            return CurrentCD <= 0f && CurrentCastingTime <= 0f;
        }
    }

    public AbilityState(Ability ability)
    {
        this.Ability = ability;
    }


}
