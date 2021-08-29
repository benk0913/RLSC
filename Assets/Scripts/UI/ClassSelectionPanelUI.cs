using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClassSelectionPanelUI : MonoBehaviour
{
    public UnityEvent OnConfirmVideo;
    public UnityEvent OnCancelVideo;

    public void SelectClass(ClassJob job)
    {
        VideoWindowUI.Instance.Show(job.ClassFeatureVideo,()=>
        {
            OnConfirmVideo?.Invoke();
        },false,()=>
        {
            OnCancelVideo?.Invoke();
        }
        );
    }
}
