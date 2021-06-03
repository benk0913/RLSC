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


        CORE.Instance.SubscribeToEvent("TimePhaseChanged", RefreshState);
        RefreshState();
    }

    public void RefreshState()
    {
        if(DefaultHandler && DefaultSun != null)
        {
            if(CORE.Instance.CurrentTimePhase == "Day")
            {
                DefaultSun.color = DefaultSunColor;
            }
            else if (CORE.Instance.CurrentTimePhase == "Night")
            {
                DefaultSun.color = DefaultNightSunColor;
            }
        }

        TimePhaseScenario scenario = TimePhaseScenarios.Find(x => x.TimePhase == CORE.Instance.CurrentTimePhase);

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
    public string TimePhase;
    public UnityEvent OnPhase;
}