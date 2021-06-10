using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WarningWindowUI : MonoBehaviour
{
    public static WarningWindowUI Instance;

    Action AcceptAction;

    Action SkipAction;

    [SerializeField]
    TextMeshProUGUI Description;


    [SerializeField]
    GameObject HideButton;

    public bool CantHide = false;

    public List<WarningWindowData> WindowQueue = new List<WarningWindowData>();


    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Hide(bool accepted = false)
    {
        this.gameObject.SetActive(false);

        if (WindowQueue.Count > 0)
        {
            Show(WindowQueue[0].Message, WindowQueue[0].AcceptCallback, WindowQueue[0].CantHide);
            WindowQueue.RemoveAt(0);
        }

        if (!accepted)
        {
            SkipAction?.Invoke();
        }

    }

    private void Update()
    {
        if (!CantHide && Input.GetKeyDown(InputMap.Map["Exit"]))
        {
            Hide();
        }
        else if (Input.GetKeyDown(InputMap.Map["Confirm"]))
        {
            Accept();
        }
    }

    public void Show(string message, Action acceptCallback, bool cantHide = false, Action skipCallback = null)
    {
        if(this.gameObject.activeInHierarchy)
        {
            WindowQueue.Add(new WarningWindowData(message, acceptCallback, cantHide, skipCallback));
            return;
        }

        CantHide = cantHide;

        HideButton.gameObject.SetActive(!CantHide);

        this.gameObject.SetActive(true);

        Description.text = message;
        AcceptAction = acceptCallback;
        SkipAction = skipCallback;
    }

    public void Accept()
    {
        AcceptAction?.Invoke();
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
