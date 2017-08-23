using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Transform Target;
    public bool PhysicsMode = true;

    public void FixedUpdate()
    {
        if (!PhysicsMode)
            return;

        transform.position = Target.position;
    }

    public void Update()
    {
        if (PhysicsMode)
            return;

        transform.position = Target.position;
    }
}
