using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnScreenKeyboard : MonoBehaviour
{
    public static OnScreenKeyboard Instance;

    private void Awake()
    {
        Instance = this;
    }
}
