
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TileMap : NetworkBehaviour
{
    // Is the networked part, contains the layers.
    // In charge of updating tile bounds and that kind of stuff.

    public int Width { get; private set; }
    public int Height { get;  private set; }

    public TileLayer[] Layers;

    public void Create(int width, int height)
    {
        // Sets size and initializes all layers.

        if (width <= 0 || height <= 0)
        {
            Debug.LogError("Width and height must be greater than zero!");
            return;
        }

        this.Width = width;
        this.Height = height;

        foreach(TileLayer layer in Layers)
        {
            layer.Create(Width, Height);
        }
    }

    public void Update()
    {
        RectInt bounds = GetCameraChunkBounds();
        foreach(TileLayer layer in Layers)
        {
            layer.SelectiveLoad(bounds.xMin / Layers[0].ChunkSize, bounds.yMin / Layers[0].ChunkSize, bounds.xMax / Layers[0].ChunkSize, bounds.yMax / Layers[0].ChunkSize);
        }
    }

    public RectInt GetCameraChunkBounds()
    {
        if (CameraBounds.Instance == null)
        {
            return new RectInt();
        }

        float chunkSize = (float)Layers[0].ChunkSize;

        float tileStartX = CameraBounds.Instance.Bounds.xMin;
        float tileStartY = CameraBounds.Instance.Bounds.yMin;

        float tileEndX = CameraBounds.Instance.Bounds.xMax;
        float tileEndY = CameraBounds.Instance.Bounds.yMax;

        int chunkStartX = (int)(Mathf.Floor(tileStartX / chunkSize) * chunkSize);
        int chunkStartY = (int)(Mathf.Floor(tileStartY / chunkSize) * chunkSize);

        int chunkEndX = (int)(Mathf.Ceil(tileEndX / chunkSize) * chunkSize);
        int chunkEndY = (int)(Mathf.Ceil(tileEndY / chunkSize) * chunkSize);

        return new RectInt(chunkStartX, chunkStartY, chunkEndX, chunkEndY);
    }

    public void OnDrawGizmos()
    {
        RectInt bounds = GetCameraChunkBounds();

        Gizmos.color = Color.red;

        Gizmos.DrawLine(new Vector3(bounds.xMin, bounds.yMin), new Vector3(bounds.xMax, bounds.yMin)); // Bottom line
        Gizmos.DrawLine(new Vector3(bounds.xMin, bounds.yMax), new Vector3(bounds.xMax, bounds.yMax)); // Top line
        Gizmos.DrawLine(new Vector3(bounds.xMin, bounds.yMin), new Vector3(bounds.xMin, bounds.yMax)); // Right line
        Gizmos.DrawLine(new Vector3(bounds.xMax, bounds.yMin), new Vector3(bounds.xMax, bounds.yMax)); // Left line
    }
}
