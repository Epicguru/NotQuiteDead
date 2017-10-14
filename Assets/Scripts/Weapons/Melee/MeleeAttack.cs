using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MeleeAttack : NetworkBehaviour
{

    [Tooltip("The collider that detects hits with the melee weapon.")]
    public Collider2D Collider;

    [Tooltip("Information about the damage that this weapon inflicts.")]
    public MeleeDamage Damage;

    private MeleeWeapon weapon;
    public List<Collider2D> touching = new List<Collider2D>();

    private Health Health;

    public void Start()
    {
        weapon = GetComponent<MeleeWeapon>();
        Health = GetComponentInParent<Health>();

        if (Collider == null)
        {
            Debug.LogError("Collider for attacking with " + weapon.Item.Name + " is null!");
            Debug.Break();
            return;
        }

        // TODO add rigidbody to item for interpolation and physics.
    }

    public void Update()
    {
        if (!Ready())
            return;

        if (Active() && DoDisabling())
        {
            foreach (Collider2D c in GetComponentsInChildren<Collider2D>())
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
        else if(DoDisabling())
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

    public virtual bool DoDisabling()
    {
        return true;
    }

    public virtual bool Ready()
    {
        return !(weapon == null || weapon.Item == null);
    }

    public virtual bool Active()
    {
        return weapon.Item.IsEquipped();
    }

    public void FixedUpdate()
    {
        // Called before physics callbacks.
        touching.Clear();
    }

    public void OnTriggerStay2D(Collider2D c)
    {
        if (Ready() && c.GetComponentInParent<Health>() != Health)
            touching.Add(c);
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

    public virtual string GetAttacker()
    {
        return Player.Local.Name;
    }

    public virtual bool TeamDamageSystemEnabled()
    {
        return true;
    }

    public virtual string GetWeaponID()
    {
        return weapon.Item.Prefab;
    }

    public virtual bool AsPlayer()
    {
        return true;
    }

    private List<Health> hitCreatures = new List<Health>();
    public void Attack()
    {
        // A local version that attacks all objects touching the attacking collider.
        hitCreatures.Clear();

        //Debug.Log("Attacking, there are " + GetContacts().Count + " colliders!");

        foreach (Collider2D coll in GetContacts())
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

            if (c.GetComponent<NeverHitMe>() != null)
                continue;

            if (!AsPlayer() && c.GetComponent<Enemy>() != null)
                continue;

            if (coll.gameObject.GetComponentInParent<Item>() != null && coll.GetComponentInParent<Placeable>() == null)
            {
                // Is item, may be held in hands. Ignore.
                continue;
            }

            Player p = coll.GetComponent<Player>();
            if (p != null)
            {
                if (AsPlayer() && p == Player.Local)
                    continue;
                if(AsPlayer() && this.TeamDamageSystemEnabled() && Teams.I.PlayersInSameTeam(p.Name, Player.Local.Name))
                {
                    continue; // DO NOT ALLOW TEAM DAMAGE. TODO IMPLEMENT GAME RULES!!!
                }
            }

            // Prevent a creature from being hit more that once per swing, if it has multiple colliders.
            if (!hitCreatures.Contains(c))
            {
                hitCreatures.Add(c);

                // Deal damage
                CmdHitCreature(c.gameObject, GetAttacker(), GetDamage());
            }
        }
    }

    [Command]
    private void CmdHitCreature(GameObject creature, string attacker, float damage)
    {
        // By using a command we do not require client authority over the creature.
        // Good for hacking prevention and that kind of stuff.
        string source = attacker + ':' + GetWeaponID();
        creature.GetComponent<Health>().ServerDamage(damage, source, false);
    }
}