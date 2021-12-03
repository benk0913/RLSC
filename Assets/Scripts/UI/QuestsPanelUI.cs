using System.Collections;
using System.Collections.Generic;
using EdgeworldBase;
using TMPro;
using UnityEngine;

public class QuestsPanelUI : MonoBehaviour
{

    [SerializeField]
    TextMeshProUGUI QuestCountLabel;

    [SerializeField]
    GameObject PanelObject;

    public Transform Container;



    void Start()
    {
        CORE.Instance.SubscribeToEvent("RefreshQuests", Refresh);
        
        Refresh();
    }


    private void Refresh()
    {
        QuestCountLabel.text = CORE.PlayerActor.quests.started.Count.ToString();

        PanelObject.SetActive(CORE.PlayerActor.quests.started.Count > 0);

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
            objSubPanel.transform.SetParent(Container,false);
            objSubPanel.transform.localScale = Vector3.one;

            objSubPanel.SetInfo(progress.QuestData);
            createdYPush -= 30f;
            objSubPanel.transform.position = Container.childCount > 0?
             Container.GetChild(0).position + new Vector3(0f,createdYPush,0) 
             :
            Container.transform.position + new Vector3(createdYPush,createdYPush,0);
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

    
}
