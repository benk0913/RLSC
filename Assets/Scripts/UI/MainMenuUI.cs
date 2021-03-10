using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    
    private void Start()
    {
        CORE.Instance.DelayedInvokation(1f,AutoLogin);
    }
    
    public void AutoLogin()
    {
        ResourcesLoader.Instance.LoadingWindowObject.SetActive(true);

        SocketHandler.Instance.SendLogin(() => ResourcesLoader.Instance.LoadingWindowObject.SetActive(false));

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

    public void QuitGame()
    {
        Application.Quit();
    }
}
