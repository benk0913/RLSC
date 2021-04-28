using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextBubbleUI : MonoBehaviour
{
    const float DELAY_PER_LETTER = 0.02f;

    [SerializeField]
    TextMeshProUGUI ContentText;

    [SerializeField]
    CanvasGroup CG;


    public void Show(string message)
    {
        StartCoroutine(ShowRoutine(message));
    }

    IEnumerator ShowRoutine(string message)
    {
        CG.alpha = 0f;
        while(CG.alpha < 1f)
        {
            CG.alpha += Time.deltaTime * 3f;
            yield return 0;
        }

        //TODO Replace with character pitch?
        float randomPitch = Random.Range(0.1f, 2f);

        while (ContentText.text.Length < message.Length)
        {
            ContentText.text += message[ContentText.text.Length];

            if (ContentText.text.Length % 3 == 0)
            {
                AudioControl.Instance.PlayWithPitch("talksound", randomPitch);
            }

            yield return 0;
        }

        yield return new WaitForSeconds(1f + (message.Length * DELAY_PER_LETTER));

        CG.alpha = 1f;
        while (CG.alpha > 0f)
        {
            CG.alpha -= Time.deltaTime * 3f;
            yield return 0;
        }

        this.gameObject.SetActive(false);
    }
}
