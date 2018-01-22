using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[System.Serializable]
public class GunDamage
{
    [Tooltip("The damage per bullet/pellet of the gun.")]
    public float Damage = 20;
    [Tooltip("The maximum range that the gun has. After this distance, the bullets will be unable to hit targets.")]
    public float Range = 10;
    [Tooltip("The minimum/maximum spread, in degrees, that the bullet can have. This is the angle of the 'spread cone', meaning that 90 degrees will mean a right angle spray pattern.")]
    public Vector2 Inaccuracy = new Vector2(5, 10);
    [Tooltip("The curve that is used when calculating final inaccuracy.")]
    public AnimationCurve CurveToInaccuracy = AnimationCurve.Linear(0, 0, 1, 1);
    [Tooltip("The amount of shots that the gun has to make before the maximum inaccuracy is reached.")]
    public int ShotsToInaccuracy = 5;
    [Tooltip("The minimum time in seconds after the gun has shot that the Shots Inaccuracy Cooldown begins to apply.")]
    public float MinIdleTimeToCooldown = 0.2f;
    [Tooltip("The amount of shots to inaccuracy that are removed every second, making the gun more accurate when it is not shooting.")]
    public float ShotsInaccuracyCooldown = 5f;
    [Tooltip("The multiplier of damage at the edge of the range. Damage is interpolated in between using the animation curve.")]
    [Range(0f, 1f)]
    public float DamageFalloff = 1f;
    [Tooltip("The curve that controlls the amount of damage falloff. This is a multiplier, which affects the DamageFalloff variable.")]
    public AnimationCurve DamageCurve = AnimationCurve.Linear(0, 1, 1, 0);
    [Tooltip("The amount of objects/enemies that each bullet of pellet can penetrate before stopping. The damage that each one will deal after penetrating an object can be adjusted using the Penetration Falloff variable.")]
    public int Penetration = 1;
    [Tooltip("The multiplier that affects damage after penetration.")]
    [Range(0f, 1f)]
    public float PenetrationFalloff = 0.5f;
    [Tooltip("If true, if this weapon is a shotgun like weapon, then each individual pellet will share the damage variable, meaning that only if all pellets hit the total value of the Damage variable will be dealt.")]
    public bool BulletsShareDamage = false;
}
