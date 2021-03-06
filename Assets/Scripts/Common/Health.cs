﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(sendInterval = 0.05f)]
public class Health : NetworkBehaviour {

    /*
    * This class represents a object that can be damaged/hurt and destroyed/killed.
    * The script executes almost exclusively on the server. Most vars are SyncVars that contain data like health and
    * max health.
    */

    [SerializeField]
    [SyncVar(hook = "HealthChange")]
    private float health = 100;
    [SyncVar]
    [SerializeField]
    private float maxHealth = 100;
    [SyncVar]
    public bool CanRevive = false;
    [Tooltip("If false this object is never shot through. If true, it may be shot through depending on the gun type.")]
    public bool CanPenetrate = true; // TODO MAKE IN GUN SCRIPT!!!!
    [SyncVar]
    public bool CanHit = true;

    public bool SpawnDamageNumbers = true;

    // START SERVER ONLY
    private string source;
    private string secondary;
    private float timeSinceSource;
    private float timeSinceSecondary;
    private bool hasBeenDead;
    // END SERVER ONLY

    public List<Collider2D> CannotHit = new List<Collider2D>();
    public DED UponDeath;
    public DED UponDeathServer;
    public delegate void DED(); // Death event delegate

    [Command]
    public void CmdSetMaxHealth(float health)
    {
        Server_SetMaxHealth(health);
    }

    [Server]
    public void Server_SetMaxHealth(float health)
    {
        if (health <= 0)
            return;
        this.maxHealth = health;

        if (this.health > maxHealth)
            this.Server_SetHealth(maxHealth);
    }

    [Server]
    public void Server_SetHealth(float health)
    {
        this.health = health;
        if (this.health < 0)
        {
            this.health = 0;
            // This is where the player dies.
            // At this point, we want to notify the local object that owns this, (the one with authority).
        }
        if (this.health > maxHealth)
            this.health = maxHealth;

        //Debug.Log("Set health to " + this.health + ".");
    }

    [Command]
    public void CmdDamage(float damage, string dealer, bool isSecondary)
    {
        ServerDamage(damage, dealer, isSecondary);
    }

    [Server]
    public void ServerDamage(float damage, string dealer, bool isSecondary)
    {
        if (!CanHit)
            return;

        Server_SetHealth(health - Mathf.Abs(damage));

        if (!isSecondary)
        {
            this.timeSinceSource = 0;
            this.source = dealer;
        }
        else
        {
            this.secondary = dealer;
            this.timeSinceSecondary = 0;
        }
    }

    [Command]
    public void CmdHeal(float health)
    {
        health = Mathf.Abs(health);
        if(health == 0)
        {
            return;
        }
        if (this.health <= 0)
        {
            if (!CanRevive)
            {
                Debug.LogWarning("Creature " + gameObject.name + " cannot be revived, but has attempted to heal for " + health + " health.");
                return;
            }
            else
            {
                // Revive
                hasBeenDead = false;
                // Simply allow the heal.
            }            
        }
        Server_SetHealth(this.health + health);
    }

    [Command]
    public void CmdSetCanHit(bool canHit)
    {
        this.CanHit = canHit;
    }

    [Server]
    private void DeadEventServer(string source, string secondary, float timeSource, float timeSecondary)
    {
        if(UponDeathServer != null)
            UponDeathServer();
        RpcDeadEventClient(source, secondary, timeSource, timeSecondary);
    }

    [ClientRpc]
    private void RpcDeadEventClient(string source, string secondary, float timeSource, float timeSecondary)
    {
        if (hasAuthority) // If we own or have authority over this object...
        {
            DeadEvent(source, secondary, timeSource, timeSecondary);
        }
    }

    [Client]
    private void DeadEvent(string source, string secondary, float timeSource, float timeSecondary)
    {
        // Assign local values.

        this.source = source;
        this.secondary = secondary;
        this.timeSinceSource = timeSource;
        this.timeSinceSecondary = timeSecondary;

        if(UponDeath != null)
            UponDeath();
    }

    public void Update()
    {
        if (!isServer)        
            return;        

        if (IsDead)
        {
            // Assume dead, check if is first frame dead...
            health = 0;
            if (!hasBeenDead)
            {
                hasBeenDead = true;
                // Dead message
                // Send message to client who has authority
                this.DeadEventServer(source, secondary, timeSinceSource, timeSinceSecondary);
            }
        }
    }

    public bool IsDead
    {
        get
        {
            return health <= 0f;
        }
    }

    private void HealthChange(float newHealth)
    {
        float change = newHealth - health;
        // For example 80 - 100 gives -20.

        health = newHealth;

        if (SpawnDamageNumbers)
        {
            if(change < 0)
            {
                DamageNumber.Spawn(transform.position, change * -1);
            }
        }
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetHealth()
    {
        return health;
    }

    public float GetHealthPercentage()
    {
        return GetHealth() / GetMaxHealth();
    }

    public bool IsActiveKiller()
    {
        return string.IsNullOrEmpty(source) ? false : source.Contains(":");
    }

    public string GetKiller()
    {
        if (string.IsNullOrEmpty(source))
        {
            return null;
        }
        if (source.Contains(":"))
        {
            return source.Split(':')[0].Trim();
        }
        else
        {
            return source.Trim();
        }
    }

    public string GetKillerItem()
    {
        if (string.IsNullOrEmpty(source))
        {
            return null;
        }
        if (source.Contains(":"))
        {
            return source.Split(':')[1].Trim();
        }
        else
        {
            return source.Trim();
        }
    }

    public string GetDamageReport(string status)
    {
        string s = "";

        if(!string.IsNullOrEmpty(secondary))
        {
            s += status + " by " + secondary + " ";
            float interval = timeSinceSource - timeSinceSecondary;
            if(source != null && interval > 0)
                s += interval + " seconds after ";
        }

        if(string.IsNullOrEmpty(source))
        {
            if (string.IsNullOrEmpty(secondary))
                s += status + " by voodoo magic.";
            else
                s += " absolutely nothing hit them.";
        }
        else
        {
            bool isPlayerAttack = source.Contains(":");

            if (isPlayerAttack)
            {
                string[] data = source.Split(':');
                string player = data[0];
                string weapon = data[1];

                if(!string.IsNullOrEmpty(secondary))
                {
                    // For example 'XYZ killed by bleeding 6 seconds after ASD hit them with Sharp Stick.'
                    s += player + " hit them with " + weapon + ".";
                }
                else
                {
                    s += status + " by " + player + " using " + weapon + ".";
                }
            }
            else
            {
                s += status + " by " + source + ".";
            }
        }

        return s;
    }

    /// <summary>
    /// Can the gameobject that owns the collider be damaged? May not be possible to hit it using bullets, check Health.CanHitObject().
    /// </summary>
    public static bool CanDamageObject(Collider2D collider, string friendlyTeam)
    {
        Health h = collider.GetComponentInParent<Health>();

        if (h != null)
        {
            if (!h.CanHit)
                return false;

            if (h.CannotHit.Contains(collider))
            {
                return false;
            }

            if (collider.gameObject.GetComponentInParent<Item>() != null)
            {
                return false;
            }

            if (collider.GetComponent<NeverHitMe>() != null)
            {
                return false;
            }

            Player p = collider.GetComponent<Player>();
            if (p != null)
            {
                if(string.IsNullOrEmpty(friendlyTeam))
                {
                    // Takes no sides, will always damage the player.
                    return true;
                }

                if (Teams.I.PlayerInTeam(p.Name, friendlyTeam))
                {
                    // Player is in the same team as me.
                    return false; // DO NOT ALLOW TEAM DAMAGE. TODO IMPLEMENT GAME RULES!!!
                }
                else
                {
                    // Fair game.
                    return true;
                }
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Checks to see if the gameobject that owns the collider can be HIT by a bullet, or if the bullet should sail right though it without affecting it.
    /// If this returns true, the bullet should treat the object as SOLID, but it may not be possible to DAMAGE it. See Health.CanDamageObject().
    /// </summary>
    public static bool CanHitObject(Collider2D collider, string friendlyTeam)
    {
        Health h = collider.GetComponentInParent<Health>();

        if (h != null)
        {
            if (!h.CanHit)
                return false;
            if (h.CannotHit.Contains(collider))
            {
                return false;
            }
            if (collider.gameObject.GetComponentInParent<Item>() != null)
            {
                return false;
            }
            if (collider.GetComponent<NeverHitMe>() != null)
            {
                return false;
            }
            Player p = collider.GetComponent<Player>();
            if (p != null)
            {
                if(string.IsNullOrEmpty(friendlyTeam))
                {
                    // This gun/turret/weapon takes no sides! They will hit any player!
                    return true;
                }

                if (Teams.I.PlayerInTeam(p.Name, friendlyTeam))
                {
                    return false; // DO NOT ALLOW TEAM DAMAGE. TODO IMPLEMENT GAME RULES!!!
                }
            }
        }
        else
        {
            if (collider.isTrigger)
            {
                // Is a trigger that does not have health, do not hit it.
                return false;
            }
            else
            {
                // Solid collider...
                return true;
            }
        }

        return true;
    }
}