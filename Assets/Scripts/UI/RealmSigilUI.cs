using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RealmSigilUI : MonoBehaviour
{
    public Image RealmIcon;

    public TextMeshProUGUI RealmName;

    public RealmData CurrentRealm;

    public TextMeshProUGUI RealmCapacity;

    Action OnCompleteAction;

    public void SetData(RealmData currentRealm, Action onComplete = null)
    {
        this.OnCompleteAction = onComplete;
        this.CurrentRealm = currentRealm;

        this.RealmIcon.sprite = CurrentRealm.RealmIcon;
        this.RealmName.text = CurrentRealm.Name;
        this.RealmName.color = CurrentRealm.RealmColor;
        // Realm capacity arrives in a delay, so default it to empty.
        this.RealmCapacity.text = "Loading...";
    }

    public void Select()
    {
        OnCompleteAction?.Invoke();
    }


    public void SetCapacity(string capacity)
    {
        RealmCapacity cap = CORE.Instance.Data.content.RealmsCapacity.Find(x => x.Text == capacity);

        this.RealmCapacity.text = capacity;
        this.RealmCapacity.color = cap.Color;

    }
}
