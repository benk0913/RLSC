using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallLoadingWindow : MonoBehaviour
{
    public static SmallLoadingWindow Instance;

    void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
