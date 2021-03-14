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
    TextMeshProUGUI ExpValueText;

    int CurrentExp;

    [SerializeField]
    CanvasGroup CG;

    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void Show(int currentEXP)
    {
        this.gameObject.SetActive(true);

        
        StopAllCoroutines();

        StartCoroutine(ShowRoutine(currentEXP));
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    IEnumerator ShowRoutine(int currentExp)
    {
        while(CG.alpha < 1f)
        {
            CG.alpha += 1f * Time.deltaTime;
            yield return 0;
        }
        
        float t = 0f;
        while(t<1f)
        {
            t += Time.deltaTime;


            CurrentExp = Mathf.RoundToInt(Mathf.Lerp(CurrentExp, currentExp, t));

            ExpValueText.text = CurrentExp.ToString();

            FillImage.fillAmount = Mathf.Lerp(FillImage.fillAmount, ((float)CurrentExp / CORE.Instance.Data.content.ExpChart[CORE.Instance.Room.PlayerActor.level]), Time.deltaTime);

            yield return 0;
        }

        CurrentExp = currentExp;

        yield return new WaitForSeconds(1f);

        while (CG.alpha > 0f)
        {
            CG.alpha -= 1f * Time.deltaTime;
            yield return 0;
        }
    }


}
