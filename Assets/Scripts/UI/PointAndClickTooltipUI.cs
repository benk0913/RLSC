using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PointAndClickTooltipUI : MonoBehaviour
{
    public static PointAndClickTooltipUI Instance;

    [SerializeField]
    TextMeshProUGUI Text;

    [SerializeField]
    CanvasGroup CG;

    [SerializeField]
    RectTransform rectT;

    Coroutine ShowRoutineInstance;

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Show()
    {

        this.gameObject.SetActive(true);

        if (ShowRoutineInstance != null)
        {
            StopCoroutine(ShowRoutineInstance);
        }

        ShowRoutineInstance = StartCoroutine(ShowRoutine());

        transform.SetAsLastSibling();
    }

    public void Show(string message, List<TooltipBonus> bonuses = null)
    {
        if(string.IsNullOrEmpty(message))
        {
            return;
        }

        bool xInRightSide = Input.mousePosition.x > Screen.width / 2;
        bool yInUpperSide = Input.mousePosition.y > Screen.height / 2;

        rectT.anchorMin = new Vector2(xInRightSide ? 1 : 0, yInUpperSide ? 1 : 0);
        rectT.anchorMax = new Vector2(xInRightSide ? 1 : 0, yInUpperSide ? 1 : 0);
        rectT.pivot = new Vector2(xInRightSide ? 1 : 0, yInUpperSide ? 1 : 0);

        Text.text = message;

        Show();

        ClearBonuses();

        if (bonuses != null)
        {
            foreach (TooltipBonus bonus in bonuses)
            {
                GameObject bonusObj = ResourcesLoader.Instance.GetRecycledObject("TooltipBonusInstance");
                bonusObj.transform.SetParent(transform);
                bonusObj.transform.SetAsLastSibling();
                bonusObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = bonus.Text;
                bonusObj.transform.GetChild(1).GetComponent<Image>().sprite = bonus.Icon;
            }
        }
    }

    void ClearBonuses()
    {
        while(transform.childCount > 1)
        {
            transform.GetChild(transform.childCount - 1).gameObject.SetActive(false);
            transform.GetChild(transform.childCount - 1).SetParent(transform.parent);
        }
    }

    IEnumerator ShowRoutine()
    {
        CG.alpha = 0f;

        yield return new WaitForSeconds(0.1f);

        while (CG.alpha < 1f)
        {
            CG.alpha += 2f * Time.deltaTime;

            yield return 0;
        }
    }

    private void Update()
    {
        transform.position = Input.mousePosition;
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
