using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    public string Prefab = "Weapons/";

    public Shooting Shooting = new Shooting();
    public Visuals Visuals = new Visuals();
    public Sound Sound = new Sound();
    public WeaponInformation Info = new WeaponInformation();
    public AttatchmentsContainer Attatchments;

    [HideInInspector] public bool BulletInChamber = false;
    [HideInInspector] public bool Reloading;
    [HideInInspector] public bool Chambering;
    [HideInInspector] public bool Stored, Dropped;

    //private bool localStored;
    private Animator anim;
    private WeaponManager hands;
    private float aimPercentage;
    private AudioSource shotAudio, animationAudio;

    private static Vector3 defaultPos = new Vector3();

    public void Start()
    {
        anim = GetComponentInChildren<Animator>();
        hands = GetComponentInParent<WeaponManager>();

        shotAudio = this.gameObject.AddComponent<AudioSource>();
        animationAudio = this.gameObject.AddComponent<AudioSource>();
    }

    public void Update()
    {
        this.SetLayer();
        this.SetPosition();
        this.UpdateAttatchments();

        if (Stored)
            return;

        this.AutoChamber();
        this.UpdateReloading();
    }

    public void UpdateAttatchments()
    {
        for(int i = 0; i < 2; i++) // UPDATE ATTACHMENT HERE!
        {
            if (this.Attatchments.Current.GetAttatchment(i) == null)
            {
                this.SetAttatchment(i, this.Attatchments.Default.GetAttatchment(i));
            }

            UpdateAttatchment(i);
        }        
    }

    public void SetAttatchment(int index, Attachment a)
    {
        this.Attatchments.Current.SetAttachment(index, a);
    }

    public void UpdateAttatchment(int index)
    {
        Attachment newObject = Attatchments.Current.GetAttatchment(index);

        if(newObject == null)
        {
            RemoveAttachment(index);
        }
        else
        {
            if (Attatchments.Positions.GetPosition(index).childCount != 0 && Attatchments.Positions.GetPosition(index).GetChild(0) != null && Attatchments.Positions.GetPosition(index).GetChild(0).GetComponent<Attachment>().Equals(Attatchments.Current.GetAttatchment(index)))
                return;

            RemoveAttachment(index);

            GameObject prefab = newObject.GetPrefab();

            GameObject instance = Instantiate(prefab, this.Attatchments.Positions.GetPosition(index));
            instance.transform.localPosition = new Vector3(0, 0, 0);
        }
    }

    public void RemoveAttachment(int index)
    {
        if (Attatchments.Positions.GetPosition(index).childCount == 0)
            return;

        Transform x = Attatchments.Positions.GetPosition(index).GetChild(0);
        Destroy(x.gameObject);
    }

    /// <summary>
    /// Attempts to reload the gun.
    /// </summary>
    public void Reload()
    {
        if (Reloading || Chambering)
            return;

        if (this.Shooting.BulletsInMag < MaxBulletsInMag())
        {
            this.anim.SetBool("Reload", true);
            this.Reloading = true;
        }
    }

    public float GetSpatialBlend()
    {
        bool isLocal = true;
        if (isLocal)
        {
            return 0;
        }
        else
        {
            return 1;
        }
    }

    public void UpdateShootingAudioSource()
    {
        shotAudio.maxDistance = this.Sound.ShootAudioRange;
        shotAudio.volume = Random.Range(this.Sound.ShootVolume.x, this.Sound.ShootVolume.y);
        shotAudio.pitch = Random.Range(this.Sound.ShootPitch.x, this.Sound.ShootPitch.y);
        shotAudio.spatialBlend = this.GetSpatialBlend();
    }

    public void UpdateAnimationAudioSource()
    {
        animationAudio.maxDistance = 50f;
        animationAudio.spatialBlend = this.GetSpatialBlend();
    }

    public Transform GetBulletSpawn()
    {
        if (Attatchments.Current.Muzzle != null)
        {
            if(Attatchments.Current.Muzzle is MuzzleAttachment)
            {
                return (Attatchments.Positions.Muzzle.GetChild(0).GetComponent<MuzzleAttachment>()).BulletSpawn;
            }
            else
            {
                Debug.LogError("Attachment on muzzle (" + Attatchments.Current.Muzzle.Name + ") does not inherit from MuzzleAttachment. Please fix this.");
                return this.Shooting.BulletSpawn;
            }
        }
        else
        {
            return this.Shooting.BulletSpawn;
        }
    }

    public void AutoChamber()
    {
        if (this.Shooting.BulletsInMag > 0 && !BulletInChamber && !Reloading && !Chambering && !Dropped && !Stored)
        {
            anim.SetBool("Chamber", true);
            Chambering = true;
        }
    }

    public void UpdateReloading()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Reload...

            // Reloading is putting new mag in gun.
            // But...
            // We also want to chamber a new round if we have bullets in the mag.

            this.Reload();
        }
    }

    public void SetPosition()
    {
        if (!Dropped && !Stored)
        {
            if (hands.Aiming && !Reloading && !Chambering)
            {
                this.aimPercentage += Time.deltaTime * (1f / this.Visuals.AimTime);
                if (aimPercentage > 1)
                    aimPercentage = 1;
            }
            else
            {
                this.aimPercentage -= Time.deltaTime * (1f / this.Visuals.AimTime);
                if (aimPercentage < 0)
                    aimPercentage = 0;
            }
        }
        else
        {
            aimPercentage = 0;
        }

        transform.localPosition = defaultPos;

        float aimDst = Mathf.Lerp(this.Visuals.MinAimDistance, this.Visuals.MaxAimDistance, aimPercentage);
        transform.Translate(aimDst * this.aimPercentage, 0, 0);
    }

    public void SetLayer()
    {
        if (Stored || Dropped)
        {
            //if (!localStored)
            {
                // Put away

                // Hide behind player
                foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>(true))
                {
                    renderer.sortingLayerName = "Stored Weapons";
                }

                //this.localStored = true;
            }
        }
        else if (!Stored && !Dropped)
        {
            //if (localStored)
            {
                // Bring out

                // Bring back to real layer
                foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>(true))
                {
                    renderer.sortingLayerName = "Weapons";
                }

                //this.localStored = false;
            }
        }
    }

    public void Anim_Sound(int index)
    {
        this.UpdateAnimationAudioSource();
        this.animationAudio.PlayOneShot(this.Sound.AnimationSounds[index]);
    }

    public void Anim_SpawnMag()
    {
        if(this.Visuals.RealMag == null || this.Visuals.MagPrefab == null)
        {
            return;
        }

        Instantiate(this.Visuals.MagPrefab, this.Visuals.RealMag.transform.position, this.Visuals.RealMag.transform.rotation).GetComponentInChildren<SpriteRenderer>().flipX = !hands.Right;
    }

    public int MaxBulletsInMag() // From OOP days...
    {
        return this.Shooting.MAX_BULLETS_IN_MAG;
    }

    public void Anim_Chamber()
    {
        this.anim.SetBool("Chamber", false);
        BulletInChamber = true;
        this.Shooting.BulletsInMag--;

        this.Chambering = false;
    }

    public void Anim_Shoot()
    { 
        // Try to chamber a round from the mag.
        // If we have no bullets left, then we will simply leave the chamber empty.
        if (this.Shooting.BulletsInMag >= 1)
        {
            this.Shooting.BulletsInMag -= 1;
            BulletInChamber = true;
        }
        else
        {
            BulletInChamber = false;
        }

        // Spawn bullet trail
        for (int i = 0; i < this.Shooting.BulletsPerShot; i++)
        {
            this.PlaceVisualBullet();
        }

        if (this.Sound.ShootSounds.Length == 0)
            return;

        // Audio
        UpdateShootingAudioSource();
        int index = Random.Range(0, this.Sound.ShootSounds.Length - 1);
        AudioClip c = this.Sound.ShootSounds[index];
        this.shotAudio.PlayOneShot(c);
    }

    public void Anim_SpawnShell()
    {
        if(this.Visuals.ShellPrefab == null || this.Visuals.ShellSpawn == null)
        {
            return;
        }

        GameObject g = Instantiate(this.Visuals.ShellPrefab, this.Visuals.ShellSpawn.position, this.Visuals.ShellSpawn.rotation);
        g.GetComponent<Shell>().Right = this.hands.Right;
    }

    public void PlaceVisualBullet()
    {
        if (this.GetBulletSpawn() == null)
        {
            Debug.LogError("Shooting/BulletSpawn is null, not shooting!");
            return;
        }

        Vector3 v3 = Input.mousePosition;
        v3.z = 0;
        v3 = Camera.main.ScreenToWorldPoint(v3);

        float dstX = transform.parent.position.x - v3.x;
        float dstY = transform.parent.position.y - v3.y;

        float angle = Mathf.Atan2(dstY, dstX);

        angle += Random.Range(-this.Shooting.Inaccuracy / 2f, this.Shooting.Inaccuracy / 2f) * Mathf.Deg2Rad;

        float x = Mathf.Cos(angle);
        float y = Mathf.Sin(angle);

        Vector3 pos = transform.parent.position - new Vector3(x, y, 0) * this.Shooting.Range;

        Vector2 endPoint = this.HitObjects(pos, angle * Mathf.Rad2Deg);

        VisualBullet b = Instantiate<VisualBullet>(Spawnables.I.BulletTrail, Vector3.zero, Quaternion.identity);
        b.TimeRemaining = 1f;
        b.Set(this.GetBulletSpawn().position, endPoint);
    }

    public Vector2 HitObjects(Vector2 endPoint, float angle)
    {
        // This is called once per bullet or pellet.
        // The list of hits is all penetrations.

        RaycastHit2D[] hits = Physics2D.LinecastAll(transform.parent.position, endPoint, hands.ShootableLayers);

        float maxDistance = Vector2.Distance(transform.parent.position, endPoint);

        int penetration = 0;
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.isTrigger)
                continue;

            float damage = this.GetDamageFor(hit.point, maxDistance, penetration);

            // Deal Damage
            hit.collider.gameObject.SendMessageUpwards("Shot", this.MakeHitObject(damage, angle, hit), SendMessageOptions.DontRequireReceiver);

            penetration++;
            if (penetration == this.Shooting.Penetration)
            {
                // Stop if we have reached penetration limit.
                // Also stop trail here.
                return hit.point;
            }
        }

        return endPoint;
    }

    public virtual Hit MakeHitObject(float damage, float angle, RaycastHit2D hit)
    {
        return new Hit()
        {
            Position = hit.point,
            Damage = damage,
            Gun = this.gameObject.name, // TODO chamge name (make name var)
            Angle = angle,
            Collider = hit.collider,
            Normal = hit.normal
        };
    }

    public virtual float GetDamageFor(Vector2 point, float totalDistance, int penetrationCount)
    {
        float basic = this.Shooting.Damage;
        float distance = Vector2.Distance(transform.parent.position, point);
        float p = distance / totalDistance; // 0 Means we hit closest, 1 is furthest.

        float reduced = basic * this.Shooting.DamageFalloff; // This is the damage at the very edge of the range
        float applied = Mathf.Lerp(basic, reduced, p); // This value is the lenear interpolation between the maximum and minimum damage.

        float final = applied * Mathf.Pow(this.Shooting.DamageFalloffPenetration, penetrationCount); // Multiply by the damage falloff per penetration raised to the number of penetrations.

        // For example:
        // Falloff = 0.5
        // Damage = 100,
        // Distance = 10
        // Total Distance = 20
        // Means that P = (10 / 20) = 0.5
        // Reduced = Damage * Falloff = 50
        // Meaning that Applied = Lerp(100, 50, 0.5)
        // Applied = 75
        // Final = Applied * PenetrationFalloff raied to Penetration
        // Final = 75 * (0.5 pow 1)
        // Final = 37.5

        return final;
    }

    public GameObject GetPrefab()
    {
        return Resources.Load<GameObject>(this.Prefab);
    }

    public void Anim_Reload()
    {
        this.Shooting.BulletsInMag = MaxBulletsInMag(); // Pool chack
        this.anim.SetBool("Reload", false);

        this.Reloading = false;

        if (!BulletInChamber)
        {
            this.anim.SetBool("Chamber", true);
            this.Chambering = true;
        }
    }
}

[System.Serializable]
public class Shooting
{
    public Transform BulletSpawn;
    public int MAX_BULLETS_IN_MAG = 20;
    public int BulletsInMag = 9;
    public float Range = 20f;
    public float Inaccuracy = 8f;
    public int BulletsPerShot = 1;
    public int Penetration = 1;
    public float Damage = 100;
    public float DamageFalloffPenetration = 0.5f;
    public float DamageFalloff = 1f;
}

[System.Serializable]
public class Sound
{
    public float ShootAudioRange = 120f;
    public Vector2 ShootVolume = new Vector2(0.9f, 1f), ShootPitch = new Vector2(0.9f, 1f);
    public AudioClip[] ShootSounds;
    public AudioClip[] AnimationSounds;
}

[System.Serializable]
public class Visuals
{
    public float MinAimDistance = 0.7f, MaxAimDistance = 1f;
    public float AimTime = 1 / 3f;
    public GameObject MagPrefab, RealMag;
    public Transform ShellSpawn;
    public GameObject ShellPrefab;
}

[System.Serializable]
public class WeaponInformation
{
    public WeaponType Type;
    [TextAreaAttribute(5,10)]
    public string ShortDescription;
    [TextAreaAttribute(10, 20)]
    public string LongDescription;
}

[System.Serializable]
public class AttatchmentsContainer
{
    public AttachmentPositions Positions;
    public Attachments Current;
    public Attachments Default;
}

[System.Serializable]
public class Attachments
{
    public Attachment Sight;
    public Attachment Muzzle;

    public Attachment GetAttatchment(int index)
    {
        switch (index)
        {
            case 0:
                return Sight;
            case 1:
                return Muzzle;
            default:
                return null;
        }
    }

    public void SetAttachment(int index, Attachment x)
    {
        switch (index)
        {
            case 0:
                Sight = x;
                break;
            case 1:
                Muzzle = x;
                break;
            default:
                return;
        }
    }
}

[System.Serializable]
public class AttachmentPositions
{
    public Transform Sight;
    public Transform Muzzle;

    public Transform GetPosition(int index)
    {
        switch (index)
        {
            case 0:
                return Sight;
            case 1:
                return Muzzle;
            default:
                return null;
        }
    }

    public void SetPosition(int index, Transform x)
    {
        switch (index)
        {
            case 0:
                Sight = x;
                break;
            case 1:
                Muzzle = x;
                break;
            default:
                return;
        }
    }
}