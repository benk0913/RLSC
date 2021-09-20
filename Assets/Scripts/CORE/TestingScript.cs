using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingScript : MonoBehaviour
{
    public bool Test;

    public Actor TESTACTOR;
    public Emote TESTEMOTE;

    void Update()
    {
        if(Test)
        {
            Test = false;
            
            TESTACTOR.Emote(TESTEMOTE);

        }
    }
}
