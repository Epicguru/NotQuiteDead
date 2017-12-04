
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TileMap : NetworkBehaviour
{
    // Is the networked part, contains the layers.
    // In charge of updating tile bounds and that kind of stuff.

    public int ChunkSize;

    public int Width
    {
        get
        {
            return WidthInChunks * ChunkSize;
        }
    }

    public int Height
    {
        get
        {
            return HeightInChunks * ChunkSize;
        }
    }

    public int WidthInChunks;
    public int HeightInChunks;

    public TileLayer[] Layers;

    public void Create()
    {
        // Sets size and initializes all layers.

        if (WidthInChunks <= 0 || HeightInChunks <= 0)
        {
            Debug.LogError("Width and height (in chunks) must be greater than zero!");
            return;
        }

        foreach(TileLayer layer in Layers)
        {
            layer.ChunkSize = ChunkSize;
            layer.Create(Width, Height);
        }
    }

    public void Update()
    {
        RectInt bounds = GetCameraChunkBounds();
        foreach(TileLayer layer in Layers)
        {
            layer.SelectiveLoad(bounds.xMin / ChunkSize, bounds.yMin / ChunkSize, bounds.xMax / ChunkSize, bounds.yMax / ChunkSize);
        }
    }

    public RectInt GetCameraChunkBounds()
    {
        float chunkSize = ChunkSize;

        float tileStartX = CameraBounds.Instance.Bounds.xMin;
        float tileStartY = CameraBounds.Instance.Bounds.yMin;

        if (tileStartX < 0)
            tileStartX = 0;

        if (tileStartY < 0)
            tileStartY = 0;

        float tileEndX = CameraBounds.Instance.Bounds.xMax;
        float tileEndY = CameraBounds.Instance.Bounds.yMax;

        if (tileEndX >= Width - 1)
            tileEndX = Width - 1;

        if (tileEndY >= Height - 1)
            tileEndY = Height - 1;

        int chunkStartX = (int)(Mathf.Floor(tileStartX / chunkSize) * chunkSize);
        int chunkStartY = (int)(Mathf.Floor(tileStartY / chunkSize) * chunkSize);

        int chunkEndX = (int)(Mathf.Ceil(tileEndX / chunkSize) * chunkSize);
        int chunkEndY = (int)(Mathf.Ceil(tileEndY / chunkSize) * chunkSize);

        return new RectInt(chunkStartX, chunkStartY, chunkEndX - chunkStartX, chunkEndY - chunkStartY);
    }

    public void OnDrawGizmos()
    {
        if (CameraBounds.Instance == null)
        {
            return;
        }

        Gizmos.color = Color.green;

        float z = 10;

        for (int x = 0; x < WidthInChunks; x++)
        {
            Gizmos.DrawLine(new Vector3(x * ChunkSize, 0, z), new Vector3(x * ChunkSize, Height, z));
        }

        for (int y = 0; y < HeightInChunks; y++)
        {
            Gizmos.DrawLine(new Vector3(0, y * ChunkSize, z), new Vector3(Width, y * ChunkSize, z));
        }

        Gizmos.color = Color.black;

        RectInt bounds = GetCameraChunkBounds();

        Gizmos.DrawLine(new Vector3(bounds.xMin, bounds.yMin), new Vector3(bounds.xMax, bounds.yMin)); // Bottom line
        Gizmos.DrawLine(new Vector3(bounds.xMin, bounds.yMax), new Vector3(bounds.xMax, bounds.yMax)); // Top line
        Gizmos.DrawLine(new Vector3(bounds.xMin, bounds.yMin), new Vector3(bounds.xMin, bounds.yMax)); // Right line
        Gizmos.DrawLine(new Vector3(bounds.xMax, bounds.yMin), new Vector3(bounds.xMax, bounds.yMax)); // Left line
    }
}
