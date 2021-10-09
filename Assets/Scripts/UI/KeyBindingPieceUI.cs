using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

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

    public void SetInfo(string title, KeyCode key)
    {
        CurrentKey = key;
        m_txtTitle.text = title;
        m_txtKey.text = CurrentKey.ToString();
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
                    InputMap.Map[m_txtTitle.text] = keyEvent.keyCode;
                    InputMap.SaveMap();
                    SetInfo(m_txtTitle.text, keyEvent.keyCode);
                    CORE.Instance.InvokeEvent("KeybindingsChanged");
                }

                CloseBinding();
                KeyBindingWindowUI.Instance.CloseBinding();
            }
        }
    }

}
