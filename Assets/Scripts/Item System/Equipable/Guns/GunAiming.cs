using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class GunAiming : NetworkBehaviour
{
    [Tooltip("The time in seconds that the gun takes to rotate to the correct angle.")]
    public float AimTime = 0.5f;
    [Tooltip("The curve that the angle of the weapon takes when rotating to the correct angle.")]
    public AnimationCurve Curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private new GunAnimation animation;

    public void Start()
    {
        animation = GetComponent<GunAnimation>();
    }

    public void Update()
    {
        if (!hasAuthority)
            return;

        bool canAim = !animation.IsReloading && !animation.IsEquipping && !animation.IsChambering;
        animation.AnimAim(InputManager.InputPressed("Aim") && canAim); // Set aiming based on input.
    }
}
