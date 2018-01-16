
using UnityEngine;
using UnityEngine.Networking;

public class TurretAiming : NetworkBehaviour
{
    public new Transform transform;

    [Range(1f, 60f)]
    public float SendRate = 20f;

    public float LerpRate = 5f;

    [SyncVar]
    public float Angle;

    public void Update()
    {
        if (isClient)
        {
            if (!isServer)
            {
                float angle = Mathf.LerpAngle(transform.localEulerAngles.z, Angle, Time.deltaTime * LerpRate);
                SetAngle(angle);
            }
            else
            {
                // Is server.
                SetAngle(Angle);
            }
        }
    }

    private void SetAngle(float angle)
    {
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }

    public override float GetNetworkSendInterval()
    {
        return 1f / SendRate;
    }
}