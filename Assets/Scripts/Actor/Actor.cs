using EdgeworldBase;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    protected ActorSkin Skin;

    [SerializeField]
    protected LayerMask GroundMask;

    [SerializeField]
    protected SpriteRenderer Shadow;

    [SerializeField]
    public SpriteColorGroup spriteColorGroup;

    [SerializeField]
    public Collider2D Collider;


    [SerializeField]
    public PassiveAbilityCollider PassiveHitCollider;

    [SerializeField]
    GameObject PlayerHalo;

    [SerializeField]
    NamePanelUI NamePanel;

    [SerializeField]
    TooltipTargetUI TooltipTarget;
    
    private Dictionary<string, Coroutine> ColliderCooldowns = new Dictionary<string, Coroutine>();
    
    public List<DamageHistoryRow> DamageHistory = new List<DamageHistoryRow>();
    private Coroutine DamageHistoryResetRoutine = null;

    public bool IsGrounded;

    public Coroutine JumpCooldownRoutine = null;

    public Collider2D CurrentGround;

    public ActorControlClient Ghost;

    public bool IsImpassive
    {
        get
        {
            return State.Data.states.ContainsKey("Impassive");
        }
    }
    public bool IsImmobile
    {
        get
        {
            return State.Data.states.ContainsKey("Immobile");
        }
    }

    public bool IsFlying;

    public bool IsGliding
    {
        get
        {
            return State.Data.states.ContainsKey("Gliding");
        }
    }

    public bool IsAttached;

    public bool IsCharmed
    {
        get
        {
            return State.Data.states.ContainsKey("Charm");
        }
    }

    public bool IsSilenced
    {
        get
        {
            return State.Data.states.ContainsKey("Silenced");
        }
    }

    public bool IsStunned
    {
        get
        {
            return State.Data.states.ContainsKey("Stunned");
        }
    }

    public bool IsInvulnerable
    {
        get
        {
            return State.Data.states.ContainsKey("Invulnerable");
        }
    }

    public bool IsDead;

    public bool InParty;

    public int ClientMovingTowardsDir;

    public float GroundCheckDistance = 10f;
    public float GroundedDistance= 1f;
    public float JumpCooldown = 0.5f;

    public float VelocityMinimumThreshold = 0.1f;

    public float KnockbackAmount = 4f;

    protected Vector2 deltaPosition;
    protected Vector2 lastPosition;


    public Ability LastAbility;

    TextBubbleUI CurrentBubble;



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
            return CanLookAround && !IsImmobile && MovementEffectRoutineInstance == null;
        }
    }

    public bool IsInputEnabled
    {
        get
        {
            return State.Data.isMob || CORE.Instance.IsInputEnabled;
        }
    }

    public bool CanInteract
    {
        get
        {
            return !IsStunned
               && !State.IsPreparingAbility
               && (State.Data.isMob || 
                       !CORE.Instance.IsLoading
                    && !CORE.Instance.IsTyping
                    && !CORE.Instance.HasWindowOpen
                    && !CameraChaseEntity.Instance.IsFocusing
                    && !DecisionContainerUI.Instance.IsActive)
               && !IsDead;
        }
    }

    public bool CanLookAround
    {
        get
        {
            return !IsStunned
            && !State.IsPreparingAbility
            && IsInputEnabled
            && !IsDead;
        }
    }
    public bool CanCastAbility
    {
        get
        {
            return !IsStunned
            && !IsSilenced
            && !State.IsPreparingAbility
            && IsInputEnabled
            && !IsDead;
        }
    }

    public void SetActorInfo(ActorData data)
    {
        State.Data = data;
        State.Data.OnRefreshStates.AddListener(RefreshStates);
        State.Data.OnRefreshAbilities.AddListener(RefreshAbilities);

        RefreshControlSource();
        if (CORE.Instance.InGame)
        {
            RefreshStates();
            RefreshAbilities();
        }
        RefreshLooks();
        RefreshName();

        // Mob abilities start with cooldown to give a breathing room.
        if (State.Data.isMob)
        {
            PutAbilitiesOnCooldown();
        }
 

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
            PlayerHalo.SetActive(State.Data.IsPlayer || !CORE.Instance.InGame);

        State.OnInterrupt.AddListener(Interrupted);

        SetIsInAir();

        if(TooltipTarget != null)
        {
            TooltipTarget.SetTooltip("<color=yellow>" + this.State.Data.name +"</color>"
                + System.Environment.NewLine + "<size=7>" + "Class: " + State.Data.ClassJobReference.name + "</color>"
                + System.Environment.NewLine + "<size=7>" + "Level: " + State.Data.level+ "</color>"
                +( InParty? System.Environment.NewLine + "<size=7><color=yellow>" + "In Party</color></size>" : "")
                + (State.Data.IsPlayer? System.Environment.NewLine + "<size=7><color=yellow>" + "This is YOU!</color></size>" : "")
                + System.Environment.NewLine + "<size=7><color=yellow>(Double Click - To Inspect)</color></size>");
        }
    }

    public void Interrupted()
    {
        if(IsDead)
        {
            return;
        }

        Animer.SetTrigger("Interrupted");
        Rigid.velocity = Vector2.zero;
    }

    public void RefreshControlSource()
    {
        if (State.Data.IsPlayer)
        {
            ControlSource = ControlSourceType.Player;
        }
        else
        {
            if (!State.Data.isCharacter && CORE.Instance.IsBitch)
            {
                ControlSource = ControlSourceType.AI;
            }
            else
            {
                ControlSource = ControlSourceType.Server;
            }
        }
    }

    public void Inspect()
    {
        CORE.Instance.ShowInventoryUiWindow(this.State.Data);
    }

    void OnEnable()
    {
        CORE.Instance.DelayedInvokation(0.1f, () => Initialize());
    }

    private void OnDestroy()
    {
        this.State.Data.OnRefreshStates.RemoveListener(RefreshStates);
        this.State.Data.OnRefreshAbilities.RemoveListener(RefreshAbilities);
    }

    private void Update()
    {

        if (!IsClientControl)
        {
            UpdateFromActorData();
        }
    }

    protected void FixedUpdate()
    {
        if(!IsClientControl)
        {
            return;
        }

        if (IsFlying)
        {
            Rigid.velocity = Vector2.Lerp(Rigid.velocity, Vector2.zero, Time.deltaTime);
        }

        if(IsGliding)
        {
            if(Rigid.velocity.y < 0)
            {
                Rigid.velocity += Vector2.up/2f;//Compensate a little...
            }
        }
        
        if(IsCharmed)
        {
            ActorData actorDat = CORE.Instance.Room.Actors.Find(x => x.actorId == State.Data.states["Charm"].linkedActorIds[0]);

            if (actorDat == null || actorDat.ActorEntity == null)
            {
                CORE.Instance.LogMessageError("No actor with actorId " + State.Data.states["Charm"].linkedActorIds[0]);
            }
            else if (actorDat == this.State.Data)
            {
                //
            }
            else
            {
                if (Vector2.Distance(actorDat.ActorEntity.transform.position, transform.position) > 1f)
                {
                    if (actorDat.ActorEntity.transform.position.x > transform.position.x)
                    {
                        AttemptMoveRight(true);
                    }
                    else
                    {
                        AttemptMoveLeft(true);
                    }
                }
            }
        }

        if(IsAttached)
        {
            ActorData actorDat = CORE.Instance.Room.Actors.Find(x => x.actorId == State.Data.states["Bind Attach"].linkedActorIds[0]);

            if (actorDat == null || actorDat.ActorEntity == null)
            {
                CORE.Instance.LogMessageError("No actor with actorId " + CORE.Instance.Room.Actors.Find(x => x.actorId == State.Data.states["Bind Buff"].linkedActorIds[0]));
            }
            else if(actorDat == this.State.Data)
            {
                //
            }
            else
            {
                Rigid.position = actorDat.ActorEntity.transform.position;
            }
            
        }

    }

    protected void LateUpdate()
    {
        RefreshVelocity();

        RefreshActorState();

        RefreshShadow();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        ContactPoint2D contact = collision.GetContact(0);
        if (Vector3.Dot(contact.normal, Vector3.up) > 0.5)
        {
            SetIsInAir(collision.collider);
        }
        // Ignore all other colliders except ground when you are dead.
        else if (IsDead)
        {
            Physics2D.IgnoreCollision(collision.collider, Collider);
        }
    }
    
    void OnCollisionStay2D(Collision2D collision)
    {
        ContactPoint2D contact = collision.GetContact(0);
        // If you touched a platform from the size but somehow got on top of it (like touching its corner), check if you are suddenly above it.
        if (!CurrentGround && Vector3.Dot(contact.normal, Vector3.up) > 0.5)
        {
            SetIsInAir(collision.collider);
        }
    }
    
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider == CurrentGround)
        {
            SetIsInAir();
        }
    }

    private void SetIsInAir(Collider2D collider = null)
    {
        CurrentGround = collider;
        IsGrounded = collider != null;
        Animer.SetBool("InAir", !IsGrounded);
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
        deltaPosition = Rigid.position - lastPosition;
        lastPosition = Rigid.position;

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

    void RefreshShadow()
    {
        RaycastHit2D CenterRhit = Physics2D.Raycast(transform.position, Vector2.down, GroundCheckDistance, GroundMask);

        if (CenterRhit)
        {
            float distanceCenter = Vector2.Distance(transform.position, CenterRhit.point);
            Shadow.transform.position = CenterRhit.point;
            Shadow.color = new Color(0f, 0f, 0f, Mathf.Lerp(0f, 0.75f, 1f - (distanceCenter / GroundCheckDistance)));
        }
        else
        {
            Shadow.color = Color.clear;
        }
    }


    void Initialize()
    {
        if (State.Data != null && State.Data.isMob && AIControl.IsBoss)
        {
            BossHealthbarUI.Instance.SetCurrentActor(this);
        }

        RefreshLooks();
    }

    void UpdateFromActorData()
    {
        Vector3 targetPosition = new Vector2(State.Data.x, State.Data.y);

        float dist = Vector2.Distance(Rigid.position, targetPosition);

        if(dist < 0.1f)
        {
            Rigid.position = targetPosition;
        }
        else
        {
            Rigid.position = Vector3.Lerp(Rigid.position, targetPosition, Time.deltaTime * InterpolationSpeed);
        }

        Body.localScale = State.Data.faceRight ? new Vector3(-1f, 1f, 1f) : new Vector3(1f, 1f, 1f);
    }

    public void PrepareAbility(Ability ability)
    {
        if (!string.IsNullOrEmpty(ability.Visuals.PreparingAnimation))
        {
            Animer.Play(ability.Visuals.PreparingAnimation);
        }

        State.IsPreparingAbility = true;

        State.PreparingAbilityCurrent = ability;

        if (!string.IsNullOrEmpty(ability.Sounds.PrepareAbilitySound))
            AudioControl.Instance.PlayInPosition(ability.Sounds.PrepareAbilitySound,transform.position);

        if (!string.IsNullOrEmpty(ability.Colliders.PrepareAbilityColliderObject))
        {
            GameObject colliderObj = AddColliderOnPosition(ability.Colliders.PrepareAbilityColliderObject);
            colliderObj.GetComponent<AbilityCollider>().SetInfo(ability, this);
            State.PreparingAbiityColliderObject = colliderObj;
        }
    }

    public void ExecuteAbility(Ability ability, Vector3 position = default, bool faceRight = default, bool castingExternal = false)
    {
        bool isCastingExternal = castingExternal || ability.IsCastingExternal;
        if(IsClientControl)
        {
            CORE.Instance.ActivateParams(ability.OnExecuteParams, null, this);

            for(int i=0;i<State.Data.equips.Keys.Count;i++)
            {
                Item item = State.Data.equips[State.Data.equips.Keys.ElementAt(i)];
                if(item == null)
                {
                    continue;
                }

                if(item.Data.OnExecuteParams == null || item.Data.OnExecuteParams.Count == 0)
                {
                    continue;
                }

                CORE.Instance.ActivateParams(item.Data.OnExecuteParams, null, this);
            }

            AbilityState abilityState = State.Abilities.Find(x => x.CurrentAbility.name == ability.name);

            if (!isCastingExternal)
            {
                if (abilityState != null)
                {
                    PutAbilityOnCooldown(abilityState);
                }

                LastAbility = ability;
            }
        }
        
        if (!string.IsNullOrEmpty(ability.Sounds.ExecuteAbilitySound))
            AudioControl.Instance.PlayInPosition(ability.Sounds.ExecuteAbilitySound, transform.position);

        if (!string.IsNullOrEmpty(ability.Visuals.ScreenEffectObject))
        {
            CORE.Instance.ShowScreenEffect(ability.Visuals.ScreenEffectObject);
        }

        if (!isCastingExternal)
        {
            if (!string.IsNullOrEmpty(ability.Visuals.ExecuteAnimation))
            {
                Animer.Play(ability.Visuals.ExecuteAnimation);
            }

            transform.position = position;
            Body.localScale = new Vector3(faceRight ? -1 : 1, 1, 1);

            State.IsPreparingAbility = false;
            State.PreparingAbiityColliderObject = null;
        }

        if (!string.IsNullOrEmpty(ability.Colliders.AbilityColliderObject))
        {
            GameObject colliderObj = AddColliderOnPosition(ability.Colliders.AbilityColliderObject);
            State.ExecutingAbilityCollider = colliderObj.GetComponent<AbilityCollider>();
            State.ExecutingAbilityCollider.SetInfo(ability, this);

            if (State.Data.isCharacter)
            {
                if(colliderObj.layer == 15)
                    colliderObj.layer = 9;
            }
            else
            {
                if (colliderObj.layer == 9)
                    colliderObj.layer = 15;
            }
        }
    }

    public void HitAbility(Actor casterActor, Ability ability)
    {
        if (IsClientControl)
        {
            CORE.Instance.ActivateParams(ability.OnHitParams, casterActor, this);

             for(int i=0;i<State.Data.equips.Keys.Count;i++)
            {
                Item item = State.Data.equips[State.Data.equips.Keys.ElementAt(i)];
                if(item == null)
                {
                    continue;
                }

                if(item.Data.OnHitParams == null || item.Data.OnHitParams.Count == 0)
                {
                    continue;
                }

                CORE.Instance.ActivateParams(item.Data.OnHitParams, casterActor, this);
            }

        }

        if (ability.Sounds.HitAbilitySoundVarriants.Count > 0)
        {
            AudioControl.Instance.PlayInPosition(ability.Sounds.HitAbilitySoundVarriants[UnityEngine.Random.Range(0,ability.Sounds.HitAbilitySoundVarriants.Count)], transform.position);
        }
        else
        {
            if (!string.IsNullOrEmpty(ability.Sounds.HitAbilitySound))
            {
                AudioControl.Instance.PlayInPosition(ability.Sounds.HitAbilitySound, transform.position);
            }
        }

        if (!string.IsNullOrEmpty(ability.Colliders.HitAbilityColliderObject))
        {
            GameObject colliderObj = AddColliderOnPosition(ability.Colliders.HitAbilityColliderObject);
            colliderObj.GetComponent<AbilityCollider>().SetInfo(ability, this);
        }
        
        if(ability.Colliders.HitConditionObjectCondition != null 
            && ability.Colliders.HitConditionObjectCondition.IsValid(this) 
            && !string.IsNullOrEmpty(ability.Colliders.HitConditionObject))
        {
            GameObject colliderObj = AddColliderOnPosition(ability.Colliders.HitConditionObject);
            colliderObj.GetComponent<AbilityCollider>().SetInfo(ability, this);
        }
    }

    public void MissAbility(Ability ability)
    {
        if (IsClientControl)
        {
            CORE.Instance.ActivateParams(ability.OnMissParams, null, this);
        }

        if (!string.IsNullOrEmpty(ability.Sounds.MissAbilitySound))
        {
            AudioControl.Instance.PlayInPosition(ability.Sounds.MissAbilitySound, transform.position);
        }

        if (!string.IsNullOrEmpty(ability.Colliders.MissAbilityColliderObject))
        {
            GameObject colliderObj = AddColliderOnPosition(ability.Colliders.MissAbilityColliderObject);
            colliderObj.GetComponent<AbilityCollider>().SetInfo(ability, this);
        }

        
    }

    public void HurtEffect(Actor source = null)
    {
        if (!State.IsPreparingAbility && !IsDead)
        {
            Animer.Play("Hurt" + UnityEngine.Random.Range(1, 5));
        }

        spriteColorGroup.SetColor(Color.black);
        CORE.Instance.DelayedInvokation(0.1f, () =>
        {
            spriteColorGroup.ResetColor();
        });


        Animer.SetFloat("WoundedBlend", Mathf.Lerp(1f, -1f,(float)State.Data.hp/ (float)State.Data.MaxHP));

        if (IsClientControl && ((AIControl != null && !AIControl.IsBoss) || State.Data.IsPlayer))
        {
            Rigid.AddForce(((transform.position - source.transform.position).normalized)*KnockbackAmount, ForceMode2D.Impulse);
        }

        if(State.Data.IsPlayer)
        {
            CORE.Instance.ShowScreenEffect("ScreenEffectHurt",null,true);
        }

        if(State.Data.ClassJobReference.UniqueHurtSounds.Count > 0)
        {
            AudioControl.Instance.PlayInPosition(State.Data.ClassJobReference.UniqueHurtSounds[UnityEngine.Random.Range(0, State.Data.ClassJobReference.UniqueHurtSounds.Count)],transform.position);
        }
    }


    public void HealEffect(Actor source = null)
    {
        spriteColorGroup.SetColor(Color.green);
        CORE.Instance.DelayedInvokation(0.1f, () =>
        {
            spriteColorGroup.ResetColor();
        });
        
        Animer.SetFloat("WoundedBlend", Mathf.Lerp(1f, -1f, (float)State.Data.hp / (float)State.Data.MaxHP));
    }


    public void Ded()
    {
        gameObject.layer = 16;
        
        Animer.Play("Dead1");
        IsDead = true;
        Animer.SetBool("IsDead", true);
        CORE.Instance.InvokeEvent("ActorDied");
        Shadow.gameObject.SetActive(false);

        if (!string.IsNullOrEmpty(State.Data.ClassJobReference.UniqueDeathSound))
        {
            AudioControl.Instance.PlayInPosition(State.Data.ClassJobReference.UniqueDeathSound, transform.position);
        }

        if (!State.Data.IsPlayer)
        {
            if (State.Data.isMob && !AIControl.IsBoss)
            {
                StartCoroutine(FadeAwayRoutine());
            }
        }
        else
        {
            CORE.Instance.ShowScreenEffect("ScreenEffectDeath");


            if (Ghost != null)
            {
                Destroy(Ghost.gameObject);
            }
            
            CORE.Instance.DelayedInvokation(3f, () => 
            {
                if(this.gameObject == null || !CORE.Instance.Room.PlayerActor.ActorEntity.IsDead || CORE.Instance.Room.Actors.Find(x=>x.actorId == State.Data.actorId) == null)
                {
                    return;
                }
                
                Ghost = Instantiate(ResourcesLoader.Instance.GetObject("ActorGhostPlayer")).GetComponent<ActorControlClient>();
                Ghost.transform.position = transform.position;
                CameraChaseEntity.Instance.ReferenceObject = Ghost.transform;
            });

            StartCoroutine(DeathSlowmo());
        }
    }

    IEnumerator DeathSlowmo()
    {
        Debug.LogError("1HAMANAMAN "+Time.timeScale);
        Time.timeScale = 0.1f;

        while(Time.timeScale < 1f)
        {
            Time.timeScale +=  (Time.fixedDeltaTime);

            Debug.LogError("HAMANAMAN "+Time.timeScale);
            yield return 0;
        }
    }

    public void Resurrect()
    {
        gameObject.layer = 8;

        IsDead = false;
        Animer.SetBool("IsDead", false);
        CORE.Instance.InvokeEvent("ActorResurrected");

        Shadow.gameObject.SetActive(true);

        if (State.Data.IsPlayer)
        {
            CORE.Instance.ShowScreenEffect("ScreenEffectRevived");

            if (Ghost != null)
            {
                Destroy(Ghost.gameObject);
                Ghost = null;
                CameraChaseEntity.Instance.ReferenceObject = this.transform;
            }
        }
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
        colliderObj.SetActive(true);
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
            BuffState existingBuff = State.Buffs.Find(x => x.CurrentBuff.BuffMaterial != null);

            if (existingBuff != null)
            {
                spriteColorGroup.SetMaterial(existingBuff.CurrentBuff.BuffMaterial);
            }
            else
            {
                spriteColorGroup.ResetMaterial();
            }
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
    
    public void ShowHurtLabel(int damage, Actor source = null)
    {
        bool isPlayerRelevant = source.State.Data.IsPlayer || this.State.Data.IsPlayer;

        HitLabelEntityUI label = ResourcesLoader.Instance.GetRecycledObject("HitLabelEntity").GetComponent<HitLabelEntityUI>();
        label.transform.position = transform.position;

        float alpha = 0.4f;
        if (State.Data.IsPlayer || (source != null && source.State.Data.IsPlayer))
        {
            alpha = 1f;
        }
        label.SetLabel(Mathf.Abs(damage).ToString(), damage >= 0 ? Color.yellow : Color.green, alpha);

        if (damage > 0)
        {
            HurtEffect(source);
        }
        else if (damage < 0)
        {
            HealEffect(source);
        }
    }

    public void AddDps(int damage)
    {
        DamageHistoryRow row = new DamageHistoryRow(damage, Time.time);
        DamageHistory.Add(row);
        if (DamageHistory.Count > 150)
        {
            DamageHistory.RemoveAt(0);
        }
        if (DamageHistoryResetRoutine != null)
        {
            StopCoroutine(DamageHistoryResetRoutine);
        }
        DamageHistoryResetRoutine = StartCoroutine(ResetDpsRoutine());
    }

    public int GetDps()
    {
        if (DamageHistory.Count == 0)
        {
            return 0;
        }
        float durationInSeconds = Mathf.Max(1, DamageHistory[DamageHistory.Count - 1].Time - DamageHistory[0].Time);
        float sum = 0;
        foreach (DamageHistoryRow row in DamageHistory)
        {
            // Consider heals as damage too
            sum += Mathf.Abs(row.Damage);
        }
        return (int)(sum / durationInSeconds);
    }

    IEnumerator FadeAwayRoutine()
    {
        yield return new WaitForSeconds(3f);

        if (!IsDead)
        {
            yield break;
        }

        float t = 1f;
        while(t>0f)
        {
            t -= 0.5f * Time.deltaTime;

            spriteColorGroup.SetAlpha(t);

            yield return 0;
        }

        Destroy(this.gameObject);
    }

    IEnumerator ResetDpsRoutine()
    {
        yield return new WaitForSeconds(8);
        DamageHistory.Clear();
    }

    public void RefreshStates()
    {
        if (State.Data.states.ContainsKey("Flying") && !IsFlying)
        {
            StartFlying();
        }
        else if (!State.Data.states.ContainsKey("Flying") && IsFlying)
        {
            StopFlying();
        }

        if(State.Data.states.ContainsKey("Bind Attach") && !IsAttached)
        {
            StartAttach();
        }
        else if(!State.Data.states.ContainsKey("Bind Attach") && IsAttached)
        {
            StopAttach();
        }

        if (State.Data.states.ContainsKey("Dead") && !IsDead)
        {
            Ded();
        }
        else if (!State.Data.states.ContainsKey("Dead") && IsDead)
        {
            Resurrect();
        }
    }

    public void RefreshAbilities()
    {
        // Interrupt before refreshing abilities, to ensure you don't prepare an ability that you can't execute because it's not there anymore. 
        State.Interrupt(false, false);

        State.Abilities.Clear();
        for (int i = 0; i < State.Data.abilities.Count; i++)
        {
            State.Abilities.Add(new AbilityState(CORE.Instance.Data.content.Abilities.Find(x => x.name == State.Data.abilities[i]),this));
        }

        if (State.Data.IsPlayer) {
            RefreshLockedSlots();
            ActorAbilitiesPanelUI.Instance.SetActor(this);
            AbilitiesUI.Instance.RefreshUI();
        }
    }

    private void RefreshLockedSlots()
    {
        int slotsLocked = 0;
        
        foreach(Item orb in State.Data.orbs)
        {
            foreach (State state in orb.Data.States)
            {
                if (state.name == "LockSlot")
                {
                    slotsLocked++;
                }
            }
        }
        slotsLocked = Mathf.Min(slotsLocked, State.Abilities.Count);

        for (int i = 0; i < slotsLocked; i++)
        {
            State.Abilities[State.Abilities.Count - 1 - i].IsOrbLocked = true;
        }
    }

    public void RefreshLooks()
    {
        if(Skin == null)
        {
            return;
        }

        Skin.RefreshLooks();
    }

    public void RefreshOrbs()
    {
        if (Skin == null)
        {
            return;
        }

        Skin.RefreshOrbs();
    }

    public void RefreshName()
    {
        if (NamePanel == null)
        {
            return;
        }

        NamePanel.Refresh(this);
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
        abilityState.CurrentCD = abilityState.CurrentAbility.CD * (1f - State.Data.attributes.CDReduction);
    }

    public bool IsColliderOnCooldown(string colliderName)
    {
        return ColliderCooldowns.ContainsKey(colliderName);
    }

    public void SetColliderOnCooldown(string colliderName, int duration)
    {
        ColliderCooldowns.Add(colliderName, StartCoroutine(ColliderCooldownRoutine(colliderName, duration)));
    }

    IEnumerator ColliderCooldownRoutine(string colliderName, int duration)
    {
        yield return new WaitForSeconds(duration);
        ColliderCooldowns.Remove(colliderName);
    }

    public void ShowTextBubble(string message)
    {
        if(CurrentBubble != null)
        {
            CurrentBubble.gameObject.SetActive(false);
            CurrentBubble = null;
        }

        CurrentBubble = ResourcesLoader.Instance.GetRecycledObject("TextBubbleUI").GetComponent<TextBubbleUI>();
        CurrentBubble.Show(transform,message,()=> { CurrentBubble = null; });
        CurrentBubble.transform.position = transform.position;
    }

    #region ClientControl

    public void ExecuteMovement(string movementKey, Actor casterActor = null)
    {
        if(MovementEffectRoutineInstance != null)
        {
            StopCoroutine(MovementEffectRoutineInstance);
            MovementEffectRoutineInstance = null;
        }

        if (this.AIControl != null && this.AIControl.ChaseBehaviour == AIChaseBehaviour.Static)
        {
            return;
        }

        // Interrupt any existing movement force added.
        Rigid.velocity = Vector2.zero;

        // Abilities that move the target
        switch (movementKey)
        {
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
            case "EngageX1.5":
                {
                    MovementEffectRoutineInstance = StartCoroutine(MovementEngageRoutine(1.5f));
                    break;
                }
            case "EngageX2":
                {
                    MovementEffectRoutineInstance = StartCoroutine(MovementEngageRoutine(2f));
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
            case "DashForwardX2":
                {
                    MovementEffectRoutineInstance = StartCoroutine(MovementDashRoutine(2f));
                    break;
                }
            case "SalamanderDash":
                {
                    MovementEffectRoutineInstance = StartCoroutine(MovementDashRoutine(2f,1.25f));
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
            case "TeleportToPlayerFar":
                {

                    Actor furthestActor = CORE.Instance.Room.GetFurthestActor(this, true);

                    if (furthestActor == null)
                    {
                        break;
                    }
                    CORE.Instance.DelayedInvokation(0.1f, () => { transform.position = furthestActor.Rigid.position; });

                    break;
                }
            case "TeleportToPlayerNear":
                {

                    Actor nearestActor = CORE.Instance.Room.GetNearestActor(this, true);

                    if (nearestActor == null)
                    {
                        break;
                    }
                    CORE.Instance.DelayedInvokation(0.1f, () => { transform.position = nearestActor.Rigid.position; });

                    break;
                }
            case "TeleportToTarget":
                {
                    CORE.Instance.DelayedInvokation(0.1f, () => { transform.position = casterActor.Rigid.position; });
                    break;
                }
            case "Backstepped":
                {

                    Actor nearestTarget = CORE.Instance.Room.GetNearestActor(this, CORE.Instance.ActiveSceneInfo.enablePvp);

                    if (nearestTarget == null)
                    {
                        break;
                    }
                    float direction = (nearestTarget.transform.position - transform.position).normalized.x;
                    float actorEdge = nearestTarget.Rigid.position.x + nearestTarget.Collider.bounds.extents.x * direction;
                    float targetOffset = Collider.bounds.size.x * 2f;
                    Vector2 targetPoint = new Vector2(actorEdge + targetOffset * direction, nearestTarget.Rigid.position.y);
                    float actorBottom = nearestTarget.Rigid.position.y - nearestTarget.Collider.bounds.extents.y;
                    Vector2 actorEdgeBottomPoint = new Vector2(actorEdge, actorBottom + GroundCheckDistance);
                    
                    // Verify there aren't walls
                    RaycastHit2D raycastBottomHit = Physics2D.Raycast(actorEdgeBottomPoint, Vector2.right * direction, targetOffset, GroundMask);
                    if (raycastBottomHit)
                    {
                        break;
                    }
                    CORE.Instance.DelayedInvokation(0.01f, () => { 
                        transform.position = targetPoint;
                        Body.localScale = new Vector3(direction, 1f, 1f);
                    });
                    
                    break;
                }

        }
    }


    public void AttemptMoveLeft(bool throughCharm = false)
    {
        if(!AttemptLookLeft())
        {
            return;
        }

        if (!CanAttemptToMove)
        {
            return;
        }

        if (!throughCharm && IsCharmed) {
            return;
        }

        ClientMovingTowardsDir = -1;

        Rigid.position += Vector2.left * Time.deltaTime * State.Data.MovementSpeed;
    }

    public void AttemptMoveRight(bool throughCharm = false)
    {
        if (!AttemptLookRight())
        {
            return;
        }

        if (!CanAttemptToMove)
        {
            return;
        }

        if (!throughCharm && IsCharmed) {
            return;
        }

        
        ClientMovingTowardsDir = 1;

        Rigid.position += Vector2.right * Time.deltaTime * State.Data.MovementSpeed;
    }

    public bool AttemptLookLeft()
    {
        if (!CanLookAround)
        {
            return false;
        }

        Body.localScale = new Vector3(1f, 1f, 1f);
        return true;
    }

    public bool AttemptLookRight()
    {
        if (!CanLookAround)
        {
            return false;
        }

        Body.localScale = new Vector3(-1f, 1f, 1f);
        return true;
    }

    public void AttemptMoveDown()
    {
        if (!CanAttemptToMove)
        {
            return;
        }
        
        if (CurrentGround && CurrentGround.GetComponent<PlatformEffector2D>() != null)
        {
            StartCoroutine(JumpDown(CurrentGround));
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
        if (JumpCooldownRoutine != null)
        {
            return;
        }
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

        JumpCooldownRoutine = StartCoroutine(JumpRoutine());
    }

    private IEnumerator JumpRoutine()
    {
        Vector2 jumpVector = Vector2.up * JumpHeight;
        jumpVector += jumpVector * State.Data.attributes.JumpHeight;
        Rigid.AddForce(jumpVector, ForceMode2D.Impulse);

        AudioControl.Instance.PlayInPosition("_ound_bloop",transform.position);

        yield return new WaitForSeconds(JumpCooldown);

        JumpCooldownRoutine = null;
    }

    private IEnumerator JumpDown(Collider2D Ground)
    {
        Physics2D.IgnoreCollision(Ground, Collider, true);
        yield return new WaitForSeconds(0.5f);
        Physics2D.IgnoreCollision(Ground, Collider, false);
    }

    public void AttemptPrepareAbility(int abilityIndex)
    {
        if(!IsAbleToUseAbility(State.Abilities[abilityIndex].CurrentAbility))
        {
            return;
        }

        AbilityState abilityState = State.Abilities[abilityIndex];

        abilityState.CurrentCastingTime = abilityState.CurrentAbility.CastingTime * (1f - State.Data.attributes.CTReduction);

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
        if(!CanCastAbility)
        {
            return false;
        }

        if(ability.OnlyIfGrounded && !IsGrounded)
        {
            TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance(ability.name + " can only be cast from the ground!",Color.red));
            return false;
        }

        AbilityState abilityState = State.Abilities.Find(x => x.CurrentAbility.name == ability.name);
        return abilityState.IsCanDoAbility;
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

    public void StartAttach()
    {
        IsAttached = true;
    }

    public void StopAttach()
    {
        IsAttached = false;
    }

    #region MovementRoutines

    Coroutine MovementEffectRoutineInstance;
    IEnumerator MovementDisengageRoutine(float multiplier = 1f)
    {
        Rigid.AddForce(new Vector2(Body.localScale.x < 0 ? -1f : 1f, 1f) * 15 * multiplier, ForceMode2D.Impulse);

        yield return new WaitForSeconds(1f);
        
        MovementEffectRoutineInstance = null;

    }

    IEnumerator MovementEngageRoutine(float power = 1f)
    {
        Rigid.AddForce(new Vector2(Body.localScale.x < 0 ? 1f : -1f, 2.2f) * 15 * power, ForceMode2D.Impulse);

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

    IEnumerator MovementDashRoutine(float duration = 1f, float speed = 1f)
    {
        
        Vector2 initDir= (Body.localScale.x < 0 ? Vector3.right : Vector3.left);

        float t = 0f;
        while(t<1f)
        {
            t += Time.deltaTime  * 2f * (1f/duration);
            Rigid.position += initDir * State.Data.MovementSpeed * speed * 4f * Time.deltaTime;

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
        Rigid.AddForce(new Vector2(-caster.Body.transform.localScale.x * 25,25f), ForceMode2D.Impulse);

        yield return new WaitForSeconds(1f);

        MovementEffectRoutineInstance = null;
    }


    IEnumerator MovementPullRoutine(Actor caster)
    {
        // Pull slightly up to ensure they are on the same platform.
        Vector2 Pullpoint = caster.transform.position + new Vector3(0, 0.01f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 1.25f;
            Rigid.position = Vector2.Lerp(Rigid.position, Pullpoint, t);

            yield return new WaitForFixedUpdate();
        }
        transform.position = Pullpoint;

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
    public AbilityCollider ExecutingAbilityCollider;

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
    
    public void Interrupt(bool putAbilityOnCd, bool putAllAbilitiesOnCd)
    {
        foreach (AbilityState state in Abilities)
        {
            if (state.CurrentCastingTime > 0f)
            {
                state.CurrentCastingTime = 0f;
                if (putAbilityOnCd) {
                    Data.ActorEntity.PutAbilityOnCooldown(state);
                }
            }
            if (putAllAbilitiesOnCd) {
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

        if(ExecutingAbilityCollider != null && ExecutingAbilityCollider.RemoveOnInterrupt)
        {
            if (!string.IsNullOrEmpty(ExecutingAbilityCollider.AbilitySource.Sounds.ExecuteAbilitySound))
            {
                AudioControl.Instance.StopSound(ExecutingAbilityCollider.AbilitySource.Sounds.ExecuteAbilitySound);
            }

            ExecutingAbilityCollider.gameObject.SetActive(false);
            ExecutingAbilityCollider = null;
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

    public Actor OfActor;

    public bool IsOrbLocked = false;

    public bool IsCanDoAbility
    {
        get
        {
            return CurrentCD <= 0f && CurrentCastingTime <= 0f && !IsAbilityLocked;
        }
    }

    public bool IsLevelLocked
    {
        get
        {
            return OfActor.State.Data.IsPlayer && OfActor.State.Data.ClassJobReference.Abilities.IndexOf(OfActor.State.Data.ClassJobReference.Abilities.Find(x=>x == CurrentAbility.name)) > OfActor.State.Data.level + 1;
        }
    }

    public bool IsAbilityLocked
    {
        get
        {
            return IsLevelLocked || IsOrbLocked;
        }
    }

    public AbilityState(Ability ability, Actor ofActor)
    {
        this.CurrentAbility = ability;
        this.OfActor = ofActor;
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

public class DamageHistoryRow
{
    public int Damage;
    public float Time;

    public DamageHistoryRow(int damage, float time)
    {
        Damage = damage;
        Time = time;
    }
}