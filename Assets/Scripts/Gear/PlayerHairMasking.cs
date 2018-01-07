using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHairMasking : MonoBehaviour
{
    public SpriteRenderer Renderer;
    public BodyGear HeadGear;

    public void Update()
    {
        if(HeadGear.GetGearItem() != null)
        {
            Renderer.maskInteraction = HeadGear.GetGearItem().HidesHair ? SpriteMaskInteraction.VisibleInsideMask : SpriteMaskInteraction.None;
        }
        else
        {
            Renderer.maskInteraction = SpriteMaskInteraction.None;
        }
    }
}