using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputLabelWindow : MonoBehaviour
{
    public static InputLabelWindow Instance;

    Action<string> AcceptAction;
    
    [SerializeField]
    TextMeshProUGUI TitleLabel;

    [SerializeField]
    TextMeshProUGUI FieldLabel;

    [SerializeField]
    TMP_InputField Field;
    


    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Hide(bool accepted = false)
    {
        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(InputMap.Map["Exit"]))
        {
            Hide();
        }
    }

    public void Show(string title = "Set Amount", string fieldLabel = "Set Amount",  Action<string> acceptCallback = null)
    {
        this.gameObject.SetActive(true);

        TitleLabel.text = title;
        FieldLabel.text = fieldLabel;
        AcceptAction = acceptCallback;
    }

    public void Accept()
    {
        AcceptAction?.Invoke(Field.text);
        Hide(true);
    }

    public class WarningWindowData
    {
        public string Message;
        public Action AcceptCallback;
        public bool CantHide = false;

        public WarningWindowData(string msg, Action callback, bool canHide = false,Action skipCallback = null)
        {
            this.Message = msg;
            this.AcceptCallback = callback;
            this.CantHide = canHide;
        }
    }
}
