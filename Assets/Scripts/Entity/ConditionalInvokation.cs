using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ConditionalInvokation : MonoBehaviour
{
    public List<AbilityCondition> Conditions;
    public List<GameCondition> GameConditions;

    public List<AbilityParam> Actions;

    public UnityEvent EventToInvoke;

    public string RefreshEventString = "RefreshQuests";
    void Start()
    {
        CORE.Instance.SubscribeToEvent(RefreshEventString, Validate);
        
        Validate();
    }

    public void Validate()
    {
        foreach(AbilityCondition cond in Conditions)
        {
            if(!cond.IsValid(null))
            {
                return;
            }
        }
        
        foreach(GameCondition cond in GameConditions)
        {
            if(!cond.IsValid(null))
            {
                return;
            }
        }

        CORE.Instance.ActivateParams(Actions);

        EventToInvoke?.Invoke();
    }
}
