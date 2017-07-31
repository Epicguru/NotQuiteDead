using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public Transform Target;
    public float Speed = 4f;
    public float Deadzone = 0.1f;
    public float Health;

    private PlayerAnim anim;
    private Vector2 vel = new Vector2();
    private bool dead;
    private float timeDead = 0;

    public void Start()
    {
        anim = GetComponentInChildren<PlayerAnim>();
    }

    public void Update()
    {
        MoveToTarget();
        CheckHealth();
        UpdateDeath();
    }

    public void Shot(Hit hit)
    {
        Health -= hit.Damage;

        // Spawn blood spurt
        GameObject o = Instantiate(Spawnables.I.BloodSpurt, hit.Position, Quaternion.Euler(Vector2.Angle(hit.Position, hit.Normal), 90, 0));
        Destroy(o, 1f);

        if(Random.Range(0f, 100f) <= 25f)
        {
            GameObject b = Instantiate(Spawnables.I.BloodBleeding, hit.Position, Quaternion.Euler(Vector2.Angle(hit.Position, hit.Normal), 90, 0), hit.Collider.gameObject.transform);
            Destroy(b, Random.Range(2f, 6f));
        }
    }

    public void UpdateDeath()
    {
        if (!dead)
            return;

        anim.Dead = true;
        timeDead += Time.deltaTime;

        if (timeDead > 30)
            Destroy(this.gameObject);
    }

    public void CheckHealth()
    {
        if (dead)
            return;
        if(Health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (dead)
            return;
        Health = 0;
        dead = true;

        foreach(Collider2D c in GetComponentsInChildren<Collider2D>())
        {
            Destroy(c);
        }
    }

    public void MoveToTarget()
    {
        if (dead)
            return;
        if (Target == null)
        {
            anim.Walking = false;
            return;
        }

        if (Vector3.Distance(Target.position, transform.position) <= Deadzone)
        {
            anim.Walking = false;
            return;
        }

        anim.Walking = true;

        vel.Set(0, 0);

        float dstX = Target.position.x - transform.position.x;
        float dstY = Target.position.y - transform.position.y;

        float angle = Mathf.Atan2(dstY, dstX);

        float xVel = Mathf.Cos(angle);

        if(xVel > 0)
        {
            anim.Right = true;
        }
        else if(xVel < 0)
        {
            anim.Right = false;
        }

        float yVel = Mathf.Sin(angle);

        vel.Set(xVel, yVel);
        vel.Normalize();
        vel *= Speed;
        vel *= Time.deltaTime;

        transform.Translate(vel.x, vel.y, 0f);
    }

}