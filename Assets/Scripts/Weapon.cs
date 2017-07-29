using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    public Transform BulletSpawn;

    public Shooting Shooting = new Shooting();
    public Visuals Visuals = new Visuals();
    public Sound Sound = new Sound();

    [HideInInspector] public bool BulletInChamber;
    [HideInInspector] public bool Reloading;
    [HideInInspector] public bool Chambering;
    [HideInInspector] public bool Stored;

    private bool localStored;
    private Animator anim;
    private Hands hands;
    private AudioSource shotAudio, animationAudio;

    public void Start()
    {
        anim = GetComponentInChildren<Animator>();
        hands = GetComponentInParent<Hands>();

        shotAudio = this.gameObject.AddComponent<AudioSource>();
        animationAudio = this.gameObject.AddComponent<AudioSource>();
    }

    public void Update()
    {
        if (Stored)
        {
            if (!localStored)
            {
                // Put away

                // Hide behind player
                foreach(SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>(true))
                {
                    renderer.sortingLayerName = "Stored Weapons";
                }

                this.localStored = true;
            }
        }
        else
        {
            if (localStored)
            {
                // Bring out

                // Bring back to real layer
                foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>(true))
                {
                    renderer.sortingLayerName = "Weapons";
                }

                this.localStored = false;
            }
        }

        if (Stored)
            return;

        if(this.Shooting.BulletsInMag > 0 && !BulletInChamber && !Reloading && !Chambering)
        {
            anim.SetBool("Chamber", true);
            Chambering = true;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            // Reload...

            // Reloading is putting new mag in gun.
            // But...
            // We also want to chamber a new round if we have bullets in the mag.

            this.Reload();
        }
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
        else
        {
            if (!BulletInChamber && this.Shooting.BulletsInMag > 0)
            {
                this.anim.SetBool("Chamber", true);
                this.Chambering = true;
            }
        }
    }

    public void Anim_Sound(int index)
    {
        this.animationAudio.PlayOneShot(this.Sound.AnimationSounds[index]);
    }

    public void SpawnMag()
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

        Debug.Log("Chambered");
    }

    public void Anim_Shoot()
    {
        BulletInChamber = false;

        // Try to chamber a round from the mag.
        // If we have no bullets left, then we will simply leave the chamber empty.
        if (this.Shooting.BulletsInMag >= 1)
        {
            this.Shooting.BulletsInMag -= 1;
            BulletInChamber = true;
        }

        // Spawn bullet trail
        for (int i = 0; i < this.Shooting.BulletsPerShot; i++)
        {
            this.PlaceVisualBullet();
        }

        if (this.Sound.ShootSounds.Length == 0)
            return;

        int index = Random.Range(0, this.Sound.ShootSounds.Length - 1);
        AudioClip c = this.Sound.ShootSounds[index];
        this.shotAudio.PlayOneShot(c);
    }

    public void PlaceVisualBullet()
    {
        if (this.BulletSpawn == null)
            return;

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
        b.Set(this.BulletSpawn.position, endPoint);
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
            float damage = this.GetDamageFor(hit.point, maxDistance, penetration);

            // Deal Damage
            hit.collider.gameObject.SendMessageUpwards("Shot", new Hit() {
                Position = hit.point,
                Damage = damage,
                Gun = this.gameObject.name, // TODO chamge name (make name var)
                Angle = angle
            }, SendMessageOptions.DontRequireReceiver);

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
    public AudioClip[] ShootSounds;
    public AudioClip[] AnimationSounds;
}

[System.Serializable]
public class Visuals
{
    public GameObject MagPrefab, RealMag;
}