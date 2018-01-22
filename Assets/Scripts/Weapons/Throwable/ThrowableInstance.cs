using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(NetPositionSync))]
public class ThrowableInstance : NetworkBehaviour
{
    [SyncVar]
    public Vector2 TargetPosition;
    public string Team;
    public float Speed = 15f;
    public float Rotation;

    [HideInInspector]
    public bool Flying { get; private set; }

    private Vector2 startPos;
    private float p;
    private float time;
    private float timer;

    public void Start()
    {
        if (isServer)
        {
            startPos = transform.position;
            float distance = Vector2.Distance(transform.position, TargetPosition);
            time = distance / Speed; // About as much physics as I know :/
            Flying = true;
        }
    }

    public void Update()
    {
        if (!isServer)
            return;

        if (Flying)
        {
            transform.Rotate(0, 0, Rotation * Time.deltaTime);

            // Move towards the target.
            timer += Time.deltaTime;

            float p = Mathf.Clamp(timer / time, 0f, 1f);

            Vector2 newPos = Vector2.Lerp(startPos, TargetPosition, p);
            float dst = Vector2.Distance(transform.position, newPos);

            // Raycast from our current position to the target position, in order to determine of we hit something.
            // Use the Health.CanHitObject to determine a hit.

            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, newPos - TargetPosition, dst);

            bool canContinue = true;
            Vector2 endPos;
            foreach (var hit in hits)
            {
                if (Health.CanHitObject(hit.collider, Team))
                {
                    // Stop!
                    canContinue = false;
                    endPos = hit.point;
                    break;
                }
            }

            if (!canContinue)
                Flying = false;
            else
                transform.position = newPos;

            // Check to see if we have reached the end of the line
            if(p == 1f)
            {
                Flying = false;
            }
        }
    }
}