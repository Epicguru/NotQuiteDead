using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(MeleeWeapon))]
public class MeleeAttack : NetworkBehaviour {

    [Tooltip("The collider that detects hits with the melee weapon.")]
    public Collider2D Collider;

    [Tooltip("Information about the damage that this weapon inflicts.")]
    public MeleeDamage Damage;

    private MeleeWeapon weapon;
    private List<Collider2D> touching = new List<Collider2D>();

    public void Start()
    {
        weapon = GetComponent<MeleeWeapon>();

        if(Collider == null)
        {
            Debug.LogError("Collider for attacking with " + weapon.Item.Name + " is null!");
            Debug.Break();
            return;
        }

        // TODO add rigidbody to item for interpolation and physics.
    }  
    
    public void Update()
    {
        if (weapon == null || weapon.Item == null)
            return;

        if (weapon.Item.IsEquipped())
        {
            foreach(Collider2D c in GetComponentsInChildren<Collider2D>())
            {
                if (c == Collider)
                {
                    c.enabled = true;
                }
                else
                {
                    c.enabled = false;
                }

            }
        }
        else
        {
            foreach (Collider2D c in GetComponentsInChildren<Collider2D>())
            {
                if (c == Collider)
                    c.enabled = false;
                else
                    c.enabled = true;
            }
        }
    } 
    
    public void OnTriggerEnter2D(Collider2D c)
    {
        if(weapon.Item.IsEquipped() && hasAuthority && c.GetComponentInParent<Player>() != Player.Local)
            touching.Add(c);
    }

    public void OnTriggerExit2D(Collider2D c)
    {
        if (touching.Contains(c))
            touching.Remove(c);
    }

    public List<Collider2D> GetContacts()
    {
        return this.touching;
    }

    public virtual float GetDamage()
    {
        // Default implementation:
        return Damage.Damage; // The damage value.
    }

    private List<Health> hitCreatures = new List<Health>();
    public void Attack()
    {
        // A local version that attacks all objects touching the attacking collider.
        hitCreatures.Clear();

        Debug.Log("Attacking, there are " + GetContacts().Count + " colliders!");

        foreach(Collider2D coll in GetContacts())
        {
            if (coll == null)
                continue; // Can happen when multi collider object is destroyed.

            Health c = coll.GetComponentInParent<Health>();

            if (c == null)
                continue;

            if (c.CannotHit.Contains(coll))
                continue;

            if (!c.CanHit)
                continue;

            if (coll.gameObject.GetComponentInParent<Item>() != null)
            {
                // Is item, may be held in hands. Ignore.
                continue;
            }

            // Prevent a creature from being hit more that once per swing, if it has multiple colliders.
            if (!hitCreatures.Contains(c))
            {
                hitCreatures.Add(c);

                // Deal damage
                CmdHitCreature(c.gameObject, Player.Local.Name, GetDamage());
            }
        }
    }

    [Command]
    private void CmdHitCreature(GameObject creature, string attacker, float damage)
    {
        // By using a command we do not require client authority over the creature.
        // Good for hacking prevention and that kind of stuff.
        string source = attacker + ':' + weapon.Item.Prefab;
        creature.GetComponent<Health>().ServerDamage(damage, source, false);
    }
}
