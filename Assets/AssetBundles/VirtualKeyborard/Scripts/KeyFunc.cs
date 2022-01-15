using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace VirtualKeyboard
{
    public class KeyFunc : MonoBehaviour
    {
        public CanvasGroup keyupCanvas;
        TextMeshProUGUI textKeyUp;
        public KeyboardFunc.KeyCode keyCode;
        public System.Action<KeyFunc> onKeyPressEvent;
        Image bg;
        Color bgOldColor;
        TextMeshProUGUI character;
        public KeyboardFunc keyboardFuncRef;

        // Start is called before the first frame update
        void Start()
        {
            bg = GetComponent<Image>();
            bgOldColor = bg.color;

            if (transform.childCount > 0)
                character = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            //if (keyupCanvas != null && character!= null)
            //{
            //    textKeyUp = keyupCanvas.GetComponentInChildren<TextMeshProUGUI>();
            //    textKeyUp.text = character.text;
            //}
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetKeyVisual(string t)
        {
            character.text = t;
            if(textKeyUp)
                textKeyUp.text = t;
        }

        public string GetKeyStr()
        {
            return character.text;
        }

        public void SetUppercase(bool flag)
        {
            if(character == null || textKeyUp == null)
            {
                return;
            }

            character.text = flag ? character.text.ToUpper() : character.text.ToLower();
            textKeyUp.text = character.text;
        }

        public void ChangeBackgroundToWhite(bool flag)
        {
            if (transform.childCount > 1)
            {
                Image icon = transform.GetChild(1).GetComponent<Image>();
                icon.sprite = flag ? keyboardFuncRef.spriteShiftFill : keyboardFuncRef.spriteShift;
                bg.color = flag ? Color.white : bgOldColor;
            }
        }

        // for capslock 
        public void ShowStringKey(bool flag)
        {
            character.gameObject.SetActive(flag);
            if (transform.childCount > 1)
            {
                Image icon = transform.GetChild(1).GetComponent<Image>();
                icon.gameObject.SetActive(!flag);
            }
        }

        public void PressKey()
        {
            //if(keyupCanvas != null)
            //{
            //    keyupCanvas.alpha = 1;
            //    AnimationHelper.playAfterDelay(this, 0.2f, () =>
            //    {
            //        AnimationHelper.lerpMe(0.05f, (float percent) => {
            //            keyupCanvas.alpha = Mathf.Lerp(1, 0, percent);
            //        });
            //    });
            //}
            onKeyPressEvent?.Invoke(this);
        }
    }
}
