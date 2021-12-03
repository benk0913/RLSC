using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EdgeworldBase;

public class ObjectiveSubPanelUI : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    Color ProgressColor;

    [SerializeField]
    Color ReadyColor;

    [SerializeField]
    TextMeshProUGUI ObjectiveLabel;

    public Image CircleImage;

    public QuestData CurrentQuest;
    
        public GameObject DragEffect;

    public bool IsDragged { get; private set; }

    void Start()
    {
        CORE.Instance.SubscribeToEvent("RefreshQuests", Refresh);
        
        Refresh();
    }

    public void SetInfo(QuestData quest)
    {
        CurrentQuest = quest;
        Refresh();
    }

    public void Refresh()
    {
        if(CurrentQuest == null) return;

        ActorQuestProgress questProgress = CORE.PlayerActor.quests.GetQuestProgress(CurrentQuest.name);
        
        if(questProgress == null)
        {
            CircleImage.color = ReadyColor;
            ObjectiveLabel.text ="Quest Complete!";    
            return;
        }
        ObjectiveLabel.text ="";
        
        if(CurrentQuest.Goals.Count == 0)
        {
            ObjectiveLabel.text = CurrentQuest.name;
        }
        else
        {
            for(int i=0;i<CurrentQuest.Goals.Count;i++)
            {
                QuestGoal objective = CurrentQuest.Goals[i];
                ObjectiveLabel.text +=
                    (string.IsNullOrEmpty(objective.Action.DisplayText)? "" : (CORE.QuickTranslate(objective.Action.DisplayText) + " - "))
                    +  (objective.ObjectValue == null? CORE.QuickTranslate(objective.Value) :  CORE.QuickTranslate(objective.ObjectValue.name)) 
                    +  " ("+questProgress.Values[i]+" / "+objective.Count+")"
                    +  System.Environment.NewLine;
            }
        }

        if(CurrentQuest.CanComplete)
        {
            if(CircleImage.color != ReadyColor)
            {
                CircleImage.color = ReadyColor;
                AudioControl.Instance.Play("QuestCanComplete");
            }
        }
        else
        {
            CircleImage.color = ProgressColor;
        }
    }

    void Update()
    {
        if(IsDragged)
        {
            transform.position = Input.mousePosition;

            if(Input.GetMouseButtonUp(0))
            {
                IsDragged = false;
                DragEffect.SetActive(false);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsDragged = true;
        DragEffect.SetActive(true);
    }


}
