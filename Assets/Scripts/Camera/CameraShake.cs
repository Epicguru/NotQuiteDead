using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // EDIT - Its late and (most of) this is not my code, too tired to think right now. Thanks to https://gist.github.com/jakevsrobots/7649147.

    public Camera Camera;

    public float ShakeStrength = 5;
    public float Shake = 0;

    Vector3 originalPosition;

    public void LateUpdate()
    {
        originalPosition = Camera.transform.position;
        Camera.transform.localPosition = originalPosition + (Random.insideUnitSphere * Shake);

        Shake = Mathf.MoveTowards(Shake, 0, Time.deltaTime * ShakeStrength);

        if (Shake == 0)
        {
            Camera.transform.localPosition = originalPosition;
        }
    }

    public void ShakeCamera(float strength)
    {
        ShakeStrength = strength;
        Shake = ShakeStrength;
    }
}
