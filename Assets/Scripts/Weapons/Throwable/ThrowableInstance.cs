using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkIdentity))]
public class ThrowableInstance : NetworkBehaviour {

    [SyncVar]
    public Vector2 Target; // Target position.
    [SyncVar]
    public Vector2 StartPosition; // Starting position
    public float ThrowTime = 1f;
    public AnimationCurve Curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public delegate void UponHitTarget();
    public event UponHitTarget AuthorityHitTarget;

    private float timer;

    public void Update()
    {
        timer += Time.deltaTime;

        // Get position based on time.
        float p = Mathf.Clamp(timer / ThrowTime, 0f, 1f);
        // Evaluate using curve to allow custom motion.
        float x = Curve.Evaluate(p);
        
        transform.position = Vector2.Lerp(StartPosition, Target, x);

        if(p >= 1f)
        {
            // Done!
            if(isServer)
                if (AuthorityHitTarget != null)
                    AuthorityHitTarget();
        }
    }
}
