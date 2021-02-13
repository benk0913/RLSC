using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField]
    HealthbarUI HPBar;

    [SerializeField]
    TextMeshProUGUI NameLabel;

    private void Start()
    {
        CORE.Instance.SubscribeToEvent("NewSceneLoaded", Refresh);
    }

    public void Refresh()
    {
        //TODO Kinda dirty implementation?
        CORE.Instance.ConditionalInvokation(X => !string.IsNullOrEmpty(CORE.Instance.Room.PlayerActor.name), () =>
        {
            NameLabel.text = CORE.Instance.Room.PlayerActor.ActorEntity.State.Data.name;
        });

        CORE.Instance.ConditionalInvokation(X => CORE.Instance.Room.PlayerActor.ActorEntity != null, () =>
        {
            HPBar.SetCurrentActor(CORE.Instance.Room.PlayerActor.ActorEntity);

        });
    }

}
