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
    TextMeshProUGUI PlaceholderLabel;

    [SerializeField]
    TMP_InputField WhisperTarget;

    [SerializeField]
    ScrollRect ChatScroll;

    [SerializeField]
    TMP_Dropdown ChannelDropdown;


    public bool IsTyping;

    public List<LogMessageInstance> LogMessages = new List<LogMessageInstance>();

    public int LogCap = 10;

    public Transform ChatLogContainer;

    public List<ChatChannel> Channels = new List<ChatChannel>();
    public string CurrentChannelKey = "all";

    private void Awake()
    {
        Instance = this;
        RefreshChannelState();
        Hide();
        
        if (Application.isEditor || CORE.Instance.Data.content.DangerousEveryoneIsAdmin)
        {
            inputField.placeholder.GetComponent<TextMeshProUGUI>().text = "/help";
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(InputMap.Map["Change Chat Channel"]))
        {
            TabChannel();
        }
        
        if(!string.IsNullOrEmpty(inputField.text))
        {
            char[] chr = new char[] {' '};
            string[] splitword = inputField.text.Split(chr, 2);
            if(splitword.Length > 0)
            {
                string firstWord = splitword [0];   

                switch(firstWord)
                {
                    case "/whisper":
                    {
                        inputField.text = inputField.text.Remove(0,firstWord.Length);
                        SetChannel(Channels.Find(X=>X.ChannelKey =="whisper"));
                        ChannelDropdown.value = ChannelDropdown.options.IndexOf(ChannelDropdown.options.Find(x=>x.text == CurrentChannelKey));
                        break;
                    }
                    case "/party":
                    {
                        inputField.text = inputField.text.Remove(0,firstWord.Length);
                        SetChannel(Channels.Find(X=>X.ChannelKey =="party"));
                        ChannelDropdown.value = ChannelDropdown.options.IndexOf(ChannelDropdown.options.Find(x=>x.text == CurrentChannelKey));
                        break;
                    }
                    case "/all":
                    {
                        inputField.text = inputField.text.Remove(0,firstWord.Length);
                        SetChannel(Channels.Find(X=>X.ChannelKey =="all"));
                        ChannelDropdown.value = ChannelDropdown.options.IndexOf(ChannelDropdown.options.Find(x=>x.text == CurrentChannelKey));
                        break;
                    }

                }
            }
        }

        
    }


    public void AddLogMessage(string message,string channel = "all")
    {
        LogMessages.Add(new LogMessageInstance(message,channel));

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
            logPiece.text = LogMessages[i].Message;
            
            if(string.IsNullOrEmpty(LogMessages[i].Channel) || LogMessages[i].Channel == "all")
                logPiece.color=Color.white;
            else
                logPiece.color = Channels.Find(x=>x.ChannelKey==LogMessages[i].Channel).ChannelColor;
        }
    }

    public void OnDropdownChangeChannel(int dropdownIndex)
    {
        SetChannel(Channels[dropdownIndex]);
    }
    
    public void SetChannel(ChatChannel channel)
    {
        CurrentChannelKey = channel.ChannelKey;
        WhisperTarget.gameObject.SetActive(CurrentChannelKey == "whisper");
        
        if(string.IsNullOrEmpty(CurrentChannelKey) || CurrentChannelKey == "all")
        {
            InputFieldLabel.color = Color.black;
            PlaceholderLabel.color = Color.black;
        }
        else
        {
            InputFieldLabel.color =channel.ChannelColor;
            PlaceholderLabel.color =channel.ChannelColor;
        }
    }

    public void TabChannel()
    {
        if(string.IsNullOrEmpty(CurrentChannelKey))
        {
            CurrentChannelKey = "all";
        }

        int nextChannel = Channels.IndexOf(Channels.Find(x=>x.ChannelKey == CurrentChannelKey))+1;

        if(nextChannel >= Channels.Count)
        {
            nextChannel = 0;
        }

        SetChannel(Channels[nextChannel]);

        ChannelDropdown.value = ChannelDropdown.options.IndexOf(ChannelDropdown.options.Find(x=>x.text == CurrentChannelKey));
    }

    public void RefreshChannelState()
    {
        ChannelDropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        foreach(ChatChannel channel in Channels)
        {
            options.Add(new TMP_Dropdown.OptionData(channel.ChannelKey));
        }

        ChannelDropdown.AddOptions(options);
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

    public void InputfieldEndEdit()
    {
        #if !UNITY_ANDROID && !UNITY_IOS
        return;
        #endif

        OnEndEdit(); 
        SendConsoleMessage();
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
            if(string.IsNullOrEmpty(CurrentChannelKey))
            {
                CurrentChannelKey = "all";
            }

            if(CurrentChannelKey == "whisper")
            {
                if(string.IsNullOrEmpty(WhisperTarget.text))
                {
                    char[] chr = new char[] {' '};
                    string[] splitword = inputField.text.Split(chr, 2);
                    string firstWord = splitword [0];   
                    inputField.text = inputField.text.Remove(0,firstWord.Length);
                    WhisperTarget.text = firstWord;
                }

                node["whisperName"] = WhisperTarget.text; 
            }

            node["message"] = inputField.text;
            node["channel"] = CurrentChannelKey;

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

public class LogMessageInstance
{
    public string Message;
    public string Channel;

    public LogMessageInstance(string message = "",string channel = "all")
    {
        this.Message = message;
        this.Channel = channel;
    }
}

[System.Serializable]
public class ChatChannel
{
    public string ChannelKey;

    public Color ChannelColor;
}

