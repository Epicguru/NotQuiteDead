using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody2D))]
public class AI : NetworkBehaviour {

    // Is a class that manages the direction and speed of an AI controlled object.

    public float Speed = 5f;
    public bool Active = true;

    private Vector2 direction;
    private int count;
    private Rigidbody2D r;

    public void Start()
    {
        r = GetComponent<Rigidbody2D>();
        r.gravityScale = 0;
        r.bodyType = RigidbodyType2D.Dynamic;
        r.freezeRotation = true;
    }

    public void Add(Vector2 direction, float weight)
    {
        this.direction += direction.normalized * weight;
        count++;
    }

    public void LateUpdate()
    {
        if (!Active || !hasAuthority)
        {
            direction.x = 0;
            direction.y = 0;
            r.velocity = direction;
            return;
        }

        direction *= Speed;
        if(direction.magnitude > Speed)
        {
            direction.Normalize();
            direction *= Speed;
        }
        // No delta time because it is moved by the rigidbody.

        r.velocity = direction;

        direction = Vector2.zero;
        count = 0;
    }
}
