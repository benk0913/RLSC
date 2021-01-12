using System;
using UnityEngine;
using UnityEngine.Events;

public class AbilitiesUI : MonoBehaviour
{
    public static AbilitiesUI Instance;

    [SerializeField]
    Transform CurrentAbilitiesContainer;

    [SerializeField]
    Transform AllAbilitiesContainer;

    public AbilitySlotDraggableUI SelectedAbility;
    public AbilitySlotDraggableUI HoveredAbility;

    Actor playerActor;

    int CurrentIndex;
    

    public bool IsSelectingReplacement;


    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(InputMap.Map["Exit"]))
        {
            this.gameObject.SetActive(false);
        }

        if(Input.GetKeyDown(InputMap.Map["Move Left"]))
        {
            CurrentIndex--;
            if(CurrentIndex < 0)
            {
                if (IsSelectingReplacement)
                    CurrentIndex = AllAbilitiesContainer.childCount - 1;
                else
                    CurrentIndex = CurrentAbilitiesContainer.childCount- 1;
            }

            RefreshHover();
        }

        if (Input.GetKeyDown(InputMap.Map["Move Right"]))
        {
            CurrentIndex++;

            if (IsSelectingReplacement && CurrentIndex >= AllAbilitiesContainer.childCount)
            {
                CurrentIndex = 0;
            }
            else if (!IsSelectingReplacement && CurrentIndex >= CurrentAbilitiesContainer.childCount)
            {
                CurrentIndex = 0;
            }

            RefreshHover();
        }

        if (Input.GetKeyDown(InputMap.Map["Confirm"]))
        {
            SelectAbility();
        }
    }

    public void MouseClick(AbilitySlotDraggableUI abilitySlotDraggableUI)
    {
        for (int i = 0; i < CurrentAbilitiesContainer.childCount; i++)
        {
            if (CurrentAbilitiesContainer.GetChild(i).GetComponent<AbilitySlotDraggableUI>() == abilitySlotDraggableUI)
            {
                if (IsSelectingReplacement)
                {
                    ResetReplacement();
                    SetHover(abilitySlotDraggableUI);
                    SelectAbility();
                }
                else
                {
                    SelectAbility();
                }
            }
        }

        for (int i = 0; i < AllAbilitiesContainer.childCount; i++)
        {
            if (AllAbilitiesContainer.GetChild(i).GetComponent<AbilitySlotDraggableUI>() == abilitySlotDraggableUI)
            {
                if(IsSelectingReplacement)
                {
                    SelectAbility();
                }
                else
                {
                    TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Please select an ability to replace first.", Color.yellow, 3f));
                }

                return;
            }
        }


    }

    public void MouseEnter(AbilitySlotDraggableUI abilitySlotDraggableUI)
    {
        if (IsSelectingReplacement)
        {
            for (int i = 0; i < AllAbilitiesContainer.childCount; i++)
            {
                if (AllAbilitiesContainer.GetChild(i).GetComponent<AbilitySlotDraggableUI>() == abilitySlotDraggableUI)
                {
                    CurrentIndex = i;
                    RefreshHover();
                }
            }
        }
        else
        {
            for (int i = 0; i < CurrentAbilitiesContainer.childCount; i++)
            {
                if(CurrentAbilitiesContainer.GetChild(i).GetComponent<AbilitySlotDraggableUI>() == abilitySlotDraggableUI)
                {
                    CurrentIndex = i;
                    RefreshHover();
                }
            }
        }
    }

    public void Show(Actor actor)
    {
        if(this.gameObject.activeInHierarchy)
        {
            this.gameObject.SetActive(false);
            return;
        }

        playerActor = actor;

        this.gameObject.SetActive(true);

        RefreshUI();
    }

    public void ResetReplacement()
    {
        if(SelectedAbility != null)
        {
            SelectedAbility.Deselect();
        }

        IsSelectingReplacement = false;
        CurrentIndex = 0;
        RefreshHover();
    }

    public void SetHover(AbilitySlotDraggableUI ability)
    {
        if (HoveredAbility != null)
        {
            HoveredAbility.Unhover();
        }

        HoveredAbility = ability;
        HoveredAbility.Hover();
    }

    public void SelectAbility()
    {
        if(HoveredAbility == null)
        {
            CORE.Instance.LogMessageError("NO HOVERED ABILITY!?");
            return;
        }

        if (IsSelectingReplacement)
        {
            SelectedAbility.Deselect();

            int indexToReplace = playerActor.State.Data.Abilities.IndexOf(SelectedAbility.CurrentAbility.CurrentAbility.name);
            playerActor.State.Data.Abilities.RemoveAt(indexToReplace);
            playerActor.State.Data.Abilities.Insert(indexToReplace, HoveredAbility.CurrentAbility.CurrentAbility.name);

            playerActor.RefreshAbilities();

            ResetReplacement();
            RefreshUI();

            //TODO Request to server
            //On RESPONSE -> 
        }
        else
        {
            SelectedAbility = HoveredAbility;
            IsSelectingReplacement = true;
            CurrentIndex = 0;
            RefreshHover();
            SelectedAbility.Select();
        }
    }

    public void RefreshHover()
    {
        if(IsSelectingReplacement)
        {
            SetHover(AllAbilitiesContainer.GetChild(CurrentIndex).GetComponent<AbilitySlotDraggableUI>());
        }
        else
        {
            SetHover(CurrentAbilitiesContainer.GetChild(CurrentIndex).GetComponent<AbilitySlotDraggableUI>());
        }
    }

    public void RefreshUI()
    {
        CORE.ClearContainer(CurrentAbilitiesContainer);
        CORE.ClearContainer(AllAbilitiesContainer);

        foreach (AbilityState abilityState in playerActor.State.Abilities)
        {
            AbilitySlotDraggableUI slot = ResourcesLoader.Instance.GetRecycledObject("AbilitySlotDraggableUI").GetComponent<AbilitySlotDraggableUI>();
            slot.transform.SetParent(CurrentAbilitiesContainer, false);
            slot.transform.localScale = Vector3.one;
            slot.transform.position = Vector3.zero;
            slot.SetAbilityState(abilityState);
        }

        ClassJob job = playerActor.State.Data.ClassJobReference;

        foreach (string abilityName in job.Abilities)
        {
            if(playerActor.State.Data.Abilities.Contains(abilityName))
            {
                continue;
            }

            AbilitySlotDraggableUI slot = ResourcesLoader.Instance.GetRecycledObject("AbilitySlotDraggableUI").GetComponent<AbilitySlotDraggableUI>();
            slot.transform.SetParent(AllAbilitiesContainer, false);
            slot.transform.localScale = Vector3.one;
            slot.transform.position = Vector3.zero;
            slot.SetAbilityState(new AbilityState(CORE.Instance.Data.content.Abilities.Find(X => X.name == abilityName)));
        }


        CurrentIndex = 0;
        RefreshHover();
    }

    

}
