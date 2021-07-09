using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenuUI : MonoBehaviour, WindowInterface
{
    public static SettingsMenuUI Instance;

    [SerializeField]
    Canvas SettingsCanvas;

    void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void Show(ActorData actorData, object data)
    {
        this.gameObject.SetActive(true);
    }
}
