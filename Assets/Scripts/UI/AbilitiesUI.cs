using System;
using EdgeworldBase;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AbilitiesUI : MonoBehaviour, WindowInterface
{
    public static AbilitiesUI Instance;

    [SerializeField]
    Transform CurrentAbilitiesContainer;

    [SerializeField]
    Transform AllAbilitiesContainer;

    [SerializeField]
    SelectionGroupUI SelectionGroup;

    [SerializeField]
    GridLayoutGroup UpperAbilitiesLG;


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

    [Obsolete("Do not call Show directly. Call `CORE.Instance.ShowWindow()` instead.")]
    public void Show(ActorData actorData, object data = null)
    {
        IsOpen = true;
        playerActor = actorData.ActorEntity;
        
        this.gameObject.SetActive(true);
        #if UNITY_ANDROID || UNITY_IOS
        UpperAbilitiesLG.startCorner = GridLayoutGroup.Corner.UpperRight;
        UpperAbilitiesLG.childAlignment = TextAnchor.UpperRight;
        #else
        UpperAbilitiesLG.startCorner = GridLayoutGroup.Corner.UpperLeft;
        UpperAbilitiesLG.childAlignment = TextAnchor.UpperLeft;
        #endif
        RefreshUI(false);


        AudioControl.Instance.Play(OpenSound);

        CORE.Instance.InvokeEvent("AbilitiesNotification_OFF");
    }

    [Obsolete("Do not call Hide directly. Call `CORE.Instance.CloseCurrentWindow()` instead.")]
    public void Hide()
    {
        IsOpen = false;
        this.gameObject.SetActive(false);

        ClearMarks();

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
        // Only allow interacting with a locked ability when it's in the action bar.
        if (HoveredAbility.transform.parent != CurrentAbilitiesContainer && HoveredAbility.CurrentAbility.IsLevelLocked)
        {
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
                ReplaceAbilities(ability1Name,ability2Name);
                
                playerActor.RefreshAbilities();
            }

            HoveredAbility.Deselect();
            ResetReplacement();

            ClearMarks();
        }
        else
        {
            SelectedAbility = HoveredAbility;

            if (SelectedAbility.transform.parent == CurrentAbilitiesContainer)
            {
                MarkAll();
            }
            else
            {
                MarkAllCurrentAbilities();
            }

            SelectedAbility.Select();

        }

        AudioControl.Instance.Play(SelectAbilitySound);
    }

    public void ReplaceAbilities(string ability1Name, string ability2Name)
    {
         int index1ToReplace = CORE.PlayerActor.abilities.IndexOf(ability1Name);
        int index2ToReplace =  CORE.PlayerActor.abilities.IndexOf(ability2Name);
        if (index1ToReplace >= 0)
        {
            CORE.PlayerActor.abilities.RemoveAt(index1ToReplace);
            CORE.PlayerActor.abilities.Insert(index1ToReplace, ability2Name);
            SendSwapAbilityEvent(index1ToReplace, ability2Name);
        }
        if (index2ToReplace >= 0)
        {
            CORE.PlayerActor.abilities.RemoveAt(index2ToReplace);
            CORE.PlayerActor.abilities.Insert(index2ToReplace, ability1Name);
            SendSwapAbilityEvent(index2ToReplace, ability1Name);
        }
    }

    void ClearMarks()
    {
        ClearMarksCurrentAbilities();
        
        ClearMarksAllAbilities();
    }

    void ClearMarksAllAbilities()
    {
        for (int i = 0; i < AllAbilitiesContainer.childCount; i++)
        {
            AbilitySlotDraggableUI slot = AllAbilitiesContainer.GetChild(i).GetComponent<AbilitySlotDraggableUI>();

            slot.Unmark();
        }
    }
    
    void ClearMarksCurrentAbilities()
    {
        for (int i = 0; i < CurrentAbilitiesContainer.childCount; i++)
        {
            AbilitySlotDraggableUI slot = CurrentAbilitiesContainer.GetChild(i).GetComponent<AbilitySlotDraggableUI>();
            slot.Unmark();
        }
    }

    void MarkAll()
    {
        MarkAllCurrentAbilities();
        MarkAllAbilities();
    }

    void MarkAllCurrentAbilities()
    {
        for (int i = 0; i < CurrentAbilitiesContainer.childCount; i++)
        {
            AbilitySlotDraggableUI slot = CurrentAbilitiesContainer.GetChild(i).GetComponent<AbilitySlotDraggableUI>();
            slot.Mark();
        }
    }
    void MarkAllAbilities()
    {
        for (int i = 0; i < AllAbilitiesContainer.childCount; i++)
        {
            AbilitySlotDraggableUI slot = AllAbilitiesContainer.GetChild(i).GetComponent<AbilitySlotDraggableUI>();

            slot.Mark();
        }
    }

    public void SendSwapAbilityEvent(int index, string abilityName)
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

        foreach(string unlockedClass in playerActor.State.Data.unlockedClassJobs)
        {
            ClassJob job = CORE.Instance.Data.content.Classes.Find(X=>X.name == unlockedClass);
            foreach (string abilityName in job.Abilities)
            {
                GenerateAbilitySlot(abilityName,job);
            }
        }

        CORE.Instance.DelayedInvokation(0f, () => SelectionGroup.RefreshGroup(restoreSelectionPlacement));

        ResetReplacement();
    }
    
    public void GenerateAbilitySlot(string abilityName,ClassJob job = null)
    {
        Ability ability = CORE.Instance.Data.content.Abilities.Find(X => X.name == abilityName);

        if(ability == null)
        {
            return;
        }
        
        AbilityState state = new AbilityState(ability, playerActor,job);

         if(playerActor.State.Data.abilities.Contains(abilityName))
         {
             return;
         }

        Ability masteryAbility = CORE.Instance.Data.content.Abilities.Find(x=>
        playerActor.State.Data.ClassJobReference.Abilities.Contains(x.name) &&
        x.Mastery &&
            x.name != ability.name &&
            x.name.Contains(ability.name));

        if(masteryAbility != null)
        {
            AbilityState masteryState = new AbilityState(masteryAbility, playerActor,job);
            
            if(!masteryState.IsAbilityLocked)
            {
                return;
            }
        }

        AbilitySlotDraggableUI slot = ResourcesLoader.Instance.GetRecycledObject("AbilitySlotDraggableUI").GetComponent<AbilitySlotDraggableUI>();
        slot.transform.SetParent(AllAbilitiesContainer, false);
        slot.transform.localScale = Vector3.one;
        slot.transform.position = Vector3.zero;
        slot.SetAbilityState(state);
    }
}
