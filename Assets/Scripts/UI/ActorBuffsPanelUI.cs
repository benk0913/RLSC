using EdgeworldBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorBuffsPanelUI : MonoBehaviour
{
    public static ActorBuffsPanelUI Instance;

    Actor playerActor;

    [SerializeField]
    Transform buffsContainer;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CORE.Instance.SubscribeToEvent("BuffStateChanged", ResetActor);
        CORE.Instance.SubscribeToEvent("OrbsChanged", ResetActor);
    }

    public void LateUpdate()
    {
        if(playerActor == null)
        {
            if (CORE.Instance.Room.PlayerActor == null || CORE.Instance.Room.PlayerActor.ActorEntity == null)
            {
                return;
            }

            SetActor(CORE.Instance.Room.PlayerActor.ActorEntity);

        }


    }

    void SetActor(Actor actor)
    {
        playerActor = actor;

        CORE.ClearContainer(buffsContainer);

        foreach(Item orb in playerActor.State.Data.orbs)
        {
            BuffSlotUI slot = GetSlot();
            slot.SetOrb(orb);
        }
        foreach(BuffState buffState in playerActor.State.Buffs)
        {
            BuffSlotUI slot = GetSlot();
            slot.SetBuffState(buffState);
        }
    }

    BuffSlotUI GetSlot()
    {
        BuffSlotUI slot = ResourcesLoader.Instance.GetRecycledObject("BuffSlotUI").GetComponent<BuffSlotUI>();
        slot.transform.SetParent(buffsContainer, false);
        slot.transform.localScale = Vector3.one;
        slot.transform.position = Vector3.zero;
        return slot;
    }

    void ResetActor()
    {
        if(playerActor == null)
        {
            return;
        }

        SetActor(playerActor);
    }
}
