using EdgeworldBase;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RealmSelectionUI : MonoBehaviour
{
    public static RealmSelectionUI Instance;
    
    [SerializeField]
    Transform RealmContainer;

    List<RealmSigilUI> realmSigils = new List<RealmSigilUI>();

    Dictionary<int, string> RealmsCapacity = new Dictionary<int, string>();

    [SerializeField]
    SelectionGroupUI SG;

    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void Show(Action<int> onComplete)
    {
        this.gameObject.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(PopulateRealms(onComplete));
        
    }

    IEnumerator PopulateRealms(Action<int> onComplete)
    {
        RealmsCapacity.Clear();
        realmSigils.Clear();
        CORE.ClearContainer(RealmContainer);

        RequestRealmsCapacity();

        while(ResourcesLoader.Instance.m_bLoading)
        {
            yield return 0;
        }

        for (int i = 0; i < CORE.Instance.Data.content.RealmCap; i++)
        {
            int realmIndex = i;
            RealmSigilUI realmSigil = ResourcesLoader.Instance.GetRecycledObject("RealmSigilUI").GetComponent<RealmSigilUI>();
            realmSigil.SetData(CORE.Instance.Data.content.Realms[realmIndex], () =>
            {
                onComplete(realmIndex);
                Hide();
            });
            // Fill capacity if it finished the request before rendering.
            if (RealmsCapacity.ContainsKey(realmIndex))
            {
                realmSigil.SetCapacity(RealmsCapacity[realmIndex]);
            }
            realmSigil.transform.SetParent(RealmContainer, false);
            
            realmSigils.Add(realmSigil);

            yield return 0;


            yield return new WaitForSeconds(0.1f);
        }


        yield return new WaitForSeconds(0.1f);
        
        SG.RefreshGroup();
    }

    void RequestRealmsCapacity()
    {
        for (int i = 0; i < CORE.Instance.Data.content.RealmCap; i++) {
            int realmIndex = i;
            SocketHandler.Instance.SendRealmCapacityRequest(realmIndex, (UnityWebRequest response) =>
                {
                    JSONNode data = JSON.Parse(response.downloadHandler.text);
                    CORE.Instance.LogMessage("Obtained Realm " + realmIndex + " Capacity - " + data["capacity"].Value);
                    RealmsCapacity.Add(realmIndex, data["capacity"].Value);
                    // Fill capacity if it finished rendering before the request.
                    if (realmSigils.Count > realmIndex)
                    {
                        realmSigils[realmIndex].SetCapacity(data["capacity"].Value);
                    }
                });
        }
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}

