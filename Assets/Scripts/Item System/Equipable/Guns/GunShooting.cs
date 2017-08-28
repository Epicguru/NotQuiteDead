using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(AudioSource))]
public class GunShooting : NetworkBehaviour
{
    public FiringMode FiringMode = FiringMode.SEMI;
    public Transform DefaultBulletSpawn;
    public GunDamage Damage;
    public GunCapacity Capacity;
    public GunAudio Audio;
    [HideInInspector] public AudioSource AudioSource;

    //[HideInInspector]
    public int bulletsInMagazine;

    //[HideInInspector]
    public bool bulletInChamber;

    private Gun gun;
    private new GunAnimation animation;
    private int shotsFired; // Since equipping!
    private bool inBurstFire = false; // Are we in the middle of a burst fire?
    private int burstFireStart;
    private float timer;
    [SerializeField]
    [Range(0f, 1f)]
    private float shotInaccuracy;

    public void Start()
    {
        gun = GetComponent<Gun>();
        animation = GetComponent<GunAnimation>();
        AudioSource = GetComponent<AudioSource>();

        if(DefaultBulletSpawn == null)
        {
            Debug.LogError("Default Bullet Spawn for gun '" + gun.GetComponent<Item>().Name + "' is null!");
            return;
        }

        AudioSource.spatialBlend = 1;

        // TODO FIXME!

        bulletsInMagazine = Capacity.MagazineCapacity;
    }

    public void Update()
    {
        // Shooting...
        if (!hasAuthority || gun.Item == null || !gun.Item.IsEquipped())
            return; // Owner only...

        bool requestingShoot = ShootNow();
        bool ready = animation.IsAiming && bulletInChamber && !animation.IsDropped && !animation.IsChambering && !animation.IsReloading;
        animation.AnimShoot(requestingShoot && ready); // Animation, bullets and damage later.


        bool requestingChamber = !bulletInChamber && CanChamber();
        ready = !animation.IsChambering && !animation.IsEquipping && !animation.IsAiming;
        if (requestingChamber && ready)
        {
            // Chamber new bullet!
            animation.AnimChamber();
        }


        bool requestingReload = InputManager.InputDown("Reload");
        ready = !animation.IsReloading && !animation.IsAiming && !animation.IsChambering && !animation.IsDropped && CanReload();

        if(requestingReload && ready)
        {
            // Reload!
            animation.AnimReload();
        }

        UpdateInaccuracy();
    }

    public float GetCurrentInaccuracy()
    {
        // As a float normally between 0 and 1 but depends on the curve.

        return Damage.CurveToInaccuracy.Evaluate(shotInaccuracy);
    }

    private void UpdateInaccuracy()
    {
        timer += Time.deltaTime;
        if(timer >= Damage.MinIdleTimeToCooldown)
        {
            shotInaccuracy -= Damage.ShotsInaccuracyCooldown * Time.deltaTime;
        }
        if (shotInaccuracy < 0)
            shotInaccuracy = 0;
        if (shotInaccuracy > 1)
            shotInaccuracy = 1;
    }

    public bool ShootNow()
    {
        // Returns true if the gun SHOULD shoot. Whether the gun will shoot is up to other factors.

        switch (FiringMode)
        {
            case FiringMode.SEMI:
                // Shoot only when the shoot button is first pressed.
                if (InputManager.InputDown("Shoot"))
                    return true;
                break;
            case FiringMode.AUTO:
                // Shoot forever if the shoot button is pressed.
                if (InputManager.InputPressed("Shoot"))
                    return true;
                break;
            case FiringMode.BURST:
                // Shoot a salvo of bullets. Does not affect firin rate or anything like that.
                if (!inBurstFire)
                {
                    if (InputManager.InputDown("Shoot"))
                    {
                        // Start salvo of bullets!
                        // Check if we can shoot...
                        if (!bulletInChamber) // Not the best test...{
                            return false;

                        inBurstFire = true;
                        burstFireStart = shotsFired;
                        return true;
                    }
                }
                else
                {
                    // Already in burst fire...
                    int fired = shotsFired - burstFireStart;
                    if(fired < Capacity.BurstShots)
                    {
                        if (!bulletInChamber || !animation.IsAiming)
                        {
                            // Just stop here.
                            inBurstFire = false;


                            return false;
                        }
                        return true;
                    }
                    else
                    {
                        // Stop burst fire...
                        inBurstFire = false;
                        return false;
                    }
                }
                break;
        }

        return false;
    }

    public void FromAnimChamber()
    {
        // Called when the chamber animation ends.
        // Set bullet in chamber to true...

        if (!CanChamber())
        {
            Debug.LogError("Chamber animation played, but the gun cannot chamber!");
            return;
        }

        bulletsInMagazine -= Capacity.BulletsConsumed;
        bulletInChamber = true;
    }

    public Transform GetBulletSpawn()
    {
        // Default position.
        return DefaultBulletSpawn;
    }

    public bool CanChamber()
    {
        return bulletsInMagazine >= Capacity.BulletsConsumed;
    }

    public void FromAnimShoot()
    {
        // Shoot for real now!

        // ALL CLIENTS!!!!

        // Place muzzle flash
        Quaternion rotation = GetBulletSpawn().rotation;
        GameObject GO = MuzzleFlash.Place(GetBulletSpawn().position, GetBulletSpawn().rotation);
        if (!GetComponentInParent<PlayerDirection>().Right)
            GO.transform.Rotate(0, 0, 180);

        // Play audio
        PlayLocalShot();

        if (!hasAuthority)
            return;

        // LOCAL CLIENT ONLY!!!

        // Remove bullet from chamber...
        bulletInChamber = false;

        // Add one to shot count
        shotsFired++;

        // Shoot!
        this.ShootBullets();

        // Replace bullet if there is one available in magazine.
        if(CanChamber())
        {
            bulletsInMagazine -= Capacity.BulletsConsumed;
            bulletInChamber = true;
        }
    }

    private void PlayLocalShot()
    {
        // Plays the local version of the shot

        float volume = Audio.GetRandomVolume();
        float pitch = Audio.GetRandomPitch();

        AudioClip clip = Audio.GetRandomClip();

        AudioSource.volume = volume;
        AudioSource.pitch = pitch;
        AudioSource.PlayOneShot(clip);
    }

    public bool CanReload()
    {
        // Can we even try to reload?

        // LOGIC : Only reload if less bullets that max capacity.

        int bullets = bulletsInMagazine;
        if (Capacity.ChamberCountsAsClip)
            if (bulletInChamber)
                bullets++;

        if (bullets < Capacity.MagazineCapacity)
            return true;

        return false;
    }

    public void FromAnimReload()
    {
        // Reload!
        if (!CanReload())
        {
            Debug.LogError("Reload animation finished, but the gun cannot reload!");
            return;
        }

        // TODO improve me.
        bulletsInMagazine = Capacity.MagazineCapacity;
    }

    public void ShootBullets()
    {
        // Shoot bullets!

        // Update inaccuracy...

        int bullets = Random.Range((int)Capacity.BulletsPerShot.y, (int)Capacity.BulletsPerShot.y + 1);
        float range = Damage.Range;
        float angleToMouse = CalculateAngle();

        for(int i = 0; i < bullets; i++)
        {
            // Get end point
            float angle = angleToMouse;

            // Apply inaccuracy...
            float min = Damage.Inaccuracy.x;
            float max = Damage.Inaccuracy.y;
            // Half becuase that creates a nice intuitive value. Such as 90 degrees being a cone.
            float current = Mathf.LerpUnclamped(min, max, GetCurrentInaccuracy()) / 2f; 

            // Get angle change and apply
            float offset = Random.Range(-current, current);
            angle += offset;

            float xOffset = Mathf.Cos(angle * Mathf.Deg2Rad) * range;
            float yOffset = Mathf.Sin(angle * Mathf.Deg2Rad) * range;
            myPos.Set(transform.position.x + xOffset, transform.position.y + yOffset);

            // Hit real objects...
            HitObjects(myPos, bullets);
        }

        // Set time since last shot...
        timer = 0; // Now!
        // This will add inaccuracy.
        shotInaccuracy += 1f / Damage.ShotsToInaccuracy;
    }

    private float GetBaseBulletDamage(int totalBullets)
    {
        if (Damage.BulletsShareDamage)
        {
            return Damage.Damage / (float)totalBullets;
        }
        else
        {
            return Damage.Damage;
        }
    }

    private float GetMinBulletDamage(int totalBullets)
    {
        if (Damage.BulletsShareDamage)
        {
            return (Damage.Damage * Damage.DamageFalloff) / (float)totalBullets;
        }
        else
        {
            return Damage.Damage * Damage.DamageFalloff;
        }
    }

    public float GetDamage(Vector2 start, Vector2 hitPoint, int penetration, int bullets)
    {
        float d = GetBaseBulletDamage(bullets);
        float m = GetMinBulletDamage(bullets);
        float dst = Vector2.Distance(startPos, hitPoint);
        if (dst > Damage.Range)
        {
            // Should never happen, but might be possible because of small mathematical inaccuracies.
            return 0;
        }
        float p = dst / Damage.Range;
        float f = Damage.DamageCurve.Evaluate(p);
        d = Mathf.LerpUnclamped(d, m, f);

        float final = d * (Mathf.Pow(Damage.PenetrationFalloff, penetration)); // Apply damage falloff.

        return final;
    }

    private Vector2 startPos = new Vector2();
    private Vector2 trailEnd = new Vector2();
    private List<Health> objects = new List<Health>();
    private void HitObjects(Vector2 end, int bullets)
    {
        objects.Clear();
        startPos.Set(transform.position.x, transform.position.y);
        RaycastHit2D[] hits = Physics2D.LinecastAll(startPos, end);

        int penetrationCount = 0;

        trailEnd.Set(end.x, end.y);

        foreach(RaycastHit2D hit in hits)
        {
            if (penetrationCount >= Damage.Penetration)
                break;

            // A hit to a collider...
            Health h = hit.collider.gameObject.GetComponentInParent<Health>();

            if(h == null)
            {
                //Debug.Log("Object hit has no health, cannot be destroyed!");
                // DEBATE - Do we allow to shoot through non destructible objects, that may have multiple colliders?
                // Hell no!
                // UNLESS - the collider is a trigger!

                if (hit.collider.isTrigger)
                {
                    //Debug.Log("Collider we hit is a trigger, continue...");
                    continue;
                }

                trailEnd.Set(hit.point.x, hit.point.y);
                break;
            }

            if(h.GetComponent<Player>() != null)
            {
                if(h.GetComponent<Player>() == Player.Local._Player)
                {
                    continue;
                }
            }
            if(h.GetComponent<Placeable>() != null && !h.GetComponent<Placeable>())
            {
                // Placeable item on floor that cannot be penetrated but should not be hit...
                continue;
            }

            if (objects.Contains(h)) // Do not allow multiple hits to one object.
                continue;

            objects.Add(h);

            float damage = GetDamage(startPos, end, penetrationCount, bullets);
            if (damage != 0)
                CmdHitObject(h.gameObject, "A Player:" + gun.Item.Name, damage);

            trailEnd.Set(hit.point.x, hit.point.y);
            penetrationCount++;

            if (!h.CanPenetrate)
            {
                break;
            }
        }

        // Spawn visual line...
        GameObject GO = Instantiate(GunEffects.Instance.BulletTrail.gameObject);
        GO.GetComponent<BulletPath>().Setup(GetBulletSpawn().position, trailEnd);

        CmdSpawnVisual(GetBulletSpawn().position, trailEnd);

        objects.Clear();
    }

    [Command]
    private void CmdHitObject(GameObject obj, string dealer, float health)
    {
        obj.GetComponent<Health>().CmdDamage(health, dealer, false);
    }

    [Command]
    private void CmdSpawnVisual(Vector2 start, Vector2 end)
    {
        RpcSpawnVisual(start, end);
    }

    [ClientRpc]
    private void RpcSpawnVisual(Vector2 start, Vector2 end)
    {
        if (!hasAuthority)
        {
            GameObject GO = Instantiate(GunEffects.Instance.BulletTrail.gameObject);
            GO.GetComponent<BulletPath>().Setup(start, end);
        }
    }

    private Vector2 myPos = new Vector2();
    private float CalculateAngle()
    {
        Vector2 mousePos = InputManager.GetMousePos();
        myPos.Set(transform.position.x, transform.position.y);

        Vector2 dst = mousePos - myPos;

        float angle = Mathf.Atan2(dst.y, dst.x) * Mathf.Rad2Deg;

        return angle;
    }
}