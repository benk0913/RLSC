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
        if(Input.GetKey(InputMap.Map["Move Left"]))
        {
            Rigid.position += Vector2.left * Time.deltaTime * MovementSpeed;
            Body.transform.localScale = new Vector3(1,1,1);
        }
        else if(Input.GetKey(InputMap.Map["Move Right"]))
        {
            Rigid.position += Vector2.right * Time.deltaTime * MovementSpeed;
            Body.transform.localScale = new Vector3(-1, 1, 1);
        }

        if (Input.GetKey(InputMap.Map["Move Up"]))
        {
            Rigid.position += Vector2.up * Time.deltaTime * MovementSpeed;
        }
        else if (Input.GetKey(InputMap.Map["Move Down"]))
        {
            Rigid.position += Vector2.down * Time.deltaTime * MovementSpeed;
        }
        
    }

    private void FixedUpdate()
    {
        Rigid.velocity = Vector2.Lerp(Rigid.velocity, Vector2.zero, Time.deltaTime);
    }
}
