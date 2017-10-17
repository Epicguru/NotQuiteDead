using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class AIMelee : NetworkBehaviour
{
    public float Damage = 10;
    public string Dealer = "AI Enemy";
    public string Weapon = "Pointy Stick";
    public float AttackRange = 1f;

    private Enemy E;
    private float distance;

    public void Attack()
    {
        if (E == null)
            E = GetComponent<Enemy>();

        if (E.TargetDirectionProvider.Target == null)
            return;

        distance = Vector2.Distance(E.TargetDirectionProvider.Target.position, transform.position);

        if(distance <= AttackRange)
        {
            Player.Local.NetUtils.CmdDamageHealth(E.TargetDirectionProvider.Target.gameObject, Damage, Dealer + ":" + Weapon, false);
        }
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, AttackRange);
        if (E == null)
            E = GetComponent<Enemy>();
        if (E != null && E.TargetDirectionProvider != null && E.TargetDirectionProvider.Target != null)
        {
            distance = Vector2.Distance(E.TargetDirectionProvider.Target.position, transform.position);
            Debug.DrawLine(transform.position, E.TargetDirectionProvider.Target.position, distance <= AttackRange ? Color.red : Color.yellow);
        }
    }
}
