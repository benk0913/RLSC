using I2.Loc;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettingsMenuUI : MonoBehaviour, WindowInterface
{
    public static SettingsMenuUI Instance;

    [SerializeField]
    Canvas Canv;

    [SerializeField]
    Dropdown RegionDropdown;

    [SerializeField]
    Dropdown LanguageDropdown;

    public GameObject KeyboardBindings;
    public GameObject ControllerBindings;

    public bool PPVignette;
    public bool PPBloom;
    public bool PPMotionBlur;
    public bool PPProjection;

    public GameObject PPVignetteCheckmark;
    public GameObject PPBloomCheckmark;
    public GameObject PPMotionBlurCheckmark;
    public GameObject PPProjectionCheckmark;

    public VolumeProfile GlobalPostProccessProfile;

    void Awake()
    {
        Instance = this;
        Hide();
    }

    void Start()
    {
        PPVignette = PlayerPrefs.GetInt("PPVignette", 1) == 1 ? true : false;
        PPBloom = PlayerPrefs.GetInt("PPBloom", 1) == 1 ? true : false;
        PPMotionBlur = PlayerPrefs.GetInt("PPMotionBlur", 1) == 1 ? true : false;
        PPProjection = PlayerPrefs.GetInt("PPProjection", 1) == 1 ? true : false;

        PPMotionBlurCheckmark.SetActive(PPMotionBlur);
        PPVignetteCheckmark.SetActive(PPVignette);
        PPBloomCheckmark.SetActive(PPBloom);
        PPProjectionCheckmark.SetActive(PPProjection);

        VolumeComponent vc = GlobalPostProccessProfile.components.Find(x => x.name == "Vignette");
        if (vc != null)
        {
            vc.active = PPVignette;
        }

        vc = GlobalPostProccessProfile.components.Find(x => x.name == "Bloom");
        if (vc != null)
        {
            vc.active = PPBloom;
        }

        vc = GlobalPostProccessProfile.components.Find(x => x.name == "PaniniProjection");
        if (vc != null)
        {
            vc.active = PPProjection;
        }

        vc = GlobalPostProccessProfile.components.Find(x => x.name == "MotionBlur");
        if (vc != null)
        {
            vc.active = PPMotionBlur;
        }

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
        // regionOptions.Add(new Dropdown.OptionData("eu"));
        //regionOptions.Add(new Dropdown.OptionData("sea"));
        RegionDropdown.AddOptions(regionOptions);
        Dropdown.OptionData currentOption =  RegionDropdown.options.Find(x => x.text == SocketHandler.Instance.ServerEnvironment.Region);
        if (currentOption == null)
        {
            currentOption = RegionDropdown.options[0];
        }
        RegionDropdown.SetValueWithoutNotify(RegionDropdown.options.IndexOf(currentOption));


        LanguageDropdown.ClearOptions();
        List<Dropdown.OptionData> langOptions = new List<Dropdown.OptionData>();
        foreach (string lang in CORE.Instance.Data.Localizator.mSource.GetLanguages())
        {
            langOptions.Add(new Dropdown.OptionData(lang));
        }
        LanguageDropdown.AddOptions(langOptions);

        Dropdown.OptionData langCurrentOption = LanguageDropdown.options.Find(x => x.text == PlayerPrefs.GetString("language", "English"));
        if (langCurrentOption == null)
        {
            langCurrentOption = LanguageDropdown.options[0];
        }

        LanguageDropdown.SetValueWithoutNotify(LanguageDropdown.options.IndexOf(langCurrentOption));

    }

    public void OnLanguageChanged(int optionIndex)
    {
        Hide();
        WarningWindowUI.Instance.Show("This will restart the application! ", () =>
        {
            string language = PlayerPrefs.GetString("language", "English");

            string newLanguage = LanguageDropdown.options[optionIndex].text;

            if (language == newLanguage)
            {
                return;
            }

            PlayerPrefs.SetString("language", newLanguage);
            PlayerPrefs.Save();

            LocalizationManager.CurrentLanguage = newLanguage;
            CORE.Instance.CurrentLanguage = newLanguage;
            CORE.Instance.InvokeEvent("LanguageChanged");

            Dropdown.OptionData langCurrentOption = LanguageDropdown.options.Find(x => x.text == PlayerPrefs.GetString("language", "English"));
            if (langCurrentOption == null)
            {
                langCurrentOption = LanguageDropdown.options[0];
            }

            LanguageDropdown.SetValueWithoutNotify(LanguageDropdown.options.IndexOf(langCurrentOption));
            
            SocketHandler.Instance.LogOut();
        });
    }

    public void OnLanguageChanged(string languageKey)
    {
        Hide();
        WarningWindowUI.Instance.Show("This will restart the application! ", () =>
        {
            PlayerPrefs.SetString("language", languageKey);
            PlayerPrefs.Save();

            LocalizationManager.CurrentLanguage = languageKey;
            CORE.Instance.CurrentLanguage = languageKey;
            CORE.Instance.InvokeEvent("LanguageChanged");

            Dropdown.OptionData langCurrentOption = LanguageDropdown.options.Find(x => x.text == PlayerPrefs.GetString("language", "English"));
            if (langCurrentOption == null)
            {
                langCurrentOption = LanguageDropdown.options[0];
            }

            LanguageDropdown.SetValueWithoutNotify(LanguageDropdown.options.IndexOf(langCurrentOption));
            
            SocketHandler.Instance.LogOut();
        });
    }

    public void OnRegionChanged(int optionIndex)
    {
        // only one region for now so this isn't needed
        
        // string region = PlayerPrefs.GetString("region");

        // string newRegion = RegionDropdown.options[optionIndex].text;

        // if (region == newRegion)
        // {
        //     return;
        // }

        // Hide();

        // System.Action changeRegionAction = () =>
        // {
        //     SocketHandler.Instance.SelectedRealmIndex = -1;
        //     PlayerPrefs.SetInt("SelectedRealmIndex", -1);
        //     PlayerPrefs.Save();

        //     PlayerPrefs.SetString("region", newRegion);
        //     PlayerPrefs.Save();

        //     SocketHandler.Instance.LogOut();
        // };

        // if (!CORE.Instance.InGame)
        // {
        //     changeRegionAction?.Invoke();
        // }
        // else
        // {
        //     WarningWindowUI.Instance.Show("Warning! Changing your region to " + newRegion + " will reuslt in disconnection from the game!", () =>
        //     {
        //         changeRegionAction.Invoke();
        //     }, false, () =>
        //   {
        //         Show(null, null);
        //     });
        // }
    }

    public void ToggleVignette()
    {
        PPVignette = !PPVignette;

        PPVignetteCheckmark.SetActive(PPVignette);

        PlayerPrefs.SetInt("PPVignette", PPVignette ? 1 : 0);
        PlayerPrefs.Save();

        VolumeComponent vc = GlobalPostProccessProfile.components.Find(x => x.name == "Vignette");

        if(vc != null)
        {
            vc.active = PPVignette;
        }
    }

    public void ToggleBloom()
    {
        PPBloom = !PPBloom;

        PPBloomCheckmark.SetActive(PPBloom);

        PlayerPrefs.SetInt("PPBloom", PPBloom ? 1 : 0);
        PlayerPrefs.Save();

        VolumeComponent vc = GlobalPostProccessProfile.components.Find(x => x.name == "Bloom");

        if (vc != null)
        {
            vc.active = PPBloom;
        }
    }

    public void TogglePaniniProjection()
    {
        PPProjection = !PPProjection;

        PPProjectionCheckmark.SetActive(PPProjection);

        PlayerPrefs.SetInt("PPProjection", PPProjection ? 1 : 0);
        PlayerPrefs.Save();

        VolumeComponent vc = GlobalPostProccessProfile.components.Find(x => x.name == "PaniniProjection");

        if (vc != null)
        {
            vc.active = PPProjection;
        }
    }

    public void ToggleMotionBlur()
    {
        PPMotionBlur = !PPMotionBlur;

        PPMotionBlurCheckmark.SetActive(PPMotionBlur);

        PlayerPrefs.SetInt("PPMotionBlur", PPMotionBlur ? 1 : 0);
        PlayerPrefs.Save();

        VolumeComponent vc = GlobalPostProccessProfile.components.Find(x => x.name == "MotionBlur");

        if (vc != null)
        {
            vc.active = PPMotionBlur;
        }
    }

    public void StartMachinemaMode()
    {
        CORE.IsMachinemaMode = true;
        CORE.Instance.InvokeEvent("MachinemaModeRefresh");
    }

    public void TryNextInDropdown(Dropdown dropdown)
    {
        if(!dropdown.interactable)
        {
            return;
        }

        int targetValue = dropdown.value+1;

        if(targetValue >= dropdown.options.Count)
        {
            targetValue = 0;
        }

        dropdown.SetValueWithoutNotify(targetValue);
    }

     public void TryPreviousDropdown(Dropdown dropdown)
    {
        if(!dropdown.interactable)
        {
            return;
        }

        int targetValue = dropdown.value - 1;

        if(targetValue < 0)
        {
            targetValue = dropdown.options.Count - 1;
        }

        dropdown.SetValueWithoutNotify(targetValue);
    }
    
    public void ConfirmDropdown(Dropdown dropdown)
    {
        if(!dropdown.interactable)
        {
            return;
        }
            
        WarningWindowUI.Instance.Show(CORE.QuickTranslate("Confirm Change to")+" "+dropdown.options[dropdown.value].text+"?",()=>
        {


            dropdown.onValueChanged.Invoke(dropdown.value);
        });
    }
}
