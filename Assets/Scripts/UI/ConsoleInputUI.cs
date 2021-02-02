using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleInputUI : MonoBehaviour
{
    public static ConsoleInputUI Instance;

    [SerializeField]
    InputField inputField;
    
    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
        inputField.ActivateInputField();
    }

    public void Submit()
    {
        bool wasEnterPressed = Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);
        if (wasEnterPressed && !string.IsNullOrEmpty(inputField.text))
        {
            SendConsoleMessage();
        }
        Hide();
    }

    private void SendConsoleMessage()
    {
        JSONNode node = new JSONClass();
        node["message"] = inputField.text;
        SocketHandler.Instance.SendEvent("console_message", node);
    }

    public void Hide()
    {
        inputField.text = "";
        this.gameObject.SetActive(false);
    }
}
