using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuestWindowTitleUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI TitleLabel;

    public QuestData CurrentQuest;


    public void SetInfo(QuestData quest,string title= "")
    {
        this.CurrentQuest = quest;
        if(CurrentQuest == null)
        {
            if(!string.IsNullOrEmpty(title))
            {
                TitleLabel.text = CORE.QuickTranslate(title);
            }
            return;
        }

        if(CORE.PlayerActor.quests.completed.ContainsKey(CurrentQuest.name))
        {
            TitleLabel.text = "<s>"+CORE.QuickTranslate(quest.name)+"</s>";
        }
        else
        {
            TitleLabel.text = CORE.QuickTranslate(quest.name);
        }
    }

    public void OnClick()
    {
        if(this.CurrentQuest == null)    return;

        QuestWindowUI.Instance.SelectQuest(this.CurrentQuest);
    }
}
