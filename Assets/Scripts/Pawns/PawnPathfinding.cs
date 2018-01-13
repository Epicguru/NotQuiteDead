
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PawnPathfinding : NetworkBehaviour
{
    [Tooltip("Measured in tiles per second.")]
    public float MovementSpeed = 5f;

    //[ReadOnly]
    [SerializeField]
    private Vector2Int target;

    public bool Pathfind = false;

    public string Layer = "Foreground";

    [HideInInspector]
    public Pawn Pawn;

    [HideInInspector]
    public List<Node> path;


    private List<Node> pathPreloaded;
    private TileLayer layer;
    private float timer;

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

    private void SetPath(List<Node> path)
    {
        this.path = path;
    }

    private void SetPathPreloaded(List<Node> path)
    {
        this.pathPreloaded = path;
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
            if (!Pathfinding.HasRequested(GetID()))
            {
                Vector2Int pos = GetCurrentPosition();
                Pathfinding.Find(GetID(), pos.x, pos.y, target.x, target.y, GetLayer(), SetPath);
            }
        }

        NavigatePath();
    }

    private void NavigatePath()
    {
        // Path is not null, and we need to walk it and also plan a few steps ahead to make a seamless walk.

        if (path == null || path.Count < 2)
        {
            path = null;
            timer = 0f;
            return;
        }

        if (!isServer)
            return;

        timer += Time.deltaTime;
        Vector2 lastNode = new Vector2(path[0].X + 0.5f, path[0].Y + 0.5f);
        Vector2 targetNode = new Vector2(path[1].X + 0.5f, path[1].Y + 0.5f);
        float distance = Vector2.Distance(lastNode, targetNode);

        // If we are 1 tile away, the duration = 1 / MovementSpeed;
        float duration = distance / MovementSpeed;

        float p = Mathf.Clamp(timer / duration, 0f, 1f);

        Vector2 position = Vector2.Lerp(lastNode, targetNode, p);

        transform.position = position;

        // Need to find a new path, if not already preloaded.
        if (pathPreloaded == null)
        {
            Pathfinding.Find(GetID(), path[1].X, path[1].Y, target.x, target.y, GetLayer(), SetPathPreloaded);
        }

        if (p == 1)
        {
            // Check for a preloaded path.
            if (pathPreloaded != null)
            {
                timer = 0;
                path = pathPreloaded;
                pathPreloaded = null;
            }
        }
    }

    public void OnDrawGizmosSelected()
    {
        if (path != null)
            Pathfinding.DrawPathLine(path, Color.magenta);

        if (GetLayer() == null)
            return;

        bool inBounds = GetLayer().InLayerBounds(target.x, target.y);

        bool inSolid = GetLayer().GetTile(target.x, target.y) != null;

        Color c = Color.green;

        if (!inBounds || inSolid)
            c = Color.red;

        Gizmos.color = c;
        Gizmos.DrawLine(new Vector3(target.x, target.y), new Vector3(target.x + 0.9f, target.y + 0.9f));
        Gizmos.DrawLine(new Vector3(target.x, target.y + 0.9f), new Vector3(target.x + 0.9f, target.y));
    }
}