using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NamePanelUI : MonoBehaviour
{
    [SerializeField]
    Actor CurrentActor;

    [SerializeField]
    TextMeshProUGUI TextLabel;
    

    void Start()
    {
        TextLabel.text = CurrentActor.State.Data.name;
    }
}
