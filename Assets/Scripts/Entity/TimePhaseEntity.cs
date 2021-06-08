using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering.Universal;

public class TimePhaseEntity : MonoBehaviour
{
    public List<TimePhaseScenario> TimePhaseScenarios = new List<TimePhaseScenario>();

    #region DefaultBehaviour
    public bool DefaultHandler = false;
    public Light2D DefaultSun;
    public Color DefaultSunColor;
    public Color DefaultNightSunColor;
    #endregion

    private void Start()
    {
        
        if (DefaultHandler && DefaultSun == null)
        {
            Light2D[] lights = FindObjectsOfType<Light2D>();

            foreach(Light2D light in lights)
            {
                if(light.lightType == Light2D.LightType.Global)
                {
                    DefaultSun = light;
                    DefaultSunColor = DefaultSun.color;
                    break;
                }
            }
        }


        CORE.Instance.SubscribeToEvent("RoomStatesChanged", RefreshState);
        RefreshState();
    }

    public void RefreshState()
    {
        if(DefaultHandler && DefaultSun != null)
        {
            if (CORE.Instance.Room.RoomStates.ContainsKey("TimePhase"))
            {
                if (CORE.Instance.Room.RoomStates["TimePhase"] == (int)TimePhase.Day)
                {
                    DefaultSun.color = DefaultSunColor;
                }
                else if (CORE.Instance.Room.RoomStates["TimePhase"] == (int)TimePhase.Day)
                {
                    DefaultSun.color = DefaultNightSunColor;
                }
            }
            else
            {
                DefaultSun.color = DefaultSunColor;
            }
        }

        TimePhaseScenario scenario = TimePhaseScenarios.Find(x => (int)x.TimePhase == CORE.Instance.Room.RoomStates["TimePhase"]);

        if(scenario == null)
        {
            return;
        }

        scenario.OnPhase?.Invoke();
    }
}

[System.Serializable]
public class TimePhaseScenario
{
    public TimePhase TimePhase;
    public UnityEvent OnPhase;
}

public enum TimePhase
{
    Day,
    Night
}