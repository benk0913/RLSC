using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public static MainMenuUI Instance;

    [SerializeField]
    Transform CharacterSelectionContainer;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CORE.Instance.DelayedInvokation(1f,AutoLogin);
    }
    
    public void AutoLogin()
    {
        ResourcesLoader.Instance.LoadingWindowObject.SetActive(true);

        SocketHandler.Instance.SendLogin(() =>
        {
            ResourcesLoader.Instance.LoadingWindowObject.SetActive(false);
            RefreshUserInfo();
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

    public void CreateCharacter()
    {
        ResourcesLoader.Instance.LoadingWindowObject.SetActive(true);

        SocketHandler.Instance.SendCreateCharacter("fire",null,() => ResourcesLoader.Instance.LoadingWindowObject.SetActive(false));
    }

    public void RefreshUserInfo()
    {
        CORE.ClearContainer(CharacterSelectionContainer);

        for(int i=0;i<SocketHandler.Instance.CurrentUser.chars.Length;i++)
        {
            ActorData character = SocketHandler.Instance.CurrentUser.chars[i];


            DisplayCharacterUI disAct = ResourcesLoader.Instance.GetRecycledObject("DisplayActor").GetComponent<DisplayCharacterUI>();
            disAct.transform.SetParent(CharacterSelectionContainer, false);
            disAct.transform.localScale = Vector3.one;
            disAct.transform.position = Vector3.one;
            disAct.AttachedCharacter.transform.position = Vector3.one;
            disAct.AttachedCharacter.SetActorInfo(character);
            int index = i;
            disAct.SetInfo(() => { SelectCharacter(index); });
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
