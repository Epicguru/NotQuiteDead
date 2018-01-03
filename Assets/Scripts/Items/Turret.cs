using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Turret : NetworkBehaviour {

    public Transform Target;
    public bool AutoTarget = true;
    public float MaxRange = 15f;
    public float MaxSpeed = 270f;
    public float ShootCone = 20f;
    public float RPS = 20f;
    public float Damage = 2.5f;
    public float Inaccuracy = 10f;
    public ShellData Shells;
    public float SendRate = 10f;

    public SpriteRenderer Colour;
    public Transform BarrelRotation;
    public Transform Rotation;
    public Transform BulletSpawn;
    public Transform MuzzleFlashSpawn;

    public Color AimingColour, IdleColour;

    [HideInInspector] public Placeable Placeable;
    private float speed;
    private float targetAngle;
    private float timer;

    [SyncVar]
    private Quaternion currentAngle;

    [SyncVar]
    public string Team = "Team That Does Not Exist"; // TODO IMPLEMENT ME!!! This enables the turret to shoot anyone!

    public void Start()
    {
        Placeable = GetComponent<Placeable>();
    }

    public void Update()
    {
        if (!Placeable.IsPlaced)
            return;

        if (isServer)
            if (AutoTarget)
                UpdateTarget();

        bool aiming = Target != null && InRange();
        if (aiming)
        {
            targetAngle = CalculateAngle();
        }

        if (isServer)
            RotateToTarget(targetAngle); // Where there is authority.
        else
            RotateToCurrentAngle(); // Other clients.
        bool shooting = ShouldShoot();
        UpdateSpinning(shooting);
        UpdateShooting(shooting);
        UpdateColour(aiming);
    }

    public void UpdateTarget()
    {
        Transform target = null;
        
        // TODO FIX THIS WHOLE SCRIPT!

        Target = target;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, MaxRange);     
    }

    public override float GetNetworkSendInterval()
    {
        return 1f / SendRate;
    }

    public bool InRange()
    {
        if (Target == null)
            return false;

        return Vector2.Distance(Target.position, transform.position) <= MaxRange;
    }

    public bool ShouldShoot()
    {
        return InRange();
    }

    public void RotateToTarget(float targetAngle)
    {
        Quaternion target = Quaternion.AngleAxis(targetAngle, Vector3.forward);
        Rotation.rotation = Quaternion.Slerp(Rotation.rotation, target, MaxSpeed * Time.deltaTime);

        currentAngle = Rotation.rotation;
    }

    public void RotateToCurrentAngle()
    {
        Rotation.rotation = currentAngle;
    }

    public void UpdateSpinning(bool shooting)
    {
        if (shooting)
        {
            speed = 2000f;
        }
        else
        {
            speed -= 500f * Time.deltaTime;
        }

        speed = Mathf.Clamp(speed, 0, 3000);
        BarrelRotation.Rotate(0, speed * Time.deltaTime, 0);
    }

    public void UpdateShooting(bool shoot)
    {
        if (!shoot)
            return;

        timer += Time.deltaTime;
        float interval = 1f / RPS;
        while(timer >= interval)
        {
            Fire();
            timer -= interval;
        }
    }

    public void UpdateColour(bool aiming)
    {
        Colour.color = aiming ? AimingColour : IdleColour;
    }

    public void Fire()
    {
        // Offset bullet path bu accuracy.
        BulletSpawn.localRotation = Quaternion.Euler(-90f + (Random.Range(-Inaccuracy * 0.5f, Inaccuracy * 0.5f)), 90f, -90f);

        MuzzleFlash.Place(MuzzleFlashSpawn.position, MuzzleFlashSpawn.rotation);
        SpawnShell();

        // TODO USE THE HEALTH FUNCTION TO DETERMINE HIT!!!

        // Raycast
        Debug.DrawRay(BulletSpawn.transform.position, BulletSpawn.transform.forward * MaxRange, Color.green);
        RaycastHit2D[] hits = Physics2D.RaycastAll(BulletSpawn.transform.position, BulletSpawn.transform.forward, MaxRange);
        foreach(RaycastHit2D hit in hits)
        {
            Health h = hit.collider.transform.GetComponentInParent<Health>();

            if (h == null)
            {
                // No health attribute.
                if (hit.collider.isTrigger)
                {
                    // Not solid...
                    // Shoot through.
                    continue;
                }
                else
                {
                    // Solid
                    Player.Local.NetUtils.CmdSpawnBulletTrail(BulletSpawn.position, hit.point);
                    return;
                }
            }

            Player p = hit.transform.GetComponent<Player>();
            if (p != null)
            {
                if (Teams.I.PlayerInTeam(p.Name, Team)) // If the player is in the turret's team, don't allow to shoot them.
                {
                    continue;
                }
            }

            if (!h.CanHit)
            {
                // Treat as ghost.
                continue;
            }
            if (h.CannotHit.Contains(hit.collider))
            {
                // This particular collider cannot be hit, but perhaps another one can!
                continue;
            }

            if (hit.collider.gameObject.GetComponentInParent<Item>() != null && hit.collider.gameObject.GetComponentInParent<Placeable>() == null)
            {
                // Is item, may be held in hands. Ignore.
                continue;
            }

            // If we get here, deal damage!
            Player.Local.NetUtils.CmdDamageHealth(h.gameObject, Damage, "Automated Turret", false);
            Player.Local.NetUtils.CmdSpawnBulletTrail(BulletSpawn.position, hit.point);
            return;
        }
        Player.Local.NetUtils.CmdSpawnBulletTrail(BulletSpawn.position, BulletSpawn.position + BulletSpawn.transform.forward * MaxRange);
    }

    public void SpawnShell()
    {
        if (Shells.ShellSpawn == null || Shells.Sprite == null)
            return;

        GameObject shell = ObjectPool.Instantiate(ShellPrefabs.Prefab, PoolType.BULLET_SHELL);
        shell.transform.position = Shells.ShellSpawn.transform.position;
        shell.transform.rotation = Shells.ShellSpawn.transform.rotation;

        float m = 1f;
        Vector3 vel = (Shells.ShellSpawn.right * Shells.Velocity.x * m) + (Shells.ShellSpawn.forward * Shells.Velocity.y);
        vel.x += (Random.Range(-vel.x * Shells.Random / 2f, vel.x * Shells.Random / 2f));
        vel.y += (Random.Range(-vel.y * Shells.Random / 2f, vel.y * Shells.Random / 2f));
        shell.GetComponent<GunShell>().Init(Shells.Sprite, vel, Shells.Gravity, Shells.Rotation * m + (Random.Range(-Shells.Rotation * Shells.Random / 2f, Shells.Rotation * Shells.Random / 2f)), Shells.Time, m != 1);
    }

    public float CalculateAngle()
    {
        if (Target == null)
            return 0f;

        float dstX = Target.position.x - transform.position.x;
        float dstY = Target.position.y - transform.position.y;

        float angle = Mathf.Atan2(dstY, dstX) * Mathf.Rad2Deg;

        return angle - 90; // Who knows why 90? Dunno...
    }
}
