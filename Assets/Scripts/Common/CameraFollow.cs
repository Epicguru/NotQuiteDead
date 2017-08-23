using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Transform Target;
    public bool PhysicsMode = true;

    public void FixedUpdate()
    {
        if (!PhysicsMode || Target == null)
            return;

        transform.position = Target.position;
        transform.Translate(transform.forward * -10);
    }

    public void Update()
    {
        if (PhysicsMode || Target == null)
            return;

        transform.position = Target.position;
        transform.Translate(transform.forward * -10);
    }
}
