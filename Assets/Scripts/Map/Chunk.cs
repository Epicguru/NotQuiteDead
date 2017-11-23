
using UnityEngine;

[RequireComponent(typeof(MeshGen), typeof(MeshTexture))]
public class Chunk : MonoBehaviour
{
    public static int InstanceCount;

    public int Width, Height;

    public MeshTexture Texture;
    public MeshGen Mesh;
    public ChunkPhysics Physics;

    public void Awake()
    {
        // Add one to instance count, debugging only.
        InstanceCount++;

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
    }

    public void LateUpdate()
    {
        // Firstly, if the texture is dirty then apply it.
        if (Texture.Dirty)
            Texture.Apply();

        // Next refresh physics geometry when required.
        if (Physics.Dirty)
            Physics.Apply();
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