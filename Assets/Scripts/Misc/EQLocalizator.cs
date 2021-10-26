using I2.Loc;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class EQLocalizator : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI label;

    string originalText;

    private void Reset()
    {
        label = GetComponent<TextMeshProUGUI>();

        CORE.Instance.Data.Localizator.mSource.AddTerm(label.text);

    }

    private void Start()
    {
        CORE.Instance.SubscribeToEvent("LanguageChanged", Translate);
    }

    private void OnEnable()
    {
        Translate();
    }

    void Translate()
    {
        System.Action translateAction = () => {

            string translation = label.text;

            if (!LocalizationManager.TryGetTranslation(label.text, out translation))
            {
                return;
            }
            else
            {
                if (string.IsNullOrEmpty(translation))
                {
                    return;
                }

                if (translation == label.text)
                {
                    return;
                }

                if (LocalizationManager.IsRTL(LocalizationManager.CurrentLanguageCode))
                {
                    label.text = LocalizationManager.ApplyRTLfix(label.text);
                    return;
                }
                else
                {
                    label.text = translation;
                }
            }


        };

        if (CORE.Instance != null)
            CORE.Instance.DelayedInvokation(0.1f, translateAction);
        else
            translateAction.Invoke();
    }
}
