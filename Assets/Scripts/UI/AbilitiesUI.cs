using System;
using EdgeworldBase;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Events;

public class AbilitiesUI : MonoBehaviour, WindowInterface
{
    public static AbilitiesUI Instance;

    [SerializeField]
    Transform CurrentAbilitiesContainer;

    [SerializeField]
    Transform AllAbilitiesContainer;

    [SerializeField]
    SelectionGroupUI SelectionGroup;

    public AbilitySlotDraggableUI SelectedAbility;

    Actor playerActor;

    int CurrentIndex;

    public bool IsOpen;

    public string OpenSound;
    public string HideSound;
    public string SelectAbilitySound;
    public string AbilitySwapSound;

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Show(ActorData actorData, object data = null)
    {
        IsOpen = true;
        playerActor = actorData.ActorEntity;

        this.gameObject.SetActive(true);

        RefreshUI(false);


        AudioControl.Instance.Play(OpenSound);
    }

    public void Hide()
    {
        IsOpen = false;
        this.gameObject.SetActive(false);

        AudioControl.Instance.Play(HideSound);
    }

    public void ResetReplacement()
    {
        if(SelectedAbility != null)
        {
            SelectedAbility.Deselect();
            SelectedAbility = null;
        }
    }

    public void SelectAbility(AbilitySlotDraggableUI HoveredAbility)
    {
        if (HoveredAbility.CurrentAbility.IsLevelLocked)
        {
            // Don't allow selecting abilities that aren't unlocked yet through levels.
            return;
        }

        if(CORE.Instance.Room.HasEnemies)
        {
            TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("You cannot switch abilities, Enemies are nearby!", Colors.AsColor(Colors.COLOR_BAD)));
            return;
        }

        if (SelectedAbility != null)
        {
            string ability1Name = SelectedAbility.CurrentAbility.CurrentAbility.name;
            string ability2Name = HoveredAbility.CurrentAbility.CurrentAbility.name;
            if (ability1Name == ability2Name)
            {
                // Same spell selected - ignore.
            }
            else
            {
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
            }

            HoveredAbility.Deselect();
            ResetReplacement();

            for(int i=0;i<CurrentAbilitiesContainer.childCount;i++)
            {
                AbilitySlotDraggableUI slot = CurrentAbilitiesContainer.GetChild(i).GetComponent<AbilitySlotDraggableUI>();
                slot.Unmark();
            }
        }
        else
        {
            SelectedAbility = HoveredAbility;
            SelectedAbility.Select();

            for(int i=0;i<CurrentAbilitiesContainer.childCount;i++)
            {
                AbilitySlotDraggableUI slot = CurrentAbilitiesContainer.GetChild(i).GetComponent<AbilitySlotDraggableUI>();
                if(slot == SelectedAbility)
                {
                    slot.Unmark();
                    continue;
                }

                slot.Mark();
            }
        }

        AudioControl.Instance.Play(SelectAbilitySound);
    }

    private void SendSwapAbilityEvent(int index, string abilityName)
    {
        JSONNode node = new JSONClass();
        node["index"].AsInt = index;
        node["abilityName"] = abilityName;
        SocketHandler.Instance.SendEvent("swapped_ability", node);

        AudioControl.Instance.Play(AbilitySwapSound);
    }

    public void RefreshUI(bool restoreSelectionPlacement = true)
    {
        if (!IsOpen)
        {
            return;
        }
        CORE.ClearContainer(CurrentAbilitiesContainer);
        CORE.ClearContainer(AllAbilitiesContainer);

        foreach (AbilityState abilityState in playerActor.State.Abilities)
        {
            int i = playerActor.State.Data.abilities.IndexOf(abilityState.CurrentAbility.name);
            AbilitySlotDraggableUI slot = ResourcesLoader.Instance.GetRecycledObject("AbilitySlotDraggableUI").GetComponent<AbilitySlotDraggableUI>();
            slot.transform.SetParent(CurrentAbilitiesContainer, false);
            slot.transform.localScale = Vector3.one;
            slot.transform.position = Vector3.zero;
            slot.SetAbilityState(abilityState,InputMap.Map["Ability"+i].ToString());
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
            slot.SetAbilityState(new AbilityState(CORE.Instance.Data.content.Abilities.Find(X => X.name == abilityName), playerActor));
        }

        CORE.Instance.DelayedInvokation(0f, () => SelectionGroup.RefreshGroup(restoreSelectionPlacement));

        ResetReplacement();
    }
}
