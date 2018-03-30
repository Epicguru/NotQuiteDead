
using UnityEngine;

public static class VectorExtensions
{
    public static Vector2 ToAbs(this Vector2 vector)
    {
        Vector2 v = new Vector2();
        v.x = Mathf.Abs(vector.x);
        v.y = Mathf.Abs(vector.y);

        return v;
    }

    public static Vector3 ToAbs(this Vector3 vector)
    {
        Vector3 v = new Vector3();
        v.x = Mathf.Abs(vector.x);
        v.y = Mathf.Abs(vector.y);
        v.z = Mathf.Abs(vector.z);

        return v;
    }

    public static Vector2 ClampToWorldIndex(this Vector2 vector, World world)
    {
        if (world == null)
            return vector;

        vector = vector.ToInt();

        vector.x = Mathf.Clamp(vector.x, 0, world.TileMap.Width - 1);
        vector.y = Mathf.Clamp(vector.y, 0, world.TileMap.Height - 1);

        return vector;
    }

    public static Vector2 ToInt(this Vector2 vector)
    {
        vector.x = (int)vector.x;
        vector.y = (int)vector.y;

        return vector;
    }

    public static Vector3 ToInt(this Vector3 vector)
    {
        vector.x = (int)vector.x;
        vector.y = (int)vector.y;
        vector.z = (int)vector.z;

        return vector;
    }
}