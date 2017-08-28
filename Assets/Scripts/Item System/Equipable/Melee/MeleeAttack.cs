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
    private Rigidbody2D body;

    public void Start()
    {
        weapon = GetComponent<MeleeWeapon>();
        body = GetComponent<Rigidbody2D>();

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
                    continue;

                c.enabled = false;
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
        if(weapon.Item.IsEquipped() && hasAuthority)
            touching.Add(c);
    }

    public void OnTriggerExit2D(Collider2D c)
    {
        if (weapon.Item.IsEquipped() && hasAuthority)
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

        foreach(Collider2D coll in GetContacts())
        {
            if (coll == null)
                continue; // Can happen when multi collider object is destroyed.
            Health c = coll.GetComponentInParent<Health>();

            if (c == null)
                continue;

            if (!Damage.AllowSelfDamage && c == Player.Local.Creature)
                continue;

            // Prevent a creature from being hit more that once per swing, if it has multiple colliders.
            if (!hitCreatures.Contains(c))
            {
                hitCreatures.Add(c);

                // Deal damage
                CmdHitCreature(c.gameObject, GetDamage());
            }
        }
    }

    [Command]
    private void CmdHitCreature(GameObject creature, float damage)
    {
        // By using a command we do not require client authority over the creature.
        // Good for hacking prevention and that kind of stuff.
        string source = "A Player" + ':' + weapon.Item.Name;
        creature.GetComponent<Health>().CmdDamage(damage, source, false);
    }
}
