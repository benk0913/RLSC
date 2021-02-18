using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICat : ActorAI
{
    public bool TheSecondCat = false;

    protected override IEnumerator AIBehaviourRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            AbilityState SelectedAbility = null;

            while (SelectedAbility == null)
            {
                if (!Act.State.Data.States.ContainsKey("Shielded"))
                {
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "CatShielding" && x.CurrentCD <= 0f);

                    if(SelectedAbility != null)
                    {
                        break;
                    }
                }

                SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "CatShieldBash" && x.CurrentCD <= 0f);

                if (SelectedAbility != null)
                {
                    break;
                }

                SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "CatSmash" && x.CurrentCD <= 0f);

                WaitBehaviour();

                yield return 0;
            }

            yield return 0;

            yield return StartCoroutine(UseAbilityRoutine(SelectedAbility));

            yield return 0;
        }
    }

    public override Actor GetCurrentTarget()
    {
        if(TheSecondCat)
        {
            return CORE.Instance.Room.LeastThreatheningActor;
        }

        return base.GetCurrentTarget();
    }

    protected override void MoveToTarget()
    {
        if (CurrentTarget == null)
        {
            return;
        }

        if (CurrentTarget.transform.position.x > transform.position.x)
        {
            if(rhitRight)
            {
                Act.AttemptJump();
            }

            Act.AttemptMoveRight();
        }
        else if (CurrentTarget.transform.position.x < transform.position.x)
        {
            if (rhitLeft)
            {
                Act.AttemptJump();
            }

            Act.AttemptMoveLeft();
        }

    }
}
