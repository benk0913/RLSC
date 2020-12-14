﻿using SimpleJSON;
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
    public ActorAI AIControl;

    [SerializeField]
    protected Animator Animer;

    [SerializeField]
    protected LayerMask GroundMask;

    [SerializeField]
    protected RaycastHit2D Rhit;

    [SerializeField]
    protected SpriteRenderer Shadow;

    [SerializeField]
    SpriteColorGroup spriteColorGroup;


    [SerializeField]
    public PassiveAbilityCollider PassiveHitCollider;

    [SerializeField]
    GameObject PlayerHalo;
    


    public bool IsGrounded;
    public bool IsInvulnerable;

    public float GroundCheckDistance = 10f;
    public float GroundedDistance= 1f;

    public float VelocityMinimumThreshold = 0.1f;

    protected Vector3 deltaPosition;
    protected Vector3 lastPosition;

    public float CustomSpeedMult = 1f;


    Ability lastAbility;
    

    public float JumpHeight = 1f;

    public float InterpolationSpeed = 1f;

    public bool IsClientControl
    {
        get
        {
            return ControlSource != ControlSourceType.Server;
        }
    }
    public ControlSourceType ControlSource;

    public bool CanAttemptToMove
    {
        get
        {
            return CanLookAround && MovementEffectRoutineInstance == null;
        }
    }

    public bool CanLookAround
    {
        get
        {
            return State.CurrentControlState != ActorState.ControlState.Immobile
            && State.CurrentControlState != ActorState.ControlState.Stunned;
        }
    }

    public void SetActorInfo(ActorData data)
    {
        this.State.Data = data;

        this.State.Abilities.Clear();

        //TODO Replace this with ability from seleted set.
        ClassJob classJob = CORE.Instance.Data.content.Classes.Find(x => x.name == State.Data.classJob);

        int abilityCount = classJob.Abilities.Count;
        for (int i = 0; i < abilityCount; i++)
        {
            string abilityName = classJob.Abilities[i];
            this.State.Abilities.Add(new AbilityState(CORE.Instance.Data.content.Abilities.Find(x => x.name == abilityName)));
        }

        RefreshControlSource();

        if (PassiveHitCollider != null)
        {
            if (!string.IsNullOrEmpty(classJob.PassiveAbility))
            {
                PassiveHitCollider.enabled = true;
                PassiveHitCollider.SetInfo(CORE.Instance.Data.content.Abilities.Find(x => x.name == classJob.PassiveAbility), this);
            }
            else
            {
                PassiveHitCollider.enabled = false;
            }
        }

        if(PlayerHalo != null)
            PlayerHalo.SetActive(State.Data.IsPlayer);
        
    }

    public void RefreshControlSource()
    {
        if (State.Data.IsPlayer)
        {
            ControlSource = ControlSourceType.Player;
        }
        else
        {
            if (AIControl.MonsterRef != null && CORE.Instance.IsBitch)
            {
                ControlSource = ControlSourceType.AI;
            }
            else
            {
                ControlSource = ControlSourceType.Server;
            }
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
        RefreshActorState();

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
                    AttemptExecuteAbility(abilityState.CurrentAbility);
                }
            }
        }

        foreach (BuffState buffState in State.Buffs)
        {
            if (buffState.CurrentLength > 0f)
            {
                buffState.CurrentLength -= Time.deltaTime;
            }
        }
    }

    void RefreshVelocity()
    {
        deltaPosition = transform.position - lastPosition;
        lastPosition = transform.position;
        Animer.SetFloat("VelocityX", deltaPosition.x);
        Animer.SetFloat("VelocityY", deltaPosition.y);
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

        Body.localScale = State.Data.faceRight ? new Vector3(-1f, 1f, 1f) : new Vector3(1f, 1f, 1f);
    }

    public void PrepareAbility(Ability ability)
    {
        Animer.Play(ability.PreparingAnimation);

        if(!string.IsNullOrEmpty(ability.PrepareAbilitySound))
            AudioControl.Instance.PlayInPosition(ability.PrepareAbilitySound,transform.position);

        if (!string.IsNullOrEmpty(ability.PrepareAbilityColliderObject))
        {
            GameObject colliderObj = ResourcesLoader.Instance.GetRecycledObject(ability.PrepareAbilityColliderObject);
            colliderObj.transform.position = transform.position;
            colliderObj.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            colliderObj.transform.localScale = new Vector3(Body.localScale.x, 1f, 1f);

            colliderObj.GetComponent<AbilityCollider>().SetInfo(ability, this);
        }
    }

    public void ExecuteAbility(Ability ability, Vector3 position = default, bool faceRight = default)
    {
        if(IsClientControl)
        {
            AbilityState abilityState = State.Abilities.Find(x=>x.CurrentAbility.name == ability.name);
            
            abilityState.CurrentCD = abilityState.CurrentAbility.CD;

            ActivateParams(abilityState.CurrentAbility.OnExecuteParams);

            lastAbility = ability;
        }

        Animer.Play(ability.ExecuteAnimation);

        if (!string.IsNullOrEmpty(ability.ExecuteAbilitySound))
            AudioControl.Instance.PlayInPosition(ability.ExecuteAbilitySound, transform.position); 

        transform.position = position;
        Body.localScale = new Vector3(faceRight ? -1 : 1, 1, 1);

        State.IsPreparingAbility = false;
        State.CurrentControlState = ActorState.ControlState.Normal;

        if(!string.IsNullOrEmpty(ability.AbilityColliderObject))
        {
            GameObject colliderObj = ResourcesLoader.Instance.GetRecycledObject(ability.AbilityColliderObject);
            colliderObj.transform.position = transform.position;
            colliderObj.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            colliderObj.transform.localScale = new Vector3(Body.localScale.x, 1f, 1f);

            colliderObj.GetComponent<AbilityCollider>().SetInfo(ability, this);
        }
    }

    public void HitAbility(Actor casterActor, Ability ability, int damage = 0, int currentHp = 0)
    {
        if (IsClientControl)
        {
            ActivateParams(ability.OnHitParams, casterActor);
        }

        if (!string.IsNullOrEmpty(ability.HitAbilitySound))
            AudioControl.Instance.PlayInPosition(ability.HitAbilitySound, transform.position);

        if (!string.IsNullOrEmpty(ability.HitAbilityColliderObject))
        {
            GameObject colliderObj = ResourcesLoader.Instance.GetRecycledObject(ability.HitAbilityColliderObject);
            colliderObj.transform.position = transform.position;
            colliderObj.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            colliderObj.transform.localScale = new Vector3(Body.localScale.x, 1f, 1f);

            colliderObj.GetComponent<AbilityCollider>().SetInfo(ability, this);
        }

        if (damage != 0)
        {
            HitLabelEntityUI label = ResourcesLoader.Instance.GetRecycledObject("HitLabelEntity").GetComponent<HitLabelEntityUI>();
            label.transform.position = transform.position;
            label.SetLabel(damage.ToString(), damage > 0 ? Color.yellow : Color.green);
            HurtEffect();
        }
        
        State.Data.hp = currentHp;
    }

    public void HurtEffect()
    {
        if (!State.IsPreparingAbility)
        {
            Animer.Play("Hurt" + UnityEngine.Random.Range(1, 5));
        }

        spriteColorGroup.SetColor(Color.black);
        CORE.Instance.DelayedInvokation(0.1f, () =>
        {
            spriteColorGroup.SetColor(Color.white);
        });
    }

    public void Ded()
    {
        Animer.Play("Dead1");
        State.CurrentControlState = ActorState.ControlState.Stunned;

        CORE.Instance.InvokeEvent("ActorDied");
    }



    public void AddBuff(Buff buff)
    {
        BuffState state = State.Buffs.Find(x => x.CurrentBuff.name == buff.name);

        if (state == null)
        {
            state = new BuffState(buff);
            State.Buffs.Add(state);

            if (!string.IsNullOrEmpty(buff.BuffColliderObject))
            {
                GameObject colliderObj = ResourcesLoader.Instance.GetRecycledObject(buff.BuffColliderObject);
                colliderObj.transform.position = transform.position;
                colliderObj.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                colliderObj.transform.localScale = new Vector3(Body.localScale.x, 1f, 1f);

                colliderObj.GetComponent<BuffCollider>().SetInfo(buff, this);

                state.EffectObject = colliderObj;
                
            }


            ActivateParams(state.CurrentBuff.OnStart);

            AddRelevantAttributes(state.CurrentBuff.Attributes);

            if (state.CurrentBuff.MakesInvulnerable) //TODO Change later to attribute ? or maybe server imp
            {
                IsInvulnerable = true;
            }
        }
        else
        {
            state.CurrentLength = buff.Length;
        }

        if (CORE.Instance.Room.PlayerActor.ActorEntity == this)
        {
            CORE.Instance.InvokeEvent("BuffStateChanged");
        }
    }

    public void RemoveBuff(Buff buff)
    {
        BuffState state = State.Buffs.Find(x => x.CurrentBuff.name == buff.name);

        if (state == null)
        {
            CORE.Instance.LogMessageError("No active buff with the name: " + buff.name);
            return;
        }

        if (state.EffectObject != null)
        {
            state.EffectObject.SetActive(false);
        }

        State.Buffs.Remove(state);

        if (state.CurrentBuff.MakesInvulnerable && State.Buffs.Find(x => x.CurrentBuff.MakesInvulnerable) == null)
        {
            IsInvulnerable = false;
        }

        if (IsClientControl)
        {
            ActivateParams(state.CurrentBuff.OnEnd);
        }

        if (IsClientControl)
        {
            RemoveRelevantAttributes(state.CurrentBuff.Attributes);
        }

        if (CORE.Instance.Room.PlayerActor.ActorEntity == this)
        {
            CORE.Instance.InvokeEvent("BuffStateChanged");
        }
    }

    #region ClientControl

    public void ActivateParams(List<AbilityParam> onExecuteParams, Actor casterActor = null)
    {
        foreach(AbilityParam param in onExecuteParams)
        {
            if(param.Type.name == "Movement")
            {
                ExecuteMovement(param.Value, casterActor);
            }
            if (param.Type.name == "Reset Last CD")
            {
                if(lastAbility == null)
                {
                    continue;
                }

                State.Abilities.Find(x => x.CurrentAbility.name == lastAbility.name).CurrentCD = 0f;
            }
            if (param.Type.name == "Change Control State")
            {
                State.CurrentControlState = (ActorState.ControlState) Enum.Parse(typeof(ActorState.ControlState), param.Value);
            }
        }
    }


    

    public void AddRelevantAttributes(AttributeData attributes)
    {
        if(attributes.MovementSpeedBoost != 0f)//TODO SPEED_IMP_SERVER
        {
            State.Data.movementSpeed += CORE.Instance.Data.content.MovementSpeed * attributes.MovementSpeedBoost;
            if(State.Data.movementSpeed < 0)
            {
                State.Data.movementSpeed = 0f;
            }
        }
    }

    public void RemoveRelevantAttributes(AttributeData attributes)
    {
        if (attributes.MovementSpeedBoost != 0f)//TODO SPEED_IMP_SERVER
        {
            State.Data.movementSpeed -= CORE.Instance.Data.content.MovementSpeed * attributes.MovementSpeedBoost;
            if (State.Data.movementSpeed < 0)
            {
                State.Data.movementSpeed = 0f;
            }
        }
    }


    public void ExecuteMovement(string movementKey, Actor casterActor = null)
    {
        if(MovementEffectRoutineInstance != null)
        {
            StopCoroutine(MovementEffectRoutineInstance);
            MovementEffectRoutineInstance = null;
        }

        switch(movementKey)
        {
            case "Disengage":
                {
                    MovementEffectRoutineInstance = StartCoroutine(MovementDisengageRoutine());
                    break;
                }
            case "DashForward":
                {
                    MovementEffectRoutineInstance = StartCoroutine(MovementDashRoutine());
                    break;
                }
            case "Pull":
                {
                    MovementEffectRoutineInstance = StartCoroutine(MovementPullRoutine(casterActor));
                    break;
                }
            case "Escape":
                {
                    MovementEffectRoutineInstance = StartCoroutine(MovementEscapeRoutine());
                    break;
                }
        }
    }


    public void AttemptMoveLeft()
    {
        if(!CanLookAround)
        {
            return;
        }

        Body.localScale = new Vector3(1f, 1f, 1f);

        if (!CanAttemptToMove)
        {
            return;
        }

        Rigid.position += Vector2.left * CustomSpeedMult * Time.deltaTime * State.Data.movementSpeed;
    }

    public void AttemptMoveRight()
    {
        if (!CanLookAround)
        {
            return;
        }

        Body.localScale = new Vector3(-1f, 1f, 1f);

        if (!CanAttemptToMove)
        {
            return;
        }

        Rigid.position += Vector2.right * CustomSpeedMult * Time.deltaTime * State.Data.movementSpeed;
    }
    
    public void AttemptJump()
    {
        if (!CanAttemptToMove)
        {
            return;
        }

        if (!IsGrounded)
        {
            return;
        }

        Rigid.AddForce(Vector2.up * JumpHeight, ForceMode2D.Impulse);

        AudioControl.Instance.PlayInPosition("_ound_bloop",transform.position);
    }


    public void AttemptPrepareAbility(int abilityIndex)
    {
        if(!IsAbleToUseAbility(State.Abilities[abilityIndex].CurrentAbility))
        {
            return;
        }

        AbilityState abilityState = State.Abilities[abilityIndex];

        abilityState.CurrentCastingTime = abilityState.CurrentAbility.CastingTime;

        State.CurrentControlState = ActorState.ControlState.Immobile;

        PrepareAbility(abilityState.CurrentAbility);

        

        JSONNode node = new JSONClass();
        node["abilityName"] = abilityState.CurrentAbility.name;
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
        node["faceRight"] = (Body.localScale.x < 0).ToString();
        SocketHandler.Instance.SendEvent("executed_ability", node);
    }


    public bool IsAbleToUseAbility(Ability ability)
    {
        AbilityState abilityState = State.Abilities.Find(x => x.CurrentAbility.name == ability.name);

        if(State.CurrentControlState == ActorState.ControlState.Silenced || State.CurrentControlState == ActorState.ControlState.Stunned)
        {
            return false;
        }

        return abilityState.CurrentCD <= 0f && abilityState.CurrentCastingTime <= 0f && !State.IsPreparingAbility;
    }

    #region MovementRoutines

    Coroutine MovementEffectRoutineInstance;
    IEnumerator MovementDisengageRoutine()
    {
        Rigid.AddForce(new Vector2(Body.localScale.x < 0 ? -1f : 1f, 1f) * 15, ForceMode2D.Impulse);

        yield return new WaitForSeconds(1f);
        
        MovementEffectRoutineInstance = null;

    }

    IEnumerator MovementDashRoutine()
    {
        
        Vector2 initDir= (Body.localScale.x < 0 ? Vector3.right : Vector3.left);

        float t = 0f;
        while(t<1f)
        {
            t += Time.deltaTime  * 2f;
            Rigid.position += initDir * CustomSpeedMult * State.Data.movementSpeed * 4f * Time.deltaTime;

            yield return new WaitForFixedUpdate();
        }

        MovementEffectRoutineInstance = null;

    }


    IEnumerator MovementPullRoutine(Actor caster)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 1.25f;
            Rigid.position = Vector2.Lerp(Rigid.position, caster.transform.position, t);

            yield return new WaitForFixedUpdate();
        }

        MovementEffectRoutineInstance = null;

    }

    IEnumerator MovementEscapeRoutine()
    {
        Rigid.AddForce(new Vector2(Body.localScale.x < 0 ? -1f : 1f, 1f) * 25, ForceMode2D.Impulse);

        yield return new WaitForSeconds(1f);

        MovementEffectRoutineInstance = null;

    }

    #endregion

    #endregion
}

[Serializable]
public class ActorState
{
    public ActorData Data;

    public List<AbilityState> Abilities = new List<AbilityState>();

    public List<BuffState> Buffs = new List<BuffState>();

    public bool IsPreparingAbility;

    public ControlState CurrentControlState
    {
        get
        {
            return _currentControlState;
        }
        set
        {
            _currentControlState = value;

            if(_currentControlState == ControlState.Stunned || _currentControlState == ControlState.Silenced)
            {
                InterruptAbilities();
            }
        }
    }
    public ControlState _currentControlState = ControlState.Normal;

    public void ClearAllObjects()
    {
        foreach(BuffState buff in Buffs)
        {
            if(buff.EffectObject != null)
            {
                buff.EffectObject.SetActive(false);
            }
        }
    }
    
    public void InterruptAbilities()
    {
        IsPreparingAbility = false;
        foreach (AbilityState state in Abilities)
        {
            if (state.CurrentCastingTime > 0f)
            {
                state.CurrentCastingTime = 0f;
            }
        }
    }

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
    public Ability CurrentAbility;
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
        this.CurrentAbility = ability;
    }


}

[Serializable]
public class BuffState
{
    public Buff CurrentBuff;
    public float CurrentLength;
    public GameObject EffectObject;

    public BuffState(Buff buff)
    {
        this.CurrentBuff = buff;
        CurrentLength = CurrentBuff.Length;
    }


}

public enum ControlSourceType
{
    Player,
    AI,
    Server
}
