
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

    [Header("Chunk & Texture")]
    public MeshTexture Texture;
    public MeshGen Mesh;
    public ChunkPhysics Physics;
    public ChunkBackground Background;

    [Header("Lighting")]
    public LightMesh LightMesh;

    public TileLayer Layer;

    public bool Loaded { get; private set; }

    public void Create(int x, int y, int width, int height, int index, TileLayer layer)
    {
        // Add one to instance count, debugging only.
        InstanceCount++;

        // Deactivate composite to avoid unecessary geometry generation when loading.
        Loaded = false;
        Physics.DeactivateComposite();

        X = x;
        Y = y;
        Width = width;
        Height = height;

        Background.ChunkSize = Width;

        Index = index;
        Layer = layer;

        Background.BG = Backgrounds.GetBG(layer.GetBackgroundAt(Index));

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
        //Mesh.GenMesh();
        Texture.BuildTexture();
        Physics.BuildMap();

        gameObject.name = "Chunk (" + X + ", " + Y + ")";

        // Set position
        transform.localPosition = new Vector3(X * Width * Mesh.TileSize, Y * Height * Mesh.TileSize, 0);

        // Light mesh init.
        LightMesh.Chunk.ChunkX = X;
        LightMesh.Chunk.ChunkY = Y;
    }

    public void DoneLoading()
    {
        // Called when the chunk is done loading, or generating. See property Loaded.
        Loaded = true;

        // Enable physics collision generation again.
        Physics.ActivateComposite();

        // Assign the texture to the background.
        Background.DoneLoading();

        // Update lighting...
        LightMesh.UpdateLighting();
    }

    public void TileChanged(BaseTile tile, int x, int y)
    {
        // Update lighting...
        UpdateLight();

        int localX = x - (X * Width);
        int localY = y - (Y * Height);

        // TODO when a chunk is loaded in, the chunks around it also need lighting updated.

        if(localX == 0)
        {
            // On left edge, update any chunk to the left too.
            if(Layer.IsChunkLoaded(X - 1, Y))
            {
                Layer.GetChunkFromChunkCoords(X - 1, Y).UpdateLight();
            }
        }
        if(localY == 0)
        {
            // On bottom edge, update any chunk below here too.
            if (Layer.IsChunkLoaded(X, Y - 1))
            {
                Layer.GetChunkFromChunkCoords(X, Y - 1).UpdateLight();
            }
        }
        if(localX == Width - 1)
        {
            // On right edge, update any chunk to the right too.
            if (Layer.IsChunkLoaded(X + 1, Y))
            {
                Layer.GetChunkFromChunkCoords(X + 1, Y).UpdateLight();
            }
        }
        if(localY == Height - 1)
        {
            // On top edge, update any chunk above too.
            if (Layer.IsChunkLoaded(X, Y + 1))
            {
                Layer.GetChunkFromChunkCoords(X, Y + 1).UpdateLight();
            }
        }
    }

    public void UpdateLight()
    {
        // Update lighting...
        LightMesh.UpdateLighting();
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