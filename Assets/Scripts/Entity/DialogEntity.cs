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
    private void Update()
    {
        if(ContinueCooldown > 0f)
        {
            ContinueCooldown -= Time.deltaTime;
        }

        if (isActiveDialog)
        {
            if (Input.anyKey)
            {
                if (Input.GetKeyDown(InputMap.Map["Interact"]) || Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Joystick 2") )
                {
                    return;
                }
                else if(Input.GetMouseButtonDown(0))
                {
                    if(!DecisionContainerUI.Instance.IsActive)
                        Continue();
                        
                    return;
                }

                if(Input.GetKeyDown(InputMap.Map["Exit"]))
                {
                    EndDialog();
                }
            }
        }
    }

    public bool isActiveDialog;
    
    public void StartDialog()
    {
        StartDialog(DefaultDialog);
    }

    public void StartDialog(Dialog dialog)
    {

        if (VendorEntity.CurrentInstance != null && VendorEntity.CurrentInstance.IsFocusing)
            return;

        if(DecisionContainerUI.Instance.IsActive)
            return;

        if(DecisionContainerUI.Instance.CurrentDecisions.Count > 0 && DecisionContainerUI.Instance.gameObject.activeInHierarchy) 
            return;


        if(isActiveDialog)
        {
            Continue();
            return;
        }

        DecisionContainerUI.Instance.ShowSkipText();

        CurrentDialog = dialog;

        CurrentInstance = this;

        CurrentBubble.gameObject.SetActive(true);

        DecisionContainerUI.Instance.Hide();

        CurrentIndex = 0;
        
        isActiveDialog = true;

        if (CurrentIndex >= CurrentDialog.DialogPieces.Count)
        {
            if (CurrentDialog.Decisions.Count > 0)
            {
                DecisionContainerUI.Instance.ShowDialogSpecific(CORE.Instance.Room.PlayerActor, CurrentDialog);
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

    float ContinueCooldown = 0f;
    public void Continue()
    {
        if(ContinueCooldown > 0f)
        {
            return;
        }
        ContinueCooldown = 0.1f;

        CurrentIndex++;

        if (CurrentIndex >= CurrentDialog.DialogPieces.Count) // End of dialog?
        {
            
            if (CurrentDialog.Decisions.Count > 0) // Has decisions to pick?
            {
                DecisionContainerUI.Instance.ShowDialogSpecific(CORE.Instance.Room.PlayerActor, CurrentDialog);
            }
            else
            {
                EndDialog();
            }

            return;
        }

        //Not end of dialog
        CORE.Instance.DelayedInvokation(0.1f, () => 
        {
            ShowIndex(CurrentIndex);
        });
        
    }

    public void ShowIndex(int index)
    {

        DecisionContainerUI.Instance.Hide();
        
        if(CurrentDialog == null) return;

        string content = CurrentDialog.DialogPieces[index].Content;

        CORE.Instance.Data.Localizator.mSource.TryGetTranslationCodywise(content, out content);

        CurrentBubble.Show(CurrentBubble.transform, content, Continue, CurrentDialog.IsFemale);
        CurrentDialog.DialogPieces[index].OnDialogPiece?.Invoke();
    }



    public void EndDialog()
    {

        CurrentDialog = null;
        this.CurrentBubble.gameObject.SetActive(false);
        isActiveDialog = false;
        DecisionContainerUI.Instance.HideSkipText();
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

        public UnityEvent OnSelectInvokation = new UnityEvent();
        
        public List<AbilityCondition> DisplayConditions = new List<AbilityCondition>();
        public List<GameCondition> DisplayGameConditions = new List<GameCondition>();

        public bool DisplayOnlyIfConditionsMet;

        public Dialog DefaultDialog;

        public void SelectDecision()
        {
            OnSelectInvokation?.Invoke();

            foreach (GameEvent gEvent in OnSelect)
            {
                gEvent.Execute();
            }

            CurrentInstance.EndDialog();

            if (DefaultDialog.DialogPieces.Count > 0)
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

        public bool IsFemale = false;
    }
}
