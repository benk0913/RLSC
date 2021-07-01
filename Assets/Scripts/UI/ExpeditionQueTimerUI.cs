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

    void Awake()
    {
        Instance = this;
        Hide();
    }

    void Update()
    {
        TimeSpan dateDifference = DateTime.Now.Subtract(QueueStartTime);
        
        TimerLabel.text = "Queued For: "+dateDifference.ToString(@"mm\:ss");
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
    }

    public void StopSearching()
    {
        SocketHandler.Instance.SendStopExpeditionQueue();
        Hide();
    }
}
