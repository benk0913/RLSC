using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogDecisionUI : MonoBehaviour
{
    public DialogEntity.DialogDecision CurrentDecision;

    [SerializeField]
    Button Butt;

    [SerializeField]
    TextMeshProUGUI DecisionLabel;

    
    public void OnClick()
    {
        if(CurrentDecision == null)
        {
            CORE.Instance.LogMessageError("DECISION CLICKED no current decision...");
            
            DecisionContainerUI.Instance.Hide();
            CurrentDecision = null;

            return;
            
        }

        if(DialogEntity.CurrentInstance == null || DialogEntity.CurrentInstance.CurrentDialog == null)
        {
            CORE.Instance.LogMessageError("DECISION CLICKED no current dialog...");

            DecisionContainerUI.Instance.Hide();
            CurrentDecision = null;
            
            return;
        }
        
        if(DialogEntity.CurrentInstance.CurrentDialog.Decisions.Find(x=>x == CurrentDecision) == null)
        {
            CORE.Instance.LogMessageError("DECISION CLICKED is not in the current dialog!?");

            DecisionContainerUI.Instance.Hide();
            CurrentDecision = null;
            
            return;
        }

        foreach (AbilityCondition condition in CurrentDecision.DisplayConditions)
        {
            if(!condition.IsValid(null))
            {
                return;
            }
        }
        
        DecisionContainerUI.Instance.Hide();
        CurrentDecision.SelectDecision();
        CurrentDecision = null;
    }

    public void SetInfo(DialogEntity.DialogDecision decision)
    {
        CurrentDecision = decision;


        Butt.interactable = true; 
        foreach (AbilityCondition condition in CurrentDecision.DisplayConditions)
        {
            if(!condition.IsValid(null))
            {
                if(decision.DisplayOnlyIfConditionsMet)
                {
                    this.gameObject.SetActive(false);
                    return;
                }
                else
                {
                    Butt.interactable = false;
                    
                    break;
                }
            }
        }
        foreach (GameCondition condition in CurrentDecision.DisplayGameConditions)
        {
            if(!condition.IsValid(null))
            {
                if(decision.DisplayOnlyIfConditionsMet)
                {
                    this.gameObject.SetActive(false);
                    return;
                }
                else
                {
                    Butt.interactable = false;
                    
                    break;
                }
            }
        }
        
        DecisionLabel.text = CORE.QuickTranslate(CurrentDecision.Content);

        
    }
}
