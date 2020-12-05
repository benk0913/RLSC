using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantForceEntity : MonoBehaviour
{
    [SerializeField]
    Rigidbody2D Rigid;

    public bool DirectionBasedOnScale = true;

    public Vector2 Direction;

    private void Update()
    {
        Rigid.position += 
            (DirectionBasedOnScale? -transform.localScale.x : 1f) 
            * (Vector2)transform.TransformDirection(Direction) 
            * Time.deltaTime;
    }
}
