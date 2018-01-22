
using UnityEngine;
using UnityEngine.Networking;

public class TurretAnimation : NetworkBehaviour
{
    [SyncVar]
    [ReadOnly]
    public bool Shooting;

    public TurretTargeting Target;
    public Animator Anim;

    public void Update()
    {
        if (isServer)
        {
            if(Target.ShouldShoot != Shooting)
            {
                Shooting = Target.ShouldShoot;
            }
        }

        Anim.SetBool("Shoot", Shooting);
    }
}