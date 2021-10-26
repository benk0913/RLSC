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
        toRefresh = true;
    }

    void Update()
    {
        if(toRefresh)
        {
            if(CORE.Instance == null)
            {
                return;
            }

            toRefresh = false;
            Refresh();
        }
    }

    void Refresh()
    {
        string message = Variety[Random.Range(0, Variety.Count)];
        string newMsg = message;
        if(!CORE.Instance.Data.Localizator.mSource.TryGetTranslation(message, out newMsg))
        {
            Debug.LogError("FAIL");
        }
        message = newMsg;

        Label.text = message;
    }
}
