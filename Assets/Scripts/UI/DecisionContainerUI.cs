using EdgeworldBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DialogEntity;

public class DecisionContainerUI : MonoBehaviour, WindowInterface
{
    public static DecisionContainerUI Instance;

    public List<DialogDecision> CurrentDecisions = new List<DialogDecision>();
    public Transform CurrentTarget;

    public SelectionGroupUI SGroup;

    public GameObject SkipText;
    public bool IsActive = false;



    Dialog CurrentDialog;
    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }
    
    public void Show(ActorData actor, object data = null)
    {
        Show(actor, null);
    }

    public void ShowDialogSpecific(ActorData actor, Dialog dialog)
    {
        if(CurrentDialog == dialog && this.gameObject.activeInHierarchy)
        {
            return;
        }

        CurrentDialog = dialog;

        Show(actor,dialog.Decisions);
    }


    public void Show(ActorData actor, List<DialogDecision> decisions)
    {
        this.gameObject.SetActive(true);
        #if UNITY_ANDROID || UNITY_IOS
        this.transform.localScale = Vector3.one *0.5f;
        #endif

        CurrentTarget = actor.ActorEntity.transform;
        CurrentDecisions = decisions;

        transform.position = CameraChaseEntity.Instance.CurrentCam.WorldToScreenPoint(CurrentTarget.position) + new Vector3(0f, Screen.height / 8,0f);

        IsActive = true;

        CORE.ClearContainer(transform);

        CORE.Instance.DelayedInvokation(0.2f, () =>
        {
            foreach (DialogDecision decision in CurrentDecisions)
            {
                DialogDecisionUI decisionUI = ResourcesLoader.Instance.GetRecycledObject("DialogDecisionUI").GetComponent<DialogDecisionUI>();
                decisionUI.SetInfo(decision);
                decisionUI.transform.SetParent(transform, false);
                decisionUI.transform.position = Vector3.zero;
                decisionUI.transform.localScale = Vector3.one;
            }

            SGroup.enabled = false;
            CORE.Instance.DelayedInvokation(1f, () =>
            {
                SGroup.enabled = true;
                SGroup.RefreshGroup(false);
            });
        
        });
    }

    public void Hide()
    {
        IsActive = false;
        CORE.ClearContainer(transform);
        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        
        if (CameraChaseEntity.Instance != null && CurrentTarget != null)
        {
            transform.position = Vector2.Lerp(transform.position, (Vector2)CameraChaseEntity.Instance.CurrentCam.WorldToScreenPoint(CurrentTarget.position) + new Vector2(0f, Screen.height / 8), Time.deltaTime);
        }

        if(Input.GetKeyDown(InputMap.Map["Exit"]) || Input.GetButtonDown("Joystick 8")||  Input.GetButtonDown("Joystick 11"))
        {
            Hide();
        }
    }

    public void ShowSkipText()
    {
        SkipText.SetActive(true);
    }

    public void HideSkipText()
    {
        SkipText.SetActive(false);
    }
}
