using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster", menuName = "Data/Monster", order = 2)]
[Serializable]
public class Monster : ScriptableObject
{
    public AIChaseBehaviourOld ChaseBehaviour;
}

[Serializable]
public enum AIChaseBehaviourOld
{
    Static,
    Patrol,
    Chase,
    Flee
}

