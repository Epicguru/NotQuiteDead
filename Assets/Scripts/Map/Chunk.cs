
using UnityEngine;

[RequireComponent(typeof(MeshGen), typeof(MeshTexture))]
public class Chunk : MonoBehaviour
{
    public static int InstanceCount;

    public int X { get; private set; }
    public int Y { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }

    public int Index { get; private set; }

    public MeshTexture Texture;
    public MeshGen Mesh;
    public ChunkPhysics Physics;

    public void Create(int x, int y, int width, int height, int index)
    {
        // Add one to instance count, debugging only.
        InstanceCount++;

        X = x;
        Y = y;
        Width = width;
        Height = height;

        Index = index;

        // Setup chunk.
        Init();
    }

    public void OnDestroy()
    {
        InstanceCount--;
    }

    public void Init()
    {
        // Gen mesh, then build a texture, then finally init the physics class.
        Mesh.GenMesh();
        Texture.BuildTexture();
        Physics.BuildMap();

        gameObject.name = "Chunk (" + X + ", " + Y + ")";

        // Set position
        transform.localPosition = new Vector3(X * Width * Mesh.TileSize, Y * Height * Mesh.TileSize, 0);
    }

    public void LateUpdate()
    {
        // If the texture is dirty then apply it.
        if (Texture.Dirty)
            Texture.Apply();
    }

    public bool InBounds(int x, int y)
    {
        // Checks if a tile in within the bounds of the chunk. Local coordinates, where zero is the bottom left of the chunk.
        if (x < 0 || y < 0)
            return false;
        if (x >= Width || y >= Height)
            return false;

        return true;
    }
}