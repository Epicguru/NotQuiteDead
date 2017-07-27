using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    public GameObject MagPrefab, RealMag;

    public int MAX_BULLETS_IN_MAG = 20;

    public bool BulletInChamber;
    public int BulletsInMag = 9;
    public bool Reloading;
    public bool Chambering;

    [HideInInspector] public bool Stored;
    private bool localStored;
    private Animator anim;
    private Hands hands;

    public void Start()
    {
        anim = GetComponentInChildren<Animator>();
        hands = GetComponentInParent<Hands>();
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

    public void SpawnMag()
    {
        if(RealMag == null || MagPrefab == null)
        {
            return;
        }

        Instantiate(MagPrefab, RealMag.transform.position, RealMag.transform.rotation).GetComponent<SpriteRenderer>().flipX = !hands.Right;
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