
using UnityEngine;
using UnityEngine.Networking;

public class TurretDetails : NetworkBehaviour
{
    [Header("System")]
    public TurretTargeting Targeting;
    public Turret Turret;

    [Header("Colours")]
    public Color LaserNormal;
    public Color LaserFoundTarget;
    public Color DetailsNormal;
    public Color DetailsNotActive;

    [Header("Sprites")]
    public SpriteRenderer TopDetails;
    public SpriteRenderer Laser;

    [Header("Variables")]
    [SyncVar]
    [ReadOnly]
    public bool FoundTarget;

    [SyncVar]
    [ReadOnly]
    public bool Active;

    [Server]
    public void SetActive(bool flag)
    {
        if(Active != flag)
        {
            Active = flag;
        }
    }

    [Server]
    public void SetFoundTarget(bool flag)
    {
        if (FoundTarget != flag)
        {
            FoundTarget = flag;
        }
    }

    public void Update()
    {
        if (isServer)
        {
            SetActive(Turret.Active);
            SetFoundTarget(Targeting.Target != null);
        }

        Laser.color = !Active ? Color.clear : (FoundTarget ? LaserFoundTarget : LaserNormal);
        TopDetails.color = Active ? DetailsNormal : DetailsNotActive;
    }
}