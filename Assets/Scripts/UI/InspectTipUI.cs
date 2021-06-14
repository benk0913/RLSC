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

        currentActor = CORE.Instance.Room.GetNearestActor(playerActor, true);

        if (currentActor != null && Vector2.Distance(currentActor.transform.position, playerActor.transform.position) > 20)
        {
            currentActor = null;
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
            string targetString = "to inspect " + currentActor.name; ;
            if (ContentLabel.text != targetString)
            {
                KeyLabel.text = InputMap.Map["Inspect"].ToString();
                ContentLabel.text = targetString;
            }

            if (CG.alpha < 1f)
            {
                CG.alpha += 3f * Time.deltaTime;
            }


            if (!CORE.Instance.IsTyping && Input.GetKeyDown(InputMap.Map["Inspect"]))
            {
                InventoryUI.Instance.Show(currentActor.State.Data);
            }
        }
    }


}
