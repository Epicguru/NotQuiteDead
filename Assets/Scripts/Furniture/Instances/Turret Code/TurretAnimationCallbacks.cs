
using UnityEngine;

public class TurretAnimationCallbacks : MonoBehaviour
{
    private TurretShooting Shooting;

    public void Shoot()
    {
        if(Shooting == null)
        {
            Shooting = GetComponentInParent<TurretShooting>();
        }

        Shooting.Fire();
    }
}