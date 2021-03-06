using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateCharacterPanelUI : MonoBehaviour
{
    public static CreateCharacterPanelUI Instance;

    [SerializeField]
    Actor DisplayActor;

    public SkinSet DefaultEars;
    public SkinSet DefaultEyes;
    public SkinSet DefaultHair;
    public SkinSet DefaultNose;
    public SkinSet DefaultMouth;

    public Color DefaultSkinColor;
    public Color DefaultHairColor;
    public Color DefaultEyeColor;



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
        DisplayActor.State.Data.Looks.Ears  = DefaultEars.name;
        DisplayActor.State.Data.Looks.Eyes  = DefaultEyes.name;
        DisplayActor.State.Data.Looks.Hair  = DefaultHair.name;
        DisplayActor.State.Data.Looks.Nose  = DefaultNose.name;
        DisplayActor.State.Data.Looks.Mouth = DefaultMouth.name;

        DisplayActor.State.Data.Looks.SkinColor = "#" + ColorUtility.ToHtmlStringRGB(DefaultSkinColor);
        DisplayActor.State.Data.Looks.HairColor = "#" + ColorUtility.ToHtmlStringRGB(DefaultHairColor);
        DisplayActor.State.Data.Looks.EyeColor  = "#" + ColorUtility.ToHtmlStringRGB(DefaultEyeColor);

        DisplayActor.RefreshLooks();
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

}
