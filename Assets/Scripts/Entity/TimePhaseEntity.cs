using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering.Universal;

public class TimePhaseEntity : MonoBehaviour
{
    public List<TimePhaseScenario> TimePhaseScenarios = new List<TimePhaseScenario>();

    public bool HasFader =true;

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

        CORE.Instance.SubscribeToEvent("GameStatesChanged", SwitchState);
        
        RefreshState();
    }

    public void SwitchState()
    {

        //TODO Remove fader?
        //if (HasFader)
        //{
        //    ScreenFaderUI.Instance.FadeToBlack(() =>
        //    {
        //        RefreshState();
        //        ScreenFaderUI.Instance.FadeFromBlack();
        //    });
        //}
        //else
        //{
            RefreshState();
        //}
    }

    public void RefreshState()
    {
        if (CORE.Instance.GameStates["phase"] == "Day")
        {
            ScreenFaderUI.Instance.FadeFromBlack(null, Color.white);
        }
        else if (CORE.Instance.GameStates["phase"] == "Night")
        {
            ScreenFaderUI.Instance.FadeFromBlack(null);
        }        

        if (DefaultHandler && DefaultSun != null)
        {
            if (CORE.Instance.GameStates.ContainsKey("phase"))
            {


                    if (CORE.Instance.GameStates["phase"] == "Day")
                    {
                        DefaultSun.color = DefaultSunColor;
                    }
                    else if (CORE.Instance.GameStates["phase"] == "Night")
                    {
                        DefaultSun.color = DefaultNightSunColor;
                    }
                    
                   

            }
            else
            {
                DefaultSun.color = DefaultSunColor;
            }
        }

        if (!CORE.Instance.GameStates.ContainsKey("phase"))
        {
            return;
        }

        TimePhaseScenario scenario = TimePhaseScenarios.Find(x => x.TimePhase.ToString() == CORE.Instance.GameStates["phase"]);

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