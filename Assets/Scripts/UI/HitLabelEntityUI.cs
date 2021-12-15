using System.Collections;
using System.Collections.Generic;
using EdgeworldBase;
using TMPro;
using UnityEngine;

public class HitLabelEntityUI : MonoBehaviour
{
    [SerializeField]
    CanvasGroup canvasGroup;

    [SerializeField]
    TextMeshProUGUI Label;

    public Color HighColor;

    public void SetLabel(string text, Color clr = default, float alpha = 1f)
    {
        Label.text = text;
        Label.color = clr;
        canvasGroup.alpha = alpha;

        int numResult;
        if(int.TryParse(text, out numResult))
        {
            if(numResult == 0)
            {
                Label.text = "BLOCK";
                
            }

            float t = ((float)numResult / 200f);
            Label.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 3, t);
            Label.color = Color.Lerp(clr, HighColor, t);

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
