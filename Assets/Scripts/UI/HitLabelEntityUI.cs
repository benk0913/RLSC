using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HitLabelEntityUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI Label;

    public void SetLabel(string text, Color clr = default)
    {
        Label.text = text;
        
        Label.color = clr;
    }
}
