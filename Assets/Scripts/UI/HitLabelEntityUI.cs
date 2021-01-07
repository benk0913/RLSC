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

        int numResult;
        if(int.TryParse(text, out numResult))
        {
            float t = ((float)numResult / 200f);
            Label.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 3, t);
            Label.color = Color.Lerp(clr, Color.red, t);


            if ((t > 0.5f))
            {
                Label.fontStyle = FontStyles.Bold;
            }
            else
            {
                Label.fontStyle = FontStyles.Normal;
            }
            
        }
    }
}
