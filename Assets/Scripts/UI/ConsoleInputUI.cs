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
        
    #if UNITY_EDITOR
        inputField.placeholder.GetComponent<Text>().text = "/help";
    #endif
    }


    public void EnterPressed()
    {
        if (IsTyping)
        {
            if (string.IsNullOrEmpty(inputField.text))
            {
                Hide();
            }
            else
            {
                SendConsoleMessage();
            }
        }
        else
        {
            this.gameObject.SetActive(true);
            inputField.ActivateInputField();
            IsTyping = true;
        }
    }

    public void HideIfEmpty()
    {
        if (IsTyping && string.IsNullOrEmpty(inputField.text))
        {
            Hide();
        }
    }

    public void OnEndEdit()
    {
        bool wasEnterPressed = Input.GetKeyDown(InputMap.Map["Console"]) || Input.GetKeyDown(InputMap.Map["Console Alt"]);
        if (!wasEnterPressed)
        {
            Hide();
        }
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
        inputField.text = "";
        inputField.ActivateInputField();
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
        IsTyping = false;
    }
}
