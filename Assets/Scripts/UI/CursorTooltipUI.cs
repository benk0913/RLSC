using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CursorTooltipUI : MonoBehaviour
{
    public static CursorTooltipUI TooltipInstance;

    [SerializeField]
    protected TextMeshProUGUI TextLabel;


    [SerializeField]
    protected CanvasGroup CG;

    Color baseColor;

    Coroutine IsShowingRoutine;

    protected List<MessageQueItem> MessageQue = new List<MessageQueItem>();

    private void Awake()
    {
        TooltipInstance = this;
    }

    private void Start()
    {
        baseColor = TextLabel.color;
    }

    public void Show(string message)
    {
        StopAllCoroutines();
        IsShowingRoutine = StartCoroutine(FadeIn());

        TextLabel.text = message;
        TextLabel.color = baseColor;
    }

    public void Show(string message, float Length)
    {
        StopAllCoroutines();
        IsShowingRoutine = StartCoroutine(FadeIn(Length));

        TextLabel.text = message;
        TextLabel.color = baseColor;
    }


    public void Show(string message, float Length, Color color)
    {
        if(IsShowingRoutine != null)
        {
            if(MessageQue.Find(x=>x.Message == message) != null)
            {
                return;
            }

            MessageQue.Add(new MessageQueItem(message, Length, color));
            return;
        }

        IsShowingRoutine = StartCoroutine(FadeIn(Length));

        TextLabel.text = message;
        TextLabel.color = color;
    }

    public void Hide()
    {
        if(MessageQue.Count > 0)
        {
            Show(MessageQue[0].Message, MessageQue[0].Length, MessageQue[0].WithColor);
            MessageQue.RemoveAt(0);
            return;
        }

        StopAllCoroutines();
        StartCoroutine(FadeOut());

    }

    protected IEnumerator FadeIn(float Length = 0f)
    {
        while (CG.alpha < 1f)
        {
            CG.alpha += 2f * Time.deltaTime;

            yield return 0;
        }

        if (Length > 0)
        {
            yield return new WaitForSeconds(Length);

            IsShowingRoutine = null;
            Hide();
            yield break;
        }

        IsShowingRoutine = null;
        Hide();
    }

    protected IEnumerator FadeOut()
    {
        while (CG.alpha > 0f)
        {
            CG.alpha -= 4f * Time.deltaTime;

            yield return 0;
        }
    }

    protected class MessageQueItem
    {
        public string Message;
        public float Length;
        public Color WithColor;

        public MessageQueItem(string message, float length, Color color)
        {
            this.Message = message;
            this.Length = length;
            this.WithColor = color;
        }
    }
}
