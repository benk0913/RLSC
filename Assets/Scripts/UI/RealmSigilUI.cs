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

    public void SetData(RealmData currentRealm)
    {
        this.CurrentRealm = currentRealm;

        this.RealmIcon.sprite = CurrentRealm.RealmIcon;
        this.RealmName.text = CurrentRealm.Name;
        this.RealmName.color = CurrentRealm.RealmColor;
    }
}
