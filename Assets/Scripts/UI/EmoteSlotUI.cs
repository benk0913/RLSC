using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class EmoteSlotUI : MonoBehaviour
{
    public Image EmoteIcon;

    public Emote CurrentEmote;
    public void SetInfo(string emoteKey)
    {

        CurrentEmote = CORE.Instance.Data.content.Emotes.Find(X=>X.name == emoteKey);

        if(CurrentEmote == null)
        {
             EmoteIcon.sprite = null;
             EmoteIcon.enabled = false;
            return;
        }

        EmoteIcon.enabled = true;
        EmoteIcon.sprite = CurrentEmote.EmoteGraphic;
    }

    public void PlayEmote()
    {
        if(CurrentEmote == null)
        {
            return;
        }

        CORE.PlayerActor.ActorEntity.Emote(CurrentEmote);
        ConsoleInputUI.Instance.Hide();
    }
}
