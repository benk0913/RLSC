using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkinSet", menuName = "Data/SkinSet", order = 2)]
[Serializable]
public class SkinSet : ScriptableObject
{
    public BodyPart Part;

    [PreviewSprite]
    public Sprite TargetSprite;

    [PreviewSprite]
    public Sprite TargetSpriteFemale;

    public bool BareSkin = false;

    public bool Hair = false;


    public Sprite GetSprite(ActorData fromData)
    {
        if(fromData == null || fromData.looks == null)
        {
            return TargetSprite;
        }

        if(fromData.looks.IsFemale)
        {
            if(TargetSpriteFemale == null)
            {
                return TargetSprite;
            }

            return TargetSpriteFemale;
        }

        return TargetSprite;
    }

    public List<SkinSet> BundledSkins = new List<SkinSet>();
}