using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyInvitePanelUI : MonoBehaviour
{
    [SerializeField]
    protected TextMeshProUGUI TitleText;

    [SerializeField]
    protected Image TimerFillImage;

    [SerializeField]
    protected CanvasGroup CG;

    [SerializeField]
    protected TextMeshProUGUI KeyOption1Label;

    [SerializeField]
    protected TextMeshProUGUI KeyOption2Label;

    protected Coroutine TimerRoutineInstance;

    public string CurrentFromPlayer;

    public bool IsTopActiveRoll
    {
        get
        {
            for (int i = 0; i < LootRollPanelUI.Instance.transform.childCount; i++)
            {
                if (LootRollPanelUI.Instance.transform.GetChild(i).gameObject.activeInHierarchy)
                {
                    if (LootRollPanelUI.Instance.transform.GetChild(i).gameObject == this.gameObject)
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }
    }

    public virtual void SetInfo()
    {
        CG.interactable = true;

        if (TimerRoutineInstance != null)
        {
            StopCoroutine(TimerRoutineInstance);
        }

        TimerRoutineInstance = StartCoroutine(TimerRoutine(CORE.Instance.Data.content.PartyInviteTimeoutSeconds));

        KeyOption1Label.text = InputMap.Map["Vote Option 1"].ToString();
        KeyOption2Label.text = InputMap.Map["Vote Option 2"].ToString();
    }

    public virtual void SetInfo(string fromPlayer)
    {
        this.CurrentFromPlayer = fromPlayer;
        TitleText.text = "Join "+CurrentFromPlayer+"'s Party?";
        SetInfo();
    }

    protected IEnumerator TimerRoutine(float timeLeft)
    {
        timeLeft -= 2;//Two second before the timer ends just in case

        float startTime = timeLeft;
        while (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;

            TimerFillImage.fillAmount = timeLeft / startTime;

            yield return 0;
        }

        TimerRoutineInstance = null;
    }

    protected virtual void Update()
    {
        if (!CG.interactable)
        {
            return;
        }

        if (Input.GetKeyDown(InputMap.Map["Vote Option 1"]))
        {
            if (IsTopActiveRoll)
                Accept();
        }
        else if (Input.GetKeyDown(InputMap.Map["Vote Option 2"]))
        {
            if (IsTopActiveRoll)
                Decline();
        }
    }

    public virtual void Decline()
    {
        SocketHandler.Instance.SendPartyInviteResponse(false);
        CG.interactable = false;
        LootRollPanelUI.Instance.RemovePartyInvitation(CurrentFromPlayer);
    }

    public virtual void Accept()
    {
        SocketHandler.Instance.SendPartyInviteResponse(true);
        CG.interactable = false;
        LootRollPanelUI.Instance.RemovePartyInvitation(CurrentFromPlayer);
    }

}
