using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PromoCode", menuName = "Data/PromoCode", order = 2)]
[Serializable]
public class PromoCode : ScriptableObject
{
    public string Key;
    public int Usages;
    public List<AbilityParam> Rewards;
}
