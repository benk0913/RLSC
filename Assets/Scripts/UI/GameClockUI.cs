using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameClockUI : MonoBehaviour
{
    public static GameClockUI Instance;

    [SerializeField]
    Image ClockfILLIMG;

    [SerializeField]
    TooltipTargetUI TimePhaseTooltip;

    public string Phase;
    public int MinutesLeft;

    void OnEnable()
    {
        Instance  = this;
    }
    public void UpdateGameClock(string phase, float phaseFraction)
    {
        ClockfILLIMG.fillAmount = phaseFraction;
        this.Phase = phase;
        this.MinutesLeft = Mathf.RoundToInt((1f-phaseFraction)*CORE.Instance.Data.content.TimePhases.Find(X=>X.Name == this.Phase).DurationInMinutes);

        TimePhaseTooltip.SetTooltip("Current Day / Night Phase - "+MinutesLeft+" minutes left.");
    }
}
