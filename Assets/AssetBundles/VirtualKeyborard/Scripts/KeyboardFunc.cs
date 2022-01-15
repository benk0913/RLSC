using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace VirtualKeyboard
{
    [System.Serializable]
    public class KeyConversion
    {
        public string character;
        public string number;
        public string symbol;
    }

    public class KeyboardFunc : MonoBehaviour
    {

        public CanvasGroup canvasKeyboard;
        [System.Serializable]
        public enum KeyboardLayout
        {
            Characters,
            Numbers,
            Symbols
        }
        [System.Serializable]
        public enum KeyCode
        {
            Back,
            Enter,
            Space,
            Capslock,
            NumberChange,
            KeyboardClose,
            Normal
        }
        public System.Action<string> onKeyPressEvent;
        public System.Action onBackKeyPressEvent;
        public System.Action onEnterKeyPressEvent;

        bool _isCapslockOn=false;
        bool _isShowing = false;
        public List<KeyConversion> line1 = new List<KeyConversion>();
        public List<KeyConversion> line2 = new List<KeyConversion>();
        public List<KeyConversion> line3 = new List<KeyConversion>();
        public List<Transform> lines = new List<Transform>();
        KeyboardLayout currentLayout;

        public Sprite spriteShiftFill;
        public Sprite spriteShift;
        

        private void Awake()
        {
            Init();
        }

        void Start()
        {
        }



        public void Init()
        {
            KeyFunc[] list = GetComponentsInChildren<KeyFunc>();
            foreach(KeyFunc key in list)
            {
                key.onKeyPressEvent = PressKey;
                key.keyboardFuncRef = this;
            }
        }

        public bool IsShowing()
        {
            return _isShowing;
        }

        public void ShowKeyboard(bool flag, bool delay)
        {
            if (_isShowing != flag)
            {
                _isShowing = flag;
                if (delay)
                {
                    canvasKeyboard.blocksRaycasts = flag;
                    canvasKeyboard.interactable = flag;
                    AnimationHelper.lerpMe(0.2f, (float percent) =>
                    {
                        canvasKeyboard.alpha = flag ? Mathf.Lerp(0, 1, percent) : Mathf.Lerp(1, 0, percent);
                    });
                }
                else
                {
                    canvasKeyboard.blocksRaycasts = flag;
                    canvasKeyboard.interactable = flag;
                    canvasKeyboard.alpha = flag ? 1 : 0;
                }
            }
        }

        List<KeyConversion> getLineData(int i)
        {
            switch (i)
            {
                case 1:
                    return line1;
                case 2:
                    return line2;
                case 3:
                    return line3;
                default:
                    return line1;
            }
        }

        void setNormalKeyMapping(int line, KeyboardLayout layout)
        {
            List<KeyConversion> data = getLineData(line);
            KeyFunc[] line1Key = lines[line-1].GetComponentsInChildren<KeyFunc>();
            int i = 0;
            foreach (KeyFunc k in line1Key)
            {
                if (k.keyCode == KeyCode.Normal)
                {
                    k.SetKeyVisual(layout == KeyboardLayout.Characters ? data[i].character :
                                (layout == KeyboardLayout.Numbers ? data[i].number : data[i].symbol));
                    i++;
                }
            }
        }

        void setLayout(KeyboardLayout layout)
        {
            currentLayout = layout;
            setNormalKeyMapping(1, currentLayout);
            setNormalKeyMapping(2, currentLayout);
            setNormalKeyMapping(3, currentLayout);
            KeyFunc[] list = GetComponentsInChildren<KeyFunc>();
            switch (currentLayout)
            {
                case KeyboardLayout.Characters:
                    foreach (KeyFunc key in list)
                    {
                        if (key.keyCode == KeyCode.Capslock)
                        {
                            key.ShowStringKey(false);
                            _isCapslockOn = false;
                            turnCapslock(_isCapslockOn);

                            key.ChangeBackgroundToWhite(false);
                        }
                        else if (key.keyCode == KeyCode.NumberChange)
                        {
                            key.SetKeyVisual("123");
                        }
                    }
                    break;
                case KeyboardLayout.Numbers:
                    foreach (KeyFunc key in list)
                    {
                        if (key.keyCode == KeyCode.Capslock)
                        {
                            key.ShowStringKey(true);
                            key.SetKeyVisual("#+=");
                            key.ChangeBackgroundToWhite(false);
                        }
                        else if (key.keyCode == KeyCode.NumberChange)
                        {
                            key.SetKeyVisual("ABC");
                        }
                    }
                    break;
                case KeyboardLayout.Symbols:
                    foreach (KeyFunc key in list)
                    {
                        if (key.keyCode == KeyCode.Capslock)
                        {
                            key.ShowStringKey(true);
                            key.SetKeyVisual("123");
                            key.ChangeBackgroundToWhite(false);
                        }
                        else if (key.keyCode == KeyCode.NumberChange)
                        {
                            key.SetKeyVisual("ABC");
                        }
                    }
                    break;
            }
        }

        void turnCapslock(bool isOn)
        {
            KeyFunc[] list = GetComponentsInChildren<KeyFunc>();
            foreach (KeyFunc k in list)
            {
                if (k.keyCode == KeyCode.Normal)
                {
                    k.SetUppercase(isOn);
                }
                else if (k.keyCode == KeyCode.Capslock)
                {
                    k.ChangeBackgroundToWhite(isOn);
                }
            }
        }

        public void PressKey(KeyFunc key)
        {
            switch (key.keyCode)
            {
                case KeyCode.Back:
                    onBackKeyPressEvent?.Invoke();
                    break;
                case KeyCode.Capslock:
                    switch (currentLayout)
                    {
                        case KeyboardLayout.Characters:
                            _isCapslockOn = !_isCapslockOn;
                            turnCapslock(_isCapslockOn);
                            break;
                        case KeyboardLayout.Numbers:
                            setLayout(KeyboardLayout.Symbols);
                            break;
                        case KeyboardLayout.Symbols:
                            setLayout(KeyboardLayout.Numbers);
                            break;
                    }

                   
                    break;
                case KeyCode.Enter:
                    onEnterKeyPressEvent?.Invoke();
                    break;
                case KeyCode.Normal:
                    onKeyPressEvent?.Invoke(key.GetKeyStr());
                    
                    break;
                case KeyCode.NumberChange:
                    switch (currentLayout)
                    {
                        case KeyboardLayout.Characters:
                            setLayout(KeyboardLayout.Numbers);
                            break;
                        case KeyboardLayout.Numbers:
                            setLayout(KeyboardLayout.Characters);
                            break;
                        case KeyboardLayout.Symbols:
                            setLayout(KeyboardLayout.Characters);
                            break;
                    }
                    break;
                case KeyCode.Space:
                    onKeyPressEvent?.Invoke(" ");
                    break;

            }
            
        }
    }
}