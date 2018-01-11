
using Priority_Queue;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public static class Pathfinding
{
    public const int MAX = 2000;
    private static FastPriorityQueue<Node> open = new FastPriorityQueue<Node>(MAX);
    private static Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
    private static Dictionary<Node, float> costSoFar = new Dictionary<Node, float>();
    private static List<Node> near = new List<Node>();
    private static bool left, right, below, above;
    private static BaseTile tile;

    public static bool Find(string ID, int startX, int startY, int endX, int endY, TileLayer layer, UnityAction<List<Node>> done)
    {
        return PathfindingManager.Find(ID, startX, startY, endX, endY, layer, done);
    }

    public static List<Node> Run(int startX, int startY, int endX, int endY, TileLayer layer, bool clean = false)
    {
        open.Clear();
        cameFrom.Clear();
        costSoFar.Clear();

        Node start = new Node() { X = startX, Y = startY };
        Node end = new Node() { X = endX, Y = endY };

        open.Enqueue(start, 0);
        cameFrom[start] = start;
        costSoFar[start] = 0;

        int count = 1;
        while ((count = open.Count) > 0)
        {
            if (count > MAX - 9)
            {
                Debug.Log("Too many open nodes!");
                if (clean)
                {
                    Clean();
                }
                return null;
            }

            Node current = open.Dequeue();

            if (current.Equals(end))
            {
                // Done!
                List<Node> path = TracePath(end);
                if (clean)
                {
                    Clean();
                }
                return path;
            }

            List<Node> neighbours = GetNear(current, layer);
            foreach (Node n in neighbours)
            {
                float newCost = costSoFar[current] + GetCost(current, n); // This is the cost of the tile! Could be the weight depending on terrain!
                if (!costSoFar.ContainsKey(n) || newCost < costSoFar[n]) // (effectiveCost ? GetCost(current, n) : 0)
                {
                    costSoFar[n] = newCost;
                    float priority = newCost + Heuristic(n, end);
                    open.Enqueue(n, priority);
                    cameFrom[n] = current;
                }
            }
        }

        if (clean)
        {
            Clean();
        }
        return null;
    }

    private static float GetCost(Node a, Node b)
    {
        // Only intended for neighbours.

        // Is directly horzontal
        if (Mathf.Abs(a.X - b.X) == 1 && a.Y == b.Y)
        {
            return 1;
        }

        // Directly vertical.
        if (Mathf.Abs(a.Y - b.Y) == 1 && a.X == b.X)
        {
            return 1;
        }

        // Assume that it is on one of the corners.
        return 1.41421356237f;
    }

    public static void Clean()
    {
        costSoFar.Clear();
        cameFrom.Clear();
        near.Clear();
        tile = null;
    }

    public static void DrawPathLine(List<Node> path, Color colour)
    {
        Gizmos.color = colour == null ? Color.black : colour;

        if (path == null)
        {
            return;
        }

        for (int i = 0; i < path.Count; i++)
        {
            Node last = null;
            if (i != 0)
            {
                last = path[i - 1];
            }
            Node current = path[i];

            if (last != null)
                Gizmos.DrawLine(new Vector3(last.X + 0.5f, last.Y + 0.5f, 0), new Vector3(current.X + 0.5f, current.Y + 0.5f, 0));
        }
    }

    private static List<Node> TracePath(Node end)
    {
        List<Node> path = new List<Node>();
        Node child = end;

        bool run = true;
        while (run)
        {
            Node previous = cameFrom[child];
            path.Add(child);
            if (previous != null && child != previous)
            {
                child = previous;
            }
            else
            {
                run = false;
            }
        }

        path.Reverse();

        return path;
    }

    private static List<Node> GetNear(Node node, TileLayer layer)
    {
        // Want to add nodes connected to the center node, if they are walkable.
        // This code stops the pathfinder from cutting corners, and going through walls that are diagonal from each other.

        near.Clear();

        // Left
        tile = layer.Unsafe_GetTile(node.X - 1, node.Y);
        left = false;
        if (CanWalk(tile))
        {
            near.Add(new Node() { X = node.X - 1, Y = node.Y });
            left = true;
        }

        // Right
        tile = layer.Unsafe_GetTile(node.X + 1, node.Y);
        right = false;
        if (CanWalk(tile))
        {
            near.Add(new Node() { X = node.X + 1, Y = node.Y });
            right = true;
        }

        // Above
        tile = layer.Unsafe_GetTile(node.X, node.Y + 1);
        above = false;
        if (CanWalk(tile))
        {
            near.Add(new Node() { X = node.X, Y = node.Y + 1 });
            above = true;
        }

        // Below
        tile = layer.Unsafe_GetTile(node.X, node.Y - 1);
        below = false;
        if (CanWalk(tile))
        {
            near.Add(new Node() { X = node.X, Y = node.Y - 1 });
            below = true;
        }

        // Above-Left
        if (left && above)
        {
            tile = layer.Unsafe_GetTile(node.X - 1, node.Y + 1);
            if (CanWalk(tile))
            {
                near.Add(new Node() { X = node.X - 1, Y = node.Y + 1 });
            }
        }

        // Above-Right
        if (right && above)
        {
            tile = layer.Unsafe_GetTile(node.X + 1, node.Y + 1);
            if (CanWalk(tile))
            {
                near.Add(new Node() { X = node.X + 1, Y = node.Y + 1 });
            }
        }

        // Below-Left
        if (left && below)
        {
            tile = layer.Unsafe_GetTile(node.X - 1, node.Y - 1);
            if (CanWalk(tile))
            {
                near.Add(new Node() { X = node.X - 1, Y = node.Y - 1 });
            }
        }

        // Below-Right
        if (right && below)
        {
            tile = layer.Unsafe_GetTile(node.X + 1, node.Y - 1);
            if (CanWalk(tile))
            {
                near.Add(new Node() { X = node.X + 1, Y = node.Y - 1 });
            }
        }

        return near;
    }

    private static bool CanWalk(BaseTile tile)
    {
        return tile == null;
    }

    private static float Heuristic(Node a, Node b)
    {
        // Gives a rough distance.
        return Mathf.Abs(a.X - b.X) + Mathf.Abs(a.Y - b.Y);
    }
}