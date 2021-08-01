using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredictionEntity : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    void Start()
    {
        if(string.IsNullOrEmpty(CORE.Instance.NextScenePrediction))
        {
            this.gameObject.SetActive(false);
        }

        SceneInfo predictionScene =  CORE.Instance.Data.content.Scenes.Find(x=>x.sceneName == CORE.Instance.NextScenePrediction);;

        if(predictionScene == null)
        {
            this.gameObject.SetActive(false);
            return;
        }
        
        if( predictionScene.PredictionImage == null)
        {
            this.gameObject.SetActive(false);
            return;
        }

        spriteRenderer.sprite = predictionScene.PredictionImage;
    }

}
