using System;
using UnityEngine;

[Serializable]
public class Portal
{
    public string GateKey;
    public string TargetScene;
    public string TargetSceneGate;
    public bool IsExpeditionLocked;

    public Portal(string gateKey, string targetScene, string targetGateKey, bool isExpeditionLocked = false)
    {
        this.GateKey = gateKey;
        this.TargetScene = targetScene;
        this.TargetSceneGate = targetGateKey;
        this.IsExpeditionLocked = isExpeditionLocked;
    }
}
