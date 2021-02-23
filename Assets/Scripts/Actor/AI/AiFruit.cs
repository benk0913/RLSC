using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiFruit : ActorAI
{
    [SerializeField]
    SpriteRenderer FaceRenderer;

    public bool IsTouched;

    List<Actor> Touchers = new List<Actor>();

    float UntouchedCooldown = 5f;


    private void LateUpdate()
    {
        if(!IsTouched && UntouchedCooldown > 0f)
        {
            UntouchedCooldown -= Time.deltaTime;
        }

        if(!IsTouched && UntouchedCooldown <= 0f && FaceRenderer.gameObject.activeInHierarchy)
        {
            FaceRenderer.gameObject.SetActive(false);
        }
    }

    protected override IEnumerator AIBehaviourRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            while(!IsTouched)
            {
                if (Act.State.Data.hp < Act.State.Data.MaxHP / 3f)
                {
                    break;
                }

                WaitBehaviour();
                yield return 0;
            }

            AbilityState SelectedAbility = null;

            while (SelectedAbility == null)
            {
                while (!IsTouched)
                {
                    if (Act.State.Data.hp < Act.State.Data.MaxHP / 3f)
                    {
                        break;
                    }

                    WaitBehaviour();
                    yield return 0;
                }

                if (Act.State.Data.hp < Act.State.Data.MaxHP / 3f)
                {
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "FruitSuicide" && x.CurrentCD <= 0f);
                }
                else
                {
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "FruitPop" && x.CurrentCD <= 0f);
                }

                WaitBehaviour();

                yield return 0;
            }

            yield return 0;

            yield return StartCoroutine(UseAbilityRoutine(SelectedAbility));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Actor")
        {
            Actor toucher = collision.collider.GetComponent<Actor>();

            if (toucher.State.Data.isCharacter)
            {
                Touchers.Add(toucher);
                IsTouched = true;
                FaceRenderer.gameObject.SetActive(true);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Actor")
        {
            Actor toucher = collision.collider.GetComponent<Actor>();

            if (Touchers.Contains(toucher))
            {
                Touchers.Remove(toucher);
            }

            if(Touchers.Count == 0)
            {
                IsTouched = false;
                UntouchedCooldown = 5f;
            }
        }
    }

    
}
