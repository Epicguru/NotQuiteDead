using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkIdentity))]
public class ThrowableInstance : NetworkBehaviour
{
    [SyncVar]
    public Vector2 TargetPosition;
    public float Speed = 15f;
    public float Rotation;

    private Vector2 startPos;
    private float p;
    private float time;
    private float timer;

    public void Start()
    {
        startPos = transform.position;
        float distance = Vector2.Distance(transform.position, TargetPosition);
        time = distance / Speed; // About as much physics as I know :/
    }

    public void Update()
    {
        if (!isServer)
            return;

        transform.Rotate(0, 0, Rotation * Time.deltaTime);

        // Move towards the target.
        timer += Time.deltaTime;

        float p = Mathf.Clamp(timer / time, 0f, 1f);
        Vector2 newPos = Vector2.Lerp(startPos, TargetPosition, p);

    }
}