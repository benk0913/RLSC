using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuUI : MonoBehaviour, WindowInterface
{
    public static SettingsMenuUI Instance;

    [SerializeField]
    Canvas Canv;

    [SerializeField]
    Dropdown RegionDropdown;

    public GameObject KeyboardBindings;
    public GameObject ControllerBindings;


    void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void Show(ActorData actorData, object data)
    {
        this.gameObject.SetActive(true);

        //TODO Yes, it's stupid, I know, remember to next time build your own settings menu.
        CORE.Instance.DelayedInvokation(0.1f,()=>{Canv.enabled = true;});

        KeyboardBindings.SetActive(!CORE.Instance.IsUsingJoystick);
        ControllerBindings.SetActive(CORE.Instance.IsUsingJoystick);

        RegionDropdown.ClearOptions();
        List<Dropdown.OptionData> regionOptions = new List<Dropdown.OptionData>();
        regionOptions.Add(new Dropdown.OptionData("us"));
        regionOptions.Add(new Dropdown.OptionData("eu"));
        //regionOptions.Add(new Dropdown.OptionData("sea"));
        RegionDropdown.AddOptions(regionOptions);
        Dropdown.OptionData currentOption =  RegionDropdown.options.Find(x => x.text == SocketHandler.Instance.ServerEnvironment.Region);
        if (currentOption == null)
        {
            currentOption = RegionDropdown.options[0];
        }
        RegionDropdown.SetValueWithoutNotify(RegionDropdown.options.IndexOf(currentOption));
    }

    public void OnRegionChanged(int optionIndex)
    {
        string region = PlayerPrefs.GetString("region");

        string newRegion = RegionDropdown.options[optionIndex].text;

        if (region == newRegion)
        {
            return;
        }

        Hide();

        WarningWindowUI.Instance.Show("Warning! Changing your region to " + newRegion + " will reuslt in disconnection from the game!", () => 
        {
            PlayerPrefs.SetString("region", newRegion);
            PlayerPrefs.Save();

            SocketHandler.Instance.LogOut();
        },false,()=> 
        {
            Show(null, null);
        });
    }

    public void StartMachinemaMode()
    {
        CORE.IsMachinemaMode = true;
        CORE.Instance.InvokeEvent("MachinemaModeRefresh");
    }
}
