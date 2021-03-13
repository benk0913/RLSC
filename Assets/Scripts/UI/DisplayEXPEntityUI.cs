using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayEXPEntityUI : MonoBehaviour
{
    public static DisplayEXPEntityUI Instance;

    [SerializeField]
    Image FillImage;

    [SerializeField]
    TextMeshProUGUI GainValueText;

    [SerializeField]
    TextMeshProUGUI ExpValueText;

    int GainExp;
    int CurrentExp;

    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        FillImage.fillAmount = Mathf.Lerp(FillImage.fillAmount, ((float)CORE.Instance.Room.PlayerActor.exp / CORE.Instance.Data.content.ExpChart[CORE.Instance.Room.PlayerActor.level]), Time.deltaTime);
    }

    public void Show(int gainEXP, int currentEXP)
    { 
        CurrentExp = currentEXP;
        GainExp = gainEXP;

        ExpValueText.text = CurrentExp.ToString();

        StopAllCoroutines();
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public IEnumerator RollText(int fromNumber, int toNumber, TextMeshProUGUI label)
    {
        float t = 0f;
        while(t<1f)
        {
            label.text = Mathf.RoundToInt(Mathf.Lerp(fromNumber, toNumber, t)).ToString();

            yield return 0;
        }
    }

    public void RollGainText()
    {
        StartCoroutine(RollText(0, GainExp, GainValueText));
    }

    public void RollExpValueText()
    {
        StartCoroutine(RollText(GainExp, 0, GainValueText));
        StartCoroutine(RollText(CurrentExp, CurrentExp+GainExp, ExpValueText));
    }
}
