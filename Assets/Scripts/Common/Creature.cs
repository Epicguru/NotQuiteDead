using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Creature : NetworkBehaviour {

    public bool Dead;
    public bool CanRevive;
    public event UponDeathDel UponDeath;
    public delegate void UponDeathDel();

    [SerializeField]
    private float Health = 100f;

    [SerializeField]
    private float MaxHealth = 100f;

    private string source;
    private float timeSinceDamage;
    private string secondary;

    public virtual void Damage(float damage, string source, bool isSecondary = false)
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
        }
    }

    public virtual void Heal(float health)
    {
        if (health <= 0)
            return;

        if(CanRevive && Health <= 0 && health >= 0)
        {
            Dead = false; // Un-Die!
        }

        Health += health;

        if (Health > MaxHealth)
            Health = MaxHealth;
    }
}
