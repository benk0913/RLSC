using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActorRollResultUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI Label;

    [SerializeField]
    Image ItemIcon;

    Actor TargetActor;

    public void SetInfo(Actor targetActor, ItemData item, int roll)
    {
        TargetActor = targetActor;
        ItemIcon.sprite = item.Icon;
        Label.text = roll.ToString();
    }

    private void LateUpdate()
    {
        if(TargetActor != null)
        {
            transform.position = Vector2.Lerp(transform.position, TargetActor.transform.position, 6f * Time.deltaTime);
        }
    }
}
