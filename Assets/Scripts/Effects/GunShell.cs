using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShell : MonoBehaviour {

    public Sprite sprite;
    public Vector2 Velocity;
    public Vector2 Gravity;
    public float Rotation;
    public float Time;

    private float StartTime;
    private SpriteRenderer r;

	public void Init(Sprite sprite, Vector2 velocity, Vector2 gravity, float rotation, float time, bool flipX)
    {
        this.Rotation = rotation;
        this.Gravity = gravity;
        this.Velocity = velocity;
        this.Time = time;
        this.StartTime = time;

        r = GetComponentInChildren<SpriteRenderer>();
        r.sprite = sprite;
        r.flipX = flipX;
    }

    private static Color colour = new Color();
    public void Update()
    {
        Time -= UnityEngine.Time.deltaTime;

        if(Time <= 0f)
        {
            ObjectPool.Destroy(gameObject, PoolType.BULLET_SHELL);
            return;
        }

        colour.r = 1;
        colour.g = 1;
        colour.b = 1;
        colour.a = Time / StartTime * 2f;
        r.color = colour;

        Velocity += Gravity * UnityEngine.Time.deltaTime;

        transform.Rotate(0, 0, Rotation * UnityEngine.Time.deltaTime);
        transform.Translate(Velocity * UnityEngine.Time.deltaTime, Space.World);
    }
}
