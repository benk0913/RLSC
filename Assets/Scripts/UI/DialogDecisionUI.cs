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
            return;
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

        string content = CurrentDecision.Content;
        CORE.Instance.Data.Localizator.mSource.TryGetTranslationCodywise(content, out content);

        DecisionLabel.text = content;

        
    }
}
