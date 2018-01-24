
using UnityEngine;

public class PawnAnimationCallbacks : MonoBehaviour
{
    private PawnMeleeAttack attack;

    public void MeleeAttack()
    {
        if (attack == null)
            attack = GetComponentInParent<PawnMeleeAttack>();

        if(attack != null)
        {
            attack.HitTarget();
        }
    }
}