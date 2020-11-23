using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster", menuName = "Data/Monster", order = 2)]
[Serializable]
public class Monster : ScriptableObject
{
    public AttributeData Attributes;

    public AIChaseBehaviour ChaseBehaviour;

    public List<string> Abilities = new List<string>();

}

[Serializable]
public enum AIChaseBehaviour
{
    Static,
    Patrol,
    Chase,
    Flee
}

