using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RandomStringEntity : MonoBehaviour
{
    public List<string> Variety = new List<string>();

    public TextMeshProUGUI Label;

    bool toRefresh = false;
    private void OnEnable()
    {
        currentMessage = Variety[Random.Range(0, Variety.Count)];
        Label.text = currentMessage;
        toRefresh = true;
    }

    string currentMessage;

    void Update()
    {
        if(toRefresh)
        {
            if(CORE.Instance == null || string.IsNullOrEmpty(CORE.Instance.CurrentLanguage))
            {
                return;
            }

            toRefresh = false;
            Refresh();
        }
    }

    void Refresh()
    {
        Debug.LogError(CORE.StripHTML(Variety[Random.Range(0, Variety.Count)]));
        Label.text = CORE.QuickTranslate(CORE.StripHTML(currentMessage));
    }
}
