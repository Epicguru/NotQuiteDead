using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AIMelee : MeleeAttack
{
    public bool IsActive = true;
    public string EnemyName;
    public string WeaponID;

    public override bool Ready()
    {
        return true;
    }

    public override bool Active()
    {
        return this.IsActive;
    }

    public override bool DoDisabling()
    {
        return false;
    }

    public override bool AsPlayer()
    {
        return false;
    }

    public override string GetAttacker()
    {
        return EnemyName;
    }

    public override string GetWeaponID()
    {
        return WeaponID;
    }

    public override bool TeamDamageSystemEnabled()
    {
        return false;
    }
}
