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
    public bool HitObject
    {
        get
        {
            return Attached != null;
        }
    }
    public GameObject Attached;

    private Vector2 start;
    private float timer;

    public void Init(Vector2 position, Quaternion rotation, Vector2 direction, float speed, float maxDistance)
    {
        this.Direction = direction.normalized;
        this.Speed = speed;
        this.MaxDistance = maxDistance;
        transform.position = position;
        transform.rotation = rotation;
        start = position;
        if(Attached != null)
            Attached = null;
        timer = 20f;
    }

    public void Update()
    {
        float dst = Vector2.Distance(start, transform.position);

        if (HitObject)
        {
            if (transform.parent == null)
                transform.SetParent(Attached.transform);
            timer -= Time.deltaTime;
            if(timer <= 0f)
            {
                ObjectPool.Destroy(gameObject, PoolType.ARROW);
                return;
            }
        }
        else
        {
            if (transform.parent != null)
                transform.SetParent(null);
        }
        if(!HitObject && dst >= MaxDistance)
        {
            ObjectPool.Destroy(gameObject, PoolType.ARROW);
            return;
        }


        if(!HitObject)
            transform.Translate(Direction * Speed * Time.deltaTime, Space.World);
    }

    public static void FireArrow(Vector2 position, Quaternion rotation, Vector2 direction, float speed, float maxDistance)
    {
        GameObject GO = ObjectPool.Instantiate(Spawnables.I.Arrow, PoolType.ARROW);
        GO.GetComponent<Arrow>().Init(position, rotation, direction, speed, maxDistance);
    }

    public void OnDisable()
    {
        Attached = null;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        // Stick in the object?
        if (collision.GetComponent<Arrow>() != null)
            return;

        Debug.Log("Trigger enter!");
        Attached = collision.transform.gameObject;
    }
}
