
using UnityEngine;
using UnityEngine.Networking;

public class TurretShooting : NetworkBehaviour
{
    public Turret Turret;
    public Transform Muzzle;
    public Transform MuzzleFlash;
    public AudioSauce Sauce;
    public AudioClip Clip;
    public Vector2 Volume;
    public Vector2 Pitch;

    public GameObject Prefab;

    public float Damage;
    public string DamageDealer;
    public string Team;

    public void Fire()
    {
        // Called on clients and servers when the animation plays!

        if (isClient)
        {
            // Play sound and muzzle flash.
            Sauce.Clip = Clip;
            Sauce.Volume = Random.Range(Volume.x, Volume.y);
            Sauce.Pitch = Random.Range(Pitch.x, Pitch.y) * Time.timeScale;
            Sauce.Play();

            // Place muzzle flash.
            GameObject g = ObjectPool.Instantiate(Prefab, PoolType.MUZZLE_FLASH);
            g.transform.position = MuzzleFlash.transform.position;
            g.transform.rotation = MuzzleFlash.transform.rotation;
        }

        if (isServer)
        {
            if (Player.Local == null)
                return;

            // Hit enemies in front of the gun.
            RaycastHit2D[] hits = Physics2D.RaycastAll(Muzzle.transform.position, Muzzle.up, Turret.Range);

            foreach(var hit in hits)
            {
                float dst = Vector2.Distance(transform.position, hit.point);
                if (dst > Turret.Range)
                {
                    break;
                }

                Collider2D c = hit.collider;

                bool shouldHit = Health.CanHitObject(c, null); // Nulls allows damage to own team!

                if (shouldHit)
                {
                    // We should HIT the object; but can we damage it?
                    bool canDamage = Health.CanDamageObject(c, null); // Null allows damage to own team!
                    if (canDamage)
                    {
                        Health h = hit.transform.GetComponentInParent<Health>();

                        // Should never be null thanks to the CanDamage check, but double check for null.
                        if (h != null)
                        {
                            h.ServerDamage(Damage, DamageDealer, false);
                        }
                    }

                    // Weather we can damage or not, break. We have hit a solid barrier.
                    break;
                }
            }

        }
    }
}