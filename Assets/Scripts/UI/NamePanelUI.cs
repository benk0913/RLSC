using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NamePanelUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI TextLabel;
    

    public void Refresh(Actor CurrentActor)
    {
        TextLabel.text = CurrentActor.State.Data.name;
    }
}
