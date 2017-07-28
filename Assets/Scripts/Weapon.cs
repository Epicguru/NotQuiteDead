using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    public Transform BulletSpawn;
    public GameObject MagPrefab, RealMag;
    public int MAX_BULLETS_IN_MAG = 20;
    public int BulletsInMag = 9;
    public float Range = 20f;
    public float Inaccuracy = 8f;
    public int BulletsPerShot = 1;
    public AudioClip[] ShootSounds;
    public AudioClip[] AnimationSounds;

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

        if(BulletsInMag > 0 && !BulletInChamber && !Reloading && !Chambering)
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

        if (BulletsInMag < MaxBulletsInMag())
        {
            this.anim.SetBool("Reload", true);
            this.Reloading = true;
        }
        else
        {
            if (!BulletInChamber && BulletsInMag > 0)
            {
                this.anim.SetBool("Chamber", true);
                this.Chambering = true;
            }
        }
    }

    public void Anim_Sound(int index)
    {
        this.animationAudio.PlayOneShot(this.AnimationSounds[index]);
    }

    public void SpawnMag()
    {
        if(RealMag == null || MagPrefab == null)
        {
            return;
        }

        Instantiate(MagPrefab, RealMag.transform.position, RealMag.transform.rotation).GetComponentInChildren<SpriteRenderer>().flipX = !hands.Right;
    }

    public int MaxBulletsInMag() // From OOP days...
    {
        return this.MAX_BULLETS_IN_MAG;
    }

    public void Anim_Chamber()
    {
        this.anim.SetBool("Chamber", false);
        BulletInChamber = true;
        BulletsInMag--;

        this.Chambering = false;

        Debug.Log("Chambered");
    }

    public void Anim_Shoot()
    {
        BulletInChamber = false;

        // Try to chamber a round from the mag.
        // If we have no bullets left, then we will simply leave the chamber empty.
        if (BulletsInMag >= 1)
        {
            BulletsInMag -= 1;
            BulletInChamber = true;
        }

        // Spawn bullet trail
        for (int i = 0; i < this.BulletsPerShot; i++)
        {
            this.PlaceVisualBullet();
        }

        if (this.ShootSounds.Length == 0)
            return;

        int index = Random.Range(0, this.ShootSounds.Length - 1);
        AudioClip c = this.ShootSounds[index];
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

        angle += Random.Range(-this.Inaccuracy / 2f, this.Inaccuracy / 2f) * Mathf.Deg2Rad;

        float x = Mathf.Cos(angle);
        float y = Mathf.Sin(angle);

        Vector3 pos = transform.parent.position - new Vector3(x, y, 0) * this.Range;

        VisualBullet b = Instantiate<VisualBullet>(Spawnables.I.BulletTrail, Vector3.zero, Quaternion.identity);
        b.TimeRemaining = 1f;
        b.Set(this.BulletSpawn.position, pos);
    }

    public void Anim_Reload()
    {
        BulletsInMag = MaxBulletsInMag(); // Pool chack
        this.anim.SetBool("Reload", false);

        this.Reloading = false;

        if (!BulletInChamber)
        {
            this.anim.SetBool("Chamber", true);
            this.Chambering = true;
        }
    }
}