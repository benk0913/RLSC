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


    [Obsolete("Do not call Show directly. Call `CORE.Instance.ShowWindow()` instead.")]
    public void Show(ActorData actorData, object data = null)
    {
        IsOpen = true;

        this.gameObject.SetActive(true);
        
        
        AudioControl.Instance.Play(OpenSound);

        RefreshUI();
    

        CORE.Instance.DelayedInvokation(0.1f, () => SelectionGroup.RefreshGroup(true));
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

        CORE.ClearContainer(QuestsContainer);

        foreach(QuestData quest in CORE.Instance.Data.content.Quests)
        {
            if(quest == null)
            {
                continue;
            }

            QuestWindowTitleUI element = ResourcesLoader.Instance.GetRecycledObject("QuestWindowTitleUI").GetComponent<QuestWindowTitleUI>();

            element.transform.SetParent(QuestsContainer, false);
            element.transform.localScale = Vector3.one;
            element.transform.position = Vector3.zero;

            if (CORE.PlayerActor.quests.started.ContainsKey(quest.name))
            {
                element.SetInfo(CORE.PlayerActor.quests.GetQuestProgress(quest.name).QuestData);
                element.transform.SetAsFirstSibling();
            }
            else if(CORE.PlayerActor.quests.completed.ContainsKey(quest.name))
            {
                element.SetInfo(null,"<s>" +quest.name+"</s>");
                element.transform.SetAsLastSibling();
            }
            else
            {
                element.SetInfo(null,"<color=grey>" +quest.name+"</color>");
                element.transform.SetAsLastSibling();
            }

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
