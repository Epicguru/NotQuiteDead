using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(ActiveObject))]
public class Furniture : NetworkBehaviour
{
    public string Prefab;
    public string Name;

    public string Layer = "Foreground";

    public int X { get; private set; }
    public int Y { get; private set; }
    public float Z = 0;

    [HideInInspector]
    public ActiveObject AO;

    private TileLayer layer;

    public void Awake()
    {
        AO = GetComponent<ActiveObject>();
    }

    public TileLayer GetLayer()
    {
        if(layer == null)
        {
            layer = World.Instance.TileMap.GetLayer(Layer);
        }

        if(layer != null)
        {
            if(layer.Name != this.Layer)
            {
                layer = World.Instance.TileMap.GetLayer(this.Layer);
            }
        }

        return layer;
    }

    public void SetPosition(int x, int y)
    {
        if(GetLayer().InLayerBounds(x, y))
        {
            this.X = x;
            this.Y = y;
        }

        transform.position = new Vector3(X, Y, Z);
    }
}