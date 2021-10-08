using EdgeworldBase;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CreateCharacterPanelUI : MonoBehaviour
{
    public static CreateCharacterPanelUI Instance;

    [SerializeField]
    Actor DisplayActor;

    [SerializeField]
    TMP_InputField NameInputField;

    [SerializeField]
    SelectionGroupUI SelectionGroup;

    [SerializeField]
    UnityEvent OnCharacterCreationComplete;

    public List<SkinSet> DefaultEars = new List<SkinSet>();
    public List<SkinSet> DefaultEyebrows = new List<SkinSet>();
    public List<SkinSet> DefaultEyes = new List<SkinSet>();
    public List<SkinSet> DefaultHair = new List<SkinSet>();
    public List<SkinSet> DefaultNose = new List<SkinSet>();
    public List<SkinSet> DefaultMouth = new List<SkinSet>();
    public List<SkinSet> DefaultIris = new List<SkinSet>();

    public Color DefaultSkinColor;
    public Color DefaultHairColor;

    public string Job = "fire";
    public List<GameObject> JobFrames = new List<GameObject>();
    public List<GameObject> JobFrames2 = new List<GameObject>();
    public List<string> Jobs = new List<string>{"fire", "water", "earth", "air"};

    public Image AlignmentGoodHalo;
    public Image AlignmentEvilHalo;

    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }


    public void Show()
    {
        this.gameObject.SetActive(true);

        DisplayActor.State = new ActorState();
        DisplayActor.State.Data = new ActorData("", "fire", DisplayActor.gameObject);

        // Randomize whether a female or not only at the beginning.
        DisplayActor.State.Data.looks.IsFemale = Random.Range(0, 2) == 0;
        UpdateClassJob(Random.Range(0, 4));
        Randomize();
        
        CORE.Instance.DelayedInvokation(0f, () => SelectionGroup.RefreshGroup(false));
    }

    public void Randomize()
    {
        DisplayActor.State.Data.looks.Ears = DefaultEars[Random.Range(0,DefaultEars.Count)].name;
        DisplayActor.State.Data.looks.Eyes = DefaultEyes[Random.Range(0, DefaultEyes.Count)].name;
        DisplayActor.State.Data.looks.Hair = DefaultHair[Random.Range(0, DefaultHair.Count)].name;
        DisplayActor.State.Data.looks.Nose = DefaultNose[Random.Range(0, DefaultNose.Count)].name;
        DisplayActor.State.Data.looks.Mouth = DefaultMouth[Random.Range(0, DefaultMouth.Count)].name;
        DisplayActor.State.Data.looks.Iris = DefaultIris[Random.Range(0, DefaultIris.Count)].name;
        DisplayActor.State.Data.looks.Eyebrows = DefaultEyebrows[Random.Range(0, DefaultEyebrows.Count)].name;

        DisplayActor.State.Data.looks.SkinColor = 
            "#" + ColorUtility.ToHtmlStringRGB(CORE.Instance.Data.content.Visuals.SkinColorPresets[Random.Range(0, CORE.Instance.Data.content.Visuals.SkinColorPresets.Count)]);
        DisplayActor.State.Data.looks.HairColor =
            "#" + ColorUtility.ToHtmlStringRGB(CORE.Instance.Data.content.Visuals.HairColorPresets[Random.Range(0, CORE.Instance.Data.content.Visuals.HairColorPresets.Count)]);

        RandomizeName();
        
        DisplayActor.RefreshLooks();
    }

    public void RandomizeName()
    {
        NameInputField.text = "";
        SocketHandler.Instance.SendGetRandomName(DisplayActor.State.Data.looks.IsFemale, (string name) =>
        {
            if (NameInputField.text == "" && this.gameObject.activeInHierarchy) {
                NameInputField.text = name;
            }
        });
    }

    public void UpdateNameValue()
    {
        if (NameInputField.text.Length > CORE.Instance.Data.content.MaxNameLength) {
            NameInputField.text = NameInputField.text.Substring(0, CORE.Instance.Data.content.MaxNameLength);
        }
        DisplayActor.State.Data.name = NameInputField.text;
        DisplayActor.RefreshName();
    }

    public void Confirm()
    {
        ResourcesLoader.Instance.LoadingWindowObject.SetActive(true);
        SocketHandler.Instance.SendCreateCharacter(DisplayActor.State.Data.classJob, DisplayActor.State.Data, () =>
        {
            this.gameObject.SetActive(false);
            OnCharacterCreationComplete?.Invoke();
            CORE.Instance.DelayedInvokation(0.1f, () => {
                MainMenuUI.Instance.RefreshUserInfo();            
                ResourcesLoader.Instance.LoadingWindowObject.SetActive(false);
            });
        },()=>
        {
            this.gameObject.SetActive(true);
            
        });
    }

   

    public void SetGender(bool isFemale)
    {
        DisplayActor.State.Data.looks.IsFemale = isFemale;
        DisplayActor.RefreshLooks();
    }

    public void SetSkinColor(string clr)
    {
        DisplayActor.State.Data.looks.SkinColor = clr;
        DisplayActor.RefreshLooks();
    }

    public void SetHairColor(string clr)
    {
        DisplayActor.State.Data.looks.HairColor = clr;
        DisplayActor.RefreshLooks();
    }

     public void SetIrisColor(int irisIndex)
    {
        DisplayActor.State.Data.looks.Iris = DefaultIris[irisIndex].name;
        DisplayActor.RefreshLooks();
    }

    public void NextHair()
    {
        int index = DefaultHair.IndexOf(DefaultHair.Find(x=>x.name == DisplayActor.State.Data.looks.Hair))+1;

        if(index >= DefaultHair.Count)
        {
            index = 0;
        }

        DisplayActor.State.Data.looks.Hair = DefaultHair[index].name;
        DisplayActor.RefreshLooks();
    }

    public void PreviousHair()
    {
        int index = DefaultHair.IndexOf(DefaultHair.Find(x => x.name == DisplayActor.State.Data.looks.Hair))-1;

        if (index < 0)
        {
            index = DefaultHair.Count - 1;
        }

        DisplayActor.State.Data.looks.Hair = DefaultHair[index].name;
        DisplayActor.RefreshLooks();
    }

    public void NextEyebrows()
    {
        int index = DefaultEyebrows.IndexOf(DefaultEyebrows.Find(x => x.name == DisplayActor.State.Data.looks.Eyebrows)) + 1;

        if (index >= DefaultEyebrows.Count)
        {
            index = 0;
        }

        DisplayActor.State.Data.looks.Eyebrows = DefaultEyebrows[index].name;
        DisplayActor.RefreshLooks();
    }

    public void PreviousEyebrows()
    {
        int index = DefaultEyebrows.IndexOf(DefaultEyebrows.Find(x => x.name == DisplayActor.State.Data.looks.Eyebrows)) - 1;

        if (index < 0)
        {
            index = DefaultEyebrows.Count - 1;
        }

        DisplayActor.State.Data.looks.Eyebrows = DefaultEyebrows[index].name;
        DisplayActor.RefreshLooks();
    }

    public void NextEyes()
    {
        int index = DefaultEyes.IndexOf(DefaultEyes.Find(x => x.name == DisplayActor.State.Data.looks.Eyes)) + 1;

        if (index >= DefaultEyes.Count)
        {
            index = 0;
        }

        DisplayActor.State.Data.looks.Eyes = DefaultEyes[index].name;
        DisplayActor.RefreshLooks();
    }

    public void PreviousEyes()
    {
        int index = DefaultEyes.IndexOf(DefaultEyes.Find(x => x.name == DisplayActor.State.Data.looks.Eyes)) - 1;

        if (index < 0)
        {
            index = DefaultEyes.Count - 1;
        }

        DisplayActor.State.Data.looks.Eyes = DefaultEyes[index].name;
        DisplayActor.RefreshLooks();
    }

    public void NextIris()
    {
        int index = DefaultIris.IndexOf(DefaultIris.Find(x => x.name == DisplayActor.State.Data.looks.Iris)) + 1;

        if (index >= DefaultIris.Count)
        {
            index = 0;
        }

        DisplayActor.State.Data.looks.Iris = DefaultIris[index].name;
        DisplayActor.RefreshLooks();
    }

    public void PreviousIris()
    {
        int index = DefaultIris.IndexOf(DefaultIris.Find(x => x.name == DisplayActor.State.Data.looks.Iris)) - 1;

        if (index < 0)
        {
            index = DefaultIris.Count - 1;
        }

        DisplayActor.State.Data.looks.Iris = DefaultIris[index].name;
        DisplayActor.RefreshLooks();
    }

    public void NextNose()
    {
        int index = DefaultNose.IndexOf(DefaultNose.Find(x => x.name == DisplayActor.State.Data.looks.Nose)) + 1;

        if (index >= DefaultNose.Count)
        {
            index = 0;
        }

        DisplayActor.State.Data.looks.Nose = DefaultNose[index].name;
        DisplayActor.RefreshLooks();
    }

    public void PreviousNose()
    {
        int index = DefaultNose.IndexOf(DefaultNose.Find(x => x.name == DisplayActor.State.Data.looks.Nose)) - 1;

        if (index < 0)
        {
            index = DefaultNose.Count - 1;
        }

        DisplayActor.State.Data.looks.Nose = DefaultNose[index].name;
        DisplayActor.RefreshLooks();
    }

    public void NextMouth()
    {
        int index = DefaultMouth.IndexOf(DefaultMouth.Find(x => x.name == DisplayActor.State.Data.looks.Mouth)) + 1;

        if (index >= DefaultMouth.Count)
        {
            index = 0;
        }

        DisplayActor.State.Data.looks.Mouth = DefaultMouth[index].name;
        DisplayActor.RefreshLooks();
    }

    public void PreviousMouth()
    {
        int index = DefaultMouth.IndexOf(DefaultMouth.Find(x => x.name == DisplayActor.State.Data.looks.Mouth)) - 1;

        if (index < 0)
        {
            index = DefaultMouth.Count-1;
        }

        DisplayActor.State.Data.looks.Mouth = DefaultMouth[index].name;
        DisplayActor.RefreshLooks();
    }

    public void NextEars()
    {
        int index = DefaultEars.IndexOf(DefaultEars.Find(x => x.name == DisplayActor.State.Data.looks.Ears)) + 1;

        if (index >= DefaultEars.Count)
        {
            index = 0;
        }

        DisplayActor.State.Data.looks.Ears = DefaultEars[index].name;
        DisplayActor.RefreshLooks();
    }

    public void PreviousEars()
    {
        int index = DefaultEars.IndexOf(DefaultEars.Find(x => x.name == DisplayActor.State.Data.looks.Ears)) - 1;

        if (index < 0)
        {
            index = DefaultEars.Count-1;
        }

        DisplayActor.State.Data.looks.Ears = DefaultEars[index].name;
        DisplayActor.RefreshLooks();
    }

    public void NextClassJob()
    {
        int index = Jobs.IndexOf(DisplayActor.State.Data.classJob);

        index++;

        if (index >= Jobs.Count)
        {
            index = 0;
        }

        SetClassJob(index);
    }

    public void PreviousClassJob()
    {
        int index = Jobs.IndexOf(DisplayActor.State.Data.classJob);

        index--;

        if (index < 0)
        {
            index = Jobs.Count-1;
        }

        SetClassJob(index);
    }

    public void SetClassJob(int index)
    {
        UpdateClassJob(index);
        DisplayActor.RefreshLooks();
    }

    public void SetAlignmentGood(bool isGood)
    {
        DisplayActor.State.Data.alignmentGood = isGood;
        if(isGood)
        {
            AlignmentGoodHalo.gameObject.SetActive(true);
            AlignmentEvilHalo.gameObject.SetActive(false);
        }
        else
        {
            AlignmentGoodHalo.gameObject.SetActive(false);
            AlignmentEvilHalo.gameObject.SetActive(true);
        }
    }

    private void UpdateClassJob(int index)
    {
        DisplayActor.State.Data.classJob = Jobs[index];
        for(int i=0;i<JobFrames.Count;i++)
        {
            JobFrames[i].gameObject.SetActive(i==index);
            JobFrames2[i].gameObject.SetActive(i==index);
        }
    }

}
