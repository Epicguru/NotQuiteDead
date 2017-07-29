using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodParticle : MonoBehaviour {

    public float XRange, MinY, MaxY, Decay;

    private new SpriteRenderer renderer;
    private Vector2 velocity = new Vector2();
    private float time = 2;

    public void Start()
    {
        renderer = GetComponentInChildren<SpriteRenderer>();
        renderer.sprite = Spawnables.I.BloodParticles[Random.Range(0, Spawnables.I.BloodParticles.Length - 1)];

        velocity.Set(Random.Range(-XRange, XRange), Random.Range(MinY, MaxY));
    }

    public void Update()
    {
        velocity.y -= Decay * Time.deltaTime;

        transform.Translate(velocity.x * Time.deltaTime, velocity.y * Time.deltaTime, 0);

        time -= Time.deltaTime;
        if (time <= 0)
            Destroy(this.gameObject);
    }
}