using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffSlotUI : MonoBehaviour
{
    BuffState ActiveBuff;

    [SerializeField]
    Image IconImage;

    [SerializeField]
    Image CooldownImage;

    [SerializeField]
    TextMeshProUGUI CooldownLabel;



    public void SetBuffState(BuffState buffState = null)
    {
        ActiveBuff = buffState;

        if(ActiveBuff == null)
        {
            IconImage.sprite = ResourcesLoader.Instance.GetSprite("emptySlot");
            return;
        }

        IconImage.sprite = ActiveBuff.CurrentBuff.Icon;
    }

    private void Update()
    {
        if(ActiveBuff == null)
        {
            return;
        }

        if(ActiveBuff.CurrentLength <= 0 && CooldownImage.gameObject.activeInHierarchy)
        {
            CooldownImage.gameObject.SetActive(false);
        }
        else if(ActiveBuff.CurrentLength > 0)
        {
            if (!CooldownImage.gameObject.activeInHierarchy)
            {
                CooldownImage.gameObject.SetActive(true);
            }

            CooldownImage.fillAmount = ActiveBuff.CurrentLength / ActiveBuff.CurrentBuff.Length;
            CooldownImage.color = ActiveBuff.CurrentBuff.isDebuff ? Color.red : Color.green;

            CooldownLabel.text = Mathf.RoundToInt(ActiveBuff.CurrentLength).ToString();
            CooldownLabel.color = CooldownImage.color;
        }
    }
}
