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


    Transform CurrentAnchor;

    public void Show(Transform anchor, string message)
    {
        CurrentAnchor = anchor;
        StartCoroutine(ShowRoutine(message));
    }

    private void Update()
    {
        transform.position = Vector2.Lerp(transform.position, CurrentAnchor.position, Time.deltaTime * 6f);
    }

    IEnumerator ShowRoutine(string message)
    {
        ContentText.text = "";

        CG.alpha = 0f;
        while(CG.alpha < 1f)
        {
            CG.alpha += Time.deltaTime * 3f;
            yield return 0;
        }

        //TODO Replace with character pitch?
        float randomPitch = Random.Range(0.5f, 1.5f);

        while (ContentText.text.Length < message.Length)
        {
            ContentText.text += message[ContentText.text.Length];

            if (ContentText.text.Length % 3 == 0)
            {
                AudioControl.Instance.PlayInPosition("talksound",transform.position,200f, randomPitch);
            }

            yield return 0;
            yield return 0;
        }

        yield return new WaitForSeconds(2f + (message.Length * DELAY_PER_LETTER));

        CG.alpha = 1f;
        while (CG.alpha > 0f)
        {
            CG.alpha -= Time.deltaTime * 3f;
            yield return 0;
        }

        this.gameObject.SetActive(false);
    }
}
