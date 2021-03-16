using System;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Events;

public class AbilitiesUI : MonoBehaviour
{
    public static AbilitiesUI Instance;
    private const int AbilitiesCountInRow = 4;

    [SerializeField]
    Transform CurrentAbilitiesContainer;

    [SerializeField]
    Transform AllAbilitiesContainer;

    public AbilitySlotDraggableUI SelectedAbility;
    public AbilitySlotDraggableUI HoveredAbility;

    Actor playerActor;

    int CurrentIndex;

    public bool IsOpen;    

    public bool IsSelectingReplacement;


    private void Awake()
    {
        Instance = this;
        Hide();
    }

    private void Update()
    {
        if (CORE.Instance.IsTyping) {
            return;
        }
        
        if (Input.GetKeyDown(InputMap.Map["Exit"]))
        {
            Hide();
        }

        if(Input.GetKeyDown(InputMap.Map["Move Left"]))
        {
            CurrentIndex--;
            if(CurrentIndex < 0)
            {
                CurrentIndex = CurrentAbilitiesContainer.childCount + AllAbilitiesContainer.childCount- 1;
            }

            RefreshHover();
        }

        if (Input.GetKeyDown(InputMap.Map["Move Right"]))
        {
            CurrentIndex++;

            if (CurrentIndex >= CurrentAbilitiesContainer.childCount + AllAbilitiesContainer.childCount)
            {
                CurrentIndex = 0;
            }

            RefreshHover();
        }
        
        if (Input.GetKeyDown(InputMap.Map["Move Down"]))
        {
            if (CurrentIndex < CurrentAbilitiesContainer.childCount - 1)
            {
                // Current abilities row
                CurrentIndex += CurrentAbilitiesContainer.childCount;
            }
            else if (CurrentIndex < CurrentAbilitiesContainer.childCount + AllAbilitiesContainer.childCount - AbilitiesCountInRow)
            {
                // Any row in the mid of all abilities.
                CurrentIndex += AbilitiesCountInRow;
            }
            else 
            {
                int AbilitiesCountInLastRow = AllAbilitiesContainer.childCount % AbilitiesCountInRow;
                // Last 4 abilities.
                CurrentIndex += AbilitiesCountInLastRow;
                if (CurrentIndex < CurrentAbilitiesContainer.childCount + AllAbilitiesContainer.childCount)
                {
                    // If it's the last 4 abilities but not in the last row, we want to pretend as if it was in the last row.
                    CurrentIndex += AbilitiesCountInRow;
                }
                // If it's over the limit, bring to start.
                CurrentIndex %= CurrentAbilitiesContainer.childCount + AllAbilitiesContainer.childCount;
            }

            RefreshHover();
        }

        if (Input.GetKeyDown(InputMap.Map["Move Up"]))
        {
            if (CurrentIndex >= CurrentAbilitiesContainer.childCount + AbilitiesCountInRow)
            {
                // Not the first 2 rows.
                CurrentIndex -= AbilitiesCountInRow;
            }
            else if (CurrentIndex >= CurrentAbilitiesContainer.childCount)
            {
                // The second row.
                CurrentIndex -= CurrentAbilitiesContainer.childCount;
            }
            else 
            {
                // The first row.
                int AbilitiesCountInLastRow = AllAbilitiesContainer.childCount % AbilitiesCountInRow;
                if (CurrentIndex >= AbilitiesCountInLastRow)
                {
                    // The first row abilities 
                    CurrentIndex -= AbilitiesCountInRow;
                    if (CurrentIndex == 0)
                    {
                        CurrentIndex--;
                    }
                }
                CurrentIndex -= AbilitiesCountInLastRow;
                CurrentIndex += CurrentAbilitiesContainer.childCount + AllAbilitiesContainer.childCount;
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
        SelectAbility();
    }

    public void MouseEnter(AbilitySlotDraggableUI abilitySlotDraggableUI)
    {
        for (int i = 0; i < CurrentAbilitiesContainer.childCount; i++)
        {
            if(CurrentAbilitiesContainer.GetChild(i).GetComponent<AbilitySlotDraggableUI>() == abilitySlotDraggableUI)
            {
                CurrentIndex = i;
                RefreshHover();
            }
        }
        for (int i = 0; i < AllAbilitiesContainer.childCount; i++)
        {
            if (AllAbilitiesContainer.GetChild(i).GetComponent<AbilitySlotDraggableUI>() == abilitySlotDraggableUI)
            {
                CurrentIndex = i + CurrentAbilitiesContainer.childCount;
                RefreshHover();
            }
        }
    }

    public void Toggle(Actor actor)
    {
        if(IsOpen)
        {
            Hide();
        }
        else
        {
            IsOpen = true;
            playerActor = actor;

            this.gameObject.SetActive(true);

            RefreshUI();
        }

    }

    public void Hide()
    {
        IsOpen = false;
        this.gameObject.SetActive(false);
    }

    public void ResetReplacement()
    {
        if(SelectedAbility != null)
        {
            SelectedAbility.Deselect();
        }

        IsSelectingReplacement = false;
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
            string ability1Name = SelectedAbility.CurrentAbility.CurrentAbility.name;
            string ability2Name = HoveredAbility.CurrentAbility.CurrentAbility.name;
            if (ability1Name == ability2Name)
            {
                // Same spell selected - ignore.
                ResetReplacement();
                return;
            }

            // Swap the slots in the UI.

            if(CORE.Instance.Room.HasEnemies)
            {
                TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("You cannot switch abilities, Enemies are nearby!", Color.red));
                return;
            }

            Transform parent1 = SelectedAbility.transform.parent;
            int index1InParent = SelectedAbility.transform.GetSiblingIndex();
            Transform parent2 = HoveredAbility.transform.parent;
            int index2InParent = HoveredAbility.transform.GetSiblingIndex();
            SelectedAbility.transform.SetParent(parent2);
            HoveredAbility.transform.SetParent(parent1);
            SelectedAbility.transform.SetSiblingIndex(index2InParent);
            HoveredAbility.transform.SetSiblingIndex(index1InParent);

            SelectedAbility.Deselect();

            int index1ToReplace = playerActor.State.Data.abilities.IndexOf(ability1Name);
            int index2ToReplace = playerActor.State.Data.abilities.IndexOf(ability2Name);
            if (index1ToReplace >= 0)
            {
                playerActor.State.Data.abilities.RemoveAt(index1ToReplace);
                playerActor.State.Data.abilities.Insert(index1ToReplace, ability2Name);
                SendSwapAbilityEvent(index1ToReplace, ability2Name);
            }
            if (index2ToReplace >= 0)
            {
                playerActor.State.Data.abilities.RemoveAt(index2ToReplace);
                playerActor.State.Data.abilities.Insert(index2ToReplace, ability1Name);
                SendSwapAbilityEvent(index2ToReplace, ability1Name);
            }
            
            playerActor.RefreshAbilities();

            ResetReplacement();
        }
        else
        {
            SelectedAbility = HoveredAbility;
            IsSelectingReplacement = true;
            RefreshHover();
            SelectedAbility.Select();
        }
    }

    private void SendSwapAbilityEvent(int index, string abilityName)
    {
        JSONNode node = new JSONClass();
        node["index"].AsInt = index;
        node["abilityName"] = abilityName;
        SocketHandler.Instance.SendEvent("swapped_ability", node);
    }

    public void RefreshHover()
    {
        if (CurrentIndex < CurrentAbilitiesContainer.childCount)
        {
            SetHover(CurrentAbilitiesContainer.GetChild(CurrentIndex).GetComponent<AbilitySlotDraggableUI>());
        }
        else
        {
            SetHover(AllAbilitiesContainer.GetChild(CurrentIndex - CurrentAbilitiesContainer.childCount).GetComponent<AbilitySlotDraggableUI>());
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
            if(playerActor.State.Data.abilities.Contains(abilityName))
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
        ResetReplacement();
    }
}
