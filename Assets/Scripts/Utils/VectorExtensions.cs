
using UnityEngine;

public static class VectorExtensions
{
    public static Vector3 ToAbs(this Vector3 vector)
    {
        Vector3 v = new Vector3();
        v.x = Mathf.Abs(vector.x);
        v.y = Mathf.Abs(vector.y);
        v.z = Mathf.Abs(vector.z);

        return v;
    }
}