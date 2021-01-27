using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster", menuName = "Data/Monster", order = 2)]
[Serializable]
public class Monster : ScriptableObject
{
    public List<State> Immunity;
    public AIChaseBehaviour ChaseBehaviour;
}

[Serializable]
public enum AIChaseBehaviour
{
    Static,
    Patrol,
    Chase,
    Flee
}

