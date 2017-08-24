using System;
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

    [HideInInspector] [SyncVar] public bool IsDropped;
    [HideInInspector] [SyncVar] public bool IsAiming;
    [HideInInspector] public bool IsReloading;
    [HideInInspector] public bool IsChambering;
    [HideInInspector] [SyncVar] public bool IsRunning;

    private Animator animator;
    private Gun gun;
    private Item item;

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
        if(IsRunning != run)
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

    public void AnimAim(bool aim)
    {
        if(IsAiming != aim)
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
    private void CmdSetAim(bool aim)
    {
        IsAiming = aim;
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

        CmdTrigger(Chamber); // For other clients only!
        IsChambering = true;
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

        CmdTrigger(Reload); // For other clients only!
        IsReloading = true;
    }

    public void AnimShoot()
    {
        // Cause shooting animation.
        if (!hasAuthority)
        {
            Debug.LogError("No authority to cause animation!");
            return;
        }
        if(!IsAiming)
        {
            Debug.LogError("Not aiming, cannot shoot!");
            return;
        }

        CmdTrigger(Shoot); // For other clients only!

        // Cause local animation.
        animator.SetTrigger(Shoot);
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
}