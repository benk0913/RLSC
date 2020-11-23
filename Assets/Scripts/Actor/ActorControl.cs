using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class ActorControl : MonoBehaviour
{
    

    [SerializeField]
    Actor CurrentActor;

    public float MovementSpeed = 1f;

    public float JumpHeight = 1f;

    float internalJumpCooldown = 0f;

    private void Reset()
    {
        CurrentActor = GetComponent<Actor>();
    }

    private void Update()
    {
        if(internalJumpCooldown > 0f)
        {
            internalJumpCooldown -= 1f * Time.deltaTime;
        }

        if(Input.GetKey(KeyCode.A))
        {
            CurrentActor.Rigid.position += Vector2.left * MovementSpeed * Time.deltaTime;
        }
        else if(Input.GetKey(KeyCode.D))
        {
            CurrentActor.Rigid.position += Vector2.right * MovementSpeed * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (CurrentActor.IsGrounded && internalJumpCooldown <= 0f)
            {
                CurrentActor.Rigid.AddForce(Vector2.up * JumpHeight, ForceMode2D.Impulse);
                internalJumpCooldown = 1f;
            }
        }
    }
}
