using EdgeworldBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActorAbilitiesPanelUI : MonoBehaviour
{
    public static ActorAbilitiesPanelUI Instance;

    Actor playerActor;

    [SerializeField]
    Transform abilitiesContainer;

    [SerializeField]
    Image CastFillBar;


    private void Awake()
    {
        Instance = this;
        CORE.Instance.SubscribeToEvent("KeybindingsChanged",()=>SetActor(playerActor));
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
        if(StartCastingRoutineInstance != null)
        {
            StopCoroutine(StartCastingRoutineInstance);
        }

        StartCastingRoutineInstance = StartCoroutine(StartCastingRoutine(time));
    }

    public void StopCasting()
    {
        if (StartCastingRoutineInstance != null)
        {
            StopCoroutine(StartCastingRoutineInstance);
        }

        CastFillBar.fillAmount = 0f;

        StartCastingRoutineInstance = null;
    }

    Coroutine StartCastingRoutineInstance;
    IEnumerator StartCastingRoutine(float castingTime)
    {
        float startValue = castingTime;
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
