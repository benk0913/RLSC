using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(PositionConstraint))]
public class InterpolatedChildEntity : MonoBehaviour
{
    PositionConstraint pc;
    [SerializeField]
    float Speed = 10f;
    void Start()
    {
        pc = GetComponent<PositionConstraint>();
        ConstraintSource cs = new ConstraintSource();
        cs.sourceTransform = CORE.Instance.transform;
        cs.weight = 1f;
        pc.AddSource(cs);
        pc.constraintActive = true;
        pc.locked = true;
        pc.translationOffset = transform.parent.position;
    }

    void Update()
    {
        pc.translationOffset = Vector3.Lerp(pc.translationOffset, transform.parent.position, Time.deltaTime * Speed);
    }
}
