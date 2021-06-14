using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberActivePanelUI : MonoBehaviour
{
    [SerializeField]
    protected Actor CurrentActor;

    [SerializeField]
    protected Image ImageFill;

    [SerializeField]
    TextMeshProUGUI NameLabel;


    protected float LastHpPercent = 1f;
    protected Coroutine UpdateBarFillRoutineInstance;

    string CurrentActorName;

    public bool IsOffline;

    void OnEnable()
    {
        CORE.Instance.SubscribeToEvent("PartyUpdated", OnPartyUpdated);
        OnPartyUpdated();
    }

    private void OnDisable()
    {
        CORE.Instance.UnsubscribeFromEvent("PartyUpdated", OnPartyUpdated);
    }
    
    private void OnPartyUpdated()
    {
        IsOffline = false;

        if(CurrentActor == null)
        {
            ActorData actor = CORE.Instance.Room.Actors.Find(x => x.name == CurrentActorName);
            if(actor == null)
            {
                IsOffline = true;
                return;
            }

            CurrentActor = actor.ActorEntity;

            if (CurrentActor == null)
            {
                IsOffline = true;
            }
        }
    }

    public void SetCurrentActor(string partyMemberName)
    {
        CurrentActorName = partyMemberName;

        NameLabel.text = CurrentActorName;

        LastHpPercent = 1f;
        ImageFill.fillAmount = 1f;

        OnPartyUpdated();

    }

    protected void Update()
    {

        float hpPercent = 1f;
        if (CurrentActor != null)
        {
            hpPercent = (float)CurrentActor.State.Data.hp / CurrentActor.State.Data.MaxHP;
        }

        if (hpPercent != LastHpPercent)
        {
            LastHpPercent = hpPercent;
            if (UpdateBarFillRoutineInstance != null)
            {
                StopCoroutine(UpdateBarFillRoutineInstance);
            }
            UpdateBarFillRoutineInstance = StartCoroutine(UpdateBarFillRoutine());
        }
    }

    protected IEnumerator UpdateBarFillRoutine()
    {
        float initialHpPercent = ImageFill.fillAmount;
        float t = 0f;
        while (t < 0.5f)
        {
            t += Time.deltaTime * 0.5f;
            // Use sin to ease the animation.
            float easedOutTime = Mathf.Sin(t * Mathf.PI);
            ImageFill.fillAmount = Mathf.Lerp(initialHpPercent, LastHpPercent, easedOutTime);

            yield return new WaitForFixedUpdate();
        }

        UpdateBarFillRoutineInstance = null;
    }
}
