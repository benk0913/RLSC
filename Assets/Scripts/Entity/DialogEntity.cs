using EdgeworldBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogEntity : MonoBehaviour
{
    public static DialogEntity CurrentInstance;

    public Dialog DefaultDialog;
    public Dialog CurrentDialog;

    public int CurrentIndex = 0;

    [SerializeField]
    TextBubbleUI CurrentBubble;

    public bool isActiveDialog;

    public Transform DecisionContainer;

    public void StartDialog()
    {
        StartDialog(DefaultDialog);
    }

    public void StartDialog(Dialog dialog)
    {
        if(CurrentDialog == dialog &&  isActiveDialog)
        {
            Continue();
            return;
        }

        CurrentDialog = dialog;

        CurrentInstance = this;

        CurrentBubble.gameObject.SetActive(true);

        CORE.ClearContainer(DecisionContainer);

        CurrentIndex = 0;
        
        isActiveDialog = true;

        if (CurrentIndex >= CurrentDialog.DialogPieces.Count)
        {
            if (CurrentDialog.Decisions.Count > 0)
            {
                ShowDecisions();
                return;
            }
            else
            {
                EndDialog();
                return;
            }
        }
        else
        {
            ShowIndex(CurrentIndex);
        }
    }

    public void Continue()
    {
        CurrentIndex++;

        if (CurrentIndex >= CurrentDialog.DialogPieces.Count)
        {
            if (CurrentDialog.Decisions.Count > 0)
            {
                ShowDecisions();
            }
            else
            {
                EndDialog();
            }

            return;
        }

        CORE.Instance.DelayedInvokation(0.1f, () => 
        {
            ShowIndex(CurrentIndex);
        });
        
    }

    public void ShowIndex(int index)
    {

        CORE.ClearContainer(DecisionContainer);

        string content = CurrentDialog.DialogPieces[index].Content;
        CurrentBubble.Show(CurrentBubble.transform, content, Continue);
        CurrentDialog.DialogPieces[index].OnDialogPiece?.Invoke();
    }

    public void ShowDecisions()
    {
        CORE.ClearContainer(DecisionContainer);

        CORE.Instance.DelayedInvokation(0.1f, () => 
        {
            foreach (DialogDecision decision in CurrentDialog.Decisions)
            {
                DialogDecisionUI decisionUI = ResourcesLoader.Instance.GetRecycledObject("DialogDecisionUI").GetComponent<DialogDecisionUI>();
                decisionUI.SetInfo(decision);
                decisionUI.transform.SetParent(DecisionContainer, false);
                decisionUI.transform.position = Vector3.zero;
                decisionUI.transform.localScale = Vector3.one;
            }

            DecisionContainer.gameObject.SetActive(false);
            CORE.Instance.DelayedInvokation(0.1f, () => 
            {
                DecisionContainer.gameObject.SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(DecisionContainer.GetComponent<RectTransform>());
            });
        });
    }

    public void EndDialog()
    {
        this.CurrentBubble.gameObject.SetActive(false);
        isActiveDialog = false;
    }

    [System.Serializable]
    public class DialogPiece
    {
        [TextArea(3, 6)]
        public string Content;

        [SerializeField]
        public UnityEvent OnDialogPiece;
        
    }

    [System.Serializable]
    public class DialogDecision
    {
        public string Content;

        public List<GameEvent> OnSelect = new List<GameEvent>();

        public List<AbilityCondition> DisplayConditions = new List<AbilityCondition>();//TODO Should rename to GameCondition...

        public bool DisplayOnlyIfConditionsMet;

        public Dialog DefaultDialog;

        public void SelectDecision()
        {
            foreach (GameEvent gEvent in OnSelect)
            {
                gEvent.Execute();
            }

            if(DefaultDialog.DialogPieces.Count > 0)
            {
                CurrentInstance.StartDialog(DefaultDialog);
            }
        }
    }

    [System.Serializable]
    public class Dialog
    {
        public List<DialogPiece> DialogPieces = new List<DialogPiece>();

        public List<DialogDecision> Decisions = new List<DialogDecision>();
    }
}
