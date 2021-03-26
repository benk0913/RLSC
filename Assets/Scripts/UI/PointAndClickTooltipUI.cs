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

    private Vector3? AnchorPosition;

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

    public void Show(string message, List<TooltipBonus> bonuses = null, Vector3? position = null)
    {
        if(string.IsNullOrEmpty(message))
        {
            return;
        }
        AnchorPosition = position;

        PositionTooltip();

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

    void PositionTooltip()
    {
        Vector3 BasePosition = AnchorPosition != null ? (Vector3)AnchorPosition : Input.mousePosition;
        bool xInRightSide = BasePosition.x > Screen.width / 2;
        bool yInUpperSide = BasePosition.y > Screen.height / 2;
        bool topLeftCorner = !xInRightSide && yInUpperSide;

        rectT.anchorMin = new Vector2(xInRightSide ? 1 : 0, yInUpperSide ? 1 : 0);
        rectT.anchorMax = new Vector2(xInRightSide ? 1 : 0, yInUpperSide ? 1 : 0);
        rectT.pivot = new Vector2(xInRightSide ? 1 : 0, yInUpperSide ? 1 : 0);

        transform.position = BasePosition + new Vector3(topLeftCorner ? 12 : 0, 0, 0);
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

        while (CG.alpha < 0.9f)
        {
            CG.alpha += Mathf.Min(6f * Time.deltaTime, 0.9f);

            yield return 0;
        }
    }

    private void Update()
    {
        PositionTooltip();
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
