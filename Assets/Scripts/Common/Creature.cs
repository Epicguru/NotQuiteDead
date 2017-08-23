using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(sendInterval = 0.05f)]
public class Creature : NetworkBehaviour {

    /*
    * This class represents a object that can be damaged/hurt and destroyed/killed.
    * The script executes almost exclusively on the server. Most vars are SyncVars that contain data like health and
    * max health.
    */

    [SerializeField]
    [SyncVar]
    private float Health = 100;
    [SyncVar]
    [SerializeField]
    private float MaxHealth = 100;
    [SyncVar]
    public bool CanRevive = false;

    // START SERVER ONLY
    private string source;
    private string secondary;
    private float timeSinceSource;
    private float timeSinceSecondary;
    private bool hasBeenDead;
    // END SERVER ONLY

    public DED UponDeath;
    public delegate void DED(); // Death event delegate

    [Command]
    public void CmdSetMaxHealth(float health)
    {
        if (health <= 0)
            return;
        this.MaxHealth = health;

        if (this.Health > MaxHealth)
            this.SetHealth(MaxHealth);
    }

    [Server]
    private void SetHealth(float health)
    {
        Health = health;
        if (Health < 0)
        {
            Health = 0;
            // This is where the player dies.
            // At this point, we want to notify the local object that owns this, (the one with authority).
            hasBeenDead = false; // This does not represent the dead state, but it is used in callbacks.
        }
        if (Health > MaxHealth)
            Health = MaxHealth;

        Debug.Log("Set health to " + Health + ".");
    }

    [Command]
    public void CmdDamage(float damage, string dealer, bool isSecondary)
    {
        SetHealth(Health - Mathf.Abs(damage));

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
        if (Health <= 0)
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
        SetHealth(Health + health);
    }

    [Server]
    private void DeadEventServer(string source, string secondary, float timeSource, float timeSecondary)
    {
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
        {
            return;
        }

        if (Health <= 0)
        {
            // Assume dead, check if is first frame dead...
            Health = 0;
            if (!hasBeenDead)
            {
                hasBeenDead = true;
                // Dead message
                // Send message to client who has authority
                this.DeadEventServer(source, secondary, timeSinceSource, timeSinceSecondary);
            }
        }
    }

    public float GetMaxHealth()
    {
        return MaxHealth;
    }

    public float GetHealth()
    {
        return Health;
    }

    public float GetHealthPercentage()
    {
        return GetHealth() / GetMaxHealth();
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
}