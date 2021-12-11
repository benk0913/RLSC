using EdgeworldBase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.Networking;
using SimpleJSON;

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

    [SerializeField]
    RealmSigilUI RealmSigil;


    public void OpenURL(string url)
    {
        Steamworks.SteamFriends.ActivateGameOverlayToWebPage(url);//TODO Change to in-steam UI 
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
        CORE.Instance.LogMessage("Auto Login");
        ResourcesLoader.Instance.LoadingWindowObject.SetActive(true);

        // string region = PlayerPrefs.GetString("region", "");
        
        // if (string.IsNullOrEmpty(region))
        // {
        //     SocketHandler.Instance.SendGeolocationRequest((UnityWebRequest response) =>
        //     {
        //         string regionToSet = "us";
        //         if (response.result == UnityWebRequest.Result.ConnectionError || response.result == UnityWebRequest.Result.ProtocolError|| response.result == UnityWebRequest.Result.DataProcessingError)
        //         {
        //             CORE.Instance.LogMessage("Did NOT obtain GEOLOCATION ...");
        //         }
        //         else
        //         {
        //             JSONNode data = JSON.Parse(response.downloadHandler.text);
        //             CORE.Instance.LogMessage("Obtained GEOLOCATION - " + data["region"].Value);

        //             regionToSet = data["region"].Value;
        //         }
        //         PlayerPrefs.SetString("region", regionToSet);
        //         PlayerPrefs.Save();

        //         AutoLogin();
        //     });
        //     return;
        // }
        // else
        // {
        //     SocketHandler.Instance.ServerEnvironment.Region = region;
        // }

        Action postRealmLogin = () => 
        {
            CORE.Instance.LogMessage("Post Realm Login ("+ SocketHandler.Instance.SelectedRealmIndex+")");
            RealmSigil.SetData(CORE.Instance.Data.content.Realms[SocketHandler.Instance.SelectedRealmIndex]);

            if (!string.IsNullOrEmpty(SocketHandler.Instance.SessionTicket))
            {
                SocketHandler.Instance.SendLogin(() =>
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
                    SocketHandler.Instance.SendLogin(() =>
                    {
                        
                    
                        ResourcesLoader.Instance.RunWhenResourcesLoaded(() =>
                        {
                            ResourcesLoader.Instance.LoadingWindowObject.SetActive(false);
                            RefreshUserInfo();
                        });
                    });
                });
            }
        };

        SocketHandler.Instance.SelectedRealmIndex = PlayerPrefs.GetInt("SelectedRealmIndex", -1);

        if(SocketHandler.Instance.SelectedRealmIndex == -1)
        {
            ResourcesLoader.Instance.LoadingWindowObject.SetActive(false);
            RealmSelectionUI.Instance.Show((int selectdRealm) => 
            {
                SocketHandler.Instance.SelectedRealmIndex = selectdRealm;
                PlayerPrefs.SetInt("SelectedRealmIndex", SocketHandler.Instance.SelectedRealmIndex);
                PlayerPrefs.Save();

                ResourcesLoader.Instance.LoadingWindowObject.SetActive(true);
                postRealmLogin.Invoke();
            });
        }
        else
        {
            postRealmLogin.Invoke();
        }

    }

    public void ChangeRealms()
    {
        PlayerPrefs.SetInt("SelectedRealmIndex", -1);
        SocketHandler.Instance.SelectedRealmIndex = -1;
        SocketHandler.Instance.LogOut();

    }

    public void AutoCreateSelect(string element = "fire")
    {
        SocketHandler.Instance.SendCreateCharacter(element, null, () =>
        {
            SocketHandler.Instance.SendSelectCharacter();
            CORE.Instance.CheckOOGInvitations();
        });
    }

    public void SelectCharacter(int index)
    {
        ResourcesLoader.Instance.LoadingWindowObject.SetActive(true);

        SocketHandler.Instance.SendSelectCharacter(()=>
        {
            ResourcesLoader.Instance.LoadingWindowObject.SetActive(false);

            CORE.Instance.CheckOOGInvitations();

        }, index);
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
        if(CharacterSelectionContainer == null)
        {
            return;
        }

        CORE.DestroyContainer(CharacterSelectionContainer);
        RemoveCharacterSelected();

        CORE.Instance.DelayedInvokation(0.1f, () =>
             {

                 for (int i = 0; i < SocketHandler.Instance.CurrentUser.chars.Count; i++)
                 {
                     ActorData character = SocketHandler.Instance.CurrentUser.chars[i];
                     int characterIndex = 0 + i;
                     
                     DisplayCharacterUI disAct = Instantiate(ResourcesLoader.Instance.GetObject("DisplayActor")).GetComponent<DisplayCharacterUI>();
                     disAct.transform.SetParent(CharacterSelectionContainer, false);
                     disAct.transform.localScale = Vector3.one;
                     disAct.transform.position = Vector3.one;
                     disAct.AttachedCharacter.transform.position = Vector3.one;
                     disAct.AttachedCharacter.SetActorInfo(character);
                     disAct.GetComponent<DoubleclickHandlerUI>().OnDoubleClick.AddListener(() => { SelectCharacter(characterIndex); });
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
                 CreateCharTooltip.SetTooltip(CreateCharButton.interactable ? "Create a new character!" : CORE.QuickTranslate("Cannot have more than")+" " + maxCharacters + " "+ CORE.QuickTranslate("characters")+".");

                 CORE.Instance.DelayedInvokation(0f, () =>
                {
             if (SelectionGroup != null)
                 SelectionGroup.RefreshGroup(false);
             });


             });
    }

    public void SetCharacterSelected(DisplayCharacterUI displayActor)
    {
        RemoveCharacterSelected();

        SelectedDisplayActor = displayActor;
        SelectedDisplayActor.Select();

        PlayClassMusic(SelectedDisplayActor.AttachedCharacter.State.Data.ClassJobReference);
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

    public void ShowForums()
    {
        Steamworks.SteamFriends.ActivateGameOverlay("community");
    }

       public void ShowStorePage()
    {
        Application.OpenURL("https://discord.gg/nnB9V4hr");
        //Steamworks.SteamFriends.ActivateGameOverlayToStore(new Steamworks.AppId_t(1780330), Steamworks.EOverlayToStoreFlag.k_EOverlayToStoreFlag_None);
    }

    public void PlayClassMusic(ClassJob cJob)
    {
        AudioControl.Instance.SetClassMusic(cJob);
    }
}

