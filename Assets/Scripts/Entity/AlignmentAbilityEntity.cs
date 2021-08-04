using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AlignmentAbilityEntity : MonoBehaviour
{
    public UnityEvent OnHasAlignmentAbility;

    public UnityEvent OnDoesntHaveAlignmentAbility;

    public AlignmentAbility RelevantAbility;

    void Start()
    {
        CORE.Instance.SubscribeToEvent("AlignmentUpdated", Refresh);
        Refresh();
    }

    void Refresh()
    {
        if(RelevantAbility.IsActive)
        {
            OnHasAlignmentAbility?.Invoke();
        }
        else
        {
            OnDoesntHaveAlignmentAbility?.Invoke();
        }
    }
}
