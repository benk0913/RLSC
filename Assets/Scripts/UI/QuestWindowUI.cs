using System;
using EdgeworldBase;
using SimpleJSON;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class QuestWindowUI : MonoBehaviour, WindowInterface
{
    public static QuestWindowUI Instance;

    [SerializeField]
    Transform QuestsContainer;
    
    [SerializeField]
    SelectionGroupUI SelectionGroup;


    [SerializeField]
    TextMeshProUGUI SelectedTitleLabel;

    [SerializeField]
    TextMeshProUGUI SelectedDescriptionLabel;

    public bool IsOpen;
    public string OpenSound;
    public string HideSound;

    public QuestData SelectedQuest;

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

        CORE.Instance.SubscribeToEvent("RefreshQuests", RefreshUI);
        CORE.Instance.SubscribeToEvent("RefreshQuests", RefreshUI);
        
        RefreshUI();
    }


    public void Show(ActorData actorData, object data = null)
    {
        IsOpen = true;

        this.gameObject.SetActive(true);
        
        
        AudioControl.Instance.Play(OpenSound);

        RefreshUI();
    

        CORE.Instance.DelayedInvokation(0.1f, () => SelectionGroup.RefreshGroup(true));
    }

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

        CORE.ClearContainer(QuestsContainer);

        foreach (string quest in CORE.PlayerActor.quests.started.Keys)
        {
            QuestWindowTitleUI element = ResourcesLoader.Instance.GetRecycledObject("QuestWindowTitleUI").GetComponent<QuestWindowTitleUI>();

            element.SetInfo(CORE.PlayerActor.quests.GetQuestProgress(quest).QuestData);
            element.transform.SetParent(QuestsContainer, false);
            element.transform.localScale = Vector3.one;
            element.transform.position = Vector3.zero;
        }

        if(SelectedQuest != null)
        {
            SelectedTitleLabel.text = CORE.QuickTranslate(SelectedQuest.name);
            SelectedDescriptionLabel.text = CORE.QuickTranslate(SelectedQuest.Description);
            SelectedDescriptionLabel.text += System.Environment.NewLine + CORE.QuickTranslate("Quest Origin")+": " + CORE.QuickTranslate(SelectedQuest.Origin);
        }
        else
        {
            SelectedTitleLabel.text = "";
            SelectedDescriptionLabel.text ="";
        }


        CORE.Instance.DelayedInvokation(0.1f,()=>SelectionGroup.RefreshGroup(true));

    }

    public void SelectQuest(QuestData data)
    {
        SelectedQuest = data;
        RefreshUI();
    }


}
