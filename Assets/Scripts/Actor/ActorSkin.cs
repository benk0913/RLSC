using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActorSkin : MonoBehaviour
{
    [SerializeField]
    public Actor Act;

    [SerializeField]
    public List<RendererPart> SkinParts = new List<RendererPart>();

    [SerializeField]
    SpriteRenderer Halo;

    [SerializeField]
    public List<GameObject> OrbEffects = new List<GameObject>();

    Emote CurrentEmote;

    Coroutine EmoteRoutineInstance;

    public List<SpriteRenderer> PartsHiddenByEmote = new List<SpriteRenderer>();

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

        if (!string.IsNullOrEmpty(looks.SkinColor))
        {
            Color relevantColor = Color.clear;
            if (ColorUtility.TryParseHtmlString(looks.SkinColor, out relevantColor))
            {
                foreach (RendererPart part in SkinParts)
                {
                    if (part.CurrentSkinSet == null || !part.CurrentSkinSet.BareSkin)
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

        ClassJob job = CORE.Instance.Data.content.Classes.Find(x => x.name == Act.State.Data.classJob);

        if (job != null && Halo != null)
        {
            Halo.color = new Color(job.ClassColor.r, job.ClassColor.g, job.ClassColor.b, 1f);
        }


        for(int i=0;i<Act.State.Data.equips.Keys.Count;i++)
        {
            string key = Act.State.Data.equips.Keys.ElementAt(i);
            Item equip = Act.State.Data.equips[key];

            if(equip == null || equip.Data == null)
            {
                continue;
            }

            foreach(SkinSet set in equip.Data.SkinOverride)
            {
                SkinSet typeBasedOverride = equip.Data.SkinTypeOverride(key);

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

        foreach(Item orb in Act.State.Data.orbs)
        {
            AddOrb(orb);
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
    

    public void SetSkin(SkinSet set, ActorData actor)
    {
        

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
}