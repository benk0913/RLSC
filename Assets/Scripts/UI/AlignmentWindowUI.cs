using System;
using System.Collections.Generic;
using EdgeworldBase;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AlignmentWindowUI : MonoBehaviour, WindowInterface
{
    public static AlignmentWindowUI Instance;

  

    [SerializeField]
    SelectionGroupUI SGroup;

    public bool IsOpen;

    public string OpenSound;
    public string HideSound;

    public UnityEvent OnAlignmentGood;
    public UnityEvent OnAlignmentEvil;

    public List<AlignmentAbilityUI> Abilities = new List<AlignmentAbilityUI>();

    public Image KarmaExpFiller;

    float targetKarmaValue = 0f;

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    void Start()
    {
        if(Instance == null)
        {
            return;
        }

        CORE.Instance.SubscribeToEvent("AlignmentUpdated", RefreshUI);
        RefreshUI();
    }

    void Update()
    {
        KarmaExpFiller.fillAmount = Mathf.Lerp(KarmaExpFiller.fillAmount, targetKarmaValue, Time.deltaTime*0.7f);
    }

    [Obsolete("Do not call Show directly. Call `CORE.Instance.ShowWindow()` instead.")]
    public void Show(ActorData actorData, object data = null)
    {
        IsOpen = true;

        this.gameObject.SetActive(true);
        
        
        AudioControl.Instance.Play(OpenSound);

        
        RefreshUI();
    }

    [Obsolete("Do not call Hide directly. Call `CORE.Instance.CloseCurrentWindow()` instead.")]
    public void Hide()
    {
        IsOpen = false;
        this.gameObject.SetActive(false);

        AudioControl.Instance.Play(HideSound);
    }
    

    public void RefreshUI()
    {
        if (!IsOpen)
        {
            return;
        }

        targetKarmaValue = (float)CORE.PlayerActor.karma / (float)CORE.Instance.Data.content.alignmentData.MaxKarma;


        if(CORE.PlayerActor.alignmentGood)
        {
            OnAlignmentGood?.Invoke();

            for(int i=0;i<Abilities.Count;i++)
            {
                if(CORE.Instance.Data.content.alignmentData.GoodAbilities.Count <= i)
                {
                    break;
                }

                Abilities[i].SetInfo(CORE.Instance.Data.content.alignmentData.GoodAbilities[i]);
            }
        }
        else 
        {
            OnAlignmentEvil?.Invoke();

            
            for(int i=0;i<Abilities.Count;i++)
            {
                if(CORE.Instance.Data.content.alignmentData.EvilAbilities.Count <= i)
                {
                    break;
                }

                Abilities[i].SetInfo(CORE.Instance.Data.content.alignmentData.EvilAbilities[i]);
            }
        }

        CORE.Instance.DelayedInvokation(0.1f,()=>SGroup.RefreshGroup(true));
    }

 
}
