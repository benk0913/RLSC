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
        Vector3? posi = null;
        PointAndClickTooltipUI.Instance.Show(Text, Bonuses,posi);
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

    public void ShowOnTransform(Transform trans, float pivotX = -1, float pivotY = -1)
    {
        PointAndClickTooltipUI.Instance.Show(Text, Bonuses, trans, pivotX, pivotY);
    }

    public void OnSelectionEnter()
    {
        ShowOnTransform(transform);
    }

    public void OnSelectionExit()
    {
        Hide();
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
