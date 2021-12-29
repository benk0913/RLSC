using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFairy : ActorAI
{

    public float ChaseDistanceMax;

    public GameObject[] NPCPoints = new GameObject[0];

    public int WaypointIndex = 0;

    protected override void Awake()
    {
        base.Awake();
        NPCPoints = GameObject.FindGameObjectsWithTag("NPC_Point");
        WaypointIndex = Random.Range(0,NPCPoints.Length);
    }

    protected override void Start()
    {
        base.Start();
        ChaseDistance = Random.Range(ChaseDistance, ChaseDistanceMax);
    }

    protected override IEnumerator AIBehaviourRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            AbilityState SelectedAbility = null;

            while (SelectedAbility == null)
            {
                

                if (!Act.State.Data.states.ContainsKey("Flying"))
                {
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "BatFlight" && x.CurrentCD <= 0f);

                    

                    if(SelectedAbility != null)
                    {
                        break;
                    }
                }

                
                if(Act.State.Data.hp < (Act.State.Data.MaxHP*0.4f))
                {
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "MirrorImage" && x.CurrentCD <= 0f);
                    
                    if(SelectedAbility != null)
                    {
                        break;
                    }

                }

                if(Act.State.Data.hp < (Act.State.Data.MaxHP*0.6f))
                {
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "FlipScreen" && x.CurrentCD <= 0f);
                    
                    if(SelectedAbility != null)
                    {
                        break;
                    }
                }

                if(CanSpawnMore)
                {

                    if(Act.State.Data.hp < (Act.State.Data.MaxHP*0.8f))
                    {
                        SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "SpawnWispsRed" && x.CurrentCD <= 0f);
                        
                        if(SelectedAbility != null)
                        {
                            break;
                        }
                    }

                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "SpawnWispsGreen" && x.CurrentCD <= 0f);
                    
                    if(SelectedAbility != null)
                    {
                        break;
                    }
                }

 




                MoveToTarget();

                yield return 0;
            }

            yield return 0;

            yield return StartCoroutine(UseAbilityRoutine(SelectedAbility));

            yield return 0;
        }
    }
    

    protected override void MoveToTarget()
    {
        if(Vector2.Distance(transform.position,NPCPoints[WaypointIndex].transform.position) < 0.5f)
        {
            WaypointIndex++;
            if(WaypointIndex >= NPCPoints.Length)
            {
                WaypointIndex = 0;
            }
        }

        float verticalDistance = Mathf.Abs(NPCPoints[WaypointIndex].transform.position.y -  transform.position.y);
        float horizontalDistance  = Mathf.Abs(NPCPoints[WaypointIndex].transform.position.x - transform.position.x);
        if(verticalDistance - horizontalDistance < 1f)//Not that high or not that low
        {
            if (NPCPoints[WaypointIndex].transform.position.x > transform.position.x && !rhitRight)
            {
                Act.AttemptMoveRight();
            }
            else if (NPCPoints[WaypointIndex].transform.position.x < transform.position.x && !rhitLeft)
            {
                Act.AttemptMoveLeft();
            }
        }

        if (NPCPoints[WaypointIndex].transform.position.y > transform.position.y)
        {
            Act.AttemptMoveUp();
        }
        else if (NPCPoints[WaypointIndex].transform.position.y < transform.position.y)
        {
            Act.AttemptMoveDown();
        }

    }

    
    bool CanSpawnMore
    {
        get
        {
            return CORE.Instance.Room.Actors.FindAll(x => x.isMob && x.ActorEntity != null && !x.ActorEntity.IsDead).Count < 20;
        }
    }

}
