using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QueWindowUI : MonoBehaviour
{
    public static QueWindowUI Instance;

    public TextMeshProUGUI MessageLabel;

    void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void Show(int position)
    {
        if(!this.gameObject.activeInHierarchy)
            this.gameObject.SetActive(true);

        MessageLabel.text ="The server is temporarily full, Your position in queue: "+position+".";
        
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
