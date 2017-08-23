﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(sendInterval = 0.05f)]
public class Creature : NetworkBehaviour {

    [SyncVar]
    public bool Dead;
    public bool CanRevive;

    public event UponDeathDel UponDeath;
    public delegate void UponDeathDel();

    [SerializeField]
    [SyncVar]
    private float Health = 100f;

    [SerializeField]
    private float MaxHealth = 100f;

    private string source;
    private float timeSinceSource;
    private float timeSinceSecondary;
    private string secondary;
    private bool hasBeenDead;

    [Command]
    public virtual void CmdDamage(float damage, string source, bool isSecondary)
    {
        /*
        * Indicates that the creature should take damage, by supplying the damage amount and source.
        * This class records the time when the damage was dealt, unless isSecondary is true.
        * If it is true then the damage is considered to be a consequence of another type of damage,
        * namely the previous damage. For example bleeding would be secondary damage. If the bleeding kills
        * the creature, then the death will be bleeding but the source will be the last damage dealt,
        * for example an arrow.
        */

        if (damage <= 0)
            return;
        Health -= damage;

        if(Health <= 0)
        {
            Dead = true;
            hasBeenDead = false;
        }

        // Log into damage system
        if (isSecondary)
        {
            timeSinceSecondary = 0;
            secondary = source;
        }
        else
        {
            timeSinceSource = 0;
            this.source = source;
        }

        Debug.Log("Damaged for " + damage + " to give " + Health);
    }

    [Command]
    public virtual void CmdHeal(float health)
    {
        if (Dead && !CanRevive)
            return;

        Health += health;

        if(CanRevive && Dead && Health >= 0)
        {
            Dead = false; // Un-Die!
            hasBeenDead = false;
        }

        if (Health > MaxHealth)
            Health = MaxHealth;

        Debug.Log("Healed for " + health + ", total " + Health);
    }

    public void Update()
    {
        if (!hasAuthority)
            return;

        if (Dead && !hasBeenDead)
        {
            hasBeenDead = true;
            if(UponDeath != null)
                UponDeath();
        }
        timeSinceSource += Time.deltaTime;
        timeSinceSecondary += Time.deltaTime;
    }

    public float GetMaxHealth()
    {
        return this.MaxHealth;
    }

    public float GetHealth()
    {
        return this.Health;
    }

    public float GetHealthPercentage()
    {
        // In a range from 0-1

        return GetHealth() / GetMaxHealth();
    }

    public string GetLatestDamage(bool includeSecondary = false)
    {
        if (includeSecondary)
        {
            if(timeSinceSecondary < timeSinceSource)
            {
                return secondary;
            }
        }
        return source;
    }

    public string GetLatestSecondary()
    {
        return secondary;
    }

    public float GetTimeSinceDamage(bool includeSecondary = false)
    {
        if (includeSecondary)
        {
            if (timeSinceSecondary < timeSinceSource)
            {
                return timeSinceSecondary;
            }
        }
        return timeSinceSource;
    }

    public float GetTimeSinceSecondary()
    {
        return timeSinceSecondary;
    }

    public string GetLatestDamageReport(bool includeSecondary = false)
    {
        return "Damage - " + GetLatestDamage(includeSecondary) + " : " + GetTimeSinceDamage(includeSecondary) + " seconds ago.";
    }

    public string GetDamageReportAsPlayer(string status)
    {
        // Format for killing by player:
        // PlayerName:Weapon

        string final = "";
        bool doneSecondary = false;
        if(GetTimeSinceSecondary() < GetTimeSinceDamage())
        {
            secondary = GetLatestSecondary();
            // Never a player action.

            final += status + " by " + secondary + " " + (timeSinceSecondary - timeSinceSource) + " seconds after ";
            doneSecondary = true;
        }

        source = GetLatestDamage(false);

        // Assume killed by player or AI
        if (string.IsNullOrEmpty(source))
        {
            if (doneSecondary)
            {
                final += " Mr.Nobody attacked them using voodoo magic.";
            }
            else
            {
                final += status + " by voodoo magic.";
            }
            return final;
        }

        if (!source.Contains("'"))
        {
            final += status + " by " + source;
            return final;
        }
        else
        {
            string[] data = source.Split(':');
            string killer = data[0];
            string weapon = data[1];

            if (doneSecondary)
            {
                final += killer + " attacked them using " + weapon + ".";
            }
            else
            {
                final += status + " by " + killer + " using " + weapon + ".";
            }

            return final;
        }
    }
}