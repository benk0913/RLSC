using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorControlClient : MonoBehaviour
{
    [SerializeField]
    Rigidbody2D Rigid;

    [SerializeField]
    Transform Body;

    [SerializeField]
    float MovementSpeed = 1f;

    [SerializeField]
    string UniqueScreenEffect;

    [SerializeField]
    GameObject UniqueScreenEffectObject;


    private void OnEnable()
    {
        if(!string.IsNullOrEmpty(UniqueScreenEffect))
        {
            UniqueScreenEffectObject = CORE.Instance.ShowScreenEffect(UniqueScreenEffect,null,true);
        }
    }

    private void OnDisable()
    {
        if(UniqueScreenEffectObject!=null)
        {
            UniqueScreenEffectObject.SetActive(false);
            UniqueScreenEffectObject = null;
        }
    }

    private void OnDestroy()
    {
        if (UniqueScreenEffectObject != null)
        {
            UniqueScreenEffectObject.SetActive(false);
            UniqueScreenEffectObject = null;
        }
    }

    private void Update()
    {
        if(!CORE.Instance.IsInputEnabled)
        {
            return;
        }

        if(Input.GetKey(InputMap.Map["Move Left"]) || Input.GetKey(InputMap.Map["Secondary Move Left"]) || (CORE.Instance.IsUsingJoystick && Input.GetAxis("Horizontal") < 0f))
        {
            Rigid.position += Vector2.left * Time.deltaTime * MovementSpeed;
            Body.transform.localScale = new Vector3(1,1,1);
        }
        else if(Input.GetKey(InputMap.Map["Move Right"]) || Input.GetKey(InputMap.Map["Secondary Move Right"]) || (CORE.Instance.IsUsingJoystick && Input.GetAxis("Horizontal") > 0f))
        {
            Rigid.position += Vector2.right * Time.deltaTime * MovementSpeed;
            Body.transform.localScale = new Vector3(-1, 1, 1);
        }

        if (Input.GetKey(InputMap.Map["Move Up"]) || Input.GetKey(InputMap.Map["Secondary Move Up"]) || (CORE.Instance.IsUsingJoystick && Input.GetAxis("Vertical") > 0f))
        {
            Rigid.position += Vector2.up * Time.deltaTime * MovementSpeed;
        }
        else if (Input.GetKey(InputMap.Map["Move Down"]) || Input.GetKey(InputMap.Map["Secondary Move Down"]) || (CORE.Instance.IsUsingJoystick && Input.GetAxis("Vertical") < 0f))
        {
            Rigid.position += Vector2.down * Time.deltaTime * MovementSpeed;
        }
        
    }

    private void FixedUpdate()
    {
        Rigid.velocity = Vector2.Lerp(Rigid.velocity, Vector2.zero, Time.deltaTime);
    }
}
