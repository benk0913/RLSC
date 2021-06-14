using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyInvitePanelUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI TitleText;

    [SerializeField]
    Image TimerFillImage;

    [SerializeField]
    CanvasGroup CG;

    [SerializeField]
    TextMeshProUGUI KeyOption1Label;

    [SerializeField]
    TextMeshProUGUI KeyOption2Label;

    Coroutine TimerRoutineInstance;

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

    public void SetInfo(string fromPlayer)
    {
        this.CurrentFromPlayer = fromPlayer;
        

        TitleText.text = "Join "+CurrentFromPlayer+"'s Party?";



        if (TimerRoutineInstance != null)
        {
            StopCoroutine(TimerRoutineInstance);
        }

        TimerRoutineInstance = StartCoroutine(TimerRoutine(CORE.Instance.Data.content.PartyInviteTimeoutSeconds));

        KeyOption1Label.text = InputMap.Map["Vote Option 1"].ToString();
        KeyOption2Label.text = InputMap.Map["Vote Option 2"].ToString();
    }

    IEnumerator TimerRoutine(float timeLeft)
    {
        timeLeft -= 2;//Two second before the timer ends just in case

        float startTime = timeLeft;
        while (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;

            TimerFillImage.fillAmount = timeLeft / startTime;

            yield return 0;
        }

        Decline();

        TimerRoutineInstance = null;
    }

    private void Update()
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

    public void Decline()
    {
        SocketHandler.Instance.SendPartyInviteResponse(false);
        CG.interactable = false;
        LootRollPanelUI.Instance.RemovePartyInvitation(CurrentFromPlayer);
    }

    public void Accept()
    {
        SocketHandler.Instance.SendPartyInviteResponse(true);
        CG.interactable = false;
        LootRollPanelUI.Instance.RemovePartyInvitation(CurrentFromPlayer);
    }

}
