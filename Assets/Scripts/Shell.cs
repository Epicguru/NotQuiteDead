using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour {

    // Move towards Y and negative X

    public bool Right = true;

    public Vector3 Velocity;
    public Transform TurnObject;
    public float Turn;
    public float TimeAlive;
    public float Randomness;

    public void Start()
    {
        Velocity.x *= (Right ? 1 : -1);
        Velocity.x += Random.Range(-Randomness, Randomness);
        Velocity.y += Random.Range(-Randomness * 0.5f, Randomness * 0.5f);

        if (!Right)
        {
            GetComponentInChildren<SpriteRenderer>().flipX = true;
        }
    }

    public void Update()
    {
        TimeAlive -= Time.deltaTime;

        if(TimeAlive <= 0)
        {
            Destroy(this.gameObject);
            return;
        }

        TurnObject.Rotate(0, 0, (Right ? 1 : -1) * Turn * Time.deltaTime);

        Velocity.y -= 15f * Time.deltaTime;

        transform.Translate(Velocity * Time.deltaTime, Space.World);
    }
}