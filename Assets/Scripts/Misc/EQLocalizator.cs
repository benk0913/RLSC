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
        CORE.Instance.DelayedInvokation(0.1f, () => {
            
            string translation = label.text;
            if (!CORE.Instance.Data.Localizator.mSource.TryGetTranslation(label.text, out translation))
            {
                return;
            }

            if (string.IsNullOrEmpty(translation))
            {
                return;
            }

            if (translation == label.text)
            {
                return;
            }

            label.text = translation;
        });
    }
}
