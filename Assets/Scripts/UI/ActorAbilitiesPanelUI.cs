using EdgeworldBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActorAbilitiesPanelUI : MonoBehaviour
{
    public static ActorAbilitiesPanelUI Instance;

    Actor playerActor;

    [SerializeField]
    Transform abilitiesContainer;

    [SerializeField]
    Image CastFillBar;

    [SerializeField]
    Image HPFillBar;

    [SerializeField]
    TextMeshProUGUI HPLabel;

    [SerializeField]
    Image StroberIMG;


    private void Awake()
    {
        Instance = this;
        CORE.Instance.SubscribeToEvent("KeybindingsChanged",()=>SetActor(playerActor));
    }
    
    float lastHP;
    public void Update()
    {
        if(CORE.PlayerActor != null && CORE.PlayerActor.MaxHP > 0)
        {
            if(CORE.PlayerActor.hp != lastHP)
            {
                lastHP = CORE.PlayerActor.hp;
                Animator animer = HPFillBar.transform.parent.GetComponent<Animator>();
                if(animer!=null)
                    animer.SetTrigger("Start");
            }

            HPFillBar.fillAmount = Mathf.Lerp(HPFillBar.fillAmount, (float)CORE.PlayerActor.hp/(float)CORE.PlayerActor.MaxHP,Time.deltaTime*2f);
            HPLabel.text = CORE.PlayerActor.hp + " / " + CORE.PlayerActor.MaxHP;
        }
    }
    //public void LateUpdate()
    //{
    //    if(playerActor == null)
    //    {
    //        if (CORE.Instance.Room.PlayerActor == null || CORE.Instance.Room.PlayerActor.ActorEntity == null)
    //        {
    //            return;
    //        }

    //        SetActor(CORE.Instance.Room.PlayerActor.ActorEntity);

    //    }


    //}

    public void SetActor(Actor actor)
    {
        playerActor = actor;
        if(playerActor == null)
            return;
            
        CORE.ClearContainer(abilitiesContainer);

        foreach(AbilityState abilityState in playerActor.State.Abilities)
        {
            int i = playerActor.State.Data.abilities.IndexOf(abilityState.CurrentAbility.name);
            AbilitySlotUI slot = ResourcesLoader.Instance.GetRecycledObject("AbilitySlotUI").GetComponent<AbilitySlotUI>();
            slot.transform.SetParent(abilitiesContainer, false);
            slot.transform.localScale = Vector3.one;
            slot.transform.position = Vector3.zero;
            slot.SetAbilityState(abilityState,InputMap.Map["Ability"+i].ToString());
        }
    }

    public void StartCasting(float time)
    {
        Animator animer = CastFillBar.transform.parent.GetComponent<Animator>();
        if(animer!=null)
            animer.SetTrigger("Start");

        if(StartCastingRoutineInstance != null)
        {
            StopCoroutine(StartCastingRoutineInstance);
        }

        StartCastingRoutineInstance = StartCoroutine(StartCastingRoutine(time));
    }

    public void StopCasting()
    {
        Animator animer = CastFillBar.transform.parent.GetComponent<Animator>();
        if(animer!=null)
            animer.SetTrigger("End");

        if (StartCastingRoutineInstance != null)
        {
            StopCoroutine(StartCastingRoutineInstance);
        }

        CastFillBar.fillAmount = 0f;

        StartCastingRoutineInstance = null;

        if(StroberRoutineInstance != null)
        {
            StopCoroutine(StroberRoutineInstance);
        }

        StroberRoutineInstance = StartCoroutine(StroberRoutine());
    }

    Coroutine StroberRoutineInstance;

    IEnumerator StroberRoutine()
    {
        
        while(StroberIMG.fillAmount < 1f)
        {
            StroberIMG.fillAmount = Time.deltaTime * 3f;
            yield return 0;
        }

        while( StroberIMG.fillAmount > 0)
        {
            StroberIMG.fillAmount -= Time.deltaTime;
            yield return 0;
        }

        StroberRoutineInstance = null;
    }

    Coroutine StartCastingRoutineInstance;
    IEnumerator StartCastingRoutine(float castingTime)
    {

        float startValue = castingTime;
        float t=0f;
        while(castingTime > 0f)
        {
            CastFillBar.fillAmount = 1f-(castingTime / startValue);
            castingTime -= Time.deltaTime;

            yield return 0;
        }

        CastFillBar.fillAmount = 0f;
        StartCastingRoutineInstance = null;
    }
}
