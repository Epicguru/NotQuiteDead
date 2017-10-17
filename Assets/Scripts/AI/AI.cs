using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AI : NetworkBehaviour {

    // Is a class that manages the direction and speed of an AI controlled object.

    public float Speed = 5f;
    public bool Active = true;
    public Vector2 Velocity = new Vector2();

    private Vector2 direction;
    private int count;

    public void Add(Vector2 direction, float weight)
    {
        this.direction += direction.normalized * weight;
        count++;
    }

    public void Update()
    {
        UpdateVelocity();
    }

    public void LateUpdate()
    {
        if (!Active || !hasAuthority)
        {
            direction.x = 0;
            direction.y = 0;
            Velocity = direction;
            return;
        }

        direction *= Speed;
        if(direction.magnitude > Speed)
        {
            direction.Normalize();
            direction *= Speed;
        }

        // No delta time, done later.
        Velocity = direction;

        direction = Vector2.zero;
        count = 0;
    }

    public void UpdateVelocity()
    {
        transform.Translate(Velocity * Time.deltaTime, Space.World);
    }
}
