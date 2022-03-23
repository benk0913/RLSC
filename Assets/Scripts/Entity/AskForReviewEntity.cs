using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AskForReviewEntity : MonoBehaviour
{
    void Start()
    {
        if(PlayerPrefs.GetInt("SeenAskForReview",0) == 0)
        {
            WarningWindowUI.Instance.Show("<color=yellow>Help us</color> stay afloat and attract new players by <color=yellow>reviewing the game on the store page!</color>",()=>
            {
                #if UNITY_ANDROID || UNITY_IOS
                    Application.OpenURL("https://play.google.com/store/apps/details?id=com.XPloria.ElementQuest");
                #else
                    Application.OpenURL("https://store.steampowered.com/app/1903270/Sunset_World_Online/");
                #endif
            }
            ,false,null,"<color=green><size=10>Do you </size>see potential <size=10>in this game</size>?</color>");

            PlayerPrefs.SetInt("SeenAskForReview",1); 
        }
    }
}
