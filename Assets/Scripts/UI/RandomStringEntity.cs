using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RandomStringEntity : MonoBehaviour
{
    public List<string> Variety = new List<string>();

    public TextMeshProUGUI Label;

    private void OnEnable()
    {
        Refresh();
    }

    void Refresh()
    {
        string message = Variety[Random.Range(0, Variety.Count)];
        string newMsg = "";
        if (CORE.Instance.Data.Localizator.mSource.TryGetTranslation(message, out newMsg))
        {
            message = newMsg;
        }

        Label.text = message;
    }
}
