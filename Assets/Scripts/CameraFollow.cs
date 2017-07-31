using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    public Transform Target;
    public float Z;

    public void Update()
    {
        if (Target == null)
            return;

        transform.Translate((Target.position - transform.position));
        transform.Translate(0, 0, Z - transform.position.z);
    }
}
