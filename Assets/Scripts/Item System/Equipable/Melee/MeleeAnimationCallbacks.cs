using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MeleeAnimationCallbacks : ItemAnimationCallback
{

    private MeleeWeapon w;

    private void Start()
    {
        w = GetComponentInParent<MeleeWeapon>();
    }

    public void AttackEnd()
    {
        w.Callback_AttackEnd();
    }

    public void EquipEnd()
    {
        w.Callback_EquipEnd();
    }    
}
