using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Alive : NetworkBehaviour {

    public bool Dead;
    public bool CanRevive;
    public event UponDeathDel UponDeath;
    public delegate void UponDeathDel();

    [SerializeField]
    private float Health = 100f;

    [SerializeField]
    private float MaxHealth = 100f;

    public virtual void Damage(float damage)
    {
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
