
using UnityEngine;

[RequireComponent(typeof(MeshGen), typeof(MeshTexture))]
public class Chunk : MonoBehaviour
{
    public static int InstanceCount;

    public int Width, Height;

    public MeshTexture Texture;
    public MeshGen Mesh;

    public void Awake()
    {
        InstanceCount++;
    }

    public void OnDestroy()
    {
        InstanceCount--;
    }

    public bool InBounds(int x, int y)
    {
        if (x < 0 || y < 0)
            return false;
        if (x >= Width || y >= Height)
            return false;

        return true;
    }
}