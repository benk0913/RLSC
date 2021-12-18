using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIWillowKing : ActorAI
{
  
    protected override IEnumerator AIBehaviourRoutine()
    {
        yield return 0;
        while (true)
        {
            AbilityState SelectedAbility = null;

            while (SelectedAbility == null)
            {
                List<AbilityState> cdProtections = Act.State.Abilities.FindAll(x => x.CurrentAbility.name.Contains("Protection") && x.CurrentCD > 0f);
                if (Act.State.Buffs.Find(X=>X.CurrentBuff.name.Contains("Protection")) == null && (cdProtections == null || cdProtections.Count == 0))
                {
                    int rndProt = Random.Range(0,3);

                    if(rndProt == 0)
                    {
                        SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Protection Of Blossom Spell" && x.CurrentCD <= 0f);
                    }
                    else if(rndProt == 1)
                    {
                        SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Protection Of Growth Spell" && x.CurrentCD <= 0f);
                    }
                    else if(rndProt == 2)
                    {
                        SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Protection Of Nature Spell" && x.CurrentCD <= 0f);
                    }
                }
                else
                {
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "WillowKingSmash" && x.CurrentCD <= 0f);
                }

                WaitBehaviour();

                yield return 0;
            }
            
            yield return new WaitForSeconds(Random.Range(0f,0.2f));

            yield return StartCoroutine(UseAbilityRoutine(SelectedAbility));
            

            yield return 0;
        }
    }



    

}
