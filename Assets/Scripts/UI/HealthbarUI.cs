using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarUI : MonoBehaviour
{
    [SerializeField]
    Actor CurrentActor;

    [SerializeField]
    Image ImageFill;

    [SerializeField]
    CanvasGroup CG;

    void Update()
    {
        if (CurrentActor.State.Data.hp == CORE.Instance.Data.content.HP)
        {
            CG.alpha = 0f;
        }
        else
        {
            CG.alpha = 1f;
            ImageFill.fillAmount = Mathf.Lerp(ImageFill.fillAmount, ((float)CurrentActor.State.Data.hp / (float)CurrentActor.State.Data.MaxHP), Time.deltaTime);
        }
    }
}
