using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookRandomizerEntity : MonoBehaviour
{ 

    [SerializeField]
    Actor Act;

    [SerializeField]
    public List<ItemData> randomItems = new List<ItemData>();
    
    void Awake()
    {
        Act.State.Data.equips = new Dictionary<string, Item>();
        Act.State.Data.states = new Dictionary<string, StateData>();
        Act.IsDisplayActor = true;
    }
    void Start()
    {
        Randomize();
    }


    public void Randomize()
    {
        Act.State.Data.looks.IsFemale = (Random.Range(0,2)==0);
        Act.State.Data.looks.Ears = CORE.Instance.Data.content.Visuals.DefaultEars[Random.Range(0,CORE.Instance.Data.content.Visuals.DefaultEars.Count)].name;
        Act.State.Data.looks.Eyes = CORE.Instance.Data.content.Visuals.DefaultEyes[Random.Range(0, CORE.Instance.Data.content.Visuals.DefaultEyes.Count)].name;
        Act.State.Data.looks.Hair = CORE.Instance.Data.content.Visuals.DefaultHair[Random.Range(0, CORE.Instance.Data.content.Visuals.DefaultHair.Count)].name;
        Act.State.Data.looks.Nose = CORE.Instance.Data.content.Visuals.DefaultNose[Random.Range(0, CORE.Instance.Data.content.Visuals.DefaultNose.Count)].name;
        Act.State.Data.looks.Mouth = CORE.Instance.Data.content.Visuals.DefaultMouth[Random.Range(0, CORE.Instance.Data.content.Visuals.DefaultMouth.Count)].name;
        Act.State.Data.looks.Iris = CORE.Instance.Data.content.Visuals.DefaultIris[Random.Range(0, CORE.Instance.Data.content.Visuals.DefaultIris.Count)].name;
        Act.State.Data.looks.Eyebrows = CORE.Instance.Data.content.Visuals.DefaultEyebrows[Random.Range(0, CORE.Instance.Data.content.Visuals.DefaultEyebrows.Count)].name;

        Act.State.Data.looks.SkinColor = 
            "#" + ColorUtility.ToHtmlStringRGB(CORE.Instance.Data.content.Visuals.DefaultSkinColor[Random.Range(0, CORE.Instance.Data.content.Visuals.DefaultSkinColor.Count)]);
        Act.State.Data.looks.HairColor =
            "#" + ColorUtility.ToHtmlStringRGB(CORE.Instance.Data.content.Visuals.DefaultHairColor[Random.Range(0, CORE.Instance.Data.content.Visuals.DefaultHairColor.Count)]);

           
         int items = Random.Range (0,4);
         List<string> KeysAdded = new List<string>();
        
         for(int i=0;i<items;i++)
         {
             Item itm = new Item();
             itm.Data = randomItems[Random.Range(0,randomItems.Count)];

            if(KeysAdded.Contains(itm.Data.Type.name))
                continue;

             KeysAdded.Add(itm.Data.Type.name);
             itm.itemName = itm.Data.name;
            
             Act.State.Data.equips.Add(itm.Data.Type.name,itm);
         }
         Act.RefreshLooks();
    }
}
