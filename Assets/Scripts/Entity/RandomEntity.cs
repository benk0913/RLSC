using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RandomEntity : MonoBehaviour
{
    public UnityEvent OnRandom;

    public float Chance = 0.1f;

    public bool IsRandomOnEnable;

    void OnEnable()
    {
        if(IsRandomOnEnable)
        {
            InvokeRandomness();
        }
    }

    public void InvokeRandomness()
    {
        if(Random.Range(0f,1f) < Chance)
        {
            OnRandom?.Invoke();
        }
    }


}
