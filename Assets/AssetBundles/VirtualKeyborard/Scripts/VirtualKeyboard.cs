using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

namespace VirtualKeyboard
{
    public class VirtualKeyboard : MonoBehaviour
    {
        public static VirtualKeyboard Instance;

        public KeyboardFunc keyboard;
        public TMP_InputField inputAnswer;
        public CanvasGroup inputArea;

        public SelectionGroupUI SG;

        public Action OnSubmit;
        public Action OnCancel;

        public bool IsTyping;

        public List<EmoteSlotUI> EmoteSlots = new List<EmoteSlotUI>();

        char[] _currentText = new char[0];
        // Start is called before the first frame update
        void Start()
        {
            //inputAnswer.onSubmit.AddListener(OnInputFieldSubmit);
            //inputAnswer.onFocusSelectAll = false;

            keyboard.gameObject.SetActive(true);
            keyboard.onKeyPressEvent = (string character) => {
                inputAnswer.text += character;
            };
            keyboard.onBackKeyPressEvent = () =>
            {
                if(inputAnswer.text.Length > 0)
                    inputAnswer.text = inputAnswer.text.Remove(inputAnswer.text.Length - 1);
            };
            keyboard.onEnterKeyPressEvent = () =>
            {
                inputAnswer.text += "\n";
            };
        }

        private void Awake()
        {
            Instance = this;
            this.gameObject.SetActive(false);
        }

        public void Show(TMP_InputField field, Action onSubmit = null, Action onCancel = null)
        {
            IsTyping = true;
            OnSubmit = onSubmit;
            OnCancel = onCancel;
            this.gameObject.SetActive(true);
            inputAnswer = field;
            ShowKeyboard(true);

            CORE.Instance.DelayedInvokation(0.5F, () => { SG.RefreshGroup(false); });

            for(int i=1;i<9;i++)
            {
                if( CORE.PlayerActor != null && CORE.PlayerActor.equips != null && CORE.PlayerActor.equips.ContainsKey("Emote "+i) && CORE.PlayerActor.equips["Emote "+i] != null)
                {
                    EmoteSlots[i-1].SetInfo(CORE.PlayerActor.equips["Emote "+i].itemName);
                }
                else
                {
                    EmoteSlots[i-1].SetInfo("");
                }
            }
        }
        
        public void Hide(bool submit = false)
        {
            IsTyping = false;

            this.gameObject.SetActive(false);
            inputAnswer = null;
            ShowKeyboard(false);

            if (submit)
                OnSubmit?.Invoke();
            else
                OnCancel?.Invoke();
        }

        // Update is called once per frame
        //void Update()
        //{
        //    if (inputAnswer.isFocused == false)
        //    {
        //        EventSystem.current.SetSelectedGameObject(inputAnswer.gameObject, null);
        //        inputAnswer.OnPointerClick(new PointerEventData(EventSystem.current));
        //    }
        //}

        private void OnInputFieldSubmit(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                // some stuff with message
                //inputAnswer.text = "";
                //inputAnswer.Select();
                //inputAnswer.ActivateInputField();
            }
        }

        public void ShowKeyboard(bool flag)
        {
            if (flag)
            {
                AnimationHelper.lerpMe(0.2f, (float percent) => {
                    inputArea.alpha = Mathf.Lerp(0, 1, percent);
                }, () => {
                    inputArea.interactable = true;
                    inputArea.blocksRaycasts = true;
                });
                //inputAnswer.ActivateInputField();
                //inputAnswer.Select();
                //inputAnswer.shouldHideMobileInput = true;
                //inputAnswer.shouldHideSoftKeyboard = false;
                keyboard.ShowKeyboard(true, false);
            }
            else
            {
                inputArea.interactable = false;
                inputArea.blocksRaycasts = false;
                inputArea.alpha = 0;
                //inputAnswer.text = "";
                //inputAnswer.shouldHideMobileInput = true;
                //inputAnswer.DeactivateInputField();
                EventSystem.current.SetSelectedGameObject(null);
                keyboard.ShowKeyboard(false, false);

            }

        }

        public void OnInputFieldChange()
        {
            string wholdWord = inputAnswer.text;
            char[] listChars = wholdWord.ToCharArray();

            // backup 
            _currentText = listChars;
            //inputAnswer.caretPosition = _currentText.Length;

        }

        void Update()
        {
            if(joystickPressed)
            {
                return;
            }

            if(Input.anyKeyDown)
            {
                ConsoleInputUI.Instance.SetStateKeyboard();          
            }
        }

        public bool joystickPressed
        {
            get
            {
                return Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 
                || Input.GetAxis("RT") != 0 || Input.GetAxis("LT") != 0 
                || Input.GetButtonDown("RT")
                || Input.GetButtonDown("LT")
                || Input.GetButtonDown("Joystick 0")
                || Input.GetButtonDown("Joystick 1") 
                || Input.GetButtonDown("Joystick 2")
                || Input.GetButtonDown("Joystick 3")
                || Input.GetButtonDown("Joystick 4")
                || Input.GetButtonDown("Joystick 5")
                || Input.GetButtonDown("Joystick 6")
                || Input.GetButtonDown("Joystick 7")
                || Input.GetButtonDown("Joystick 8")
                || Input.GetButtonDown("Joystick 9")
                || Input.GetButtonDown("Joystick 10")
                || Input.GetButtonDown("Joystick 11");
            }
        }


    }
}