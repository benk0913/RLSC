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

    public bool IsActive = false;

    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }
    
    public void Show(ActorData actor, object data = null)
    {
        Show(actor, null);
    }

    public void Show(ActorData actor, List<DialogDecision> decisions)
    {
        this.gameObject.SetActive(true);

        CurrentTarget = actor.ActorEntity.transform;
        CurrentDecisions = decisions;

        CORE.ClearContainer(transform);

        IsActive = true;

        CORE.Instance.DelayedInvokation(0.1f, () =>
        {
            foreach (DialogDecision decision in CurrentDecisions)
            {
                DialogDecisionUI decisionUI = ResourcesLoader.Instance.GetRecycledObject("DialogDecisionUI").GetComponent<DialogDecisionUI>();
                decisionUI.SetInfo(decision);
                decisionUI.transform.SetParent(transform, false);
                decisionUI.transform.position = Vector3.zero;
                decisionUI.transform.localScale = Vector3.one;
            }

            CORE.Instance.DelayedInvokation(0.1f, () => 
            {
                SGroup.RefreshGroup(false);
            });
        });
    }

    public void Hide()
    {
        IsActive = false;
        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        
        if (CameraChaseEntity.Instance != null && CurrentTarget != null)
        {
            transform.position = Vector2.Lerp(transform.position, (Vector2)CameraChaseEntity.Instance.CurrentCam.WorldToScreenPoint(CurrentTarget.position) + new Vector2(0f, Screen.height / 8), Time.deltaTime);
        }

        if(Input.GetKeyDown(InputMap.Map["Escape"]))
        {
            Hide();
        }
    }
}
