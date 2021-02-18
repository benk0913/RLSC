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

    public bool IsTyping;
    
    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Show()
    {
        if (!CORE.Instance.IsTyping) {
            this.gameObject.SetActive(true);
            inputField.ActivateInputField();
            IsTyping = true;
        }
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
        if (inputField.text == "/dps")
        {
            int dps = CORE.Instance.Room.PlayerActor.ActorEntity.GetDps();
            TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("The total damage per second is: " + dps, Color.green, 3f, true));
        }
        else
        {
            node["message"] = inputField.text;
            SocketHandler.Instance.SendEvent("console_message", node);
        }
    }

    public void Hide()
    {
        inputField.text = "";
        this.gameObject.SetActive(false);
        IsTyping = false;
    }
}
