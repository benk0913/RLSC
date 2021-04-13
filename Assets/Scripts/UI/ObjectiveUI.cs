using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectiveUI : MonoBehaviour
{
    public static ObjectiveUI Instance;

    [SerializeField]
    TextMeshProUGUI ObjectiveLabel;

    public void SetInfo(string objectiveText)
    {
        ObjectiveLabel.text = objectiveText;
    }

    private void Awake()
    {
        Instance = this;
    }

}
