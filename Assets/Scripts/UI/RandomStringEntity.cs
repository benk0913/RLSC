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
        Label.text = Variety[Random.Range(0, Variety.Count)];
    }
}
