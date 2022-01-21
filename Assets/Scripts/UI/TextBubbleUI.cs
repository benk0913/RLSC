using EdgeworldBase;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextBubbleUI : MonoBehaviour, IPointerClickHandler
{
    const float DELAY_PER_LETTER = 0.02f;

    [SerializeField]
    TextMeshProUGUI ContentText;

    [SerializeField]
    CanvasGroup CG;

    [SerializeField]
    RectTransform RectTrans;

    [SerializeField]
    Image ChatBubbleImage;

    [SerializeField]
    VerticalLayoutGroup VerticalGroup;


    Transform CurrentAnchor;

    Action OnHide;


    void Start()
    {
        CORE.Instance.SubscribeToEvent("NewSceneLoaded",()=>{this.gameObject.SetActive(false);});
    }
    public void Show(Transform anchor, string message, Action onHide = null, bool isFemale = false,Sprite skinSprite = null)
    {
        this.gameObject.SetActive(true);
        StopAllCoroutines();
        
        CurrentAnchor = anchor;
        StartCoroutine(ShowRoutine(message,isFemale));

        if(skinSprite == null)
        {
            skinSprite = CORE.Instance.Data.content.DefaultChatBubble.Icon;
        }

        ChatBubbleImage.sprite = skinSprite;

        OnHide = onHide;
    }

    private void Update()
    {
        if(CurrentAnchor == null || CurrentAnchor==transform)
        {
            return;
        }

        transform.position = Vector2.Lerp(transform.position, CurrentAnchor.position, Time.deltaTime * 8f);
    }

    IEnumerator ShowRoutine(string message, bool isFemale = false)
    {
        ContentText.text = "";

        yield return 0; 

        CG.alpha = 1f;

        //TODO Replace with character pitch?
        float pitch = 1f;
        if(isFemale)
        {
            pitch = UnityEngine.Random.Range(1f, 1.75f);
        }
        else
        {
            pitch = UnityEngine.Random.Range(0.25f, 1.25f);
        }


        while (ContentText.text.Length < message.Length)
        {
            ContentText.text += message[ContentText.text.Length];

            if (ContentText.text.Length % 3 == 0)
            {
                if(message[ContentText.text.Length-1] == '!' || message[ContentText.text.Length-1] == '?')
                {
                    AudioControl.Instance.PlayInPosition("talksound", transform.position, 200f, pitch + +0.5f);
                }
                else
                {
                    AudioControl.Instance.PlayInPosition("talksound", transform.position, 200f, pitch);
                }
            }


            if(message[ContentText.text.Length-1] == '.' || message[ContentText.text.Length - 1] == ',' || message[ContentText.text.Length-1] == '!' || message[ContentText.text.Length-1] == '?')
            {
                yield return new WaitForSeconds(0.1f);
            }
            else if(!Char.IsLower(message[ContentText.text.Length-1]))
            {
                yield return new WaitForSeconds(0.025f);
            }



            yield return new WaitForSeconds(0.025f);
        }

        if (ContentText.text.Length < 5)
        {
            VerticalGroup.enabled = false;
            yield return 0;
            VerticalGroup.enabled = true;
        }

        #if UNITY_ANDROID || UNITY_IOS
        yield return new WaitForSeconds(1f + (message.Length * DELAY_PER_LETTER));
        #else
        yield return new WaitForSeconds(2f + (message.Length * DELAY_PER_LETTER));
        #endif
        CG.alpha = 1f;
        while (CG.alpha > 0f)
        {
            CG.alpha -= Time.deltaTime * 3f;
            yield return 0;
        }

        this.gameObject.SetActive(false);


        OnHide?.Invoke();
    }

    public void ForceHide()
    {
        StopAllCoroutines();

        this.gameObject.SetActive(false);


        OnHide?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ForceHide();
    }
}
