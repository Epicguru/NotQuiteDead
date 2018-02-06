
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PawnPathfinding : NetworkBehaviour
{
    public const int MAX_SKIPPED = 20;

    [Tooltip("Measured in tiles per second.")]
    public float MovementSpeed = 5f;

    [SerializeField]
    private Vector2Int target;

    public bool Pathfind = false;

    public string Layer = "Foreground";

    [HideInInspector]
    public Pawn Pawn;

    [HideInInspector]
    public List<Node> path;

    public bool MovingRight { get; private set; }
    public bool Moving { get; private set; }

    private List<Node> pathPreloaded;
    private List<Node> backupPath;
    private TileLayer layer;
    private float timer;
    private int skipped;
    private bool forcePause;

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
        if (World.Instance == null)
            return null;

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

    public void Update()
    {
        if (!isServer)
            return;

        if(Pawn.Health.GetHealth() <= 0f)
        {
            Moving = false;
            Pathfind = false;
            return;
        }

        Moving = false;
        NavigatePath();
    }

    public bool AtTarget()
    {
        // TODO optimise;
        return Vector2.Distance(transform.position, target + new Vector2(0.5f, 0.5f)) <= 0f;
    }

    private bool WalkSegment()
    {
        // Path is NOT null.
        // Only on server.
        // Just move from the first node to the second based on time and distance.

        if (path == null)
            return true;
        if (path.Count < 2) // Can't walk from A to B if there is no B!
            return true;

        // Increase time.
        timer += Time.deltaTime;
        Moving = true;

        Vector2 lastNode = new Vector2(path[0].X + 0.5f, path[0].Y + 0.5f);
        Vector2 targetNode = new Vector2(path[1].X + 0.5f, path[1].Y + 0.5f);

        float diff = targetNode.x - lastNode.x;
        if(diff != 0)
        {
            MovingRight = diff > 0;
        }

        bool nextIsUnwalkable = false;
        BaseTile tile = GetLayer().GetTile(path[1].X, path[1].Y);
        if (tile != null)
            nextIsUnwalkable = true;

        if (nextIsUnwalkable)
        {
            // Tile that we are moving towards is unwalkable.
            // Indicate that we have reached the end of the segment and snap back to zero.
            timer = 0;
            Moving = false;
            // Any preloaded path is invalid, because the next tile is blocked.
            pathPreloaded = null;
            return true;
        }

        if (lastNode == targetNode)
        {
            Moving = false;
            return true;
        }

        // Get distance.
        float distance = Vector2.Distance(lastNode, targetNode);

        // Time = distance / speed.
        float duration = distance / MovementSpeed;

        // Point along segment = time / total time.
        float p = Mathf.Clamp(timer / duration, 0f, 1f);

        // Forcibly move to that point.
        Vector2 position = Vector2.Lerp(lastNode, targetNode, p);
        transform.position = position;

        // Potato code.
        if (!forcePause)
        {
            timer %= duration;
        }
        else
        {
            if (timer > duration)
                timer = duration;
        }

        // Are we at the end of the segment?
        return p == 1;
    }

    private void PreLoad()
    {
        // Load the path from the 'next' node to the target.

        if(path.Count < 2)
        {
            return;
        }

        if (AtTarget())
        {
            return;
        }

        if(pathPreloaded != null)
        {
            return;
        }

        Pathfinding.Find(GetID(), path[1].X, path[1].Y, target.x, target.y, GetLayer(), SetPathPreloaded);
    }

    private void SetPath(List<Node> path)
    {
        this.path = path;
    }

    private void SetPathPreloaded(List<Node> path)
    {
        this.pathPreloaded = path;
    }

    private void SetBackupPath(List<Node> path)
    {
        backupPath = path;
    }

    private void NavigatePath()
    {
        // Only on server.
        // Beyond that, anything goes.

        if (AtTarget())
        {
            timer = 0;
            Moving = false;
            return;
        }

        if (forcePause)
        {
            Moving = false;
        }

        if (Pathfind == false)
            return;

        if(path == null)
        {
            // Path is null.
            // Are we preloading?
            if (!Pathfinding.HasRequested(GetID()))
            {
                // We are not at the target, we are pathfinding but the path is null.
                // Lets find the path.
                Pathfinding.Find(GetID(), GetCurrentPosition().x, GetCurrentPosition().y, target.x, target.y, GetLayer(), SetPath);
                // Note that if execution is stuck in this part, it is likely that the path is impossible to calculate.
            }
        }
        else
        {
            // There currently is a path to walk! Walk it!
            bool endOfSegment = WalkSegment();

            // Pre-load the next part of the path. We want seamless transition between segments.
            // Don't bother pre loading if at the end of the segment, it should have already been done.
            if(!endOfSegment)
                PreLoad();

            // Are we at the end of the segment?
            if (endOfSegment)
            {
                // This means that we need to recalculate the path.
                // It should already be done loading...
                if(pathPreloaded != null && pathPreloaded[0].Equals(path[1]))
                {
                    // Set the current path to this preloaded, if it is still relevant.
                    path = pathPreloaded;
                    pathPreloaded = null;
                    skipped = 0;
                    backupPath = null;
                    forcePause = false;
                }
                else
                {
                    // Check to see if we have a useless preloaded.
                    if (pathPreloaded != null)
                    {
                        // Dispose of it, we need another one calculated ASAP.
                        pathPreloaded = null;
                    }

                    // We don't have a preloaded path. Maybe we have completed the path.
                    if (AtTarget())
                    {
                        // Cool! We are done.
                        Pathfind = false;
                        timer = 0;
                        path = null;
                    }
                    else
                    {
                        // We have not reached the target, but there is no preloaded...
                        // The pathfinder must be running slow.
                        // Solution for now:
                        // Keep following the existing path.
                        if(path.Count <= 2)
                        {
                            // Reached the end of the path, are at the targets last know location.
                            path = null;
                            Pathfind = false;
                        }
                        else
                        {
                            // Check if the next tile is blocked.
                            bool blocked = GetLayer().GetTile(path[1].X, path[1].Y) != null;

                            // Keep moving.
                            if (!forcePause)
                            {
                                if (!blocked)
                                {
                                    path.RemoveAt(0);
                                }
                                skipped += 1;
                            }
                            if(skipped >= MAX_SKIPPED)
                            {
                                //DebugText.Log("Critical mode: " + GetID() + " x" + skipped, Color.red);
                                //Alright, we have missed the preload deadline too many times now, and more of this leads the AI to walk to completely the wrong place,
                                //assuming that the target is moving.
                                //Now, we just prioritize getting our main Path calculated.
                                // Do this by using our backup path.

                                // We need to stand still until our new path arrives, because otherwise the pathfinder does not have time to catch up.                         

                                forcePause = true;
                                if (backupPath == null)
                                {
                                    Pathfinding.Find(GetID(), path[0].X, path[0].Y, target.x, target.y, GetLayer(), SetBackupPath);
                                }
                                else
                                {
                                    // Finally, our backup path.
                                    path = backupPath;
                                    backupPath = null;
                                    forcePause = false;
                                    skipped = 0;
                                    timer = 0;
                                }
                            }
                        }
                    }
                }
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