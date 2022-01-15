using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class TimePhaseEntity : MonoBehaviour
{
    public List<TimePhaseScenario> TimePhaseScenarios = new List<TimePhaseScenario>();

    public bool HasFader =true;

    #region DefaultBehaviour
    public bool DefaultHandler = false;
    public UnityEngine.Rendering.Universal.Light2D DefaultSun;
    public Color DefaultSunColor;
    public Color DefaultNightSunColor;

    #endregion

    private void Start()
    {
        if (DefaultHandler && DefaultSun == null)
        {
            UnityEngine.Rendering.Universal.Light2D[] lights = FindObjectsOfType<UnityEngine.Rendering.Universal.Light2D>();

            foreach(UnityEngine.Rendering.Universal.Light2D light in lights)
            {
                if(light.lightType == UnityEngine.Rendering.Universal.Light2D.LightType.Global)
                {
                    DefaultSun = light;
                    DefaultSunColor = DefaultSun.color;
                    break;
                }
            }
        }

        CORE.Instance.SubscribeToEvent("PhaseChanged", SwitchState);
        
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
        if (CORE.Instance.TimePhase == "Day")
        {
            ScreenFaderUI.Instance.FadeFromBlack(null, Color.white);
        }
        else if (CORE.Instance.TimePhase == "Night")
        {
            ScreenFaderUI.Instance.FadeFromBlack(null);
        }        

        if (DefaultHandler && DefaultSun != null)
        {

            if (CORE.Instance.TimePhase == "Day")
            {
                DefaultSun.color = DefaultSunColor;
            }
            else if (CORE.Instance.TimePhase == "Night")
            {
                DefaultSun.color = DefaultNightSunColor;
            }
                    
        }

        TimePhaseScenario scenario = TimePhaseScenarios.Find(x => x.TimePhase.ToString() == CORE.Instance.TimePhase);

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