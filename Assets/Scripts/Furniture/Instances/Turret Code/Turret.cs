
using UnityEngine;
using UnityEngine.Networking;

public class Turret : NetworkBehaviour
{
    public float Range = 10f;
    public float Deadzone = 1f;

    public bool Active = true;

    public TurretTargeting Targeting;

    public void Update()
    {
        if (isServer)
        {
            Targeting.MaxSearchRange = Range;
            Targeting.Active = this.Active;
        }
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Targeting.Target != null ? Color.red : Color.white;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0.5f, 0.5f), Range);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0.5f, 0.5f), Deadzone);
    }
}