using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuestWindowTitleUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    TextMeshProUGUI TitleLabel;

    public QuestData CurrentQuest;


    public void SetInfo(QuestData quest)
    {
        this.CurrentQuest = quest;

        if(CORE.PlayerActor.quests.completed.ContainsKey(CurrentQuest.name))
        {
            TitleLabel.text = "<s>"+CORE.QuickTranslate(quest.name)+"</s>";
        }
        else
        {
            TitleLabel.text = CORE.QuickTranslate(quest.name);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        QuestWindowUI.Instance.SelectQuest(this.CurrentQuest);
    }
}
