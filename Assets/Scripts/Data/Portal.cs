using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Portal", menuName = "Data/Portal", order = 2)]
[Serializable]
public class Portal : ScriptableObject
{
    public string TargetScene;
    public bool IsExpeditionLocked;
}
