using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationCallbacks : MonoBehaviour {

    public AIMelee Melee
    {
        get
        {
            if (melee == null)
                melee = GetComponentInParent<AIMelee>();
            return melee;
        }
    }
    private AIMelee melee;

    public void DeathFinished()
    {

    }

    public void MeleeAttack()
    {
        Melee.Attack();
    }
}
