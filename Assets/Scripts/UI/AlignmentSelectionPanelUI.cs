using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AlignmentSelectionPanelUI : MonoBehaviour
{
    public UnityEvent OnGood;
    public UnityEvent OnEvil;

    public void SelectAlignment(bool isGood = true)
    {
        WarningWindowUI.Instance.Show("Are you sure you want to be "+(isGood?"GOOD?" : "EVIL!?"),()=>
        {
            if(isGood)
            {
                OnGood?.Invoke();
            }
            else
            {
                OnEvil?.Invoke();
            }
        });

        
    }
}
