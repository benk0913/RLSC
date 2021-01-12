using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorAbilitiesPanelUI : MonoBehaviour
{
    public static ActorAbilitiesPanelUI Instance;

    Actor playerActor;

    [SerializeField]
    Transform abilitiesContainer;

    private void Awake()
    {
        Instance = this;
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

        CORE.ClearContainer(abilitiesContainer);

        foreach(AbilityState abilityState in playerActor.State.Abilities)
        {
            AbilitySlotUI slot = ResourcesLoader.Instance.GetRecycledObject("AbilitySlotUI").GetComponent<AbilitySlotUI>();
            slot.transform.SetParent(abilitiesContainer, false);
            slot.transform.localScale = Vector3.one;
            slot.transform.position = Vector3.zero;
            slot.SetAbilityState(abilityState);
        }
    }
}
