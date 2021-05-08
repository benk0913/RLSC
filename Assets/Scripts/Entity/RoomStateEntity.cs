using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoomStateEntity : MonoBehaviour
{
    [SerializeField]
    public string RoomStateKey;

    public int PreviousStateValue;

    public UnityEvent OnActivated;
    public UnityEvent OnDeactivated;

    private void Start()
    {
        CORE.Instance.SubscribeToEvent("RoomStatesChanged", OnStateChanged);
        ReinvokeState();
    }

    public void OnStateChanged()
    {
        if(!CORE.Instance.Room.RoomStates.ContainsKey(RoomStateKey))
        {
            return;
        }

        int newValue = CORE.Instance.Room.RoomStates[RoomStateKey];

        if(PreviousStateValue == newValue)
        {
            return;
        }

        PreviousStateValue = newValue;

        ReinvokeState();
    }

    public void ReinvokeState()
    {
        if (PreviousStateValue == 0 || PreviousStateValue % 2 == 0)
        {
            OnDeactivated.Invoke();
        }
        else
        {
            OnActivated.Invoke();
        }
    }
}
