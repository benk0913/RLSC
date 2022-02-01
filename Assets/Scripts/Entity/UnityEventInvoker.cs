using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventInvoker : MonoBehaviour
{
    public List<UnityEvent> Events = new List<UnityEvent>();

    public void InvokeEvent(int index)
    {
        Events[index].Invoke();
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }

    public void SetAnimatorVelocityXEqualOne(Animator animer)
    {
        animer.SetFloat("VelocityX",1f);
    }

    public void SetAnimatorVelocityXEqualZero(Animator animer)
    {
        animer.SetFloat("VelocityX",0f);
    }
}
