﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(sendInterval = 0.05f)]
public class GunAnimation : NetworkBehaviour
{
    public string Shoot = "Shoot";
    public string Reload = "Reload";
    public string Chamber = "Chamber";
    public string Aim = "Aim";
    public string Run = "Run";
    public string Dropped = "Dropped";

    [Tooltip("Is this on a gun that has a repeated reload action, such as a shotgun or bolt action rifle?")]
    public bool IsRecursive = false;

    [HideInInspector] [SyncVar] public bool IsDropped;
    [HideInInspector] [SyncVar] public bool IsAiming;
    [HideInInspector] [SyncVar] public bool IsRunning;
    [HideInInspector] [SyncVar] public bool IsShooting;
    [HideInInspector] [SyncVar] public bool IsRecursiveReload;
    [HideInInspector] public bool IsEquipping = true; // Only true when the object is created, then false forever more!
    [HideInInspector] public bool IsReloading;
    [HideInInspector] public bool IsChambering;

    private Animator animator;
    private Gun gun;
    private Item item;

    public override void PreStartClient()
    {
        base.PreStartClient();
        Start();
    }

    public void Start()
    {
        animator = GetComponentInChildren<Animator>();
        gun = GetComponent<Gun>();
        item = GetComponent<Item>();
    }

    public void Update()
    {
        // Apply animation vars
        animator.SetBool(Run, IsRunning);
        animator.SetBool(Dropped, IsDropped);
        animator.SetBool(Aim, IsAiming);
        animator.SetBool(Shoot, IsShooting);

        if (IsRecursive)
        {
            animator.SetBool(Reload, IsRecursiveReload);
        }

        Hand.SetEquipHandRender(transform, !IsDropped);

        if (!isServer)
            return;

        AnimDropped(!item.IsEquipped());
    }

    public void CancelAnimation(string trigger)
    {
        animator.ResetTrigger(trigger);
    }

    public void AnimRun(bool run)
    {
        if (IsRunning != run)
        {
            CmdSetRun(run);
        }
    }

    public void AnimDropped(bool dropped)
    {
        if(IsDropped != dropped)
        {
            CmdSetDropped(dropped);
        }
    }

    public void AnimShoot(bool shooting)
    {
        if (IsShooting != shooting)
        {
            CmdSetShoot(shooting);
        }
    }

    public void AnimAim(bool aim)
    {
        if (IsAiming != aim)
        {
            CmdSetAim(aim);
        }
    }

    [Command]
    private void CmdSetRun(bool run)
    {
        this.IsRunning = run;
    }

    [Command]
    private void CmdSetDropped(bool dropped)
    {
        this.IsDropped = dropped;
    }

    [Command]
    private void CmdSetShoot(bool shoot)
    {
        IsShooting = shoot;
    }

    [Command]
    private void CmdSetAim(bool aim)
    {
        IsAiming = aim;
    }

    [Command] // FOR RECURSIVE RELOADS ONLY, such as shotguns or old rifles.
    private void CmdSetReload(bool reload)
    {
        IsRecursiveReload = reload;
    }

    public void AnimChamber()
    {
        // Cause chamber animation.
        if (!hasAuthority)
        {
            Debug.LogError("No authority to cause animation!");
            return;
        }
        if (IsChambering)
        {
            // Already reloading!
            Debug.LogError("Already chambering!");
            return;
        }
        if (IsAiming)
        {
            Debug.LogError("Aiming, cannot chamber!");
            return;
        }
        if (IsEquipping)
        {
            Debug.LogError("Equipping, cannot chamber!");
            return;
        }

        CmdTrigger(Chamber); // For other clients only!

        // Cause local animation.
        IsChambering = true;
        animator.SetTrigger(Chamber);
    }
    
    public void AnimReload()
    {
        // Cause reload animation.
        if (!hasAuthority)
        {
            Debug.LogError("No authority to cause animation!");
            return;
        }
        if (IsReloading)
        {
            // Already reloading!
            Debug.LogError("Already reloading!");
            return;
        }
        if (IsAiming)
        {
            Debug.LogError("Aiming, cannot reload!");
            return;
        }
        if(IsEquipping)
        {
            Debug.LogError("Equipping, cannot reload!");
            return;
        }

        if (!IsRecursiveReload)
        {
            CmdTrigger(Reload); // For other clients only!

            // Cause local animation.
            animator.SetTrigger(Reload);
        }
        else
        {
            AnimReload(true); // Set the recursive state.
            animator.SetBool(Reload, true); // Apply local state.
        }
        IsReloading = true;
    }

    // For recursive reload only!
    public void AnimReload(bool reload)
    {
        if (IsRecursiveReload != reload)
            CmdSetReload(reload);
    }

    [Command]
    public void CmdTrigger(string name)
    {
        RpcTrigger(name);
    }

    [ClientRpc]
    public void RpcTrigger(string name)
    {
        // If is owner of weapon, return. Animation has already been caused.
        if (hasAuthority)
            return;
        animator.SetTrigger(name);
    }

    public void CallbackReloadEnd()
    {
        if (!hasAuthority)
            return;

        IsReloading = false;

        gun.Shooting.FromAnimReload();
    }

    public void CallbackChamberEnd()
    {
        if (!hasAuthority)
            return;

        IsChambering = false;

        gun.Shooting.FromAnimChamber();
    }

    public void CallbackShoot()
    {
        // NOTE: We continue even with no authority!

        // Pew pew!
        gun.Shooting.FromAnimShoot();
    }

    public void CallbackEquipEnd()
    {
        if (!hasAuthority)
            return;

        IsEquipping = false;
    }
}