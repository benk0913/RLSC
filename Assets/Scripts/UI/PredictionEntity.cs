using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredictionEntity : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    void Start()
    {
        CORE.Instance.SubscribeToEvent("PredictionUpdate",RefreshState);
        RefreshState();
    }

    public void RefreshState()
    {
        if(string.IsNullOrEmpty(CORE.Instance.NextScenePrediction))
        {
            this.gameObject.SetActive(false);
        }

        SceneInfo predictionScene =  CORE.Instance.Data.content.Scenes.Find(x=>x.sceneName == CORE.Instance.NextScenePrediction);;

        if(predictionScene == null)
        {
            CORE.Instance.LogMessageError("No Prediction Scene");
            this.gameObject.SetActive(false);
            return;
        }
        
        if( predictionScene.PredictionImage == null)
        {
            CORE.Instance.LogMessageError("No Prediction Scene Image");
            this.gameObject.SetActive(false);
            return;
        }

        this.gameObject.SetActive(true);
        spriteRenderer.sprite = predictionScene.PredictionImage;
    }

}
