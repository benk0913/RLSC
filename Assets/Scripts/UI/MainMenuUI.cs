using EdgeworldBase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class MainMenuUI : MonoBehaviour
{
    public static MainMenuUI Instance;

    [SerializeField]
    Transform CharacterSelectionContainer;

    [SerializeField]
    DisplayCharacterUI SelectedDisplayActor;

    [SerializeField]
    SelectionGroupUI SelectionGroup;

    [SerializeField]
    Button CreateCharButton;

    [SerializeField]
    TooltipTargetUI CreateCharTooltip;

    [SerializeField]
    TextMeshProUGUI VersionLabel;

    [SerializeField]
    GameObject ClassSelectionPanel;

    [SerializeField]
    UnityEvent OnNoCharacters;


    public void OpenURL(string url)
    {
        Application.OpenURL(url);//TODO Change to in-steam UI 
    }
    private void Awake()
    {
        Instance = this;
        VersionLabel.text = Application.version;
    }

    private void Start()
    {
        AutoLogin();
    }
    
    public void AutoLogin()
    {
        ResourcesLoader.Instance.LoadingWindowObject.SetActive(true);

    
        if(!string.IsNullOrEmpty(SocketHandler.Instance.SessionTicket))
        {
            SocketHandler.Instance.SendLogin(()=>
            {
                ResourcesLoader.Instance.RunWhenResourcesLoaded(() => 
                {
                    ResourcesLoader.Instance.LoadingWindowObject.SetActive(false);
                    RefreshUserInfo();
                });
            });
        }
        else
        {
            SocketHandler.Instance.GetSteamSession(() =>
            {
                SocketHandler.Instance.SendLogin(()=>
                {
                    ResourcesLoader.Instance.RunWhenResourcesLoaded(() => 
                    {
                        ResourcesLoader.Instance.LoadingWindowObject.SetActive(false);
                        RefreshUserInfo();
                    });
                });
            });
        }

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

                SocketHandler.Instance.CurrentUser.chars = characters;
                RefreshUserInfo();
            });

        });
    }

    public void RefreshUserInfo()
    {
        CORE.ClearContainer(CharacterSelectionContainer);
        RemoveCharacterSelected();

        for(int i=0;i<SocketHandler.Instance.CurrentUser.chars.Count; i++)
        {
            ActorData character = SocketHandler.Instance.CurrentUser.chars[i];
            int characterIndex = 0 + i;

            DisplayCharacterUI disAct = ResourcesLoader.Instance.GetRecycledObject("DisplayActor").GetComponent<DisplayCharacterUI>();
            disAct.transform.SetParent(CharacterSelectionContainer, false);
            disAct.transform.localScale = Vector3.one;
            disAct.transform.position = Vector3.one;
            disAct.AttachedCharacter.transform.position = Vector3.one;
            disAct.AttachedCharacter.SetActorInfo(character);
            disAct.GetComponent<DoubleclickHandlerUI>().OnDoubleClick.AddListener(()=> { SelectCharacter(characterIndex); });
            disAct.SetInfo(() => { SetCharacterSelected(disAct); });
            if (i == 0)
            {
                SetCharacterSelected(disAct);
            }

            disAct.AttachedCharacter.RefreshLooks();
        }

        if (SocketHandler.Instance.CurrentUser.chars.Count <= 0)
        {
            OnNoCharacters?.Invoke();
        }
        int maxCharacters = CORE.Instance.Data.content.MaxCharacters + SocketHandler.Instance.CurrentUser.info.additionalCharSlots;
        CreateCharButton.interactable = SocketHandler.Instance.CurrentUser.chars.Count < maxCharacters;
        CreateCharTooltip.SetTooltip(CreateCharButton.interactable ? "Create a new character!" : "Cannot have more than " + maxCharacters + " characters.");

        CORE.Instance.DelayedInvokation(0f, () => SelectionGroup.RefreshGroup(false));
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
            TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("No selected character...", Colors.AsColor(Colors.COLOR_HIGHLIGHT),1f));
            return;
        }

        for (int i = 0; i < SocketHandler.Instance.CurrentUser.chars.Count; i++)
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
            TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("No selected character...", Colors.AsColor(Colors.COLOR_HIGHLIGHT), 1f));
            return;
        }

        DeleteCharacter(SelectedDisplayActor.AttachedCharacter.State.Data.actorId);

    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ShowOptionsMenu()
    {
        CORE.Instance.ShowWindow(SettingsMenuUI.Instance);
    }
}
