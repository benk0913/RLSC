using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ActorSkin : MonoBehaviour
{
    [SerializeField]
    public Actor Act;

    [SerializeField]
    public List<RendererPart> SkinParts = new List<RendererPart>();

    [SerializeField]
    SpriteRenderer Halo;


    [SerializeField]
    Image NameTagImage;


    [SerializeField]
    public List<GameObject> OrbEffects = new List<GameObject>();

    Emote CurrentEmote;

    public Coroutine EmoteRoutineInstance;

    public List<SpriteRenderer> PartsHiddenByEmote = new List<SpriteRenderer>();

    public Animator BlinkAnimer;

    float timeTillBlink = 4f;

    void Update()
    {
        if(timeTillBlink > 0f || EmoteRoutineInstance != null)
        {
            timeTillBlink -= Time.deltaTime;
        }
        else
        {
            timeTillBlink = Random.Range(3f,6f);
            BlinkAnimer.SetTrigger("Blink");
        }
        
    }
    void OnDisable()
    {
        EmoteRoutineInstance = null;
    }

    public void RefreshLooks()
    {
        ActorLooks looks = Act.State.Data.looks;

        SetDefaultSkin();

        SetSkin(looks.Hair);
        SetSkin(looks.Ears);
        SetSkin(looks.Eyes);
        SetSkin(looks.Nose);
        SetSkin(looks.Mouth);
        SetSkin(looks.Iris);
        SetSkin(looks.Eyebrows);

        
        ClassJob job = CORE.Instance.Data.content.Classes.Find(x => x.name == Act.State.Data.classJob);

        if (job != null && Halo != null)
        {
            Halo.color = new Color(job.ClassColor.r, job.ClassColor.g, job.ClassColor.b, 1f);
        }

        List<Item> cashShopItems = new List<Item>();
        List<Item> normalItems = new List<Item>();
        for(int i=0;i<Act.State.Data.equips.Keys.Count;i++)
        {
            string key = Act.State.Data.equips.Keys.ElementAt(i);
            Item equip = Act.State.Data.equips[key];

            if(equip == null || equip.Data == null)
            {
                continue;
            }
        
            equip.equipKey = key;

            if(equip.Data.CashShopItem)
            {
                cashShopItems.Add(equip);
            }
            else
            {
                normalItems.Add(equip);
            }

            if(equip.Data.Type.name == "Name Tag")
            {
                NameTagImage.sprite = equip.Data.Icon;
            }


        }

        if(!Act.State.Data.equips.ContainsKey("Name Tag"))
        {
            NameTagImage.sprite = CORE.Instance.Data.content.DefaultNameTag.Icon;
        }

        foreach(Item overrideItem in cashShopItems)
        {
            foreach(ItemType type in overrideItem.Data.HidingItemTypes)
            {
                normalItems.RemoveAll(x=>x.Data.Type == type);
            }
        }

        foreach(Item normalItem in normalItems)
        {
            foreach(NSkinSet set in normalItem.Data.NewSkinOverride)
                {   
                    NSkinSet typeBasedOverride = normalItem.Data.SkinTypeOverride(normalItem.equipKey);

                    if (typeBasedOverride != null)
                    {
                        SetSkin(typeBasedOverride);
                    }
                    else
                    {
                        SetSkin(set);
                    }
                }

        }

        foreach(Item overrideItem in cashShopItems)
        {
            foreach(NSkinSet set in overrideItem.Data.NewSkinOverride)
                {   
                    NSkinSet typeBasedOverride = overrideItem.Data.SkinTypeOverride(overrideItem.equipKey);


                    if (typeBasedOverride != null)
                    {
                        SetSkin(typeBasedOverride);
                    }
                    else
                    {
                        SetSkin(set);
                    }
                }
        }


        if (!string.IsNullOrEmpty(looks.SkinColor))
        {
            Color relevantColor = Color.clear;
            if (ColorUtility.TryParseHtmlString(looks.SkinColor, out relevantColor))
            {
                foreach (RendererPart part in SkinParts)
                {
                    if ((part.CurrentSkinSet != null && part.CurrentSkinSet.BareSkin) || (part.NCurrentSkinSet != null && part.NCurrentSkinSet.BareSkin))
                    {
                        SetSkinColor(part, relevantColor);
                    }
                    else
                    {
                        SetSkinColor(part, Color.white);
                    }
                }
            }
            else
            {
                CORE.Instance.LogMessageError("Could not parse color - " + looks.SkinColor);
            }
        }
        
        if (!string.IsNullOrEmpty(looks.SkinColor))
        {
            Color relevantColor = Color.clear;
            if (ColorUtility.TryParseHtmlString(looks.HairColor, out relevantColor))
            {
                foreach (RendererPart part in SkinParts)
                {
                    if (part.CurrentSkinSet == null || !part.CurrentSkinSet.Hair)
                    {
                        continue;
                    }

                    SetSkinColor(part, relevantColor);
                }
                
            }
            else
            {
                CORE.Instance.LogMessageError("Could not parse color - " + looks.SkinColor);
            }
        }

        
        RefreshOrbs();

        
    }

    public void RefreshOrbs()
    {
        if(Act.IsDisplayActor)
        {
            return;
        }

        while(OrbEffects.Count > 0)
        {
            OrbEffects[0].SetActive(false);
            OrbEffects.RemoveAt(0);
        }

        List<string> orbsAdded = new List<string>();
        foreach(Item orb in Act.State.Data.orbs)
        {
            if(orbsAdded.Contains(orb.itemName))
            {
                continue;
            }
            
            AddOrb(orb);
            orbsAdded.Add(orb.itemName);
        }
    }

    public void AddOrb(Item orb)
    {
        if (!string.IsNullOrEmpty(orb.Data.OrbColliderObject))
        {
            GameObject colliderObj = Act.AddColliderOnPosition(orb.Data.OrbColliderObject);
            colliderObj.GetComponent<OrbCollider>().SetInfo(orb, Act);

        }

        if (orb.OrbMaterial != null)
        {
            Act.spriteColorGroup.SetMaterial(orb.OrbMaterial);
        }
    }

    public void SetSkinColor(string bodypart, string hexColor)
    {
        Color relevantColor = Color.clear;
        if (ColorUtility.TryParseHtmlString(hexColor, out relevantColor))
        {
            SetSkinColor(bodypart, relevantColor);
        }
        else
        {
            CORE.Instance.LogMessageError("Could not parse color - " + hexColor);
        }
    }

    public void SetSkinColor(string bodyPart, Color clr)
    {
        BodyPart part =  CORE.Instance.Data.content.Visuals.BodyParts.Find(x => x.name == bodyPart);

        if(part == null)
        {
            CORE.Instance.LogMessageError("Could not find bodypart - " + bodyPart);
            return;
        }

        SetSkinColor(part, clr);
    }

    public void SetSkinColor(BodyPart part, Color clr)
    {
        RendererPart renderPart = SkinParts.Find(X => X.Part == part);

        if(renderPart == null)
        {
            CORE.Instance.LogMessageError("Could not find render part- " + part.name);
            return;
        }

        SetSkinColor(renderPart, clr);
    }

    public void SetSkinColor(RendererPart renderPart, Color clr)
    {
        renderPart.Renderer.color = clr;
    }

    public void SetSkin(string skinKey)
    {
        if (string.IsNullOrEmpty(skinKey))
        {
            return;
        }

        SkinSet set = CORE.Instance.Data.content.Visuals.SkinSets.Find(X => X.name == skinKey);
        

        if (set == null)
        {
            CORE.Instance.LogMessageError("The skin - " + skinKey + " was not found...");
            return;
        }

        SetSkin(set);
    }

    public void SetSkin(SkinSet set)
    {
        RendererPart renderPart = SkinParts.Find(x => x.Part == set.Part);

        if(renderPart == null)
        {
            CORE.Instance.LogMessageError("The body part - " + set.Part.name + " was not found...");
            return;
        }

        renderPart.SetSkin(set, Act.State.Data);

        foreach (SkinSet bundledSet in set.BundledSkins)
        {
            SetSkin(bundledSet);
        }
    }

      public void SetSkin(NSkinSet set)
    {
        RendererPart renderPart = SkinParts.Find(x => x.Part == set.Part);

        if(renderPart == null)
        {
            CORE.Instance.LogMessageError("The body part - " + set.Part.name + " was not found...");
            return;
        }

        renderPart.SetSkin(set, Act.State.Data);

    }

    public void SetSkin(BodyPart part, Sprite targetSprite)
    {
        RendererPart renderPart = SkinParts.Find(x => x.Part == part);

        if(renderPart == null)
        {
            CORE.Instance.LogMessageError("The body part - " + part.name + " was not found...");
            return;
        }

        renderPart.SetSkin(part,targetSprite);

    }

    public void SetDefaultSkin()//TODO Not very performance friendly, should probably change all of those lists to 1 operation data structures.
    {
        foreach(RendererPart rendererPart in SkinParts)
        {
            SkinSet set = CORE.Instance.Data.content.Visuals.DefaultSkin.Find(X => X.Part == rendererPart.Part);

            //if (set == null)
            //{
            //    rendererPart.Renderer.enabled = false;
            //    return;
            //}

            //rendererPart.Renderer.enabled = true;
            rendererPart.SetSkin(set,Act.State.Data);
        }
    }

    public void SetEmote(Emote emote)
    {
        if(EmoteRoutineInstance != null)
        {
            return;
        }

        EmoteRoutineInstance = StartCoroutine(EmoteRoutine(emote));
    }

    IEnumerator EmoteRoutine(Emote emote)
    {
        RendererPart renderPart = SkinParts.Find(x => x.Part.name == "Emote");

        if(renderPart == null)
        {
            CORE.Instance.LogMessageError("The body part - Emote was not found...");
            yield break;
        }


        renderPart.Renderer.sprite = emote.EmoteGraphic;


        foreach(SpriteRenderer hiddenRenderer in PartsHiddenByEmote)
        {
            hiddenRenderer.enabled = false;
        }

        yield return new WaitForSeconds(3f);

        StopEmote();
    }

    public void StopEmote()
    {
        RendererPart renderPart = SkinParts.Find(x => x.Part.name == "Emote");

        renderPart.Renderer.sprite = null;

        foreach(SpriteRenderer hiddenRenderer in PartsHiddenByEmote)
        {
            hiddenRenderer.enabled = true;
        }

        EmoteRoutineInstance = null;
    }
}

[System.Serializable] 
public class RendererPart
{
    public SpriteRenderer Renderer;
    public BodyPart Part;

    public SkinSet CurrentSkinSet;
    public NSkinSet NCurrentSkinSet;
    
    public void SetSkin(BodyPart part, Sprite targetSprite)
    {
        NCurrentSkinSet = null;
        CurrentSkinSet = null;
        Renderer.sprite = targetSprite;
    }

    public void SetSkin(SkinSet set, ActorData actor)
    {

        NCurrentSkinSet = null;
        CurrentSkinSet = set;

        if (set == null)
        {
            Renderer.sprite = null;
        }
        else
        {
            Renderer.sprite = CurrentSkinSet.GetSprite(actor);
        }
    }

    
    public void SetSkin(NSkinSet set, ActorData actor)
    {
        CurrentSkinSet = null;
        NCurrentSkinSet = set;

        if (set == null)
        {
            Renderer.sprite = null;
        }
        else
        {
            Renderer.sprite = NCurrentSkinSet.GetSprite(actor);
        }

        if (set.BareSkin)
        {
            if (!string.IsNullOrEmpty(actor.looks.SkinColor))
            {
                Color relevantColor = Color.clear;
                if (ColorUtility.TryParseHtmlString(actor.looks.SkinColor, out relevantColor))
                {
                    

                    Renderer.color =  relevantColor;
                }
                else
                {
                    CORE.Instance.LogMessageError("Could not parse color - " + actor.looks.SkinColor);
                }
            }
        }
    }
}