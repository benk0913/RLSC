using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTargetUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    public string Text;

    public List<TooltipBonus> Bonuses;

    public void OnPointerEnterSimple()
    {
        OnPointerEnter(null);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PointAndClickTooltipUI.Instance.Show(Text, Bonuses);
    }

    public void OnPointerExitSimple()
    {
        OnPointerExit(null);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Hide();
    }

    public void Hide()
    {
        PointAndClickTooltipUI.Instance.Hide();
    }

    public void SetTooltip(string text, List<TooltipBonus> tooltipBonuses = null)
    {
        this.Text = text;

        this.Bonuses = tooltipBonuses;
    }

    public void ShowOnPosition(Vector3 position, float pivotX = -1, float pivotY = -1)
    {
        PointAndClickTooltipUI.Instance.Show(Text, Bonuses, position, pivotX, pivotY);
    }

    void OnDisable()
    {
        if(PointAndClickTooltipUI.Instance == null)
        {
            return;
        }

        PointAndClickTooltipUI.Instance.Hide();
    }
}

public class TooltipBonus
{
    public string Text;
    public Sprite Icon;

    public TooltipBonus(string text ,Sprite icon)
    {
        this.Text = text;
        this.Icon = icon;
    }
}
