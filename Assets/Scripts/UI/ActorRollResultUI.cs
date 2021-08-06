using System.Collections;
using System.Collections.Generic;
using EdgeworldBase;
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

    public string OnDisplaySound;
    public string OnIsPlayerSound;

    public void SetInfo(Actor targetActor, ItemData item, int roll)
    {
        TargetActor = targetActor;
        ItemIcon.sprite = item.Icon;
        Label.text = roll.ToString();
        if(!string.IsNullOrEmpty(OnDisplaySound))
        {
            AudioControl.Instance.PlayInPosition(OnDisplaySound,transform.position);
        }

        if(!string.IsNullOrEmpty(OnIsPlayerSound) && TargetActor.State.Data.name == CORE.PlayerActor.name)
        {
            AudioControl.Instance.PlayInPosition(OnIsPlayerSound,transform.position);
        }
    }

    private void LateUpdate()
    {
        if(TargetActor != null)
        {
            transform.position = Vector2.Lerp(transform.position, TargetActor.transform.position, 6f * Time.deltaTime);
        }
    }
}
