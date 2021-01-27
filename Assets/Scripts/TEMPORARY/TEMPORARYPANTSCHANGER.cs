using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEMPORARYPANTSCHANGER : MonoBehaviour
{
    public Sprite FireClassSpriteLeft;
    public Sprite FireClassSpriteRight;
    public Sprite FireClassSpriteCrotch;

    public Sprite WaterClassSpriteLeft;
    public Sprite WaterClassSpriteRight;
    public Sprite WaterClassSpriteCrotch;


    public Sprite EarthClassSpriteLeft;
    public Sprite EarthClassSpriteRight;
    public Sprite EarthClassSpriteCrotch;

    public Sprite AirClassSpriteLeft;
    public Sprite AirClassSpriteRight;
    public Sprite AirClassSpriteCrotch;


    public SpriteRenderer Crotch;
    public SpriteRenderer LeftHip;
    public SpriteRenderer RightHip;

    public ClassJob FireClass;
    public ClassJob AirClass;
    public ClassJob WaterClass;
    public ClassJob EarthClass;

    public Actor actor;

    private void OnEnable()
    {

        CORE.Instance.DelayedInvokation(2f, () => 
        {
            if (actor.State.Data.ClassJobReference == FireClass)
            {
                Crotch.sprite = FireClassSpriteLeft;
                LeftHip.sprite = FireClassSpriteLeft;
                RightHip.sprite = FireClassSpriteRight;
            }
            else if (actor.State.Data.ClassJobReference == WaterClass)
            {
                Crotch.sprite = WaterClassSpriteCrotch;
                LeftHip.sprite = WaterClassSpriteLeft;
                RightHip.sprite = WaterClassSpriteRight;
            }
            else if (actor.State.Data.ClassJobReference == EarthClass)
            {
                Crotch.sprite = EarthClassSpriteCrotch;
                LeftHip.sprite = EarthClassSpriteLeft;
                RightHip.sprite = EarthClassSpriteRight;
            }
            else if (actor.State.Data.ClassJobReference == AirClass)
            {
                Crotch.sprite = AirClassSpriteCrotch;
                LeftHip.sprite = AirClassSpriteLeft;
                RightHip.sprite = AirClassSpriteRight;
            }
        });
        
    }
}
