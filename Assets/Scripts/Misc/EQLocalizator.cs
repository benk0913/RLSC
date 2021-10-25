using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EQLocalizator : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI label;
    private void Start()
    {
        System.Action<Object> translateAction = (Object obj) =>
        {
            if (obj != label)
                return;

            string translation = label.text;
            if(!CORE.Instance.Data.Localizator.mSource.TryGetTranslation(label.text, out translation))
            {
                CORE.Instance.Data.Localizator.mSource.AddTerm(label.text);
                return;
            }

            if (translation != label.text)
            {
                label.text = translation;
            }
        };

        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(translateAction);
        translateAction.Invoke(label);
    }
}
