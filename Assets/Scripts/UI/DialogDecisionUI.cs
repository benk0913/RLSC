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
        CurrentDecision.SelectDecision();
        DecisionContainerUI.Instance.Hide();
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

        DecisionLabel.text = CurrentDecision.Content;

        
    }
}
