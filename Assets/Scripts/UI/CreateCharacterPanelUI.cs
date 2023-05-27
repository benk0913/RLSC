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

    [SerializeField]
    GameObject envObject;

    public List<GameObject> JobFrames = new List<GameObject>();
    public List<GameObject> JobFrames2 = new List<GameObject>();
    public List<string> Jobs = new List<string>{"fire", "water", "earth", "air"};

    public Image AlignmentGoodHalo;
    public Image AlignmentEvilHalo;

    int classJobIndex = 0;

    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }


    public void Show()
    {
        this.gameObject.SetActive(true);

        #if UNITY_ANDROID || UNITY_IOS
        envObject.transform.localScale = new Vector3(75,75,envObject.transform.localScale.z);
        #endif
        DisplayActor.State = new ActorState();
        DisplayActor.State.Data = new ActorData("", "fire", DisplayActor.gameObject);

        // Randomize whether a female or not only at the beginning.
        DisplayActor.State.Data.looks.IsFemale = Random.Range(0, 2) == 0;
        
        RandomizeName();
        Randomize();

        UpdateClassJob(classJobIndex);

        CORE.Instance.DelayedInvokation(0f, () => SelectionGroup.RefreshGroup(false));
    }

    public void Randomize()
    {
        DisplayActor.State.Data.looks.Ears = CORE.Instance.Data.content.Visuals.DefaultEars[Random.Range(0,CORE.Instance.Data.content.Visuals.DefaultEars.Count)].name;
        DisplayActor.State.Data.looks.Eyes = CORE.Instance.Data.content.Visuals.DefaultEyes[Random.Range(0, CORE.Instance.Data.content.Visuals.DefaultEyes.Count)].name;
        DisplayActor.State.Data.looks.Hair = CORE.Instance.Data.content.Visuals.DefaultHair[Random.Range(0, CORE.Instance.Data.content.Visuals.DefaultHair.Count)].name;
        DisplayActor.State.Data.looks.Nose = CORE.Instance.Data.content.Visuals.DefaultNose[Random.Range(0, CORE.Instance.Data.content.Visuals.DefaultNose.Count)].name;
        DisplayActor.State.Data.looks.Mouth = CORE.Instance.Data.content.Visuals.DefaultMouth[Random.Range(0, CORE.Instance.Data.content.Visuals.DefaultMouth.Count)].name;
        DisplayActor.State.Data.looks.Iris = CORE.Instance.Data.content.Visuals.DefaultIris[Random.Range(0, CORE.Instance.Data.content.Visuals.DefaultIris.Count)].name;
        DisplayActor.State.Data.looks.Eyebrows = CORE.Instance.Data.content.Visuals.DefaultEyebrows[Random.Range(0, CORE.Instance.Data.content.Visuals.DefaultEyebrows.Count)].name;

        DisplayActor.State.Data.looks.SkinColor = 
            "#" + ColorUtility.ToHtmlStringRGB(CORE.Instance.Data.content.Visuals.DefaultSkinColor[Random.Range(0, CORE.Instance.Data.content.Visuals.DefaultSkinColor.Count)]);
        DisplayActor.State.Data.looks.HairColor =
            "#" + ColorUtility.ToHtmlStringRGB(CORE.Instance.Data.content.Visuals.DefaultHairColor[Random.Range(0, CORE.Instance.Data.content.Visuals.DefaultHairColor.Count)]);
        
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
                MainMenuUI.Instance.AutoConfirmOnlyCharacter();
            });
        },()=>
        {
            ResourcesLoader.Instance.LoadingWindowObject.SetActive(false);
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
        DisplayActor.State.Data.looks.Iris = CORE.Instance.Data.content.Visuals.DefaultIris[irisIndex].name;
        DisplayActor.RefreshLooks();
    }

    public void NextHair()
    {
        int index = CORE.Instance.Data.content.Visuals.DefaultHair.IndexOf(CORE.Instance.Data.content.Visuals.DefaultHair.Find(x=>x.name == DisplayActor.State.Data.looks.Hair))+1;

        if(index >= CORE.Instance.Data.content.Visuals.DefaultHair.Count)
        {
            index = 0;
        }

        DisplayActor.State.Data.looks.Hair = CORE.Instance.Data.content.Visuals.DefaultHair[index].name;
        DisplayActor.RefreshLooks();
    }

    public void PreviousHair()
    {
        int index = CORE.Instance.Data.content.Visuals.DefaultHair.IndexOf(CORE.Instance.Data.content.Visuals.DefaultHair.Find(x => x.name == DisplayActor.State.Data.looks.Hair))-1;

        if (index < 0)
        {
            index = CORE.Instance.Data.content.Visuals.DefaultHair.Count - 1;
        }

        DisplayActor.State.Data.looks.Hair = CORE.Instance.Data.content.Visuals.DefaultHair[index].name;
        DisplayActor.RefreshLooks();
    }

    public void NextEyebrows()
    {
        int index = CORE.Instance.Data.content.Visuals.DefaultEyebrows.IndexOf(CORE.Instance.Data.content.Visuals.DefaultEyebrows.Find(x => x.name == DisplayActor.State.Data.looks.Eyebrows)) + 1;

        if (index >= CORE.Instance.Data.content.Visuals.DefaultEyebrows.Count)
        {
            index = 0;
        }

        DisplayActor.State.Data.looks.Eyebrows = CORE.Instance.Data.content.Visuals.DefaultEyebrows[index].name;
        DisplayActor.RefreshLooks();
    }

    public void PreviousEyebrows()
    {
        int index = CORE.Instance.Data.content.Visuals.DefaultEyebrows.IndexOf(CORE.Instance.Data.content.Visuals.DefaultEyebrows.Find(x => x.name == DisplayActor.State.Data.looks.Eyebrows)) - 1;

        if (index < 0)
        {
            index = CORE.Instance.Data.content.Visuals.DefaultEyebrows.Count - 1;
        }

        DisplayActor.State.Data.looks.Eyebrows = CORE.Instance.Data.content.Visuals.DefaultEyebrows[index].name;
        DisplayActor.RefreshLooks();
    }

    public void NextEyes()
    {
        int index = CORE.Instance.Data.content.Visuals.DefaultEyes.IndexOf(CORE.Instance.Data.content.Visuals.DefaultEyes.Find(x => x.name == DisplayActor.State.Data.looks.Eyes)) + 1;

        if (index >= CORE.Instance.Data.content.Visuals.DefaultEyes.Count)
        {
            index = 0;
        }

        DisplayActor.State.Data.looks.Eyes = CORE.Instance.Data.content.Visuals.DefaultEyes[index].name;
        DisplayActor.RefreshLooks();
    }

    public void PreviousEyes()
    {
        int index = CORE.Instance.Data.content.Visuals.DefaultEyes.IndexOf(CORE.Instance.Data.content.Visuals.DefaultEyes.Find(x => x.name == DisplayActor.State.Data.looks.Eyes)) - 1;

        if (index < 0)
        {
            index = CORE.Instance.Data.content.Visuals.DefaultEyes.Count - 1;
        }

        DisplayActor.State.Data.looks.Eyes = CORE.Instance.Data.content.Visuals.DefaultEyes[index].name;
        DisplayActor.RefreshLooks();
    }

    public void NextIris()
    {
        int index = CORE.Instance.Data.content.Visuals.DefaultIris.IndexOf(CORE.Instance.Data.content.Visuals.DefaultIris.Find(x => x.name == DisplayActor.State.Data.looks.Iris)) + 1;

        if (index >= CORE.Instance.Data.content.Visuals.DefaultIris.Count)
        {
            index = 0;
        }

        DisplayActor.State.Data.looks.Iris = CORE.Instance.Data.content.Visuals.DefaultIris[index].name;
        DisplayActor.RefreshLooks();
    }

    public void PreviousIris()
    {
        int index = CORE.Instance.Data.content.Visuals.DefaultIris.IndexOf(CORE.Instance.Data.content.Visuals.DefaultIris.Find(x => x.name == DisplayActor.State.Data.looks.Iris)) - 1;

        if (index < 0)
        {
            index = CORE.Instance.Data.content.Visuals.DefaultIris.Count - 1;
        }

        DisplayActor.State.Data.looks.Iris = CORE.Instance.Data.content.Visuals.DefaultIris[index].name;
        DisplayActor.RefreshLooks();
    }

    public void NextNose()
    {
        int index = CORE.Instance.Data.content.Visuals.DefaultNose.IndexOf(CORE.Instance.Data.content.Visuals.DefaultNose.Find(x => x.name == DisplayActor.State.Data.looks.Nose)) + 1;

        if (index >= CORE.Instance.Data.content.Visuals.DefaultNose.Count)
        {
            index = 0;
        }

        DisplayActor.State.Data.looks.Nose = CORE.Instance.Data.content.Visuals.DefaultNose[index].name;
        DisplayActor.RefreshLooks();
    }

    public void PreviousNose()
    {
        int index = CORE.Instance.Data.content.Visuals.DefaultNose.IndexOf(CORE.Instance.Data.content.Visuals.DefaultNose.Find(x => x.name == DisplayActor.State.Data.looks.Nose)) - 1;

        if (index < 0)
        {
            index = CORE.Instance.Data.content.Visuals.DefaultNose.Count - 1;
        }

        DisplayActor.State.Data.looks.Nose = CORE.Instance.Data.content.Visuals.DefaultNose[index].name;
        DisplayActor.RefreshLooks();
    }

    public void NextMouth()
    {
        int index = CORE.Instance.Data.content.Visuals.DefaultMouth.IndexOf(CORE.Instance.Data.content.Visuals.DefaultMouth.Find(x => x.name == DisplayActor.State.Data.looks.Mouth)) + 1;

        if (index >= CORE.Instance.Data.content.Visuals.DefaultMouth.Count)
        {
            index = 0;
        }

        DisplayActor.State.Data.looks.Mouth = CORE.Instance.Data.content.Visuals.DefaultMouth[index].name;
        DisplayActor.RefreshLooks();
    }

    public void PreviousMouth()
    {
        int index = CORE.Instance.Data.content.Visuals.DefaultMouth.IndexOf(CORE.Instance.Data.content.Visuals.DefaultMouth.Find(x => x.name == DisplayActor.State.Data.looks.Mouth)) - 1;

        if (index < 0)
        {
            index = CORE.Instance.Data.content.Visuals.DefaultMouth.Count-1;
        }

        DisplayActor.State.Data.looks.Mouth = CORE.Instance.Data.content.Visuals.DefaultMouth[index].name;
        DisplayActor.RefreshLooks();
    }

    public void NextEars()
    {
        int index = CORE.Instance.Data.content.Visuals.DefaultEars.IndexOf(CORE.Instance.Data.content.Visuals.DefaultEars.Find(x => x.name == DisplayActor.State.Data.looks.Ears)) + 1;

        if (index >= CORE.Instance.Data.content.Visuals.DefaultEars.Count)
        {
            index = 0;
        }

        DisplayActor.State.Data.looks.Ears = CORE.Instance.Data.content.Visuals.DefaultEars[index].name;
        DisplayActor.RefreshLooks();
    }

    public void PreviousEars()
    {
        int index = CORE.Instance.Data.content.Visuals.DefaultEars.IndexOf(CORE.Instance.Data.content.Visuals.DefaultEars.Find(x => x.name == DisplayActor.State.Data.looks.Ears)) - 1;

        if (index < 0)
        {
            index = CORE.Instance.Data.content.Visuals.DefaultEars.Count-1;
        }

        DisplayActor.State.Data.looks.Ears = CORE.Instance.Data.content.Visuals.DefaultEars[index].name;
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
        classJobIndex = index;
        DisplayActor.State.Data.classJob = Jobs[classJobIndex];
        AudioControl.Instance.SetClassMusic(DisplayActor.State.Data.ClassJobReference);
        for(int i=0;i<JobFrames.Count;i++)
        {
            JobFrames[i].gameObject.SetActive(i==index);
            JobFrames2[i].gameObject.SetActive(i==index);
        }
    }

}
