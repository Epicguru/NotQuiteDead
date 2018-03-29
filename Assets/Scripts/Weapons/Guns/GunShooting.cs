using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[DisallowMultipleComponent]
public class GunShooting : RotatingItem
{
    [Header("Firing Modes")]
    [ReadOnly]
    public FiringMode FiringMode = FiringMode.SEMI;
    public FiringMode[] AllowedModes = new FiringMode[] { FiringMode.SEMI };

    [Header("Bullet Firing")]
    public Transform DefaultBulletSpawn;
    [Tooltip("The type of projectile that this gun fires.")]
    public GunBulletType BulletType = GunBulletType.HITSCAN;
    [Header("Bullet Firing - Subsonic")]
    public float SubsonicBulletSpeed = 50f;
    [Header("Bullet Firing - Other")]
    [Tooltip("If true, when the reload animation ends, if there is not a round in the chamber then the first bullet from the magazine is automatically chambered, so the chamber animation never plays.")]
    public bool ReloadAutoChambers = false;
    [Tooltip("If true, when the reload animation starts, the chambered round is discarded (not visually), which could cause the chamber animation every reload unless ReloadAutoChambers is set to true.")]
    public bool ReloadEjectsChambered = false;

    [Header("Visuals")]
    public float GunBlockedDistance = 1.2f;
    public GunDamage Damage;
    public GunCapacity Capacity;
    public ShellData Shells;
    public GunAudio Audio;
    public AudioSauce AudioSauce
    {
        get
        {
            if(audioSauce == null)
            {
                audioSauce = GetComponentInChildren<AudioSauce>();
            }
            return audioSauce;
        }
    }
    private AudioSauce audioSauce;

    [HideInInspector]
    [SyncVar]
    public float ReloadSpeedMultiplier = 1f;
    [HideInInspector]
    [SyncVar]
    public float ShootSpeedMultiplier = 1f;

    [ReadOnly]
    public int bulletsInMagazine;

    public bool BulletInChamber;

    private Gun gun;
    private new GunAnimation animation;
    private int shotsFired; // Since equipping!
    private bool inBurstFire = false; // Are we in the middle of a burst fire?
    private int burstFireRemaining;
    private float timer;
    private float shotInaccuracy;
    private AnimationClip shootClip;
    //private AnimationClip reloadClip;
    //private AnimationClip chamberClip;

    [HideInInspector]
    public int firingModeIndex = 0;

    public void Awake()
    {
        gun = GetComponent<Gun>();
        animation = GetComponent<GunAnimation>();
    }

    public virtual void Start()
    {
        if(DefaultBulletSpawn == null)
        {
            Debug.LogError("Default Bullet Spawn for gun '" + gun.GetComponent<Item>().Name + "' is null!");
            return;
        }

        if (AllowedModes.Length == 0)
            Debug.LogError("No allowed firing modes on " + gun.Item.Name);
    }

    public virtual void Update()
    {
        if (!isClient)
            return; // Standalone server does not animate.

        // Animation speeds.
        if(animation.Animator != null)
        {
            animation.Animator.SetFloat("ReloadMultiplier", ReloadSpeedMultiplier);
            animation.Animator.SetFloat("ShootMultiplier", ShootSpeedMultiplier);
        }

        // Shooting...
        if (!hasAuthority || gun.Item == null || !gun.Item.IsEquipped())
            return; // Owner only...

        // Change firing mode...
        if(InputManager.InputDown("Firing Mode"))
        {
            NextFiringMode();
        }

        if (bulletsInMagazine > Capacity.MagazineCapacity)
        {
            bulletsInMagazine = Capacity.MagazineCapacity;
            Debug.Log("Reduced bullet amount");
        }

        FiringMode = AllowedModes[firingModeIndex];

        bool requestingShoot = ShootNow();
        bool ready = animation.IsAiming && BulletInChamber && !animation.IsDropped && !animation.IsChambering && !animation.IsReloading;

        if(requestingShoot && bulletsInMagazine == 0 && !BulletInChamber)
        {
            if(!ErrorMessageUI.Instance.IsDisplaying)
                ErrorMessageUI.Instance.DisplayMessage = "Reload [" + InputManager.GetInput("Reload").ToString() + "]";
        }

        // Problem: We tell server to shoot more than once, beucase:
        // 1. We request one shot, one frame.
        // 2. Shoot bool reaches server.
        // 3. Shoot bool reaches client again...
        // 4. The user is not longer requesting a shot - but the server doesn't know that yet!
        // 5. The client is not updated in time, and shoots too many times.

        // Solution:
        // If in single or burst fire modes, CMD and RPC are used?

        //animation.AnimShoot(requestingShoot && ready); // Pew pew!
        if((this.FiringMode == FiringMode.SEMI || this.FiringMode == FiringMode.BURST) && requestingShoot && ready && !animation.IsBlocked)
        {
            animation.CmdAnimShootOnce(this.FiringMode == FiringMode.SEMI);
        }
        else if(requestingShoot && ready)
        {
            animation.AnimShoot(true);
        }
        else
        {
            animation.AnimShoot(false);
        }

        bool requestingChamber = !BulletInChamber && CanChamber();
        ready = !animation.IsChambering && !animation.IsEquipping && !animation.IsAiming && !ReloadAutoChambers;
        if (requestingChamber && ready)
        {
            // Chamber new bullet!
            animation.AnimChamber();
        }

        bool requestingReload = InputManager.InputDown("Reload");
        ready = !animation.IsReloading && !animation.IsAiming && !animation.IsChambering && !animation.IsDropped && CanReload();

        if(requestingReload && ready)
        {
            if (ReloadEjectsChambered)
            {
                BulletInChamber = false;
            }

            // Reload!
            animation.AnimReload();
        }

        UpdateInaccuracy();
    }

    public void NextFiringMode()
    {
        firingModeIndex++;
        if (firingModeIndex == AllowedModes.Length)
            firingModeIndex = 0;
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
                // Shoot a salvo of bullets. Does not affect firing rate or anything like that.
                if (!inBurstFire && animation.SingleShotsPending == 0)
                {
                    if (InputManager.InputDown("Shoot"))
                    {
                        // Start salvo of bullets!
                        // Check if we can shoot...
                        if (!BulletInChamber) // Not the best test...
                            return false;

                        inBurstFire = true;
                        burstFireRemaining = Capacity.BurstShots - 1;
                        return true;
                    }
                }
                else
                {
                    // Already in burst fire...
                    if(burstFireRemaining > 0)
                    {
                        burstFireRemaining--; // This means max once every frame... Thats fine.
                        return true;
                    }
                    else
                    {
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
            //Debug.LogError("Chamber animation played, but the gun cannot chamber!");
            return;
        }

        bulletsInMagazine -= Capacity.BulletsConsumed;
        BulletInChamber = true;
    }

    public Transform GetBulletSpawn()
    {
        // Default position.

        // Check for bullet spawin in muzzle attachment.
        if(gun.Attachments.MuzzleMount != null && gun.Attachments.MuzzleMount.childCount > 0)
        {
            Attachment a = gun.Attachments.MuzzleMount.GetChild(0).GetComponent<Attachment>();
            if (a != null)
            {
                if (a.BulletSpawn != null)
                    return a.BulletSpawn;
            }
        }

        return DefaultBulletSpawn;
    }

    public bool CanChamber()
    {
        return bulletsInMagazine >= Capacity.BulletsConsumed;
    }

    public virtual void FromAnimShoot()
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

        // Spwan shell, if enabled.
        if(Shells.SpawnOnShoot)
            SpawnShell();

        // Callback, all clients.
        if(gun.OnShoot != null)
            gun.OnShoot.Invoke();

        if (!hasAuthority)
            return;

        // LOCAL CLIENT ONLY!!!

        // Remove bullet from chamber...
        BulletInChamber = false;

        // Add one to shot count
        shotsFired++;

        // Shoot!
        this.ShootBullets();

        // Replace bullet if there is one available in magazine.
        if(CanChamber())
        {
            bulletsInMagazine -= Capacity.BulletsConsumed;
            BulletInChamber = true;
        }
    }

    public void SpawnShell()
    {
        if (Shells.ShellSpawn == null || Shells.Sprite == null)
            return;

        GameObject shell = ObjectPool.Instantiate(ShellPrefabs.Prefab, PoolType.BULLET_SHELL);
        shell.transform.position = Shells.ShellSpawn.transform.position;
        shell.transform.rotation = Shells.ShellSpawn.transform.rotation;

        float m = GetComponentInParent<PlayerDirection>().Right ? 1 : -1;
        Vector3 vel = new Vector2(Shells.Velocity.x * m, Shells.Velocity.y);
        vel.x += (Random.Range(-vel.x * Shells.Random / 2f, vel.x * Shells.Random / 2f));
        vel.y += (Random.Range(-vel.y * Shells.Random / 2f, vel.y * Shells.Random / 2f));
        shell.GetComponent<GunShell>().Init(Shells.Sprite, vel, Shells.Gravity, Shells.Rotation * m + (Random.Range(-Shells.Rotation * Shells.Random / 2f, Shells.Rotation * Shells.Random / 2f)), Shells.Time, m != 1);
    }

    private void PlayLocalShot()
    {
        // Plays the local version of the shot. Done on all clients.

        float volume = Audio.GetRandomVolume();
        float pitch = Audio.GetRandomPitch() * Time.timeScale;

        AudioClip clip = Audio.GetRandomClip();

        AudioSauce.Volume = volume;
        AudioSauce.Pitch = pitch;
        AudioSauce.Clip = clip;
        AudioSauce.Play();
    }

    public virtual bool CanReload()
    {
        // Can we even try to reload?

        // LOGIC : Only reload if less bullets that max capacity.

        int bullets = bulletsInMagazine;
        if (Capacity.ChamberCountsAsMag)
            if (BulletInChamber)
                bullets++;

        if (bullets < Capacity.MagazineCapacity)
            return true;

        return false;
    }

    public virtual void FromAnimReload()
    {
        // Reload!
        if (!CanReload())
        {
            Debug.LogError("Reload animation finished, but the gun cannot reload!");
            return;
        }

        // Full up out current magazine.
        bulletsInMagazine = Capacity.MagazineCapacity;

        // If this is a self chambering gun, such as a bolt action sniper rifle, chamber a round.
        // Only if not already chambered.
        if (ReloadAutoChambers)
        {
            if(!BulletInChamber && bulletsInMagazine >= 1)
            {
                BulletInChamber = true;
                bulletsInMagazine--;
            }
        }
    }

    public void FromAnimSpawnShell()
    {
        SpawnShell();
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

            // Hit real objects using the current bullet type.
            EvaluateBulletType(myPos, bullets);            
        }

        // Set time since last shot...
        timer = 0; // Now!
        // This will add inaccuracy.
        shotInaccuracy += 1f / Damage.ShotsToInaccuracy;
    }

    private void EvaluateBulletType(Vector2 endPos, int bulletCount)
    {
        switch (BulletType)
        {
            case GunBulletType.HITSCAN:
                Hit_Hitscan(endPos, bulletCount);
                break;

            case GunBulletType.SUBSONIC:
                Hit_Subsonic(endPos, bulletCount);
                break;
        }
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
        float x = Mathf.LerpUnclamped(m, d, f);


        float final = x * (Mathf.Pow(Damage.PenetrationFalloff, penetration)); // Apply damage falloff.
        //Debug.Log("Hit at " + dst + "/" + Damage.Range + "(" + p + ") that evaluated to " + f + " then lerped from " + m + " to " + d + " to give " + x + " finally applied penetration x" + penetration + " @" + (Damage.PenetrationFalloff * 100f) + "% producing " + final + ".");

        return final;
    }

    public float GetRPS()
    {
        // Aprox rounds per second.
        if(shootClip == null)
        {
            foreach (var c in GetComponentInChildren<Animator>().runtimeAnimatorController.animationClips)
            {
                if (c.name == "Shoot")
                {
                    shootClip = c;
                }
                //if (c.name == "Reload")
                //{
                //    reloadClip = c;
                //}
                //if (c.name == "Chamber")
                //{
                //    chamberClip = c;
                //}
            }
        }

        if (shootClip == null)
            return 0f;

        return 1f / shootClip.length;
    }

    private Vector2 startPos = new Vector2();
    private Vector2 trailEnd = new Vector2();
    private List<Health> objects = new List<Health>();
    private void Hit_Hitscan(Vector2 end, int bullets)
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
                float angle = Mathf.Atan2(hit.normal.y, hit.normal.x) * Mathf.Rad2Deg + 180;
                HitEffect.Spawn(hit.point, angle, hit.collider, true);
                break;
            }

            // Do not allow to hit local player : Maybe some sort of team system??? EDIT - Done.
            Player p = h.GetComponent<Player>();
            if (p != null)
            {
                if(p == Player.Local._Player)
                {
                    continue;
                }
                if (Teams.I.PlayersInSameTeam(Player.Local.Name, p.Name))
                {
                    continue; // Do not allow team damage from guns. TODO IMPLEMENT GAMES RULES.
                }
            }
            if (!h.CanHit)
                continue;

            if (h.CannotHit.Contains(hit.collider))
                continue;

            if (hit.collider.gameObject.GetComponent<NeverHitMe>() != null)            
                continue;            

            if (objects.Contains(h)) // Do not allow multiple hits to one object.
                continue;

            if(hit.collider.gameObject.GetComponentInParent<Item>() != null)
            {
                // Is item, may be held in hands. Ignore.
                continue;
            }

            objects.Add(h);

            float damage = GetDamage(startPos, hit.point, penetrationCount, bullets);
            if (damage != 0)
            {
                CmdHitObject(h.gameObject, Player.Local.Name + ":" + gun.Item.Prefab, damage);
                float angle = Mathf.Atan2(hit.normal.y, hit.normal.x) * Mathf.Rad2Deg + 180;
                HitEffect.Spawn(hit.point, angle, hit.collider, true);
            }

            penetrationCount++;
            if (penetrationCount >= Damage.Penetration)
                trailEnd.Set(hit.point.x, hit.point.y);

            //Debug.Log("Hit object, can penetrate :" + h.CanPenetrate);

            if (!h.CanPenetrate)
            {
                trailEnd.Set(hit.point.x, hit.point.y);
                break;
            }
        }

        // Spawn visual line...
        GameObject GO = ObjectPool.Instantiate(GunEffects.Instance.BulletTrail.gameObject, PoolType.BULLET_PATH);
        GO.GetComponent<BulletPath>().Setup(GetBulletSpawn().position, trailEnd);

        CmdSpawnBulletTrail(GetBulletSpawn().position, trailEnd);

        objects.Clear();
    }

    private void Hit_Subsonic(Vector2 end, int bullets)
    {
        Vector2 a = transform.position;
        Vector2 b = GetBulletSpawn().position;

        RaycastHit2D[] hits = Physics2D.LinecastAll(a, b);

        foreach(var hit in hits)
        {
            if(Health.CanHitObject(hit.collider, Player.Local.Team))
            {                
                // The bullet will definitely hit something before it exits the barrel, because of the way shooting works in this game.
                if(!Health.CanDamageObject(hit.collider, Player.Local.Team))
                {
                    // If we cant damage this object, return.
                    return;
                }
                else
                {
                    // We are intersecting an object, but it can be damaged...
                    // Hit it with out shot!
                    // No shot is spawned, we just automatically damage it.

                    if (isServer)
                    {
                        hit.collider.GetComponentInParent<Health>().ServerDamage(Damage.BulletsShareDamage ? Damage.Damage / bullets : Damage.Damage, Player.Local.Name + ":" + gun.Item.Prefab, false);
                    }
                    else
                    {
                        Player.Local.NetUtils.CmdDamageHealth(hit.collider.GetComponentInParent<Health>().gameObject, Damage.BulletsShareDamage ? Damage.Damage / bullets : Damage.Damage, Player.Local.Name + ":" + gun.Item.Prefab, false);
                    }
                }
            }
        }

        Vector2 start = GetBulletSpawn().position;
        Vector2 realEnd = end;
        realEnd -= (Vector2)transform.position;
        realEnd *= 1000f;
        realEnd += (Vector2)transform.position;
        Vector2 vector = realEnd - start;
        float angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;

        if (isServer)
        {
            SpawnSubsonic(start, angle, SubsonicBulletSpeed, Player.Local.Team, gun.Item.Prefab, Player.Local.Name, gameObject, bullets, true);
        }
        else
        {
            CmdSpawnSubsonic(start, angle, SubsonicBulletSpeed, Player.Local.Team, gun.Item.Prefab, Player.Local.Name, gameObject, bullets);
        }
    }

    [Command]
    private void CmdSpawnSubsonic(Vector2 start, float angle, float speed, string team, string weapon, string shooter, GameObject gun, int bullets)
    {
        SpawnSubsonic(start, angle, speed, team, weapon, shooter, gun, bullets, true);
    }

    [Server]
    private void SpawnSubsonic(Vector2 start, float angle, float speed, string team, string weapon, string shooter, GameObject gun, int bullets, bool network)
    {
        // Deals real damage and is the authorative version. The other spawned projectiles are just ghosts and unfortunately will often be innacurate due to latency.

        if (gun == null)
            return;

        GunShooting shooting = gun.GetComponent<GunShooting>();
        float baseDamage = shooting.Damage.Damage / (shooting.Damage.BulletsShareDamage ? bullets : 1f);
        float falloffDamage = (shooting.Damage.Damage * shooting.Damage.DamageFalloff) / (shooting.Damage.BulletsShareDamage ? bullets : 1f);
        AnimationCurve curve = shooting.Damage.DamageCurve;
        float range = shooting.Damage.Range;

        SubsonicBullet.Spawn(start, angle, speed, team, shooter, weapon, new Vector2(baseDamage, falloffDamage), curve, range);

        if (network)
        {
            // These are the only values that average clients need.
            RpcSpawnSubsonic(start, angle, speed, range);
        }
    }

    [ClientRpc]
    public void RpcSpawnSubsonic(Vector2 start, float angle, float speed, float range)
    {
        if (isServer)
            return; // Don't spawn on the server side.

        SubsonicBullet.Spawn(start, angle, speed, null, null, null, Vector2.zero, null, range);
    }

    [Command]
    public void CmdSpawnBulletTrail(Vector2 start, Vector2 end)
    {
        RpcSpawnBulletTrail(start, end);
    }

    [ClientRpc]
    private void RpcSpawnBulletTrail(Vector2 start, Vector2 end)
    {
        if (hasAuthority)
            return;
        GameObject GO = Instantiate(GunEffects.Instance.BulletTrail.gameObject);
        GO.GetComponent<BulletPath>().Setup(start, end);
    }

    [Command]
    private void CmdHitObject(GameObject obj, string dealer, float health)
    {
        obj.GetComponent<Health>().ServerDamage(health, dealer, false);
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

    public override bool AllowRotateNow()
    {
        return !animation.IsReloading && !animation.IsChambering;
    }

    public override float GetAimTime()
    {
        return gun.Aiming.AimTime;
    }

    public override float GetCurvedTime(float rawTime)
    {
        return gun.Aiming.Curve.Evaluate(rawTime);
    }
}