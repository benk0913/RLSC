using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ExpeditionQueTimerUI : MonoBehaviour
{
    public static ExpeditionQueTimerUI Instance;

    public TextMeshProUGUI ExpeiditonNameLabel;
    public TextMeshProUGUI TimerLabel;

    public bool IsSearching;

    public DateTime QueueStartTime;
    string msg = "";
    void Awake()
    {
        Instance = this;
        Hide();

        msg = CORE.QuickTranslate("Queued For: ");
    }

    void Update()
    {
        TimeSpan dateDifference = DateTime.Now.Subtract(QueueStartTime);

        TimerLabel.text = msg;

        TimerLabel.text += dateDifference.ToString(@"mm\:ss");
    }

    public void Show(string ExeditionName)
    {
        QueueStartTime = DateTime.Now;

        this.gameObject.SetActive(true);
        IsSearching = true;

        ExpeiditonNameLabel.text = ExeditionName;
    }


    public void Hide()
    {
        this.gameObject.SetActive(false);
        IsSearching = false;
    }

    public void StopSearching()
    {
        SocketHandler.Instance.SendStopExpeditionQueue();
        IsSearching = false;
        Hide();
    }
}
