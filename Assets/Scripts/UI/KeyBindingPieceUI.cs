using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System;

public class KeyBindingPieceUI : MonoBehaviour {

    [SerializeField]
    TextMeshProUGUI m_txtTitle;

    [SerializeField]
    TextMeshProUGUI m_txtKey;

    [SerializeField]
    Image m_Image;

    [SerializeField]
    public Button m_btn;

    public KeyCode CurrentKey;

    public bool isWaitingForKey = false;
    Event keyEvent;
    Color initColor;

    string internalTitle;
    public void SetInfo(string title, KeyCode key)
    {
        internalTitle = title;
        CurrentKey = key;

        Action translateTitle = () =>
        {
            string toTitle = title;
            CORE.Instance.Data.Localizator.mSource.TryGetTranslationCodywise(title, out toTitle);

            m_txtTitle.text = toTitle;
        };

        translateTitle.Invoke();
        m_txtKey.text = CurrentKey.ToString();

        CORE.Instance.SubscribeToEvent("LanguageChanged", translateTitle.Invoke);
    }

    public void OnClick()
    {
        KeyBindingWindowUI.Instance.OnKeyBindingPieceClicked(this);
    }

    public void SetBinding()
    {
        isWaitingForKey = true;
        m_Image.color = Colors.AsColor(Colors.COLOR_HIGHLIGHT);
    }

    public void CloseBinding()
    {
        isWaitingForKey = false;
        m_Image.color = Colors.AsColor(Colors.COLOR_TEXT);
    }

    void OnGUI()
    {
        if(isWaitingForKey)
        {   
            keyEvent = Event.current;
            if (keyEvent != null && keyEvent.isKey)
            {
                if(keyEvent.keyCode != InputMap.Map["Exit"] && keyEvent.keyCode != InputMap.Map["Confirm"])
                {
                    InputMap.Map[internalTitle] = keyEvent.keyCode;
                    InputMap.SaveMap();
                    SetInfo(internalTitle, keyEvent.keyCode);
                    CORE.Instance.InvokeEvent("KeybindingsChanged");
                }

                CloseBinding();
                KeyBindingWindowUI.Instance.CloseBinding();
            }
        }
    }

}
