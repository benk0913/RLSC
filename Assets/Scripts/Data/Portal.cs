using System;
using UnityEngine;

[Serializable]
public class Portal
{
    public string name;
    public string TargetScene;
    public string TargetSceneGate;
    public bool IsExpeditionLocked;
    public float portalPositionX;
    public float portalPositionY;


    public Portal(string gateKey, string targetScene, string targetGateKey, Vector2 portalPos , bool isExpeditionLocked = false)
    {
        this.name = gateKey;
        this.TargetScene = targetScene;
        this.TargetSceneGate = targetGateKey;

        this.portalPositionX = portalPos.x;
        this.portalPositionY = portalPos.y;

        this.IsExpeditionLocked = isExpeditionLocked;
    }
}
