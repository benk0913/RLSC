using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingObject : MonoBehaviour
{
    public void HomeToTarget(Vector3 target,Action OnComplete = null)
    {
        StopAllCoroutines();
        StartCoroutine(HomingRoutine(target,OnComplete));
    }

    IEnumerator HomingRoutine(Vector3 targetPosition, Action OnComplete = null)
    {
        Vector2 initPos = transform.position;

        float  rndHeight = UnityEngine.Random.Range(0f,10f);
        float t=0f;
        while(t<0.5f)
        {
            t+=Time.deltaTime * 0.5f;

            transform.position = Util.SplineLerpX(initPos,new Vector3(targetPosition.x,targetPosition.y,0),rndHeight,t+t);

            yield return 0;
        }
        OnComplete?.Invoke();
    }
}
