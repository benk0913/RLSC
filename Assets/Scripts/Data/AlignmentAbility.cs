using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AlignmentAbility", menuName = "Data/AlignmentAbility", order = 2)]
[Serializable]
public class AlignmentAbility : ScriptableObject
{
    [JsonIgnore]
    [PreviewSprite]
    public Sprite Icon;

    [JsonIgnore]
    [TextArea(3, 6)]
    public string Description;

    public float CD = 1f;

    public List<AbilityParam> OnActivateParams = new List<AbilityParam>();

    [JsonIgnore]
    public bool IsActive
    {
        get
        {
            return CORE.PlayerActor.karma > RequiredKarma;
        }
    }

    [JsonIgnore]
    public int RequiredKarma
    {
        get
        {
            int abilityIndex = -1;
            int maxIndex = -1;
            if(CORE.Instance.Data.content.alignmentData.GoodAbilities.Contains(this))
            {
                abilityIndex = CORE.Instance.Data.content.alignmentData.GoodAbilities.IndexOf(this);
                maxIndex =  CORE.Instance.Data.content.alignmentData.GoodAbilities.Count;
            }
            else if(CORE.Instance.Data.content.alignmentData.EvilAbilities.Contains(this))
            {
                abilityIndex = CORE.Instance.Data.content.alignmentData.EvilAbilities.IndexOf(this);
                maxIndex =  CORE.Instance.Data.content.alignmentData.EvilAbilities.Count;
            }
            else
            {
                return -1;
            }

            float percentage = (float)abilityIndex/(float)maxIndex;

            return Mathf.RoundToInt(CORE.Instance.Data.content.alignmentData.MaxKarma * percentage);
        }
    }

}
