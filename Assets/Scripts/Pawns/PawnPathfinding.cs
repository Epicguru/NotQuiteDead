
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PawnPathfinding : NetworkBehaviour
{
    [Tooltip("Measured in tiles per second.")]
    public float MovementSpeed = 5f;

    [ReadOnly]
    [SerializeField]
    private Vector2Int target;

    public bool Pathfind = false;

    public string Layer = "Foreground";

    [HideInInspector]
    public Pawn Pawn;

    [HideInInspector]
    public List<Node> path;

    private TileLayer layer;

    public void Awake()
    {
        Pawn = GetComponent<Pawn>();
    }

    public void SetTarget(Vector2Int pos)
    {
        SetTarget(pos.x, pos.y);
    }

    public TileLayer GetLayer()
    {
        if (layer == null)
            layer = World.Instance.TileMap.GetLayer(Layer);

        return layer;
    }

    public void SetTarget(int x, int y)
    {
        // Enable pathfinding, if it was disabled.
        Pathfind = true;

        // To do pathfinding, we plot a path to the target.
        // The we move to the first point. Once we reach the first point, we plot again.
        // This ensure that we always reach the target.
        // When moving towards the first point, check to see if it is solid.
        // If it becomes solid while we are walking towards it, plot again.


        // Validations.
        if (GetLayer() == null)
            return;
        if (x < 0)
            x = 0;
        if (y < 0)
            y = 0;
        if (x >= GetLayer().Width)
            x = GetLayer().Width - 1;
        if (y >= GetLayer().Height)
            y = GetLayer().Height - 1;

        this.target = new Vector2Int(x, y);
    }

    public virtual Vector2Int GetCurrentPosition()
    {
        return new Vector2Int((int)transform.position.x, (int)transform.position.y);
    }

    public virtual string GetID()
    {
        return Pawn.Prefab + Pawn.ID;
    }

    public void SetPath(List<Node> path)
    {
        if(path != null)
            Pathfind = true;

        this.path = path;
    }

    public void Update()
    {
        if (!isServer)
            return;

        if (!Pathfind)
            return;

        if(path != null)
        {
            // TODO walk here.
        }
        else
        {
            Vector2Int pos = GetCurrentPosition();
            Pathfinding.Find(GetID(), pos.x, pos.y, target.x, target.y, GetLayer(), SetPath);
        }
    }

    public void OnDrawGizmosSelected()
    {
        if (path != null)
            Pathfinding.DrawPathLine(path, Color.magenta);
    }
}