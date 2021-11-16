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
    TMP_InputField inputField;

    [SerializeField]
    TextMeshProUGUI InputFieldLabel;

    [SerializeField]
    ScrollRect ChatScroll;


    public bool IsTyping;

    public List<string> LogMessages = new List<string>();

    public int LogCap = 10;

    public Transform ChatLogContainer;


    private void Awake()
    {
        Instance = this;
        Hide();
        
        if (Application.isEditor || CORE.Instance.Data.content.DangerousEveryoneIsAdmin)
        {
            inputField.placeholder.GetComponent<TextMeshProUGUI>().text = "/help";
        }
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
        
        CORE.Instance.DelayedInvokation(0.05f,()=>
        {
            ChatScroll.verticalNormalizedPosition = 0f;
        });
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

        if (CORE.Instance.IsUsingJoystick)
        {
            VirtualKeyboard.VirtualKeyboard.Instance.Show(inputField, () => { OnEndEdit(); SendConsoleMessage();  }, () => { OnEndEdit();  });
        }
        else
        {
            inputField.ActivateInputField();

            IsTyping = true;
        }


        RefreshChatLog();
    }


    public void Hide()
    {
        if(VirtualKeyboard.VirtualKeyboard.Instance != null && VirtualKeyboard.VirtualKeyboard.Instance.IsTyping)
        {
            VirtualKeyboard.VirtualKeyboard.Instance.Hide();
        }

        this.gameObject.SetActive(false);
        IsTyping = false;
    }

    public void HideIfEmpty()
    {
        if(CORE.Instance.IsUsingJoystick)
        {
            return;
        }

        if (IsTyping && string.IsNullOrEmpty(inputField.text))
        {
            Hide();
        }
    }

    public void OnEndEdit()
    {
        bool wasEnterPressed = Input.GetKeyDown(InputMap.Map["Console"]) || Input.GetKeyDown(InputMap.Map["Console Alt"]) || Input.GetButtonDown("Joystick 9") || Input.GetButtonDown("Joystick 10");
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
            TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("The total damage per second is: " + dps, Colors.AsColor(Colors.COLOR_GOOD), 3f, true));
        }
        else
        {
            node["message"] = inputField.text;
            SocketHandler.Instance.SendEvent("console_message", node);
        }
        inputField.text = "";
        inputField.ActivateInputField();
    }

    public void ClearLog()
    {
        CORE.ClearContainer(ChatLogContainer);
        LogMessages.Clear();
    }
}
