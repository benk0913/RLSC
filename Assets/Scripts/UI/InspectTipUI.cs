using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InspectTipUI : MonoBehaviour
{
    [SerializeField]
    CanvasGroup CG;

    [SerializeField]
    TextMeshProUGUI KeyLabel;

    [SerializeField]
    TextMeshProUGUI ContentLabel;

    Actor currentActor;



    private void LateUpdate()
    {
        if(CORE.Instance == null || CORE.Instance.Room == null || CORE.Instance.Room.PlayerActor == null || CORE.Instance.Room.PlayerActor.ActorEntity == null)
        {
            return;
        }

        Actor playerActor = CORE.Instance.Room.PlayerActor.ActorEntity;

        float smallestDist = 10;
        currentActor = null;
        for (int i = 0; i < CORE.Instance.Room.Actors.Count; i++)
        {
            if (CORE.Instance.Room.Actors[i].isMob)
            {
                continue;
            }

            if (CORE.Instance.Room.Actors[i].IsPlayer)
            {
                continue;
            }

            if(CORE.Instance.Room.Actors[i].ActorEntity == null)
            {
                continue;
            }

            float dist = Vector2.Distance(playerActor.transform.position, CORE.Instance.Room.Actors[i].ActorEntity.transform.position);
            if (dist < smallestDist)
            {
                currentActor = CORE.Instance.Room.Actors[i].ActorEntity;
                smallestDist = dist;
            }
        }


        if (currentActor == null)
        {
            if (CG.alpha > 0f)
            {
                CG.alpha -= 3f * Time.deltaTime;
            }
        }
        else
        {
            string targetString = "to inspect " + currentActor.State.Data.name;
            if (ContentLabel.text != targetString)
            {
                if (CORE.Instance.IsUsingJoystick)
                {
                    KeyLabel.text = "JS8";
                }
                else
                {
                    KeyLabel.text = InputMap.Map["Inspect"].ToString();
                }
                ContentLabel.text = targetString;
            }

            if (CG.alpha < 1f)
            {
                CG.alpha += 3f * Time.deltaTime;
            }


            if ((!CORE.Instance.IsTyping && Input.GetKeyDown(InputMap.Map["Inspect"])) || (CORE.Instance.IsUsingJoystick && (Input.GetButtonDown("Joystick 8") ||  Input.GetButtonDown("Joystick 11"))))
            {
                CORE.Instance.ShowInventoryUiWindow(currentActor.State.Data);
            }
        }
    }


}
