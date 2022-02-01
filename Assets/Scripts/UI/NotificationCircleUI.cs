using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationCircleUI : MonoBehaviour
{
    public string NotificationKey;

    void Start()
    {
        CORE.Instance.SubscribeToEvent(NotificationKey,()=>
        {
            this.gameObject.SetActive(true);
        });
        CORE.Instance.SubscribeToEvent(NotificationKey+"_OFF",()=>{this.gameObject.SetActive(false);});
        this.gameObject.SetActive(false);
    }
}
