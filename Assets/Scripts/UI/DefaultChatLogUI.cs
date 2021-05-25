using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DefaultChatLogUI : MonoBehaviour
{
    public static DefaultChatLogUI Instance;

    public CanvasGroup ShortLogCG;
    
    
    public int ShortLogCap = 3;

    private void Awake()
    {
        Instance = this;
        ShortLogCG.alpha = 0f;
    }

    private void Update()
    {
        if (ConsoleInputUI.Instance.IsTyping)
        {
            if (ShortLogCG.alpha > 0f)
            {
                StopAllCoroutines();
                ShortLogCG.alpha = 0f;
            }

        }
    }

    public void AddLogMessage(string message)
    {

        TextMeshProUGUI logPiece = ResourcesLoader.Instance.GetRecycledObject("LogMessagePiece").GetComponent<TextMeshProUGUI>();
        logPiece.transform.SetParent(ShortLogCG.transform, false);
        logPiece.transform.localScale = Vector3.one;
        logPiece.transform.position = Vector3.zero;
        logPiece.text = message;


        if (ShortLogCG.transform.childCount > ShortLogCap)
        {
            ShortLogCG.transform.GetChild(0).gameObject.SetActive(false);
            ShortLogCG.transform.GetChild(0).SetParent(transform);
        }

        if(ConsoleInputUI.Instance.IsTyping)
        {
            return;
        }

        StopAllCoroutines();
        StartCoroutine(DisplayShortLog());
    }


    IEnumerator DisplayShortLog()
    {
        while (ShortLogCG.alpha < 1f)
        {
            ShortLogCG.alpha += 1f * Time.deltaTime;
            yield return 0;
        }

        yield return new WaitForSeconds(3f);

        while (ShortLogCG.alpha > 0f)
        {
            ShortLogCG.alpha -= 1f * Time.deltaTime;
            yield return 0;
        }
    }
}
