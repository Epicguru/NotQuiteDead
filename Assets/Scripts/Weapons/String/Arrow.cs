using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Arrow : MonoBehaviour {

    // Must network accurately as accurately as possible. We will simply indicate that
    // arrow should be spawned upon animation, but that it will only deal damage on the server side. (Server authority mode activated!).
    // Also this has to be pooled. Yea.

    public float Speed = 10f;
    public Vector2 Direction;
    public float MaxDistance;
    public bool HitObject;
    public float Damage;
    public string DamageTag;
    public string Team;

    private Vector2 start;
    private float timer;
    private bool HitHealth;

    public void Init(Vector2 position, Quaternion rotation, Vector2 direction, float speed, float maxDistance, float damage, string damageTag, string team)
    {
        this.Direction = direction.normalized;
        this.Speed = speed;
        this.MaxDistance = maxDistance;
        transform.position = position;
        transform.rotation = rotation;
        this.Damage = damage;
        this.DamageTag = damageTag;
        this.Team = team;
        start = position;
        HitObject = false;
        timer = 20f;
        GetComponent<Rigidbody2D>().simulated = true;
        HitHealth = false;
    }

    public void Update()
    {
        float dst = Vector2.Distance(start, transform.position);

        if (HitObject)
        {
            timer -= Time.deltaTime;
            if(timer <= 0f)
            {
                ObjectPool.Destroy(gameObject, PoolType.ARROW);
                return;
            }
        }
        if(!HitObject && dst >= MaxDistance)
        {
            ObjectPool.Destroy(gameObject, PoolType.ARROW);
            return;
        }

        if (!HitObject)
            transform.Translate(Direction * Speed * Time.deltaTime, Space.World);
    }

    public static void FireArrow(Vector2 position, Quaternion rotation, Vector2 direction, float speed, float maxDistance, float damage, string damageTag, string team)
    {
        GameObject GO = ObjectPool.Instantiate(Spawnables.I.Arrow, PoolType.ARROW);
        GO.GetComponent<Arrow>().Init(position, rotation, direction, speed, maxDistance, damage, damageTag, team);
    }

    public bool CanHit(Collider2D collider)
    {
        if (collider.GetComponent<Arrow>() != null)
            return false;

        return Health.ShouldHit(collider, this.Team);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        // Stick in the object?

        if (!CanHit(collision))
            return;

        if(transform.parent == null)
            transform.SetParent(collision.transform);

        HitObject = true;
        GetComponent<Rigidbody2D>().simulated = false;

        if(Damage <= 0f)
        {
            return; // Does not deal damage.
        }

        Health h = collision.GetComponentInParent<Health>();
        if (!HitHealth && h != null)
        {
            HitHealth = true;

            if (h.CanHit)
            {
                Player.Local.NetUtils.CmdDamageHealth(h.gameObject, Damage, DamageTag, false);
            }
        }
    }
}
