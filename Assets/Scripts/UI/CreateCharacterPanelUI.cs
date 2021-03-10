using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateCharacterPanelUI : MonoBehaviour
{
    public static CreateCharacterPanelUI Instance;

    [SerializeField]
    Actor DisplayActor;

    public List<SkinSet> DefaultEars = new List<SkinSet>();
    public List<SkinSet> DefaultEyebrows = new List<SkinSet>();
    public List<SkinSet> DefaultEyes = new List<SkinSet>();
    public List<SkinSet> DefaultHair = new List<SkinSet>();
    public List<SkinSet> DefaultNose = new List<SkinSet>();
    public List<SkinSet> DefaultMouth = new List<SkinSet>();
    public List<SkinSet> DefaultIris = new List<SkinSet>();

    public Color DefaultSkinColor;
    public Color DefaultHairColor;



    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void Show()
    {
        this.gameObject.SetActive(true);

        DisplayActor.State = new ActorState();
        DisplayActor.State.Data = new ActorData("preloader", "whatever", "fire", DisplayActor.gameObject);
        DisplayActor.State.Data.Looks = new ActorLooks();

        DisplayActor.State.Data.Looks.IsFemale = false;
        DisplayActor.State.Data.Looks.Ears  = DefaultEars[0].name;
        DisplayActor.State.Data.Looks.Eyes  = DefaultEyes[0].name;
        DisplayActor.State.Data.Looks.Hair  = DefaultHair[0].name;
        DisplayActor.State.Data.Looks.Nose  = DefaultNose[0].name;
        DisplayActor.State.Data.Looks.Mouth = DefaultMouth[0].name;
        DisplayActor.State.Data.Looks.Iris  = DefaultIris[0].name;
        DisplayActor.State.Data.Looks.Eyebrows = DefaultEyebrows[0].name;

        DisplayActor.State.Data.Looks.SkinColor = "#" + ColorUtility.ToHtmlStringRGB(DefaultSkinColor);
        DisplayActor.State.Data.Looks.HairColor = "#" + ColorUtility.ToHtmlStringRGB(DefaultHairColor);

        DisplayActor.RefreshLooks();
    }

    public void Randomize()
    {
        
    }

    public void SetGender(bool isFemale)
    {
        DisplayActor.State.Data.Looks.IsFemale = isFemale;
        DisplayActor.RefreshLooks();
    }

    public void SetSkinColor(string clr)
    {
        DisplayActor.State.Data.Looks.SkinColor = clr;
        DisplayActor.RefreshLooks();
    }

    public void SetHairColor(string clr)
    {
        DisplayActor.State.Data.Looks.HairColor = clr;
        DisplayActor.RefreshLooks();
    }

    public void NextHair()
    {
        int index = DefaultHair.IndexOf(DefaultHair.Find(x=>x.name == DisplayActor.State.Data.Looks.Hair))+1;

        if(index >= DefaultHair.Count)
        {
            index = 0;
        }

        DisplayActor.State.Data.Looks.Hair = DefaultHair[index].name;
        DisplayActor.RefreshLooks();
    }

    public void PreviousHair()
    {
        int index = DefaultHair.IndexOf(DefaultHair.Find(x => x.name == DisplayActor.State.Data.Looks.Hair))-1;

        if (index < 0)
        {
            index = DefaultHair.Count - 1;
        }

        DisplayActor.State.Data.Looks.Hair = DefaultHair[index].name;
        DisplayActor.RefreshLooks();
    }

    public void NextEyebrows()
    {
        int index = DefaultEyebrows.IndexOf(DefaultEyebrows.Find(x => x.name == DisplayActor.State.Data.Looks.Eyebrows)) + 1;

        if (index >= DefaultEyebrows.Count)
        {
            index = 0;
        }

        DisplayActor.State.Data.Looks.Eyebrows = DefaultEyebrows[index].name;
        DisplayActor.RefreshLooks();
    }

    public void PreviousEyebrows()
    {
        int index = DefaultEyebrows.IndexOf(DefaultEyebrows.Find(x => x.name == DisplayActor.State.Data.Looks.Eyebrows)) - 1;

        if (index < 0)
        {
            index = DefaultEyebrows.Count - 1;
        }

        DisplayActor.State.Data.Looks.Eyebrows = DefaultEyebrows[index].name;
        DisplayActor.RefreshLooks();
    }

    public void NextEyes()
    {
        int index = DefaultEyes.IndexOf(DefaultEyes.Find(x => x.name == DisplayActor.State.Data.Looks.Eyes)) + 1;

        if (index >= DefaultEyes.Count)
        {
            index = 0;
        }

        DisplayActor.State.Data.Looks.Eyes = DefaultEyes[index].name;
        DisplayActor.RefreshLooks();
    }

    public void PreviousEyes()
    {
        int index = DefaultEyes.IndexOf(DefaultEyes.Find(x => x.name == DisplayActor.State.Data.Looks.Eyes)) - 1;

        if (index < 0)
        {
            index = DefaultEyes.Count - 1;
        }

        DisplayActor.State.Data.Looks.Eyes = DefaultEyes[index].name;
        DisplayActor.RefreshLooks();
    }

    public void NextIris()
    {
        int index = DefaultIris.IndexOf(DefaultIris.Find(x => x.name == DisplayActor.State.Data.Looks.Iris)) + 1;

        if (index >= DefaultIris.Count)
        {
            index = 0;
        }

        DisplayActor.State.Data.Looks.Iris = DefaultIris[index].name;
        DisplayActor.RefreshLooks();
    }

    public void PreviousIris()
    {
        int index = DefaultIris.IndexOf(DefaultIris.Find(x => x.name == DisplayActor.State.Data.Looks.Iris)) - 1;

        if (index < 0)
        {
            index = DefaultIris.Count - 1;
        }

        DisplayActor.State.Data.Looks.Iris = DefaultIris[index].name;
        DisplayActor.RefreshLooks();
    }

    public void NextNose()
    {
        int index = DefaultNose.IndexOf(DefaultNose.Find(x => x.name == DisplayActor.State.Data.Looks.Nose)) + 1;

        if (index >= DefaultNose.Count)
        {
            index = 0;
        }

        DisplayActor.State.Data.Looks.Nose = DefaultNose[index].name;
        DisplayActor.RefreshLooks();
    }

    public void PreviousNose()
    {
        int index = DefaultNose.IndexOf(DefaultNose.Find(x => x.name == DisplayActor.State.Data.Looks.Nose)) - 1;

        if (index < 0)
        {
            index = DefaultNose.Count - 1;
        }

        DisplayActor.State.Data.Looks.Nose = DefaultNose[index].name;
        DisplayActor.RefreshLooks();
    }

    public void NextMouth()
    {
        int index = DefaultMouth.IndexOf(DefaultMouth.Find(x => x.name == DisplayActor.State.Data.Looks.Mouth)) + 1;

        if (index >= DefaultMouth.Count)
        {
            index = 0;
        }

        DisplayActor.State.Data.Looks.Mouth = DefaultMouth[index].name;
        DisplayActor.RefreshLooks();
    }

    public void PreviousMouth()
    {
        int index = DefaultMouth.IndexOf(DefaultMouth.Find(x => x.name == DisplayActor.State.Data.Looks.Mouth)) - 1;

        if (index < 0)
        {
            index = DefaultMouth.Count-1;
        }

        DisplayActor.State.Data.Looks.Mouth = DefaultMouth[index].name;
        DisplayActor.RefreshLooks();
    }

    public void NextEars()
    {
        int index = DefaultEars.IndexOf(DefaultEars.Find(x => x.name == DisplayActor.State.Data.Looks.Ears)) + 1;

        if (index >= DefaultEars.Count)
        {
            index = 0;
        }

        DisplayActor.State.Data.Looks.Ears = DefaultEars[index].name;
        DisplayActor.RefreshLooks();
    }

    public void PreviousEars()
    {
        int index = DefaultEars.IndexOf(DefaultEars.Find(x => x.name == DisplayActor.State.Data.Looks.Ears)) - 1;

        if (index < 0)
        {
            index = DefaultEars.Count-1;
        }

        DisplayActor.State.Data.Looks.Ears = DefaultEars[index].name;
        DisplayActor.RefreshLooks();
    }
}
