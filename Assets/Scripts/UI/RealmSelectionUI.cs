using EdgeworldBase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RealmSelectionUI : MonoBehaviour
{
    public static RealmSelectionUI Instance;
    
    Action<int> OnCompleteAction;

    [SerializeField]
    Transform RealmContainer;

    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void Show(Action<int> onComplete)
    {
        this.gameObject.SetActive(true);

        this.OnCompleteAction = onComplete;

        StopAllCoroutines();
        StartCoroutine(PopulateRealms());
        
    }

    IEnumerator PopulateRealms()
    {
        CORE.ClearContainer(RealmContainer);

        while(ResourcesLoader.Instance.m_bLoading)
        {
            yield return 0;
        }

        for (int i = 0; i < CORE.Instance.Data.content.RealmCap; i++)
        {
            int realmIndex = i;
            RealmSigilUI realmSigil = ResourcesLoader.Instance.GetRecycledObject("RealmSigilUI").GetComponent<RealmSigilUI>();
            realmSigil.SetData(CORE.Instance.Data.content.Realms[i]);
            realmSigil.transform.SetParent(RealmContainer, false);
            realmSigil.GetComponent<Button>().onClick.RemoveAllListeners();

            yield return 0;

            realmSigil.GetComponent<Button>().onClick.AddListener(() =>
            {
                this.OnCompleteAction(realmIndex); Hide();
            });

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}

