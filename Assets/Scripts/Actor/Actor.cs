using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    Collider2D Collider;


    [SerializeField]
    public PassiveAbilityCollider PassiveHitCollider;

    [SerializeField]
    GameObject PlayerHalo;
    


    public bool IsGrounded;

    public bool IsImpassive
    {
        get
        {
            return State.Data.States.ContainsKey("Impassive");
        }
    }

    public bool IsFlying;

    public bool IsStunned;

    public bool IsSilenced;

    public bool IsDead;

    public int ClientMovingTowardsDir;

    public float GroundCheckDistance = 10f;
    public float GroundedDistance= 1f;

    public float VelocityMinimumThreshold = 0.1f;

    protected Vector3 deltaPosition;
    protected Vector3 lastPosition;


    public Ability LastAbility;



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
            return !State.Data.States.ContainsKey("Immobile")
            && !IsStunned
            && !State.IsPreparingAbility
            && !IsDead;
               
        }
    }

    public void SetActorInfo(ActorData data)
    {
        this.State.Data = data;
        this.State.Data.OnRefreshStates.AddListener(RefreshStates);
        this.State.Data.OnRefreshAbilities.AddListener(RefreshAbilities);

        RefreshStates();
        RefreshAbilities();
        PutAbilitiesOnCooldown();
 
        RefreshControlSource();

        ClassJob classJob = CORE.Instance.Data.content.Classes.Find(x => x.name == State.Data.classJob);

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

        this.State.OnInterrupt.AddListener(Interrupted);
        
    }

    public void Interrupted()
    {
        if(IsDead)
        {
            return;
        }

        Animer.SetTrigger("Interrupted");
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


    private void OnDestroy()
    {
        this.State.Data.OnRefreshStates.RemoveListener(RefreshStates);
        this.State.Data.OnRefreshAbilities.RemoveListener(RefreshAbilities);
    }

    protected void FixedUpdate()
    {

        if (IsFlying)
        {
            Rigid.velocity = Vector2.Lerp(Rigid.velocity, Vector2.zero, Time.deltaTime);
        }

    }

    protected void Update()
    {
        if (!IsClientControl)
        {
            UpdateFromActorData();
        }
    }

    protected void LateUpdate()
    {
        RefreshVelocity();

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

        Animer.SetBool("Stunned", IsStunned);
        Animer.SetBool("Dead", IsDead);
    }

    void RefreshVelocity()
    {
        deltaPosition = transform.position - lastPosition;
        lastPosition = transform.position;

        if (IsClientControl)
        {
            Animer.SetFloat("VelocityX", ClientMovingTowardsDir);
            Animer.SetFloat("VelocityY", deltaPosition.y);
        }
        else
        {
            Animer.SetFloat("VelocityX", deltaPosition.x);
            Animer.SetFloat("VelocityY", deltaPosition.y);
        }

        ClientMovingTowardsDir = 0;
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

    public void SnapToPosition()
    {
        Rigid.position = new Vector2(State.Data.x, State.Data.y);
    }

    public void PrepareAbility(Ability ability)
    {
        Animer.Play(ability.PreparingAnimation);

        State.IsPreparingAbility = true;

        State.PreparingAbilityCurrent = ability;

        if (!string.IsNullOrEmpty(ability.PrepareAbilitySound))
            AudioControl.Instance.PlayInPosition(ability.PrepareAbilitySound,transform.position);

        if (!string.IsNullOrEmpty(ability.PrepareAbilityColliderObject))
        {
            GameObject colliderObj = AddColliderOnPosition(ability.PrepareAbilityColliderObject);
            colliderObj.GetComponent<AbilityCollider>().SetInfo(ability, this);
            State.PreparingAbiityColliderObject = colliderObj;
        }
    }

    public void ExecuteAbility(Ability ability, Vector3 position = default, bool faceRight = default)
    {
        if(IsClientControl)
        {

            CORE.Instance.ActivateParams(ability.OnExecuteParams, null, this);

            AbilityState abilityState = State.Abilities.Find(x => x.CurrentAbility.name == ability.name);

            if (!ability.IsCastingExternal)
            {
                if (abilityState != null)
                {
                    PutAbilityOnCooldown(abilityState);
                }

                LastAbility = ability;
            }
        }
        
        if (!string.IsNullOrEmpty(ability.ExecuteAbilitySound))
            AudioControl.Instance.PlayInPosition(ability.ExecuteAbilitySound, transform.position);

        if (!string.IsNullOrEmpty(ability.ScreenEffectObject))
        {
            CORE.Instance.ShowScreenEffect(ability.ScreenEffectObject);
        }

        if (!ability.IsCastingExternal)
        {
            Animer.Play(ability.ExecuteAnimation);

            transform.position = position;
            Body.localScale = new Vector3(faceRight ? -1 : 1, 1, 1);

            State.IsPreparingAbility = false;
            State.PreparingAbiityColliderObject = null;
        }

        if (!string.IsNullOrEmpty(ability.AbilityColliderObject))
        {
            GameObject colliderObj = AddColliderOnPosition(ability.AbilityColliderObject);
            colliderObj.GetComponent<AbilityCollider>().SetInfo(ability, this);
        }
    }

    public void HitAbility(Actor casterActor, Ability ability)
    {
        if (IsClientControl)
        {
            CORE.Instance.ActivateParams(ability.OnHitParams, casterActor, this);
        }

        if (!string.IsNullOrEmpty(ability.HitAbilitySound))
        {
            AudioControl.Instance.PlayInPosition(ability.HitAbilitySound, transform.position);
        }

        if (!string.IsNullOrEmpty(ability.HitAbilityColliderObject))
        {
            GameObject colliderObj = AddColliderOnPosition(ability.HitAbilityColliderObject);
            colliderObj.GetComponent<AbilityCollider>().SetInfo(ability, this);
        }
        
        if(ability.HitConditionObjectCondition != null 
            && ability.HitConditionObjectCondition.IsValid(this) 
            && !string.IsNullOrEmpty(ability.HitConditionObject))
        {
            GameObject colliderObj = AddColliderOnPosition(ability.HitConditionObject);
            colliderObj.GetComponent<AbilityCollider>().SetInfo(ability, this);
        }
    }

    public void MissAbility(Ability ability)
    {
        if (IsClientControl)
        {
            CORE.Instance.ActivateParams(ability.OnMissParams, null, this);
        }

        if (!string.IsNullOrEmpty(ability.MissAbilitySound))
        {
            AudioControl.Instance.PlayInPosition(ability.MissAbilitySound, transform.position);
        }

        if (!string.IsNullOrEmpty(ability.MissAbilityColliderObject))
        {
            GameObject colliderObj = AddColliderOnPosition(ability.MissAbilityColliderObject);
            colliderObj.GetComponent<AbilityCollider>().SetInfo(ability, this);
        }

        
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
        IsDead = true;
        Animer.SetBool("IsDead", true);
        CORE.Instance.InvokeEvent("ActorDied");
    }

    public void Resurrect()
    {
        IsDead = false;
        Animer.SetBool("IsDead", false);
        CORE.Instance.InvokeEvent("ActorResurrected");
    }

    public void AddBuff(Buff buff, float duration)
    {
        BuffState state = State.Buffs.Find(x => x.CurrentBuff.name == buff.name);

        if (state == null)
        {
            state = new BuffState(buff, duration);
            State.Buffs.Add(state);

            if (!string.IsNullOrEmpty(buff.BuffColliderObject))
            {
                GameObject colliderObj = AddColliderOnPosition(buff.BuffColliderObject);
                colliderObj.GetComponent<BuffCollider>().SetInfo(state, this);

                state.EffectObject = colliderObj;
                
            }

            if(!string.IsNullOrEmpty(buff.OnStartSound))
            {
                AudioControl.Instance.PlayInPosition(buff.OnStartSound,transform.position);
            }

            CORE.Instance.ActivateParams(state.CurrentBuff.OnStart, null, this);

            if(buff.BuffMaterial != null)
            {
                spriteColorGroup.SetMaterial(buff.BuffMaterial);
            }
        }
        else
        {
            state.CurrentLength = duration;
        }

        if (CORE.Instance.Room.PlayerActor.ActorEntity == this)
        {
            CORE.Instance.InvokeEvent("BuffStateChanged");
        }

        CORE.Instance.InvokeEvent("ActorAddedBuff");
    }

    public GameObject AddColliderOnPosition(string colliderKey)
    {
        GameObject colliderObj = ResourcesLoader.Instance.GetRecycledObject(colliderKey);
        colliderObj.transform.position = transform.position;
        colliderObj.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        colliderObj.transform.localScale = new Vector3(Body.localScale.x, 1f, 1f);
        return colliderObj;
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

        if (!string.IsNullOrEmpty(buff.OnEndSound))
        {
            AudioControl.Instance.PlayInPosition(buff.OnEndSound, transform.position);
        }

        State.Buffs.Remove(state);

        if (buff.BuffMaterial != null)
        {
            spriteColorGroup.ResetMaterial();
        }

        if (IsClientControl)
        {
            CORE.Instance.ActivateParams(state.CurrentBuff.OnEnd, null, this);
        }

        if (CORE.Instance.Room.PlayerActor.ActorEntity == this)
        {
            CORE.Instance.InvokeEvent("BuffStateChanged");
        }

        CORE.Instance.InvokeEvent("ActorRemovedBuff");
    }
    
    public void ShowHurtLabel(int damage)
    {
        HitLabelEntityUI label = ResourcesLoader.Instance.GetRecycledObject("HitLabelEntity").GetComponent<HitLabelEntityUI>();
        label.transform.position = transform.position;
        label.SetLabel(Mathf.Abs(damage).ToString(), damage > 0 ? Color.yellow : Color.green);
        HurtEffect();
    }

    public void RefreshStates()
    {
        if (State.Data.States.ContainsKey("Stunned") && !IsStunned)
        {
            State.InterruptAbilities();
            IsStunned = true;
        }
        else if (!State.Data.States.ContainsKey("Stunned") && IsStunned)
        {
            IsStunned = false;
        }

        if(State.Data.States.ContainsKey("Silenced") && !IsSilenced)
        {
            IsSilenced = true;
            State.InterruptAbilities();
        }
        else if (!State.Data.States.ContainsKey("Silenced") && IsSilenced)
        {
            IsSilenced = false;
        }

        if (State.Data.States.ContainsKey("Flying") && !IsFlying)
        {
            StartFlying();
        }
        else if (!State.Data.States.ContainsKey("Flying") && IsFlying)
        {
            StopFlying();
        }

        if (State.Data.States.ContainsKey("Dead") && !IsDead)
        {
            Ded();
            State.InterruptAbilities();
        }
        else if (!State.Data.States.ContainsKey("Dead") && IsDead)
        {
            Resurrect();
        }
    }

    public void RefreshAbilities()
    {
        this.State.Abilities.Clear();
        for (int i = 0; i < State.Data.abilities.Count; i++)
        {
            this.State.Abilities.Add(new AbilityState(CORE.Instance.Data.content.Abilities.Find(x => x.name == State.Data.abilities[i])));
        }

        if (State.Data.IsPlayer)
            ActorAbilitiesPanelUI.Instance.SetActor(this);
    }

    public void PutAbilitiesOnCooldown()
    {
        foreach (AbilityState abilityState in this.State.Abilities)
        {
            PutAbilityOnCooldown(abilityState);
        }
    }

    public void PutAbilityOnCooldown(AbilityState abilityState)
    {
        abilityState.CurrentCD = abilityState.CurrentAbility.CD * (1f - State.Data.Attributes.CDReduction);
    }

    #region ClientControl

    public void ExecuteMovement(string movementKey, Actor casterActor = null)
    {
        if(MovementEffectRoutineInstance != null)
        {
            StopCoroutine(MovementEffectRoutineInstance);
            MovementEffectRoutineInstance = null;
        }

        switch(movementKey)
        {
            case "InterruptMovement":
                {
                    Rigid.velocity = Vector2.zero;
                    break;
                }
            case "Disengage":
                {
                    MovementEffectRoutineInstance = StartCoroutine(MovementDisengageRoutine());
                    break;
                }
            case "DisengageX1.5":
                {
                    MovementEffectRoutineInstance = StartCoroutine(MovementDisengageRoutine(1.5f));
                    break;
                }
            case "Engage":
                {
                    MovementEffectRoutineInstance = StartCoroutine(MovementEngageRoutine());
                    break;
                }
            case "Earth Push":
                {
                    MovementEffectRoutineInstance = StartCoroutine(MovementEarthPushRoutine(casterActor));
                    break;
                }
            case "Wind Push":
                {
                    MovementEffectRoutineInstance = StartCoroutine(MovementWindPushRoutine(casterActor));
                    break;
                }
            case "DashForward":
                {
                    MovementEffectRoutineInstance = StartCoroutine(MovementDashRoutine());
                    break;
                }
            case "DashUpwards":
                {
                    MovementEffectRoutineInstance = StartCoroutine(MovementDashUpwardsRoutine());
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
            case "Pounce":
                {
                    MovementEffectRoutineInstance = StartCoroutine(MovementPounceRoutine());
                    break;
                }
            case "Backstepped":
                {
                    MovementEffectRoutineInstance = StartCoroutine(MovementBacksteppedRoutine(casterActor));
                    break;
                }
            case "TeleportToFriendFar":
                {

                    float furthestDist = 0f;
                    Actor furthestActor = null;
                    for(int i=0;i<CORE.Instance.Room.Actors.Count;i++)
                    {
                        if(CORE.Instance.Room.Actors[i].ActorEntity == this)
                        {
                            continue;
                        }

                        if(CORE.Instance.Room.Actors[i].isMob)
                        {
                            continue;
                        }

                        float currentDist = Vector2.Distance(transform.position, CORE.Instance.Room.Actors[i].ActorEntity.transform.position);
                        if (currentDist > furthestDist)
                        {
                            furthestDist = currentDist;
                            furthestActor = CORE.Instance.Room.Actors[i].ActorEntity;
                        }
                    }

                    if (furthestActor == null)
                    {
                        TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("No friendly actor to move towards...", Color.red, 3f));
                        break;
                    }

                    CORE.Instance.DelayedInvokation(0.1f, () => { transform.position = furthestActor.Rigid.position; });
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

        ClientMovingTowardsDir = -1;

        Rigid.position += Vector2.left * Time.deltaTime * State.Data.MovementSpeed;
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

        ClientMovingTowardsDir = 1;

        Rigid.position += Vector2.right * Time.deltaTime * State.Data.MovementSpeed;
    }

    public void AttemptMoveDown()
    {
        if (!CanLookAround)
        {
            return;
        }

        if (!CanAttemptToMove)
        {
            return;
        }
        
        if(!IsFlying)
        {
            return;
        }

        Rigid.position += Vector2.down * Time.deltaTime * State.Data.MovementSpeed;
    }

    public void AttemptMoveUp()
    {
        if (!CanLookAround)
        {
            return;
        }

        if (!CanAttemptToMove)
        {
            return;
        }

        if (!IsFlying)
        {
            return;
        }

        Rigid.position += Vector2.up * Time.deltaTime * State.Data.MovementSpeed;
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

        if(IsFlying)
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

        abilityState.CurrentCastingTime = abilityState.CurrentAbility.CastingTime * (1f - State.Data.Attributes.CTReduction);

        PrepareAbility(abilityState.CurrentAbility);

        JSONNode node = new JSONClass();
        node["abilityName"] = abilityState.CurrentAbility.name;
        node["actorId"] = State.Data.actorId;
        SocketHandler.Instance.SendEvent("prepared_ability", node);

        State.IsPreparingAbility = true;
        State.PreparingAbilityCurrent = abilityState.CurrentAbility;
    }

    public void AttemptExecuteAbility(Ability ability, Actor caster = null)
    {
        JSONNode node = new JSONClass();
        node["abilityName"] = ability.name;
        node["actorId"] = caster == null? this.State.Data.actorId : caster.State.Data.actorId;
        node["x"] = transform.position.x.ToString();
        node["y"] = transform.position.y.ToString();
        node["faceRight"] = (Body.localScale.x < 0).ToString();
        SocketHandler.Instance.SendEvent("executed_ability", node);
    }


    public bool IsAbleToUseAbility(Ability ability)
    {
        AbilityState abilityState = State.Abilities.Find(x => x.CurrentAbility.name == ability.name);

        if(IsStunned || IsSilenced || IsDead || State.IsPreparingAbility)
        {
            return false;
        }

        if(ability.OnlyIfGrounded && !IsGrounded)
        {
            TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance(ability.name + " can only be cast from the ground!",Color.red));
            return false;
        }

        return abilityState.CurrentCD <= 0f && abilityState.CurrentCastingTime <= 0f && !State.IsPreparingAbility;
    }


    public void StartFlying()
    {
        Rigid.velocity = Vector2.zero;
        Rigid.gravityScale = 0f;
        Animer.SetBool("IsFlying", true);
        IsFlying = true;
    }

    public void StopFlying()
    {
        Rigid.velocity = Vector2.zero;
        Rigid.gravityScale = 1f;
        Animer.SetBool("IsFlying", false);
        IsFlying = false;
    }

    public void HaltAbility(int haltAbilityIndex)
    {

        if (State.Abilities[haltAbilityIndex].CurrentAbility != State.PreparingAbilityCurrent)
        {
            return;
        }

        HaltAbility();
    }

    public void HaltAbility()
    {
        if (!State.IsPreparingAbility)
        {
            return;
        }

        State.IsPreparingAbility = false;
        State.PreparingAbiityColliderObject = null;
    }

    #region MovementRoutines

    Coroutine MovementEffectRoutineInstance;
    IEnumerator MovementDisengageRoutine(float multiplier = 1f)
    {
        Rigid.AddForce(new Vector2(Body.localScale.x < 0 ? -1f : 1f, 1f) * 15 * multiplier, ForceMode2D.Impulse);

        yield return new WaitForSeconds(1f);
        
        MovementEffectRoutineInstance = null;

    }

    IEnumerator MovementEngageRoutine()
    {
        Rigid.AddForce(new Vector2(Body.localScale.x < 0 ? 1f : -1f, 2f) * 15, ForceMode2D.Impulse);

        yield return new WaitForSeconds(1f);

        MovementEffectRoutineInstance = null;

    }

    IEnumerator MovementDashUpwardsRoutine()
    {
        Vector2 initDir = Vector2.up;
        

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 1;
            Rigid.position += initDir* State.Data.MovementSpeed *4* (1f-t) * Time.deltaTime;

            Rigid.velocity = Vector2.zero;

            yield return new WaitForFixedUpdate();
        }
  
        Rigid.velocity = Vector2.zero;

        MovementEffectRoutineInstance = null;

    }
    
    IEnumerator MovementDashRoutine()
    {
        
        Vector2 initDir= (Body.localScale.x < 0 ? Vector3.right : Vector3.left);

        float t = 0f;
        while(t<1f)
        {
            t += Time.deltaTime  * 2f;
            Rigid.position += initDir * State.Data.MovementSpeed * 4f * Time.deltaTime;

            yield return new WaitForFixedUpdate();
        }

        MovementEffectRoutineInstance = null;

    }

    IEnumerator MovementEarthPushRoutine(Actor caster)
    {
        Rigid.AddForce(new Vector2(caster.transform.position.x < transform.position.x ? 1f : -1f, 1f) * 15, ForceMode2D.Impulse);


        yield return new WaitForSeconds(1f);

        MovementEffectRoutineInstance = null;
    }

    IEnumerator MovementWindPushRoutine(Actor caster)
    {
        Rigid.AddForce(new Vector2(caster.transform.position.x < transform.position.x ? 1f : -1f, 1f) * 25, ForceMode2D.Impulse);

        yield return new WaitForSeconds(1f);

        MovementEffectRoutineInstance = null;
    }


    IEnumerator MovementPullRoutine(Actor caster)
    {
        Vector2 Pullpoint = caster.transform.position;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 1.25f;
            Rigid.position = Vector2.Lerp(Rigid.position, Pullpoint, t);

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


    IEnumerator MovementPounceRoutine()
    {
        Vector2 initDir;
        
        if(IsGrounded)
            initDir = new Vector2((Body.localScale.x < 0 ? Vector3.right : Vector3.left).x,1f);
        else
            initDir = new Vector2((Body.localScale.x < 0 ? Vector3.right : Vector3.left).x,-1f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 4f;
            Rigid.position += initDir * State.Data.MovementSpeed * 6f * Time.deltaTime;

            yield return new WaitForFixedUpdate();
        }

        MovementEffectRoutineInstance = null;

    }

    IEnumerator MovementBacksteppedRoutine(Actor casterActor)
    {
        Vector2 stepDir;

        stepDir = transform.position - casterActor.transform.position;

        Vector2 targetPoint = new Vector2(Rigid.position.x + (Collider.bounds.extents.x * (stepDir).normalized.x * 1.1f), Rigid.position.y);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 4f;

            casterActor.Rigid.position = Vector2.Lerp(casterActor.Rigid.position, targetPoint, t);

            yield return new WaitForFixedUpdate();
        }

        casterActor.Body.localScale = new Vector3(stepDir.x > 0f? 1f:-1f, 1f, 1f);

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

    public Ability PreparingAbilityCurrent;

    public GameObject PreparingAbiityColliderObject;

    public UnityEvent OnInterrupt = new UnityEvent();

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
        foreach (AbilityState state in Abilities)
        {
            if (state.CurrentCastingTime > 0f)
            {
                state.CurrentCastingTime = 0f;
                Data.ActorEntity.PutAbilityOnCooldown(state);
            }
        }

        if(IsPreparingAbility)
        {
            if (PreparingAbiityColliderObject != null)
            {
                PreparingAbiityColliderObject.SetActive(false);
            }

            GameObject colliderObj = Data.ActorEntity.AddColliderOnPosition("InterruptAbilityCollider");
            colliderObj.GetComponent<AbilityCollider>().SetInfo(null, Data.ActorEntity);
        }

        IsPreparingAbility = false;
        PreparingAbiityColliderObject = null;

        OnInterrupt?.Invoke();
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
    public float Length;
    public float CurrentLength;
    public GameObject EffectObject;

    public BuffState(Buff buff, float duration)
    {
        this.CurrentBuff = buff;
        CurrentLength = duration;
        Length = duration;
    }
}

public enum ControlSourceType
{
    Player,
    AI,
    Server
}
