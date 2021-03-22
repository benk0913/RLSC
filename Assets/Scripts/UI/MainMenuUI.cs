using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public static MainMenuUI Instance;

    [SerializeField]
    Transform CharacterSelectionContainer;

    [SerializeField]
    DisplayCharacterUI SelectedDisplayActor;

    [SerializeField]
    SelectionGroupUI SelectionGroup;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        AutoLogin();
    }
    
    public void AutoLogin()
    {
        ResourcesLoader.Instance.LoadingWindowObject.SetActive(true);

        SocketHandler.Instance.SendLogin(() =>
        {
            ResourcesLoader.Instance.RunWhenResourcesLoaded(() => {
                ResourcesLoader.Instance.LoadingWindowObject.SetActive(false);
                RefreshUserInfo();
            });
        });

    }

    public void AutoCreateSelect(string element = "fire")
    {
        SocketHandler.Instance.SendCreateCharacter(element,null,() => SocketHandler.Instance.SendSelectCharacter());
    }

    public void SelectCharacter(int index)
    {
        ResourcesLoader.Instance.LoadingWindowObject.SetActive(true);

        SocketHandler.Instance.SendSelectCharacter(()=> ResourcesLoader.Instance.LoadingWindowObject.SetActive(false), index);
    }

    public void DeleteCharacter (string actorId)
    {
        WarningWindowUI.Instance.Show("Delete this character forever and ever!?", () =>
        {
            ResourcesLoader.Instance.LoadingWindowObject.SetActive(true);

            SocketHandler.Instance.SendDeleteCharacter(actorId, () =>
            {
                ResourcesLoader.Instance.LoadingWindowObject.SetActive(false);
                List<ActorData> characters = new List<ActorData>();
                foreach(ActorData chara in SocketHandler.Instance.CurrentUser.chars)
                {
                    if(chara.actorId == actorId)
                    {
                        continue;
                    }

                    characters.Add(chara);
                }

                SocketHandler.Instance.CurrentUser.chars = characters.ToArray();
                RefreshUserInfo();
            });
        });
    }

    public void RefreshUserInfo()
    {
        CORE.ClearContainer(CharacterSelectionContainer);
        RemoveCharacterSelected();

        for(int i=0;i<SocketHandler.Instance.CurrentUser.chars.Length;i++)
        {
            ActorData character = SocketHandler.Instance.CurrentUser.chars[i];


            DisplayCharacterUI disAct = ResourcesLoader.Instance.GetRecycledObject("DisplayActor").GetComponent<DisplayCharacterUI>();
            disAct.transform.SetParent(CharacterSelectionContainer, false);
            disAct.transform.localScale = Vector3.one;
            disAct.transform.position = Vector3.one;
            disAct.AttachedCharacter.transform.position = Vector3.one;
            disAct.AttachedCharacter.SetActorInfo(character);
            disAct.SetInfo(() => { SetCharacterSelected(disAct); });
            if (i == 0)
            {
                SetCharacterSelected(disAct);
            }
        }

        if (SocketHandler.Instance.CurrentUser.chars.Length <= 0)
        {
            CreateCharacterPanelUI.Instance.Show();
        }

        CORE.Instance.DelayedInvokation(1f, SelectionGroup.RefreshGroup);
    }

    public void SetCharacterSelected(DisplayCharacterUI displayActor)
    {
        RemoveCharacterSelected();

        SelectedDisplayActor = displayActor;
        SelectedDisplayActor.Select();
    }

    public void RemoveCharacterSelected()
    {
        if(SelectedDisplayActor != null)
        {
            SelectedDisplayActor.Deselect();
            SelectedDisplayActor = null;
        }
    }

    public void ConfirmSelectedCharacter()
    {
        if(SelectedDisplayActor == null)
        {
            TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("No selected character...",Color.yellow,1f));
            return;
        }

        for (int i = 0; i < SocketHandler.Instance.CurrentUser.chars.Length; i++)
        {
            if (SocketHandler.Instance.CurrentUser.chars[i].name == SelectedDisplayActor.AttachedCharacter.State.Data.name)
            {
                SelectCharacter(i);
                break;
            }
        }
    }

    public void DeleteSelectedCharacter()
    {
        if (SelectedDisplayActor == null)
        {
            TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("No selected character...", Color.yellow, 1f));
            return;
        }

        DeleteCharacter(SelectedDisplayActor.AttachedCharacter.State.Data.actorId);

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
