using System.Collections;
using System.Collections.Generic;
using EdgeworldBase;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuestsPanelUI : MonoBehaviour, IPointerClickHandler
{
    public static QuestsPanelUI Instance;

    [SerializeField]
    TextMeshProUGUI QuestCountLabel;

    [SerializeField]
    GameObject PanelObject;

    [SerializeField]
    Animator ShowHideAnimator;

    bool IsShowingQuests;

    public Transform Container;


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CORE.Instance.SubscribeToEvent("RefreshQuests", Refresh);
        
        Refresh();
        
        ShowQuests();
    }

    public void ToggleQuests()
    {
        if(IsShowingQuests)
        {
            HideQuests();
        }
        else
        {
            ShowQuests();
        }
    }
    void HideQuests()
    {
        ShowHideAnimator.SetTrigger("Hide");
        IsShowingQuests = false;
    }

    void ShowQuests()
    {
        ShowHideAnimator.SetTrigger("Show");
        IsShowingQuests = true;
        CORE.Instance.InvokeEvent("QuestsNotification_OFF");
    }


    private void Refresh()
    {
        QuestCountLabel.text = CORE.PlayerActor.quests.started.Count.ToString();
    
        PanelObject.SetActive(CORE.PlayerActor.quests.started.Count > 0);

        if(!IsShowingQuests) 
        {
            CORE.Instance.InvokeEvent("QuestsNotification");
        }

        float createdYPush = 0f;
        //CREATE NEW
        foreach(string questKey in CORE.PlayerActor.quests.started.Keys)
        {
            bool skip = false;
            for(int i=0;i<Container.childCount;i++)
            {
                ObjectiveSubPanelUI objSubpanel = Container.GetChild(i).GetComponent<ObjectiveSubPanelUI>();
                if(objSubpanel.CurrentQuest.name == questKey)
                {
                    skip = true;
                    break;
                }
            }

            if(skip)
            {
                continue;
            }

            ActorQuestProgress progress = CORE.PlayerActor.quests.GetQuestProgress(questKey);
            if(progress == null)
            {
                CORE.Instance.LogMessageError("NO PRGORESS? " + questKey);
                continue;
            }

            ObjectiveSubPanelUI objSubPanel = ResourcesLoader.Instance.GetRecycledObject("ObjectiveSubPanelUI").GetComponent<ObjectiveSubPanelUI>();
            
            objSubPanel.transform.localScale = Vector3.one;

            objSubPanel.SetInfo(progress.QuestData);
            createdYPush = 40f;

            if(Container.childCount > 0)
            {
                Transform mostLowLeftChild = Container.GetChild(Container.childCount-1);
                objSubPanel.transform.SetParent(Container,false);
                for(int i=0;i<Container.childCount;i++)
                {
                    if(Container.GetChild(i) == objSubPanel.transform)
                    {
                        continue;
                    }
                    
                    if(Container.GetChild(i).position.x < mostLowLeftChild.position.x && Container.GetChild(i).position.y < mostLowLeftChild.position.y)
                    {
                        mostLowLeftChild = Container.GetChild(i);
                    }
                }

                objSubPanel.transform.position = mostLowLeftChild.position + new Vector3(0f,-createdYPush,0);
            }
            else
            {
                objSubPanel.transform.SetParent(Container,false);
                objSubPanel.transform.position = Container.transform.position + new Vector3(-createdYPush,-createdYPush,0);
            }
            
        }

        //REMOVE OLD
        for(int i=0;i<Container.childCount;i++)
        {
            ObjectiveSubPanelUI objSubpanel = Container.GetChild(i).GetComponent<ObjectiveSubPanelUI>();
            if(!CORE.PlayerActor.quests.started.ContainsKey(objSubpanel.CurrentQuest.name))
            {
                objSubpanel.transform.SetParent(transform);
                objSubpanel.gameObject.SetActive(false);
            }
        }
    }

    
    public void Wipe()
    {
         CORE.ClearContainer(Container);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            CORE.Instance.ShowQuestsUIWindow();
        }
        else
        {
            ToggleQuests();
        }
    }
}
