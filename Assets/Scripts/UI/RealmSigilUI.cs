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

    public void SetData(RealmData currentRealm)
    {
        this.CurrentRealm = currentRealm;

        this.RealmIcon.sprite = CurrentRealm.RealmIcon;
        this.RealmName.text = CurrentRealm.Name;
        this.RealmName.color = CurrentRealm.RealmColor;
        // Realm capacity arrives in a delay, so default it to empty.
        this.RealmCapacity.text = "";
    }

    public void SetCapacity(string capacity)
    {
        this.RealmCapacity.text = capacity;
    }
}
