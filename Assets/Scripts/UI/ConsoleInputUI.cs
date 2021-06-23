using System.Collections;
using System.Collections.Generic;
using EdgeworldBase;
using SimpleJSON;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleInputUI : MonoBehaviour
{
    public static ConsoleInputUI Instance;

    [SerializeField]
    InputField inputField;

    public bool IsTyping;

    public List<string> LogMessages = new List<string>();

    public int LogCap = 10;

    public Transform ChatLogContainer;

    private void Awake()
    {
        Instance = this;
        Hide();
        
    #if UNITY_EDITOR
        inputField.placeholder.GetComponent<Text>().text = "/help";
    #endif
    }

    public void AddLogMessage(string message)
    {
        LogMessages.Add(message);
        if (LogMessages.Count > LogCap)
        {
            LogMessages.RemoveAt(0);
        }

        if (this.gameObject.activeInHierarchy)
        {
            RefreshChatLog();
        }
    }

    public void RefreshChatLog()
    {
        CORE.ClearContainer(ChatLogContainer);

        for (int i = 0; i < LogMessages.Count; i++)
        {
            TextMeshProUGUI logPiece = ResourcesLoader.Instance.GetRecycledObject("LogMessagePiece").GetComponent<TextMeshProUGUI>();
            logPiece.transform.SetParent(ChatLogContainer, false);
            logPiece.transform.localScale = Vector3.one;
            logPiece.transform.position = Vector3.zero;
            logPiece.text = LogMessages[i];
        }
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
        else if (!CORE.Instance.HasWindowOpen)
        {
            Show();
        }
    }

    public void Show()
    {
        this.gameObject.SetActive(true);

        inputField.ActivateInputField();
        IsTyping = true;

        RefreshChatLog();
    }


    public void Hide()
    {
        this.gameObject.SetActive(false);
        IsTyping = false;
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
            CORE.Instance.DelayedInvokation(0, () => Hide());
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


}
