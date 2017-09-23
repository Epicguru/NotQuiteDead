using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingMag : MonoBehaviour {

    public static GameObject Prefab;

    public Vector2 Gravity;
    public Vector2 Velocity;
    public float AngularVelocity;
    public float Time = 2f;
    private float maxTime;

    public void Init(Vector2 position, Vector2 velocity, float rotation, float angularVelocity, float time, Sprite sprite, bool flip)
    {
        transform.position = position;
        transform.rotation = Quaternion.Euler(0, 0, rotation);
        this.Velocity = velocity;
        this.AngularVelocity = angularVelocity;
        this.Time = time;
        maxTime = Time;
        GetComponent<SpriteRenderer>().sprite = sprite;
        GetComponent<SpriteRenderer>().flipX = flip;
    }

    public void Update()
    {
        Time -= UnityEngine.Time.deltaTime;

        if(Time <= 0)
        {
            ObjectPool.Destroy(gameObject, PoolType.FLYING_MAG);
            return;
        }

        transform.Rotate(0, 0, AngularVelocity * UnityEngine.Time.deltaTime);
        transform.Translate(Velocity * UnityEngine.Time.deltaTime, Space.World);

        Velocity.y += Gravity.y * UnityEngine.Time.deltaTime;
        float dst = Velocity.x;
        if (dst < 0)
            dst *= -1f;

        Velocity.x -= Mathf.Clamp(dst * (Velocity.x > 0 ? 1f : -1f), 0f, 2.5f * UnityEngine.Time.deltaTime);

        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, Time / maxTime);
    }

    public static void Spawn(Vector2 position, Vector2 velocity, float rotation, float angularVelocity, Sprite sprite, bool flip)
    {
        if(Prefab == null)
        {
            Prefab = Resources.Load<GameObject>("Items/Effects/Mag");
        }

        // Spawn at position and z rotation.
        GameObject GO = ObjectPool.Instantiate(Prefab, PoolType.FLYING_MAG);
        FlyingMag mag = GO.GetComponent<FlyingMag>();

        mag.Init(position, velocity, rotation, angularVelocity, 3f, sprite, flip);
    }
}