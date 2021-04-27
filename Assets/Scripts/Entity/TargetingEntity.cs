using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingEntity : MonoBehaviour
{
    ActorAI CurrentAI;

    [SerializeField]
    LineRenderer Renderer;


    public void SetInfo(ActorAI ai)
    {
        CurrentAI = ai;
        StartCoroutine(FadeLine());
    }

    void Update()
    {
        if(CurrentAI == null)
        {
            this.gameObject.SetActive(false);
            return;
        }
        
        if(!CurrentAI.gameObject.activeInHierarchy)
        {
            CurrentAI = null;
        }

        if(CurrentAI.CurrentTarget == null)
        {
            return;
        }
        
        List<Vector3> linkPositions = new List<Vector3>();

        linkPositions.Add(CurrentAI.Act.Body.transform.position + new Vector3(0f,2f));
        linkPositions.Add(CurrentAI.CurrentTarget.Body.transform.position + new Vector3(0f, 2));

        Renderer.positionCount = linkPositions.Count;
        Renderer.SetPositions(linkPositions.ToArray());


    }

    IEnumerator FadeLine()
    {

        float opacity = 0f;

        while (true)
        {

            opacity = 0f;

            while (opacity < 1f)
            {
                opacity += Time.deltaTime * 2f;
                Renderer.startColor = new Color(Renderer.startColor.r, Renderer.startColor.g, Renderer.startColor.b, opacity);
                Renderer.endColor = new Color(Renderer.endColor.r, Renderer.endColor.g, Renderer.endColor.b, opacity);
                yield return 0;
            }

            while (opacity > 0f)
            {
                opacity -= Time.deltaTime * 2f;
                Renderer.startColor = new Color(Renderer.startColor.r, Renderer.startColor.g, Renderer.startColor.b, opacity);
                Renderer.endColor = new Color(Renderer.endColor.r, Renderer.endColor.g, Renderer.endColor.b, opacity);
                yield return 0;
            }

            yield return new WaitForSeconds(5f);

        }
    }
}
