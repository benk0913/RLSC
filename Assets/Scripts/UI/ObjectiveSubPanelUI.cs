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

    [SerializeField]
    Animator Anim;

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
        #if UNITY_ANDROID || UNITY_IOS
        transform.localScale = Vector3.one * 2f;
        #endif
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

                string ObjectValueString =objective.Value;
                if(objective.ObjectValue != null)
                {
                    if(objective.ObjectValue.GetType() == typeof(ClassJob))
                    {
                        ObjectValueString = ((ClassJob)objective.ObjectValue).DisplayName;
                    }
                    else
                    {
                        ObjectValueString = objective.ObjectValue.name;
                    }
                }

                ObjectiveLabel.text +=
                    (string.IsNullOrEmpty(objective.Action.DisplayText)? "" : (CORE.QuickTranslate(objective.Action.DisplayText) + " - "))
                    +  CORE.QuickTranslate(ObjectValueString)
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
                
                if(Vector2.Distance(PickPosition, Input.mousePosition) < 0.1f)
                {
                    Anim.SetTrigger("Toggle");
                }
            }
        }
    }

    Vector3 PickPosition;

    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            CORE.Instance.ShowQuestsUIWindow();
            QuestWindowUI.Instance.SelectQuest(CurrentQuest);
            return;
        }

        
        IsDragged = true;
        DragEffect.SetActive(true);
        PickPosition = Input.mousePosition;
    }



}
