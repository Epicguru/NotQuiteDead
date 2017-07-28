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

    public void Start()
    {
        anim = GetComponentInChildren<PlayerAnim>();
    }

    public void Update()
    {
        MoveToTarget();
        CheckHealth();
    }

    public void Shot(Hit hit)
    {
        Health -= hit.Damage;
    }

    public void CheckHealth()
    {
        if(Health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Health = 0;

        // TODO
        Destroy(this.gameObject);
    }

    public void MoveToTarget()
    {
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